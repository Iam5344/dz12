using Microsoft.EntityFrameworkCore;

public class Group
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public ICollection<Student> Students { get; set; } = new List<Student>();
}

public class Student
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public string Email { get; set; } = null!;
    public decimal Stipend { get; set; }
    public int GroupId { get; set; }
    public Group Group { get; set; } = null!;
    public Passport? Passport { get; set; }
}

public class Passport
{
    public int Id { get; set; }
    public string PassportNumber { get; set; } = null!;
    public int StudentId { get; set; }
    public Student Student { get; set; } = null!;
}

public class Teacher
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public decimal Salary { get; set; }
    public ICollection<Subject> Subjects { get; set; } = new List<Subject>();
}

public class Subject
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public string? Description { get; set; }
    public int TeacherId { get; set; }
    public Teacher Teacher { get; set; } = null!;
    public int DepartmentId { get; set; }
    public Department Department { get; set; } = null!;
}

public class Department
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public ICollection<Subject> Subjects { get; set; } = new List<Subject>();
}

public class AppContext : DbContext
{
    public DbSet<Group> Groups { get; set; }
    public DbSet<Student> Students { get; set; }
    public DbSet<Passport> Passports { get; set; }
    public DbSet<Teacher> Teachers { get; set; }
    public DbSet<Subject> Subjects { get; set; }
    public DbSet<Department> Departments { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder options)
    {
        options.UseSqlServer("Data Source=DESKTOP-8UTPR8Q\\IAM5344;Initial Catalog=AcademyDb;Integrated Security=True;Encrypt=False;");
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Group>().Property(g => g.Name).HasMaxLength(10).IsRequired();

        modelBuilder.Entity<Student>().Property(s => s.Name).HasMaxLength(50);
        modelBuilder.Entity<Student>().Property(s => s.Email).IsRequired();
        modelBuilder.Entity<Student>().HasIndex(s => s.Email).IsUnique();
        modelBuilder.Entity<Student>().Property(s => s.Stipend).HasColumnType("decimal(6,2)");
        modelBuilder.Entity<Student>().HasOne(s => s.Group).WithMany(g => g.Students).HasForeignKey(s => s.GroupId);
        modelBuilder.Entity<Student>().HasOne(s => s.Passport).WithOne(p => p.Student).HasForeignKey<Passport>(p => p.StudentId);

        modelBuilder.Entity<Passport>().Property(p => p.PassportNumber).HasMaxLength(9).IsRequired();

        modelBuilder.Entity<Teacher>().Property(t => t.Salary).HasColumnType("decimal(8,2)").HasDefaultValue(25000);
        modelBuilder.Entity<Teacher>().Property(t => t.Name).HasMaxLength(50).IsRequired();

        modelBuilder.Entity<Subject>().Property(s => s.Name).HasMaxLength(50).IsRequired();
        modelBuilder.Entity<Subject>().Property(s => s.Description).IsRequired(false);
        modelBuilder.Entity<Subject>().HasOne(s => s.Teacher).WithMany(t => t.Subjects).HasForeignKey(s => s.TeacherId);
        modelBuilder.Entity<Subject>().HasOne(s => s.Department).WithMany(d => d.Subjects).HasForeignKey(s => s.DepartmentId);

        modelBuilder.Entity<Department>().Property(d => d.Name).HasMaxLength(50).IsRequired();
    }
}

class Program
{
    static void Main(string[] args)
    {
        using var db = new AppContext();
        db.Database.EnsureCreated();
        var department = new Department { Name = "IT" };
        db.Departments.Add(department);
        db.SaveChanges();

        var teacher = new Teacher { Name = "Іван Іванов", Salary = 30000 };
        db.Teachers.Add(teacher);
        db.SaveChanges();

        var subject = new Subject { Name = "Math", TeacherId = teacher.Id, DepartmentId = department.Id };
        db.Subjects.Add(subject);
        db.SaveChanges();

        var group = new Group { Name = "Group-1" };
        db.Groups.Add(group);
        db.SaveChanges();

        var student = new Student { Name = "Петро Петров", Email = "petro@gmail.com", Stipend = 1500, GroupId = group.Id };
        db.Students.Add(student);
        db.SaveChanges();

        var passport = new Passport { PassportNumber = "123456789", StudentId = student.Id };
        db.Passports.Add(passport);
        db.SaveChanges();

        Console.WriteLine("Групи:"); 
        foreach (var g in db.Groups) Console.WriteLine($"{g.Id} | {g.Name}");

        Console.WriteLine("Студенти:");
        foreach (var s in db.Students) Console.WriteLine($"{s.Id} | {s.Name} | {s.Email}");

        Console.WriteLine("Викладачі:");
        foreach (var t in db.Teachers) Console.WriteLine($"{t.Id} | {t.Name} | {t.Salary}");

        Console.WriteLine("Кафедри:");
        foreach (var d in db.Departments) Console.WriteLine($"{d.Id} | {d.Name}");

        Console.WriteLine("Предмети:");
        foreach (var s in db.Subjects) Console.WriteLine($"{s.Id} | {s.Name}");
    }
}
