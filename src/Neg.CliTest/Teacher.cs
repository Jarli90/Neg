
using Neg.EqualityGenerator;

namespace Neg.CliTest
{
    [Equality]
    public partial class Teacher
    {
        public string? Name { get; set; }

        public List<Student> Students { get;} = new List<Student>();

        public void AddStudent(Student student)
        {
            Students.Add(student);
        }
    }
}
