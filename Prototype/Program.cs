using System;
using System.Collections.Generic;

public class Weapon : ICloneable
{
    public string Name   { get; set; }
    public int    Damage { get; set; }

    public Weapon(string name, int damage) { Name = name; Damage = damage; }
    public object Clone() => new Weapon(Name, Damage);
    public override string ToString() => $"{Name} (урон: {Damage})";
}

public class Armor : ICloneable
{
    public string Name    { get; set; }
    public int    Defense { get; set; }

    public Armor(string name, int defense) { Name = name; Defense = defense; }
    public object Clone() => new Armor(Name, Defense);
    public override string ToString() => $"{Name} (защита: {Defense})";
}

public class Skill : ICloneable
{
    public string Name        { get; set; }
    public string Type        { get; set; }
    public int    PowerPoints { get; set; }

    public Skill(string name, string type, int pp) { Name = name; Type = type; PowerPoints = pp; }
    public object Clone() => new Skill(Name, Type, PowerPoints);
    public override string ToString() => $"{Name} [{Type}] PP:{PowerPoints}";
}

public class Character : ICloneable
{
    public string Name         { get; set; }
    public int    Health       { get; set; }
    public int    Strength     { get; set; }
    public int    Agility      { get; set; }
    public int    Intelligence { get; set; }
    public Weapon      Weapon { get; set; }
    public Armor       Armor  { get; set; }
    public List<Skill> Skills { get; set; } = new();

    public Character(string name, int hp, int str, int agi, int intel, Weapon weapon, Armor armor)
    {
        Name = name; Health = hp; Strength = str;
        Agility = agi; Intelligence = intel;
        Weapon = weapon; Armor = armor;
    }

    public object Clone()
    {
        var clone = new Character(Name, Health, Strength, Agility, Intelligence,
                                  (Weapon)Weapon.Clone(), (Armor)Armor.Clone());
        foreach (var skill in Skills)
            clone.Skills.Add((Skill)skill.Clone());
        return clone;
    }

    public void Print()
    {
        Console.WriteLine($"=== {Name} ===");
        Console.WriteLine($"  HP:{Health} STR:{Strength} AGI:{Agility} INT:{Intelligence}");
        Console.WriteLine($"  Оружие: {Weapon}");
        Console.WriteLine($"  Броня:  {Armor}");
        foreach (var s in Skills) Console.WriteLine($"  - {s}");
    }
}

class Program
{
    static void Main()
    {
        var knight = new Character("Рыцарь", 200, 80, 50, 30,
            new Weapon("Меч", 45), new Armor("Латные доспехи", 70));
        knight.Skills.Add(new Skill("Удар щитом",     "Physical", 10));
        knight.Skills.Add(new Skill("Священный свет", "Magic",    15));

        Console.WriteLine("ОРИГИНАЛ:");
        knight.Print();

        var darkKnight = (Character)knight.Clone();
        darkKnight.Name          = "Тёмный Рыцарь";
        darkKnight.Weapon.Name   = "Проклятый меч";
        darkKnight.Weapon.Damage = 75;
        darkKnight.Skills.Add(new Skill("Тёмная аура", "Magic", 20));

        Console.WriteLine("\nКЛОН:");
        darkKnight.Print();

        Console.WriteLine("\nОРИГИНАЛ (не изменился):");
        knight.Print();
    }
}