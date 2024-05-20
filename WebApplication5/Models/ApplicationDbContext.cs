using System.Data.Entity;

namespace WebApplication5.Models
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext() : base("name=DefaultConnection")
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Student> Students { get; set; }
        public DbSet<Faculty> Faculties { get; set; }
        public DbSet<Staff> Staffs { get; set; }
        public DbSet<Class> Classes { get; set; }
        public DbSet<ClassEnrolment> ClassEnrolments { get; set; }
        public DbSet<Class_StdResult> ClassStdResults { get; set; }
        public DbSet<Class_Test> ClassTests { get; set; }
        public DbSet<Subject> Courses { get; set; }
        public DbSet<TestType> TestTypes { get; set; }
        public DbSet<Grade> Grades { get; set; }
    }
}
