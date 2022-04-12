using Microsoft.CodeAnalysis;

namespace Neg.SourceGenerator
{
    internal struct EqualityToGenerate
    {
        public EqualityToGenerate(
            string @namespace,
            Accessibility accessibility,
            string className, 
            List<PropertyData> properties
            )
        {
            Namespace = @namespace;
            ClassName = className;
            Properties = properties;
            Accessibility = accessibility;
        }

        public string Namespace { get; }
        public string ClassName { get;  }

        public Accessibility Accessibility { get; }
        public List<PropertyData> Properties { get;  }
    }

    internal struct PropertyData
    {
        public string Name { get; set; }
        public bool Nullable { get; set; }
        public bool Enumerable { get; set; }
    }
}
