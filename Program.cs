using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq; 
using System.Text;

// -----------------------------------------------------------------
// 1. ОПИС ПРЕДМЕТНОЇ ОБЛАСТІ (КЛАСИ)
// -----------------------------------------------------------------

public class Category
{
    public int CategoryId { get; set; }
    public string Name { get; set; }
    public char CategoryCode { get; set; }
}

public class Product
{
    public int ProductId { get; set; }
    public string Name { get; set; }
    public double Price { get; set; }
    public int StockQuantity { get; set; }
    public bool IsAvailable { get; set; }
    public int CategoryId { get; set; }
}

public class Customer
{
    public int CustomerId { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Email { get; set; }
    public bool IsEmailVerified { get; set; }
}

public class OrderItem
{
    public int OrderItemId { get; set; }
    public int OrderId { get; set; }
    public int ProductId { get; set; }
    public int Quantity { get; set; }
    public double PriceAtPurchase { get; set; }
}

public class Order
{
    public int OrderId { get; set; }
    public int CustomerId { get; set; }
    public DateTime OrderDate { get; set; }
    public string Status { get; set; }
    public double TotalAmount { get; set; }
    public List<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
}

// -----------------------------------------------------------------
// 2. ГОЛОВНА ПРОГРАМА З МЕНЮ
// -----------------------------------------------------------------

public class Program
{
    // "База даних" у пам'яті (статичні списки)
    static List<Product> products = new List<Product>();
    static List<Customer> customers = new List<Customer>();
    static List<Order> orders = new List<Order>();

    public static void Main(string[] args)
    {
        // Налаштування
        Console.OutputEncoding = Encoding.UTF8;
        CultureInfo.CurrentCulture = new CultureInfo("uk-UA");

        // 1. Заповнюємо "базу даних" початковими значеннями
        SeedData();

        bool isRunning = true;

        // 2. Головний цикл програми
        while (isRunning)
        {
            Console.Clear(); // Очищує консоль перед показом меню
            Console.WriteLine("=== СИСТЕМА УПРАВЛІННЯ МАГАЗИНОМ ===");
            Console.WriteLine("1. Показати список товарів");
            Console.WriteLine("2. Показати список клієнтів");
            Console.WriteLine("3. Зробити нове замовлення");
            Console.WriteLine("4. Переглянути історію замовлень");
            Console.WriteLine("0. Вихід");
            Console.Write("\nВаш вибір > ");

            string choice = Console.ReadLine();

            switch (choice)
            {
                case "1":
                    ShowProducts();
                    break;
                case "2":
                    ShowCustomers();
                    break;
                case "3":
                    CreateOrder();
                    break;
                case "4":
                    ShowOrders();
                    break;
                case "0":
                    isRunning = false;
                    Console.WriteLine("До побачення!");
                    break;
                default:
                    Console.WriteLine("Невірний вибір. Натисніть Enter.");
                    Console.ReadLine();
                    break;
            }
        }
    }

    // --- ДОПОМІЖНІ МЕТОДИ (ФУНКЦІОНАЛ) ---

    static void SeedData()
    {
        // Створюємо тестові товари
        products.Add(new Product { ProductId = 101, Name = "Ноутбук 'Alpha'", Price = 32500.00, StockQuantity = 10, IsAvailable = true, CategoryId = 10 });
        products.Add(new Product { ProductId = 102, Name = "Ноутбук 'Omega'", Price = 42000.00, StockQuantity = 5, IsAvailable = true, CategoryId = 10 });
        products.Add(new Product { ProductId = 103, Name = "Мишка бездротова", Price = 500.00, StockQuantity = 50, IsAvailable = true, CategoryId = 20 });
        products.Add(new Product { ProductId = 104, Name = "Клавіатура мех.", Price = 2100.00, StockQuantity = 0, IsAvailable = false, CategoryId = 20 });

        // Створюємо тестових клієнтів
        customers.Add(new Customer { CustomerId = 1, FirstName = "Іван", LastName = "Петренко", Email = "ivan@mail.com" });
        customers.Add(new Customer { CustomerId = 2, FirstName = "Марія", LastName = "Коваль", Email = "maria@mail.com" });
    }

    static void ShowProducts()
    {
        Console.WriteLine("\n--- СПИСОК ТОВАРІВ ---");
        foreach (var p in products)
        {
            string status = p.IsAvailable ? "Є в наявності" : "Немає";
            Console.WriteLine($"ID: {p.ProductId} | {p.Name} | {p.Price:C} | Склад: {p.StockQuantity} | {status}");
        }
        WaitUser();
    }

