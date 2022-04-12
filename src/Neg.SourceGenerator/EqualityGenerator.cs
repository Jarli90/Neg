using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;
using System.Collections.Immutable;
using System.Text;

namespace Neg.SourceGenerator
{
    [Generator]
    public class EqualityGenerator : IIncrementalGenerator
    {
        public void Initialize(IncrementalGeneratorInitializationContext context)
        {
            // Add the marker attribute to the compilation
            context.RegisterPostInitializationOutput(ctx => ctx.AddSource(
                "EqualityAttribute.g.cs",
                SourceText.From(SourceGeneratorTemplates.EqualityAttributeSource(), System.Text.Encoding.UTF8)));

            // Do a simple filter for enums
            IncrementalValuesProvider<ClassDeclarationSyntax> equalityDeclarations = context.SyntaxProvider
                .CreateSyntaxProvider(
                    predicate: static (s, _) => IsSyntaxTargetForGeneration(s),
                    transform: static (ctx, _) => GetSemanticTargetForGeneration(ctx))
                .Where(static m => m is not null)!;

            IncrementalValueProvider<(Compilation, ImmutableArray<ClassDeclarationSyntax>)> compilationAndClasses
                    = context.CompilationProvider.Combine(equalityDeclarations.Collect());

            context.RegisterSourceOutput(compilationAndClasses,
                static (spc, source) => Execute(source.Item1,source.Item2, spc));
        }

        static bool IsSyntaxTargetForGeneration(SyntaxNode node)
            => node is ClassDeclarationSyntax m && m.AttributeLists.Count > 0 && IsPartial(m);
        public static bool IsPartial(ClassDeclarationSyntax classDeclaration)
        {
            return classDeclaration.Modifiers.Any(m => m.IsKind(SyntaxKind.PartialKeyword));
        }
        public const string EqualityAttribute = "Neg.EqualityGenerator.EqualityAttribute";

        static ClassDeclarationSyntax? GetSemanticTargetForGeneration(GeneratorSyntaxContext context)
        {
            // we know the node is a ClassDeclarationSyntax thanks to IsSyntaxTargetForGeneration
            var classDeclarationSyntax = (ClassDeclarationSyntax)context.Node;

            // loop through all the attributes on the method
            foreach (AttributeListSyntax attributeListSyntax in classDeclarationSyntax.AttributeLists)
            {
                foreach (AttributeSyntax attributeSyntax in attributeListSyntax.Attributes)
                {
                    if (context.SemanticModel.GetSymbolInfo(attributeSyntax).Symbol is not IMethodSymbol attributeSymbol)
                    {
                        // weird, we couldn't get the symbol, ignore it
                        continue;
                    }

                    INamedTypeSymbol attributeContainingTypeSymbol = attributeSymbol.ContainingType;
                    string fullName = attributeContainingTypeSymbol.ToDisplayString();

                    // Is the attribute the [Equality] attribute?
                    if (fullName == EqualityAttribute)
                    {
                        // return the class
                        return classDeclarationSyntax;
                    }
                }
            }

            // we didn't find the attribute we were looking for
            return null;
        }

        static void Execute(Compilation compilation, ImmutableArray<ClassDeclarationSyntax> classes, SourceProductionContext context)
        {
            if (classes.IsDefaultOrEmpty)
            {
                return;
            }

            IEnumerable<ClassDeclarationSyntax> distinctClasses = classes.Distinct();

            // Convert each EnumDeclarationSyntax to an EnumToGenerate
            List<EqualityToGenerate> classesToGenerate = GetTypesToGenerate(compilation, distinctClasses, context.CancellationToken);

            // If there were errors in the EnumDeclarationSyntax, we won't create an
            // EnumToGenerate for it, so make sure we have something to generate
            if (classesToGenerate.Count > 0)
            {
                foreach(EqualityToGenerate @class in classesToGenerate)
                {
                    // generate the source code and add it to the output
                    string result = SourceGeneratorTemplates.EqualityClassSource(@class);
                    context.AddSource($"{@class.ClassName}.g.cs", SourceText.From(result, Encoding.UTF8));
                }
                
            }
        }

        static List<EqualityToGenerate> GetTypesToGenerate(Compilation compilation, IEnumerable<ClassDeclarationSyntax> classes, CancellationToken ct)
        {
            var classesToGenerate = new List<EqualityToGenerate>();

            // Get the semantic representation of our marker attribute 
            INamedTypeSymbol? equalityAttribute = compilation.GetTypeByMetadataName(EqualityAttribute);

            if (equalityAttribute == null)
            {
                return classesToGenerate;
            }

            foreach (ClassDeclarationSyntax classDeclarationSyntax in classes)
            {
                ct.ThrowIfCancellationRequested();

                // Get the semantic representation of the enum syntax
                SemanticModel semanticModel = compilation.GetSemanticModel(classDeclarationSyntax.SyntaxTree);
                if (semanticModel.GetDeclaredSymbol(classDeclarationSyntax) is not INamedTypeSymbol classSymbol)
                {
                    continue;
                }

                //if (!Debugger.IsAttached) Debugger.Launch();
                string className = classSymbol.Name;
                string @namespace = classSymbol.ContainingNamespace.OriginalDefinition.ToDisplayString();
                
                // Get all the members in the enum
                ImmutableArray<ISymbol> classMembers = classSymbol.GetMembers();
                var properties = new List<PropertyData>();
                var enumerableProperties = new List<string>();

                // Get all the properties from the class, and add their name to the list
                foreach (ISymbol member in classMembers)
                {
                    if (member is IPropertySymbol property 
                        && property.DeclaredAccessibility == Accessibility.Public)
                    {
                        var propertyType = property.Type;
                        var propertyData = new PropertyData
                        {
                            Name = property.Name,
                            Nullable = !propertyType.IsValueType || propertyType.NullableAnnotation == NullableAnnotation.Annotated,
                            Enumerable = property.Type.Name != "String" && property.Type.AllInterfaces.Any(i => i.Name.Contains("IEnumerable"))
                        };
                        properties.Add(propertyData);
                    }
                }

                // Create an EnumToGenerate for use in the generation phase
                classesToGenerate.Add(new EqualityToGenerate(@namespace, classSymbol.DeclaredAccessibility, className, properties));
            }

            return classesToGenerate;
        }
    }
}