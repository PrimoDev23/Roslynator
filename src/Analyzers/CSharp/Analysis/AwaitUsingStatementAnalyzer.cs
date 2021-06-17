using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using Roslynator.CSharp.Analysis;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Text;

namespace Roslynator.CSharp.CSharp.Analysis
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class AwaitUsingStatementAnalyzer : BaseDiagnosticAnalyzer
    {
        private static ImmutableArray<DiagnosticDescriptor> _supportedDiagnostics;

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics
        {
            get
            {
                if (_supportedDiagnostics.IsDefault)
                {
                    Immutable.InterlockedInitialize(
                        ref _supportedDiagnostics,
                        DiagnosticRules.AwaitUsingStatement);
                }

                return _supportedDiagnostics;
            }
        }

        public override void Initialize(AnalysisContext context)
        {
            base.Initialize(context);

            context.RegisterSyntaxNodeAction(f => CheckUsing(f), SyntaxKind.UsingStatement);
        }

        private static void CheckUsing(SyntaxNodeAnalysisContext context)
        {
            UsingStatementSyntax usingStatement = (UsingStatementSyntax)context.Node;

            var keyword = usingStatement.AwaitKeyword;
            ITypeSymbol declaredSymbol = context.SemanticModel.GetTypeSymbol(usingStatement.Declaration.Type, context.CancellationToken);
            var asyncSymbol = context.SemanticModel.GetTypeByMetadataName("System.IAsyncDisposable");
            var inherits = declaredSymbol.Interfaces.Contains(asyncSymbol);
            if (inherits && keyword.IsKind(SyntaxKind.None))
            {
                DiagnosticHelpers.ReportDiagnostic(context, DiagnosticRules.AwaitUsingStatement, usingStatement.UsingKeyword);
            }
        }
    }
}
