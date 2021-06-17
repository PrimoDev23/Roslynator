// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.IO;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Roslynator.CSharp.CodeFixes;
using Roslynator.CSharp.CSharp.Analysis;
using Roslynator.Testing;
using Roslynator.Testing.CSharp;
using Xunit;

namespace Roslynator.CSharp.Analysis.Tests
{
    public class RCS1249AwaitUsingStatementTest : AbstractCSharpDiagnosticVerifier<AwaitUsingStatementAnalyzer, DummyCodeFixProvider>
    {
        public override DiagnosticDescriptor Descriptor { get; } = DiagnosticRules.AwaitUsingStatement;

        [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.AwaitUsingStatement)]
        public async Task Test_AwaitedUsing()
        {
            await VerifyDiagnosticAsync(@"
using System.IO;

class C
{
    void M()
    {
        [|using|] (Stream fs = new FileStream(null, FileMode.Open)) {
        }
    }
}
");
        }
    }
}
