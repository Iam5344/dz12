using Microsoft.EntityFrameworkCore;


public class Student
{
    public int Id { get; set; }
    public string FullName { get; set; }
    public int Age { get; set; }
}

public class Teacher
{
    public int Id { get; set; }
    public string FullName { get; set; }
    public int Age { get; set; }
    public decimal Salary { get; set; }
}

public class AppContext : DbContext
{
    public DbSet<Student> Students { get; set; }
    public DbSet<Teacher> Teachers { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder options)
    {
        options.UseSqlServer("Data Source=DESKTOP-8UTPR8Q\\IAM5344;Initial Catalog=EFDb;Integrated Security=True;Encrypt=False;");
    }
}


class Program
{
    static void Main()
    {
        using var db = new AppContext();
        db.Database.Migrate();

        while (true)
        {
            Console.WriteLine("\n1. Створити студента");
            Console.WriteLine("2. Створити викладача");
            Console.WriteLine("3. Показати всіх студентів");
            Console.WriteLine("4. Показати всіх викладачів");
            Console.WriteLine("0. Вихід");
            Console.Write("Вибір: ");
            string choice = Console.ReadLine();

            if (choice == "1")
            {
                Console.Write("ПІБ: "); string name = Console.ReadLine();
                Console.Write("Вік: "); int age = int.Parse(Console.ReadLine());
                db.Students.Add(new Student { FullName = name, Age = age });
                db.SaveChanges();
                Console.WriteLine("Студента додано.");
            }
            else if (choice == "2")
            {
                Console.Write("ПІБ: "); string name = Console.ReadLine();
                Console.Write("Вік: "); int age = int.Parse(Console.ReadLine());
                Console.Write("Зарплата: "); decimal salary = decimal.Parse(Console.ReadLine());
                db.Teachers.Add(new Teacher { FullName = name, Age = age, Salary = salary });
                db.SaveChanges();
                Console.WriteLine("Викладача додано.");
            }
            else if (choice == "3")
            {
                foreach (var s in db.Students)
                    Console.WriteLine($"{s.Id} | {s.FullName} | {s.Age}");
            }
            else if (choice == "4")
            {
                foreach (var t in db.Teachers)
                    Console.WriteLine($"{t.Id} | {t.FullName} | {t.Age} | {t.Salary}");
            }
            else if (choice == "0") return;
        }
    }
}
