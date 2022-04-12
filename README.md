# NEG, Naive Equality Generator
Simple Source generator for generating IEquality for classes. Will probably not work for certain cases.

## How to use
Use [Neg.EqualityGenerator.Equality] over any partial class

## Strategy
- Compare public properties by running Property.Equals(other.Property)
- Compare public enumerable properties by running Property.SequenceEquals(other.Property)

## Generated Methods:
- bool Equals(object obj)  
- bool Equals(ClassType other)  
- == and != overrides
- int GetHashCode()

