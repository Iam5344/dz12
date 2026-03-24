using Microsoft.Data.SqlClient;
using Dapper;

class Program
{
    static string cs = "Data Source=DESKTOP-8UTPR8Q\\IAM5344;Initial Catalog=LibraryDb;Integrated Security=True;Encrypt=False;";

    static void Main()
    {
        using var con = new SqlConnection(cs);
        con.Open();

        con.Execute(@"
            IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='Books' AND xtype='U')
            CREATE TABLE Books (
                Id INT PRIMARY KEY IDENTITY(1,1),
                Title NVARCHAR(200) NOT NULL,
                Author NVARCHAR(200) NOT NULL
            )");

        while (true)
        {
            Console.WriteLine("\n1. Додати книгу");
            Console.WriteLine("2. Всі книги");
            Console.WriteLine("3. Пошук за назвою");
            Console.WriteLine("4. Пошук за автором");
            Console.WriteLine("5. Пошук за Id");
            Console.WriteLine("6. Видалити за Id");
            Console.WriteLine("7. Оновити за Id");
            Console.WriteLine("0. Вихід");
            Console.Write("Вибір: ");
            string choice = Console.ReadLine();

            if (choice == "1")
            {
                Console.Write("Назва: "); string title = Console.ReadLine();
                Console.Write("Автор: "); string author = Console.ReadLine();
                con.Execute("INSERT INTO Books (Title, Author) VALUES (@Title, @Author)",
                    new { Title = title, Author = author });
                Console.WriteLine("Додано.");
            }
            else if (choice == "2")
            {
                PrintBooks(con.Query("SELECT * FROM Books"));
            }
            else if (choice == "3")
            {
                Console.Write("Назва: "); string title = Console.ReadLine();
                PrintBooks(con.Query("SELECT * FROM Books WHERE Title = @Title", new { Title = title }));
            }
            else if (choice == "4")
            {
                Console.Write("Автор: "); string author = Console.ReadLine();
                PrintBooks(con.Query("SELECT * FROM Books WHERE Author = @Author", new { Author = author }));
            }
            else if (choice == "5")
            {
                Console.Write("Id: "); int id = int.Parse(Console.ReadLine());
                PrintBooks(con.Query("SELECT * FROM Books WHERE Id = @Id", new { Id = id }));
            }
            else if (choice == "6")
            {
                Console.Write("Id: "); int id = int.Parse(Console.ReadLine());
                con.Execute("DELETE FROM Books WHERE Id = @Id", new { Id = id });
                Console.WriteLine("Видалено.");
            }
            else if (choice == "7")
            {
                Console.Write("Id: "); int id = int.Parse(Console.ReadLine());
                Console.Write("Нова назва: "); string title = Console.ReadLine();
                Console.Write("Новий автор: "); string author = Console.ReadLine();
                con.Execute("UPDATE Books SET Title = @Title, Author = @Author WHERE Id = @Id",
                    new { Title = title, Author = author, Id = id });
                Console.WriteLine("Оновлено.");
            }
            else if (choice == "0") return;
        }
    }

    static void PrintBooks(IEnumerable<dynamic> books)
    {
        foreach (var b in books)
            Console.WriteLine($"{b.Id} | {b.Title} | {b.Author}");
    }
}
