using System;
using System.IO;
using System.Text.Json;
using System.Threading;

public enum LogLevel { INFO = 0, WARNING = 1, ERROR = 2 }

public class LoggerConfig
{
    public string LogFilePath { get; set; } = "app.log";
    public LogLevel MinLevel  { get; set; } = LogLevel.INFO;
}

public sealed class Logger
{
    private static readonly Lazy<Logger> _instance =
        new Lazy<Logger>(() => new Logger());

    private readonly object _lock = new();
    private string _logFilePath;
    private LogLevel _currentLevel;
    private long _maxFileSizeBytes = 5 * 1024 * 1024;

    private Logger() => LoadConfig("logger.json");

    public static Logger GetInstance() => _instance.Value;

    private void LoadConfig(string configPath)
    {
        if (File.Exists(configPath))
        {
            var cfg = JsonSerializer.Deserialize<LoggerConfig>(File.ReadAllText(configPath))!;
            _logFilePath  = cfg.LogFilePath;
            _currentLevel = cfg.MinLevel;
        }
        else
        {
            _logFilePath  = "app.log";
            _currentLevel = LogLevel.INFO;
        }
    }

    public void SetLogLevel(LogLevel level)
    {
        lock (_lock) { _currentLevel = level; }
    }

    public void Log(string message, LogLevel level)
    {
        lock (_lock)
        {
            if (level < _currentLevel) return;

            RotateIfNeeded();

            string entry = $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] " +
                           $"[{level}] [Thread-{Thread.CurrentThread.ManagedThreadId}] " +
                           $"{message}";

            File.AppendAllText(_logFilePath, entry + Environment.NewLine);

            Console.ForegroundColor = level switch
            {
                LogLevel.WARNING => ConsoleColor.Yellow,
                LogLevel.ERROR   => ConsoleColor.Red,
                _                => ConsoleColor.White
            };
            Console.WriteLine(entry);
            Console.ResetColor();
        }
    }

    private void RotateIfNeeded()
    {
        if (!File.Exists(_logFilePath)) return;
        if (new FileInfo(_logFilePath).Length >= _maxFileSizeBytes)
        {
            string archived = _logFilePath.Replace(".log",
                $"_{DateTime.Now:yyyyMMdd_HHmmss}.log");
            File.Move(_logFilePath, archived);
        }
    }
}

public class LogReader
{
    private readonly string _path;
    public LogReader(string path) => _path = path;

    public void PrintLogs(LogLevel? filter = null)
    {
        if (!File.Exists(_path)) { Console.WriteLine("Файл не найден."); return; }
        foreach (var line in File.ReadAllLines(_path))
            if (filter == null || line.Contains($"[{filter}]"))
                Console.WriteLine(line);
    }
}

class Program
{
    static void Main()
    {
        File.WriteAllText("logger.json", "{\"LogFilePath\":\"app.log\",\"MinLevel\":0}");

        var logger = Logger.GetInstance();

        var threads = new Thread[5];
        for (int i = 0; i < 5; i++)
        {
            int id = i;
            threads[i] = new Thread(() =>
            {
                logger.Log($"Поток {id}: INFO",    LogLevel.INFO);
                logger.Log($"Поток {id}: WARNING", LogLevel.WARNING);
                logger.Log($"Поток {id}: ERROR",   LogLevel.ERROR);
            });
        }
        foreach (var t in threads) t.Start();
        foreach (var t in threads) t.Join();

        logger.SetLogLevel(LogLevel.ERROR);
        logger.Log("Это INFO — не запишется", LogLevel.INFO);
        logger.Log("Это ERROR — запишется",   LogLevel.ERROR);

        Console.WriteLine("\n=== Только ERROR ===");
        new LogReader("app.log").PrintLogs(LogLevel.ERROR);
    }
}