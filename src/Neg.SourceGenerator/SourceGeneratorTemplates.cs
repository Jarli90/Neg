using System;
using System.Collections.Generic;
using System.Text;

namespace Neg.SourceGenerator
{
    internal static class SourceGeneratorTemplates
    {
        public static string EqualityAttributeSource()
        {
            return 
$@"
namespace Neg.EqualityGenerator
{{
    [System.AttributeUsage(System.AttributeTargets.Class)]
    public class EqualityAttribute : System.Attribute
    {{
    }}
}}
";
        }

        public static string EqualityClassSource(EqualityToGenerate equality)
        {
            return 
$@"
namespace {equality.Namespace}
{{
    {equality.Accessibility.ToString().ToLower()} partial class {equality.ClassName} : System.IEquatable<{equality.ClassName}>
    {{
        public bool Equals({equality.ClassName} other)
        {{
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            {string.Join("",equality.Properties.Where(p => !p.Enumerable).Select(property =>
$@"
            if(!{property.Name}.Equals(other.{property.Name})) return false;
"))}
            {string.Join("", equality.Properties.Where(p => p.Enumerable).Select(property =>
 $@"
            if(!{property.Name}.SequenceEqual(other.{property.Name})) return false;
"))}
            return true;
        }}

        public override bool Equals(object obj)  
        {{
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType())  
                return false;  
  
            return this.Equals(obj as {equality.ClassName});
        }}

        public override int GetHashCode()
        {{
            unchecked
            {{
                int result = 0; 
                {string.Join("", equality.Properties.Where(p => !p.Enumerable && p.Nullable).Select(property =>
$@"
                result = (result * 397) ^ ({property.Name} != null ? {property.Name}.GetHashCode() : 0);
"))}
                {string.Join("", equality.Properties.Where(p => !p.Enumerable && !p.Nullable).Select(property =>
$@"
                result = (result * 397) ^ {property.Name}.GetHashCode();
"))}

                {string.Join("", equality.Properties.Where(p => p.Enumerable).Select(property =>
$@"
                foreach(var item in {property.Name})
                {{
                    result = (result * 397) ^ item.GetHashCode();
                }}
"))}
                return result;
            }}
        }}

        public static bool operator ==({equality.ClassName} lhs, {equality.ClassName} rhs) 
        {{
            if(lhs is null && rhs is null) return true;

            return lhs?.Equals(rhs) ?? false;
        }}

        public static bool operator !=({equality.ClassName} lhs, {equality.ClassName} rhs) 
        {{
            return !(lhs == rhs);
        }}
    }}
}}
";
        }
    }
}
