using Microsoft.EntityFrameworkCore;

// Entities
public class Group
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public List<Student> Students { get; set; } = new();
}

public class Student
{
    public int Id { get; set; }
    public string FullName { get; set; } = null!;
    public int GroupId { get; set; }
    public Group Group { get; set; } = null!;
}

public class Teacher
{
    public int Id { get; set; }
    public string FullName { get; set; } = null!;
    public List<Subject> Subjects { get; set; } = new();
}

public class Subject
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public int TeacherId { get; set; }
    public Teacher Teacher { get; set; } = null!;
}

public class TeacherSubjectView
{
    public string TeacherName { get; set; } = null!;
    public string SubjectName { get; set; } = null!;
}
public class AppDbContext : DbContext
{
    public DbSet<Group> Groups { get; set; }
    public DbSet<Student> Students { get; set; }
    public DbSet<Teacher> Teachers { get; set; }
    public DbSet<Subject> Subjects { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder options)
    {
        options.UseSqlServer("Data Source=DESKTOP-8UTPR8Q\\IAM5344;Initial Catalog=SchoolDb;Integrated Security=True;Encrypt=False;");
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Subject>()
            .HasOne(s => s.Teacher)
            .WithMany(t => t.Subjects)
            .HasForeignKey(s => s.TeacherId);

        modelBuilder.Entity<Student>()
            .HasOne(s => s.Group)
            .WithMany(g => g.Students)
            .HasForeignKey(s => s.GroupId);

        modelBuilder.Entity<TeacherSubjectView>()
            .HasNoKey()
            .ToView("TeacherSubjectsView");
    }
}

class Program
{
    static AppDbContext db = new AppDbContext();

