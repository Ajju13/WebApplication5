namespace WebApplication5.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class check : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.ClassEnrolments",
                c => new
                    {
                        ClassEnrolmentId = c.Long(nullable: false, identity: true),
                        ClassId = c.Long(nullable: false),
                        StudentId = c.Long(nullable: false),
                    })
                .PrimaryKey(t => t.ClassEnrolmentId)
                .ForeignKey("dbo.Classes", t => t.ClassId, cascadeDelete: true)
                .ForeignKey("dbo.Students", t => t.StudentId, cascadeDelete: true)
                .Index(t => t.ClassId)
                .Index(t => t.StudentId);
            
            CreateTable(
                "dbo.Classes",
                c => new
                    {
                        ClassId = c.Long(nullable: false, identity: true),
                        CourseId = c.Long(nullable: false),
                        FacultyId = c.Long(nullable: false),
                        ClassDay = c.String(),
                        ClassSession = c.Long(nullable: false),
                    })
                .PrimaryKey(t => t.ClassId)
                .ForeignKey("dbo.Faculties", t => t.FacultyId, cascadeDelete: true)
                .ForeignKey("dbo.Subjects", t => t.CourseId, cascadeDelete: true)
                .Index(t => t.CourseId)
                .Index(t => t.FacultyId);
            
            CreateTable(
                "dbo.Class_StdResult",
                c => new
                    {
                        CSRId = c.Long(nullable: false, identity: true),
                        ClassId = c.Long(nullable: false),
                        StudentId = c.Long(nullable: false),
                        Marks = c.Long(nullable: false),
                    })
                .PrimaryKey(t => t.CSRId)
                .ForeignKey("dbo.Classes", t => t.ClassId, cascadeDelete: true)
                .ForeignKey("dbo.Students", t => t.StudentId, cascadeDelete: true)
                .Index(t => t.ClassId)
                .Index(t => t.StudentId);
            
            CreateTable(
                "dbo.Students",
                c => new
                    {
                        StudentId = c.Long(nullable: false, identity: true),
                        StudentEmail = c.String(nullable: false),
                        StudentPhone = c.String(nullable: false),
                        StudentName = c.String(nullable: false),
                        StudentAddress = c.String(),
                        StudentImage = c.String(),
                    })
                .PrimaryKey(t => t.StudentId);
            
            CreateTable(
                "dbo.Class_Test",
                c => new
                    {
                        CTId = c.Long(nullable: false, identity: true),
                        ClassId = c.Long(nullable: false),
                        TestTypeId = c.Long(nullable: false),
                        TestDate = c.String(),
                        MaxMarks = c.Long(nullable: false),
                    })
                .PrimaryKey(t => t.CTId)
                .ForeignKey("dbo.Classes", t => t.ClassId, cascadeDelete: true)
                .ForeignKey("dbo.TestTypes", t => t.TestTypeId, cascadeDelete: true)
                .Index(t => t.ClassId)
                .Index(t => t.TestTypeId);
            
            CreateTable(
                "dbo.TestTypes",
                c => new
                    {
                        TestTypeId = c.Long(nullable: false, identity: true),
                        TestTypeName = c.String(nullable: false),
                    })
                .PrimaryKey(t => t.TestTypeId);
            
            CreateTable(
                "dbo.Faculties",
                c => new
                    {
                        FacultyId = c.Long(nullable: false, identity: true),
                        FacultyEmail = c.String(nullable: false),
                        FacultyPhone = c.String(nullable: false),
                        FacultyName = c.String(nullable: false),
                        FacultyAddress = c.String(),
                        FacultyImage = c.String(),
                    })
                .PrimaryKey(t => t.FacultyId);
            
            CreateTable(
                "dbo.Subjects",
                c => new
                    {
                        CourseId = c.Long(nullable: false, identity: true),
                        CourseName = c.String(nullable: false),
                        PreRequisiteCourseId = c.Long(),
                        CreditHours = c.Long(nullable: false),
                    })
                .PrimaryKey(t => t.CourseId)
                .ForeignKey("dbo.Subjects", t => t.PreRequisiteCourseId)
                .Index(t => t.PreRequisiteCourseId);
            
            CreateTable(
                "dbo.Grades",
                c => new
                    {
                        GradeId = c.Long(nullable: false, identity: true),
                        GradeValue = c.Long(nullable: false),
                        MinMarks = c.Long(nullable: false),
                        MaxMarks = c.Long(nullable: false),
                    })
                .PrimaryKey(t => t.GradeId);
            
            CreateTable(
                "dbo.Staffs",
                c => new
                    {
                        StaffId = c.Long(nullable: false, identity: true),
                        StaffEmail = c.String(nullable: false),
                        StaffPhone = c.String(nullable: false),
                        StaffName = c.String(nullable: false),
                        StaffAddress = c.String(),
                        StaffImage = c.String(),
                    })
                .PrimaryKey(t => t.StaffId);
            
            CreateTable(
                "dbo.Users",
                c => new
                    {
                        UserId = c.Long(nullable: false, identity: true),
                        Email = c.String(nullable: false),
                        Password = c.String(nullable: false),
                        UserType = c.String(nullable: false),
                    })
                .PrimaryKey(t => t.UserId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.ClassEnrolments", "StudentId", "dbo.Students");
            DropForeignKey("dbo.ClassEnrolments", "ClassId", "dbo.Classes");
            DropForeignKey("dbo.Classes", "CourseId", "dbo.Subjects");
            DropForeignKey("dbo.Subjects", "PreRequisiteCourseId", "dbo.Subjects");
            DropForeignKey("dbo.Classes", "FacultyId", "dbo.Faculties");
            DropForeignKey("dbo.Class_Test", "TestTypeId", "dbo.TestTypes");
            DropForeignKey("dbo.Class_Test", "ClassId", "dbo.Classes");
            DropForeignKey("dbo.Class_StdResult", "StudentId", "dbo.Students");
            DropForeignKey("dbo.Class_StdResult", "ClassId", "dbo.Classes");
            DropIndex("dbo.Subjects", new[] { "PreRequisiteCourseId" });
            DropIndex("dbo.Class_Test", new[] { "TestTypeId" });
            DropIndex("dbo.Class_Test", new[] { "ClassId" });
            DropIndex("dbo.Class_StdResult", new[] { "StudentId" });
            DropIndex("dbo.Class_StdResult", new[] { "ClassId" });
            DropIndex("dbo.Classes", new[] { "FacultyId" });
            DropIndex("dbo.Classes", new[] { "CourseId" });
            DropIndex("dbo.ClassEnrolments", new[] { "StudentId" });
            DropIndex("dbo.ClassEnrolments", new[] { "ClassId" });
            DropTable("dbo.Users");
            DropTable("dbo.Staffs");
            DropTable("dbo.Grades");
            DropTable("dbo.Subjects");
            DropTable("dbo.Faculties");
            DropTable("dbo.TestTypes");
            DropTable("dbo.Class_Test");
            DropTable("dbo.Students");
            DropTable("dbo.Class_StdResult");
            DropTable("dbo.Classes");
            DropTable("dbo.ClassEnrolments");
        }
    }
}
