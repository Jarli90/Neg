
namespace Neg.CliTest
{
    public partial class Student : System.IEquatable<Student>
    {
        public bool Equals(Student other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            
            if(!Age.Equals(other.Age)) return false;

            if(!Name.Equals(other.Name)) return false;

            if(!Enlisted.Equals(other.Enlisted)) return false;

            
            if(!Classes.SequenceEqual(other.Classes)) return false;

            return true;
        }

        public override bool Equals(object obj)  
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType())  
                return false;  
  
            return this.Equals(obj as Student);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int result = 0; 
                
                result = (result * 397) ^ (Name != null ? Name.GetHashCode() : 0);

                
                result = (result * 397) ^ Age.GetHashCode();

                result = (result * 397) ^ Enlisted.GetHashCode();


                
                foreach(var item in Classes)
                {
                    result = (result * 397) ^ item.GetHashCode();
                }

                return result;
            }
        }

        public static bool operator ==(Student lhs, Student rhs) 
        {
            if(lhs is null && rhs is null) return true;

            return lhs?.Equals(rhs) ?? false;
        }

        public static bool operator !=(Student lhs, Student rhs) 
        {
            return !(lhs == rhs);
        }
    }
}