    static void Main(string[] args)
    {
        db.Database.Migrate();

        db.Database.ExecuteSqlRaw(@"
            IF NOT EXISTS (SELECT * FROM sys.views WHERE name = 'TeacherSubjectsView')
            EXEC('CREATE VIEW TeacherSubjectsView AS
                SELECT T.FullName AS TeacherName, S.Name AS SubjectName
                FROM Teachers T
                INNER JOIN Subjects S ON T.Id = S.TeacherId')");

        db.Database.ExecuteSqlRaw(@"
            IF NOT EXISTS (SELECT * FROM sys.procedures WHERE name = 'AddTeacher')
            EXEC('CREATE PROCEDURE AddTeacher @FullName NVARCHAR(MAX)
                AS BEGIN
                    INSERT INTO Teachers (FullName) VALUES (@FullName)
                END')");

        while (true)
        {
            Console.WriteLine("\n1. Групи");
            Console.WriteLine("2. Студенти");
            Console.WriteLine("3. Викладачі");
            Console.WriteLine("4. Предмети");
            Console.WriteLine("5. Подання викладачі/предмети");
            Console.WriteLine("0. Вихід");
            Console.Write("Вибір: ");
            string choice = Console.ReadLine();

            if (choice == "1") GroupsMenu();
            else if (choice == "2") StudentsMenu();
            else if (choice == "3") TeachersMenu();
            else if (choice == "4") SubjectsMenu();
            else if (choice == "5")
            {
                foreach (var v in db.Set<TeacherSubjectView>().FromSqlRaw("SELECT * FROM TeacherSubjectsView"))
                    Console.WriteLine($"{v.TeacherName} | {v.SubjectName}");
            }
            else if (choice == "0") return;
        }
    }

    static void GroupsMenu()
    {
        Console.WriteLine("\n1. Додати 2. Показати 3. Оновити 4. Видалити");
        Console.Write("Вибір: ");
        string c = Console.ReadLine();

        if (c == "1")
        {
            Console.Write("Назва: "); string name = Console.ReadLine();
            db.Groups.Add(new Group { Name = name });
            db.SaveChanges();
        }
        else if (c == "2")
        {
            foreach (var g in db.Groups)
                Console.WriteLine($"{g.Id} | {g.Name}");
        }
        else if (c == "3")
        {
            Console.Write("ID: "); int id = int.Parse(Console.ReadLine());
            var g = db.Groups.Find(id);
            Console.Write("Нова назва: "); g.Name = Console.ReadLine();
            db.SaveChanges();
        }
        else if (c == "4")
        {
            Console.Write("ID: "); int id = int.Parse(Console.ReadLine());
            var g = db.Groups.Find(id);
            db.Groups.Remove(g);
            db.SaveChanges();
        }
    }

    static void StudentsMenu()
    {
        Console.WriteLine("\n1. Додати 2. Показати 3. Оновити 4. Видалити");
        Console.Write("Вибір: ");
        string c = Console.ReadLine();

        if (c == "1")
        {
            Console.Write("ПІБ: "); string name = Console.ReadLine();
            Console.Write("ID групи: "); int groupId = int.Parse(Console.ReadLine());
            db.Students.Add(new Student { FullName = name, GroupId = groupId });
            db.SaveChanges();
        }
        else if (c == "2")
        {
            foreach (var s in db.Students.Include(s => s.Group))
                Console.WriteLine($"{s.Id} | {s.FullName} | {s.Group.Name}");
        }
        else if (c == "3")
        {
            Console.Write("ID: "); int id = int.Parse(Console.ReadLine());
            var s = db.Students.Find(id);
            Console.Write("Нове ПІБ: "); s.FullName = Console.ReadLine();
            db.SaveChanges();
        }
        else if (c == "4")
        {
            Console.Write("ID: "); int id = int.Parse(Console.ReadLine());
            var s = db.Students.Find(id);
            db.Students.Remove(s);
            db.SaveChanges();
        }
    }

    static void TeachersMenu()
    {
        Console.WriteLine("\n1. Додати 2. Показати 3. Оновити 4. Видалити");
        Console.Write("Вибір: ");
        string c = Console.ReadLine();

        if (c == "1")
        {
            Console.Write("ПІБ: "); string name = Console.ReadLine();
            db.Database.ExecuteSqlRaw("EXEC AddTeacher @p0", name);
        }
        else if (c == "2")
        {
            foreach (var t in db.Teachers)
                Console.WriteLine($"{t.Id} | {t.FullName}");
        }
        else if (c == "3")
        {
            Console.Write("ID: "); int id = int.Parse(Console.ReadLine());
            var t = db.Teachers.Find(id);
            Console.Write("Нове ПІБ: "); t.FullName = Console.ReadLine();
            db.SaveChanges();
        }
        else if (c == "4")
        {
            Console.Write("ID: "); int id = int.Parse(Console.ReadLine());
            var t = db.Teachers.Find(id);
            db.Teachers.Remove(t);
            db.SaveChanges();
        }
    }

    static void SubjectsMenu()
    {
        Console.WriteLine("\n1. Додати 2. Показати 3. Оновити 4. Видалити");
        Console.Write("Вибір: ");
        string c = Console.ReadLine();

        if (c == "1")
        {
            Console.Write("Назва: "); string name = Console.ReadLine();
            Console.Write("ID викладача: "); int teacherId = int.Parse(Console.ReadLine());
            db.Subjects.Add(new Subject { Name = name, TeacherId = teacherId });
            db.SaveChanges();
        }
        else if (c == "2")
        {
            foreach (var s in db.Subjects.Include(s => s.Teacher))
                Console.WriteLine($"{s.Id} | {s.Name} | {s.Teacher.FullName}");
        }
        else if (c == "3")
        {
            Console.Write("ID: "); int id = int.Parse(Console.ReadLine());
            var s = db.Subjects.Find(id);
            Console.Write("Нова назва: "); s.Name = Console.ReadLine();
            db.SaveChanges();
        }
        else if (c == "4")
        {
            Console.Write("ID: "); int id = int.Parse(Console.ReadLine());
            var s = db.Subjects.Find(id);
            db.Subjects.Remove(s);
            db.SaveChanges();
        }
    }
}
