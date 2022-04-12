using Microsoft.CodeAnalysis;
using System;
using System.Collections.Generic;
using System.Text;

namespace Neg.SourceGenerator
{
    internal static class IsPartialDiagnosticDescriptor
    {
        public static readonly DiagnosticDescriptor ClassMustBePartial
              = new("SG001",                            // id
                    "Class must be partial",           // title
                    "The class '{0}' must be partial", // message
                    "NegAnalyzer",                     // category
                    DiagnosticSeverity.Error,
                    true);
    }
}