    static void ShowCustomers()
    {
        Console.WriteLine("\n--- СПИСОК КЛІЄНТІВ ---");
        foreach (var c in customers)
        {
            Console.WriteLine($"ID: {c.CustomerId} | {c.FirstName} {c.LastName} ({c.Email})");
        }
        WaitUser();
    }

    static void CreateOrder()
    {
        Console.WriteLine("\n--- НОВЕ ЗАМОВЛЕННЯ ---");
        
        // 1. Вибір клієнта
        Console.Write("Введіть ID клієнта: ");
        if (!int.TryParse(Console.ReadLine(), out int custId)) { Console.WriteLine("Помилка вводу!"); WaitUser(); return; }

        var client = customers.FirstOrDefault(c => c.CustomerId == custId);
        if (client == null) { Console.WriteLine("Клієнта не знайдено!"); WaitUser(); return; }
        
        Console.WriteLine($"Вибрано: {client.FirstName} {client.LastName}");

        // 2. Створення об'єкта замовлення
        Order newOrder = new Order
        {
            OrderId = orders.Count + 5001, // Генеруємо новий ID
            CustomerId = client.CustomerId,
            OrderDate = DateTime.Now,
            Status = "Нове"
        };

        // 3. Додавання товарів
        bool addingProducts = true;
        while (addingProducts)
        {
            Console.Write("\nВведіть ID товару (або 0 щоб завершити): ");
            if (!int.TryParse(Console.ReadLine(), out int prodId)) continue;

            if (prodId == 0) break;

            var product = products.FirstOrDefault(p => p.ProductId == prodId);
            if (product == null)
            {
                Console.WriteLine("Товар не знайдено.");
                continue;
            }

            if (!product.IsAvailable || product.StockQuantity == 0)
            {
                Console.WriteLine("Товару немає в наявності.");
                continue;
            }

            Console.Write($"Товар '{product.Name}'. Введіть кількість: ");
            if (!int.TryParse(Console.ReadLine(), out int qty) || qty <= 0) 
            {
                Console.WriteLine("Невірна кількість.");
                continue;
            }

            if (qty > product.StockQuantity)
            {
                Console.WriteLine($"Недостатньо на складі (доступно: {product.StockQuantity}).");
                continue;
            }

            // Додаємо товар у кошик замовлення
            newOrder.OrderItems.Add(new OrderItem
            {
                OrderItemId = newOrder.OrderItems.Count + 1,
                OrderId = newOrder.OrderId,
                ProductId = product.ProductId,
                Quantity = qty,
                PriceAtPurchase = product.Price
            });

            // Зменшуємо склад
            product.StockQuantity -= qty;
            Console.WriteLine("Товар додано у кошик!");
        }

        // 4. Завершення
        if (newOrder.OrderItems.Count > 0)
        {
            // Рахуємо суму
            newOrder.TotalAmount = newOrder.OrderItems.Sum(x => x.PriceAtPurchase * x.Quantity);
            orders.Add(newOrder);
            Console.WriteLine($"\nЗамовлення #{newOrder.OrderId} успішно створено! Сума: {newOrder.TotalAmount:C}");
        }
        else
        {
            Console.WriteLine("\nЗамовлення скасовано (порожній кошик).");
        }

        WaitUser();
    }

    static void ShowOrders()
    {
        Console.WriteLine("\n--- ІСТОРІЯ ЗАМОВЛЕНЬ ---");
        if (orders.Count == 0) Console.WriteLine("Замовлень поки немає.");

        foreach (var ord in orders)
        {
            Console.WriteLine($"#{ord.OrderId} від {ord.OrderDate} | Сума: {ord.TotalAmount:C} | Статус: {ord.Status}");
            foreach(var item in ord.OrderItems)
            {
                // Знаходимо назву товару для красивого виводу
                var prodName = products.FirstOrDefault(p => p.ProductId == item.ProductId)?.Name ?? "Видалений товар";
                Console.WriteLine($"   -> {prodName} x {item.Quantity} шт.");
            }
            Console.WriteLine("-");
        }
        WaitUser();
    }

    // Просто щоб зупинити екран і почекати Enter
    static void WaitUser()
    {
        Console.WriteLine("\nНатисніть Enter щоб продовжити...");
        Console.ReadLine();
    }
}