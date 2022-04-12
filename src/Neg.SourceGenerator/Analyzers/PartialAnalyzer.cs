using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Text;

namespace Neg.SourceGenerator.Analyzers
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    internal class PartialAnalyzer : DiagnosticAnalyzer
    {
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; }
           = ImmutableArray.Create(IsPartialDiagnosticDescriptor.ClassMustBePartial);

        public override void Initialize(AnalysisContext context)
        {
            context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
            context.EnableConcurrentExecution();

            context.RegisterSymbolAction(AnalyzeNamedType, SymbolKind.NamedType);
        }
        
        private static void AnalyzeNamedType(SymbolAnalysisContext context)
        {
            if (context.Symbol is not INamedTypeSymbol namedSymbol)
                return;

            var type = (INamedTypeSymbol)context.Symbol;

            foreach (var declaringSyntaxReference in type.DeclaringSyntaxReferences)
            {
                
                if (declaringSyntaxReference.GetSyntax() is not ClassDeclarationSyntax classDeclaration 
                    || IsPartial(classDeclaration))
                    continue;

                var error = Diagnostic.Create(IsPartialDiagnosticDescriptor.ClassMustBePartial,
                                              classDeclaration.Identifier.GetLocation(),
                                              type.Name);
                context.ReportDiagnostic(error);
            }
        }

        public static bool IsPartial(ClassDeclarationSyntax classDeclaration)
        {
            return classDeclaration.Modifiers.Any(m => m.IsKind(SyntaxKind.PartialKeyword));
        }
    }
}
