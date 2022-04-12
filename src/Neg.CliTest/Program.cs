// See https://aka.ms/new-console-template for more information


using Neg.CliTest;

Console.WriteLine("Hello, World!");


Student student1 = new Student();
student1.Name = "Tom";
student1.Age = 18;
student1.Enlisted = true;

Student student2 = new Student();
student2.Name = "Tom";
student2.Age = 18;
student2.Enlisted = true;

Teacher teacher1 = new Teacher();
teacher1.Name = "Bob";
teacher1.AddStudent(student1);

Teacher teacher2 = new Teacher();
teacher2.Name = "Bob";
teacher2.AddStudent(student2);

bool teachersAreEqual = teacher1 == teacher2;

Console.WriteLine($"Teachers are equal: {teachersAreEqual}");

Console.WriteLine("Goodbye, World!");