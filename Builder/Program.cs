using System;
using System.Collections.Generic;
using System.Text;

public class ReportStyle
{
    public string BackgroundColor { get; set; } = "#FFFFFF";
    public string FontColor       { get; set; } = "#000000";
    public int    FontSize        { get; set; } = 14;
}

public class Report
{
    public string Header  { get; set; } = "";
    public string Content { get; set; } = "";
    public string Footer  { get; set; } = "";
    public ReportStyle Style { get; set; } = new();
    public List<(string Name, string Content)> Sections { get; } = new();

    public void AddSection(string name, string content) => Sections.Add((name, content));

    public string Export(string format = "text") =>
        format.ToLower() == "html" ? ToHtml() : ToText();

    private string ToText()
    {
        var sb = new StringBuilder();
        sb.AppendLine($"=== {Header} ===");
        sb.AppendLine(Content);
        foreach (var (name, cnt) in Sections)
            sb.AppendLine($"[{name}]\n{cnt}");
        sb.AppendLine($"--- {Footer} ---");
        return sb.ToString();
    }

    private string ToHtml()
    {
        var sb = new StringBuilder();
        sb.AppendLine($"<html><body style='background:{Style.BackgroundColor};" +
                      $"color:{Style.FontColor};font-size:{Style.FontSize}px'>");
        sb.AppendLine($"<h1>{Header}</h1><p>{Content}</p>");
        foreach (var (name, cnt) in Sections)
            sb.AppendLine($"<h2>{name}</h2><p>{cnt}</p>");
        sb.AppendLine($"<footer>{Footer}</footer></body></html>");
        return sb.ToString();
    }
}

public interface IReportBuilder
{
    IReportBuilder SetHeader(string header);
    IReportBuilder SetContent(string content);
    IReportBuilder SetFooter(string footer);
    IReportBuilder AddSection(string name, string content);
    IReportBuilder SetStyle(ReportStyle style);
    Report GetReport();
}

public class TextReportBuilder : IReportBuilder
{
    private readonly Report _report = new();
    public IReportBuilder SetHeader(string h)  { _report.Header  = h; return this; }
    public IReportBuilder SetContent(string c) { _report.Content = c; return this; }
    public IReportBuilder SetFooter(string f)  { _report.Footer  = f; return this; }
    public IReportBuilder AddSection(string n, string c) { _report.AddSection(n, c); return this; }
    public IReportBuilder SetStyle(ReportStyle s) { _report.Style = s; return this; }
    public Report GetReport() => _report;
}

public class HtmlReportBuilder : IReportBuilder
{
    private readonly Report _report = new();
    public IReportBuilder SetHeader(string h)  { _report.Header  = h; return this; }
    public IReportBuilder SetContent(string c) { _report.Content = c; return this; }
    public IReportBuilder SetFooter(string f)  { _report.Footer  = f; return this; }
    public IReportBuilder AddSection(string n, string c) { _report.AddSection(n, c); return this; }
    public IReportBuilder SetStyle(ReportStyle s) { _report.Style = s; return this; }
    public Report GetReport() => _report;
}

public class PdfReportBuilder : IReportBuilder
{
    private readonly Report _report = new();
    public IReportBuilder SetHeader(string h)  { _report.Header  = $"[PDF] {h}"; return this; }
    public IReportBuilder SetContent(string c) { _report.Content = $"[PDF] {c}"; return this; }
    public IReportBuilder SetFooter(string f)  { _report.Footer  = $"[PDF] {f}"; return this; }
    public IReportBuilder AddSection(string n, string c) { _report.AddSection(n, c); return this; }
    public IReportBuilder SetStyle(ReportStyle s) { _report.Style = s; return this; }
    public Report GetReport() => _report;
}

public class ReportDirector
{
    public Report ConstructReport(IReportBuilder builder, ReportStyle style) =>
        builder
            .SetStyle(style)
            .SetHeader("Отчёт Q4 2024")
            .SetContent("Общие данные по продажам.")
            .AddSection("Продажи", "Выручка: 4 500 000 руб.")
            .AddSection("Расходы", "Затраты: 2 100 000 руб.")
            .SetFooter("Конфиденциально")
            .GetReport();
}

class Program
{
    static void Main()
    {
        var director = new ReportDirector();
        var style = new ReportStyle { BackgroundColor = "#F0F8FF", FontColor = "#333", FontSize = 14 };

        Console.WriteLine("--- TEXT ---");
        Console.WriteLine(director.ConstructReport(new TextReportBuilder(), style).Export("text"));

        Console.WriteLine("--- HTML ---");
        Console.WriteLine(director.ConstructReport(new HtmlReportBuilder(), style).Export("html"));

        Console.WriteLine("--- PDF ---");
        Console.WriteLine(director.ConstructReport(new PdfReportBuilder(), style).Export("text"));
    }
}