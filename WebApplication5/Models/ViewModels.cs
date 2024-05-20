using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApplication5.Models
{
    public class FacultyClassViewModel
    {
        public Class Class { get; set; }
        public int NumberOfUsersEnrolled { get; set; }
    }

    public class GradeTestViewModel
    {
        public long CSR_id { get; set; }
        public long Class_ID { get; set; }
        public string CourseName { get; set; }
        public List<StudentGradeViewModel> Students { get; set; }
    }

    public class StudentGradeViewModel
    {
        public long Student_ID { get; set; }
        public string StudentName { get; set; }
        public long Marks { get; set; }
    }

    public class ViewTestResultViewModel
    {
        public long Class_ID { get; set; }
        public string CourseName { get; set; }
        public List<StudentGradeViewModelForFaculty> Students { get; set; } // Change the type here
    }

    public class StudentGradeViewModelForFaculty
    {
        public long Student_ID { get; set; }
        public string StudentName { get; set; }
        public long Marks { get; set; }
    }

    public class RegisterClassesViewModel
    {
        public List<AvailableClassViewModel> AvailableClasses { get; set; }
        public List<SelectedClassViewModel> SelectedClasses { get; set; }
        public List<long> SelectedClassIds { get; set; }

        public RegisterClassesViewModel()
        {
            AvailableClasses = new List<AvailableClassViewModel>();
            SelectedClasses = new List<SelectedClassViewModel>();
            SelectedClassIds = new List<long>();
        }
    }

    public class AvailableClassViewModel
    {
        public long ClassId { get; set; }
        public string CourseName { get; set; }
        public string ClassDay { get; set; }
        public string ClassSchedule { get; set; }
        public bool Selected { get; set; }
    }

    public class SelectedClassViewModel
    {
        public long ClassId { get; set; }
        public string CourseName { get; set; }
        public string ClassDay { get; set; }
        public string ClassSchedule { get; set; }
        public bool Selected { get; set; } // Add Selected property
    }

    public class EnrolledCourseViewModel
    {
        public long ClassId { get; set; }
        public string ClassName { get; set; }
        public string ClassDay { get; set; }
        public string ClassSession { get; set; }
        // Include other properties as needed
    }

    public class TestViewModel
    {
        public long ClassId { get; set; }
        public string CourseName { get; set; }
        public string TestType { get; set; }
        public string TestDate { get; set; }
        public string RemainingTime { get; set; }
        public int MaxMarks { get; set; }
    }

    public class MarkViewModel
    {
        public string ClassName { get; set; }
        public long Marks { get; set; }
    }

    public class GradeViewModel
    {
        public string ClassName { get; set; }
        public long Marks { get; set; }
        public string Grade { get; set; }
        public double CGPA { get; set; }
    }

    public class GradeHistoryViewModel
    {
        public string ClassName { get; set; }
        public long Marks { get; set; }
        public string Grade { get; set; }
        public double CGPA { get; set; }
    }
    public class ClassTestViewModel
    {
        public long CT_id { get; set; }
        public string CourseName { get; set; }
        public string ClassSession { get; set; }
        public string TestTypeName { get; set; }
        public string TestDate { get; set; }
        public long MaxMarks { get; set; }
    }

}