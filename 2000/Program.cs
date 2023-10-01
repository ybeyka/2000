using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

public class Kontakt
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Name { get; set; }
    public string PhoneNumber { get; set; }
    public string Address { get; set; }

    public override string ToString() => $"{Id},{Name},{PhoneNumber},{Address}";

    public static Kontakt FromString(string str)
    {
        var parts = str.Split(',');
        return new Kontakt
        {
            Id = Guid.Parse(parts[0]),
            Name = parts[1],
            PhoneNumber = parts[2],
            Address = parts[3]
        };
    }
}

public class Zmina
{
    public DateTime Date { get; set; }
    public string ActionType { get; set; }
    public Kontakt Kontakt { get; set; }

    public override string ToString() => $"{Date},{ActionType},{Kontakt}";

    public static Zmina FromString(string str)
    {
        var parts = str.Split(',');
        return new Zmina
        {
            Date = DateTime.Parse(parts[0]),
            ActionType = parts[1],
            Kontakt = Kontakt.FromString(string.Join(",", parts.Skip(2)))
        };
    }
}

public class TelefonnaKnyha
{
    private readonly string historyPath = "history.txt";
    public List<Kontakt> Kontakty { get; set; } = new List<Kontakt>();

    public void AddKontakt(Kontakt k)
    {
        Kontakty.Add(k);
        AppendToHistory(new Zmina
        {
            Date = DateTime.Now,
            ActionType = "Додавання",
            Kontakt = k
        });
    }

    public void EditKontakt(Guid id, Kontakt newKontakt)
    {
        var oldKontakt = Kontakty.FirstOrDefault(k => k.Id == id);
        if (oldKontakt != null)
        {
            oldKontakt.Name = newKontakt.Name;
            oldKontakt.PhoneNumber = newKontakt.PhoneNumber;
            oldKontakt.Address = newKontakt.Address;
            AppendToHistory(new Zmina
            {
                Date = DateTime.Now,
                ActionType = "Редагування",
                Kontakt = oldKontakt
            });
        }
    }

    private void AppendToHistory(Zmina z)
    {
        File.AppendAllLines(historyPath, new[] { z.ToString() });
    }

    public List<Zmina> LoadHistory(Guid id)
    {
        if (File.Exists(historyPath))
        {
            return File.ReadAllLines(historyPath)
                       .Select(Zmina.FromString)
                       .Where(z => z.Kontakt.Id == id)
                       .ToList();
        }
        return new List<Zmina>();
    }
}

public class Program
{
    public static void Main()
    {
        Console.InputEncoding = System.Text.Encoding.Unicode;
        Console.OutputEncoding = System.Text.Encoding.Unicode;

        TelefonnaKnyha knyha = new TelefonnaKnyha();
        while (true)
        {
            Console.WriteLine("Оберіть опцію:");
            Console.WriteLine("1. Додати контакт");
            Console.WriteLine("2. Видалити контакт");
            Console.WriteLine("3. Редагувати контакт");
            Console.WriteLine("4. Показати контакт");
            Console.WriteLine("5. Завантажити історію");
            Console.WriteLine("6. Вийти з програми");

            var choice = Console.ReadLine();

            switch (choice)
            {
                case "1":
                    Kontakt k = new Kontakt();
                    Console.WriteLine("Введіть ім'я:");
                    k.Name = Console.ReadLine();
                    Console.WriteLine("Введіть номер телефону:");
                    k.PhoneNumber = Console.ReadLine();
                    Console.WriteLine("Введіть адресу:");
                    k.Address = Console.ReadLine();
                    knyha.AddKontakt(k);
                    break;
                case "3":
                    Console.WriteLine("Введіть ім'я контакту, який хочете редагувати:");
                    string nameToEdit = Console.ReadLine();
                    var kontaktToEdit = knyha.Kontakty.FirstOrDefault(kont => kont.Name == nameToEdit);
                    if (kontaktToEdit == null)
                    {
                        Console.WriteLine("Контакт не знайдено.");
                    }
                    else
                    {
                        Kontakt newK = new Kontakt();
                        Console.WriteLine("Введіть нове ім'я:");
                        newK.Name = Console.ReadLine();
                        Console.WriteLine("Введіть новий номер телефону:");
                        newK.PhoneNumber = Console.ReadLine();
                        Console.WriteLine("Введіть нову адресу:");
                        newK.Address = Console.ReadLine();
                        knyha.EditKontakt(kontaktToEdit.Id, newK);
                    }
                    break;
                case "5":
                    Console.WriteLine("Введіть ім'я контакту, історію змін для якого ви хочете побачити:");
                    string nameToLoad = Console.ReadLine();
                    var kontaktToLoad = knyha.Kontakty.FirstOrDefault(kont => kont.Name == nameToLoad);
                    if (kontaktToLoad != null)
                    {
                        var zmyny = knyha.LoadHistory(kontaktToLoad.Id);
                        foreach (var z in zmyny)
                        {
                            Console.WriteLine($"{z.Date},{z.ActionType},{z.Kontakt.Name},{z.Kontakt.PhoneNumber},{z.Kontakt.Address}");
                        }
                    }
                    else
                    {
                        Console.WriteLine("Контакт не знайдено.");
                    }
                    break;
                case "6":
                    return;
            }
        }
    }
}
