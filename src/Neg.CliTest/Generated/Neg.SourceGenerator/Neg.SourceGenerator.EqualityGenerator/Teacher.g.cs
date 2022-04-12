
namespace Neg.CliTest
{
    public partial class Teacher : System.IEquatable<Teacher>
    {
        public bool Equals(Teacher other)
        {
            if(Object.ReferenceEquals(this,other)) return true;
            
            if(!Name.Equals(other.Name)) return false;

            
            if(!Students.SequenceEqual(other.Students)) return false;

            return true;
        }

        public override bool Equals(object obj)  
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType())  
                return false;  
  
            return this.Equals(obj as Teacher);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int result = 0; 
                
                result = (result * 397) ^ (Name != null ? Name.GetHashCode() : 0);

                

                
                foreach(var item in Students)
                {
                    result = (result * 397) ^ item.GetHashCode();
                }

                return result;
            }
        }
        

        public static bool operator ==(Teacher lhs, Teacher rhs) 
        {
            if(lhs is null && rhs is null) return true;

            return lhs?.Equals(rhs) ?? false;
        }

        public static bool operator !=(Teacher lhs, Teacher rhs) 
        {
            return !(lhs == rhs);
        }

        
    }
}
