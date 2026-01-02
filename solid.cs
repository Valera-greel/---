using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

// Інтерфейси
public interface ILogger { void Log(string message); }
public interface INotificationService { void Notify(string message); }

// Реалізація інтерфейсів
public class ConsoleLogger : ILogger
{
    public void Log(string message) => Console.WriteLine($"[LOG]: {message}");
}

public class EmailService : INotificationService
{
    public void Notify(string message) => Console.WriteLine($"[EMAIL SENT]: {message}");
}

// Класи бізнесу
public class Client { public int Id; public string Name; }
public class BankAccount
{
    public string Number = Guid.NewGuid().ToString().Substring(0, 5);
    public decimal Balance;
    public void Deposit(decimal amount) => Balance += amount;
}

// Банківський сервіс (з Dependency Injection)
public class BankService
{
    private ILogger _logger;
    private INotificationService _notifier;
    public List<BankAccount> Accounts = new List<BankAccount>();

    public BankService(ILogger logger, INotificationService notifier)
    {
        _logger = logger;
        _notifier = notifier;
    }

    public void OpenAccount(string clientName)
    {
        var acc = new BankAccount();
        Accounts.Add(acc);
        _logger.Log($"Відкрито рахунок {acc.Number} для {clientName}");
        _notifier.Notify($"Вітаємо, {clientName}! Ваш рахунок готовий.");
    }
}

class Program
{
    static void Main(string[] args)
    {
        Console.OutputEncoding = Encoding.UTF8;
        
        // Налаштування залежностей
        ILogger logger = new ConsoleLogger();
        INotificationService email = new EmailService();
        BankService bank = new BankService(logger, email);

        Console.WriteLine("=== БАНК SOLID (Лабораторна 2) ===");
        bank.OpenAccount("Ілон Маск");

        var acc = bank.Accounts[0];
        acc.Deposit(1000);
        Console.WriteLine($"Баланс: {acc.Balance}");
        
        Console.ReadLine();
    }
}