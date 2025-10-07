using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.Json;

enum Category { Food, Transport, Fun, School, Other }

class Income
{
    public DateTime Date { get; set; }
    public string Source { get; set; }
    public decimal Amount { get; set; }
}

class Expense
{
    public DateTime Date { get; set; }
    public Category Category { get; set; }
    public decimal Amount { get; set; }
    public string Note { get; set; }
}

class Subscription
{
    public string Name { get; set; }
    public decimal MonthlyPrice { get; set; }
    public DateTime StartDate { get; set; }
    public bool IsActive { get; set; }
}

class Program
{
    static List<Income> incomes = new();
    static List<Expense> expenses = new();
    static List<Subscription> subscriptions = new();

    static void Main()
    {
        while (true)
        {
            Console.WriteLine("\n1) Ienākumi 2) Izdevumi 3) Abonementi 4) Saraksti 5) Filtri 6) Mēneša pārskats 7) JSON 0) Iziet");
            Console.Write("Izvēlies opciju: ");
            string choice = Console.ReadLine();

            switch (choice)
            {
                case "1": MenuIncome(); break;
                case "2": MenuExpense(); break;
                case "3": MenuSubscription(); break;
                case "4": ShowAllEntries(); break;
                case "5": FilterMenu(); break;
                case "6": MonthlyReport(); break;
                case "7": JsonMenu(); break;
                case "0": return;
                default: Console.WriteLine("Nederīga izvēle."); break;
            }
        }
    }

    static void MenuIncome()
    {
        Console.WriteLine("\n1) Pievienot 2) Rādīt 3) Dzēst");
        string ch = Console.ReadLine();
        if (ch == "1")
        {
            Console.Write("Datums (yyyy-mm-dd): ");
            if (!DateTime.TryParse(Console.ReadLine(), out var date)) return;

            Console.Write("Avots: ");
            string source = ReadNonEmpty();

            Console.Write("Summa: ");
            if (!TryParseDecimal(Console.ReadLine(), out var amount)) return;

            if (amount <= 0) { Console.WriteLine("Summai jābūt > 0"); return; }

            incomes.Add(new Income { Date = date, Source = source, Amount = amount });
        }
        else if (ch == "2")
        {
            foreach (var i in incomes.OrderByDescending(i => i.Date))
                Console.WriteLine($"{i.Date:yyyy-MM-dd} | {i.Source} | {i.Amount} €");
        }
        else if (ch == "3")
        {
            incomes.Clear();
            Console.WriteLine("Ienākumi dzēsti.");
        }
    }

    static void MenuExpense()
    {
        Console.WriteLine("\n1) Pievienot 2) Rādīt 3) Dzēst");
        string ch = Console.ReadLine();
        if (ch == "1")
        {
            Console.Write("Datums (yyyy-mm-dd): ");
            if (!DateTime.TryParse(Console.ReadLine(), out var date)) return;

            Console.Write("Kategorija (Food, Transport, Fun, School, Other): ");
            if (!Enum.TryParse<Category>(Console.ReadLine(), true, out var category)) return;

            Console.Write("Summa: ");
            if (!TryParseDecimal(Console.ReadLine(), out var amount)) return;

            Console.Write("Piezīme: ");
            string note = ReadNonEmpty();

            if (amount <= 0) { Console.WriteLine("Summai jābūt > 0"); return; }

            expenses.Add(new Expense { Date = date, Category = category, Amount = amount, Note = note });
        }
        else if (ch == "2")
        {
            foreach (var e in expenses.OrderByDescending(e => e.Date))
                Console.WriteLine($"{e.Date:yyyy-MM-dd} | {e.Category} | {e.Amount} € | {e.Note}");
        }
        else if (ch == "3")
        {
            expenses.Clear();
            Console.WriteLine("Izdevumi dzēsti.");
        }
    }

