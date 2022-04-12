
using Neg.EqualityGenerator;

namespace Neg.CliTest
{
    [Equality]
    public partial class Student
    {
        public int Age { get; set; }
        public string? Name { get; set; }

        public bool Enlisted { get; set; }

        public List<string> Classes { get; set; } = new List<string>();
    }
}