    static void MenuSubscription()
    {
        Console.WriteLine("\n1) Pievienot 2) Aktivizēt/deaktivizēt 3) Dzēst");
        string ch = Console.ReadLine();
        if (ch == "1")
        {
            Console.Write("Nosaukums: ");
            string name = ReadNonEmpty();

            Console.Write("Mēneša maksa: ");
            if (!TryParseDecimal(Console.ReadLine(), out var price)) return;

            Console.Write("Sākuma datums (yyyy-mm-dd): ");
            if (!DateTime.TryParse(Console.ReadLine(), out var date)) return;

            subscriptions.Add(new Subscription { Name = name, MonthlyPrice = price, StartDate = date, IsActive = true });
        }
        else if (ch == "2")
        {
            foreach (var (s, i) in subscriptions.Select((s, i) => (s, i)))
                Console.WriteLine($"{i}: {s.Name} | Aktīvs: {s.IsActive}");

            Console.Write("Ievadi indeksu: ");
            if (int.TryParse(Console.ReadLine(), out var index) && index >= 0 && index < subscriptions.Count)
                subscriptions[index].IsActive = !subscriptions[index].IsActive;
        }
        else if (ch == "3")
        {
            subscriptions.Clear();
            Console.WriteLine("Abonementi dzēsti.");
        }
    }

    static void MonthlyReport()
    {
        Console.Write("Ievadi mēnesi (yyyy-MM): ");
        string input = Console.ReadLine();
        if (!DateTime.TryParseExact(input + "-01", "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out var month))
        {
            Console.WriteLine("Nederīgs formāts.");
            return;
        }

        var start = new DateTime(month.Year, month.Month, 1);
        var end = start.AddMonths(1);

        var incomeSum = incomes.Where(i => i.Date >= start && i.Date < end).Sum(i => i.Amount);
        var expenseSum = expenses.Where(e => e.Date >= start && e.Date < end).Sum(e => e.Amount);
        var activeSubs = subscriptions.Where(s => s.IsActive).Sum(s => s.MonthlyPrice);

        Console.WriteLine($"Ienākumi: {incomeSum} €");
        Console.WriteLine($"Izdevumi: {expenseSum} €");
        Console.WriteLine($"Abonementi: {activeSubs} €");
        Console.WriteLine($"Neto: {incomeSum - expenseSum - activeSubs} €");
    }

    static void ShowAllEntries()
    {
        Console.WriteLine("\n--- Visi Ieraksti ---");
        foreach (var i in incomes.OrderByDescending(i => i.Date))
            Console.WriteLine($"[Ienākums] {i.Date:yyyy-MM-dd} | {i.Source} | {i.Amount} €");

        foreach (var e in expenses.OrderByDescending(e => e.Date))
            Console.WriteLine($"[Izdevums] {e.Date:yyyy-MM-dd} | {e.Category} | {e.Amount} € | {e.Note}");

        foreach (var s in subscriptions)
            Console.WriteLine($"[Abon.] {s.Name} | {s.MonthlyPrice} € | Aktīvs: {s.IsActive}");
    }

    static void FilterMenu()
    {
        Console.WriteLine("Filtrēt pēc kategorijas (Food, Transport, Fun, School, Other): ");
        if (!Enum.TryParse<Category>(Console.ReadLine(), true, out var cat)) return;

        var filtered = expenses.Where(e => e.Category == cat);
        foreach (var e in filtered)
            Console.WriteLine($"{e.Date:yyyy-MM-dd} | {e.Category} | {e.Amount} € | {e.Note}");

        var total = filtered.Sum(e => e.Amount);
        Console.WriteLine($"Kopā: {total} €");
    }

    static void JsonMenu()
    {
        Console.WriteLine("1) Eksportēt 2) Importēt");
        string ch = Console.ReadLine();
        if (ch == "1")
        {
            var data = new { incomes, expenses, subscriptions };
            string json = JsonSerializer.Serialize(data, new JsonSerializerOptions { WriteIndented = true });
            Console.WriteLine(json);
        }
        else if (ch == "2")
        {
            Console.WriteLine("Ievadi JSON:");
            string input = Console.ReadLine();
            try
            {
                var obj = JsonSerializer.Deserialize<TempData>(input);
                if (obj != null)
                {
                    incomes = obj.incomes ?? new();
                    expenses = obj.expenses ?? new();
                    subscriptions = obj.subscriptions ?? new();
                    Console.WriteLine("Dati importēti.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Importēšanas kļūda: " + ex.Message);
            }
        }
    }

    static string ReadNonEmpty()
    {
        string input;
        do
        {
            input = Console.ReadLine();
        } while (string.IsNullOrWhiteSpace(input));
        return input;
    }

    static bool TryParseDecimal(string input, out decimal value) =>
        decimal.TryParse(input, NumberStyles.Number, CultureInfo.InvariantCulture, out value);

    class TempData
    {
        public List<Income>? incomes { get; set; }
        public List<Expense>? expenses { get; set; }
        public List<Subscription>? subscriptions { get; set; }
    }
}
