﻿// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading.Tasks;
using Roslynator.CSharp.Refactorings;
using Xunit;

#pragma warning disable RCS1090

namespace Roslynator.Refactorings.Tests
{
    public class RR0127InlineConstantValueTests : AbstractCSharpCodeRefactoringVerifier
    {
        public override string RefactoringId { get; } = RefactoringIdentifiers.InlineConstantValue;

        [Fact]
        public async Task TestCodeRefactoring_Field()
        {
            await VerifyRefactoringAsync(@"
namespace A.B
{
    class C
    {
        public const string K = @""x"";
        public const string K2 = K;
        public const string K3 = K2;
        public const string K4 = null;

        void M(string s)
        {
            s = [|K|];
            s = [|K3|];
            s = [|C.K|];
            s = [|A.B.C.K|];
            s = [|K4|];
        }
    }
}
", @"
namespace A.B
{
    class C
    {
        public const string K = @""x"";
        public const string K2 = K;
        public const string K3 = K2;
        public const string K4 = null;

        void M(string s)
        {
            s = @""x"";
            s = @""x"";
            s = @""x"";
            s = @""x"";
            s = null;
        }
    }
}
", RefactoringId);
        }

        [Fact]
        public async Task TestCodeRefactoring_Field2()
        {
            await VerifyRefactoringAsync(@"
class C
{
    public const bool KB = true;
    public const char KC = '\n';
    public const int KI = int.MaxValue;
    public const long KL = 1;

    void M(string s)
    {
        bool b = [|KB|];
        char c = [|KC|];
        int i = [|KI|];
        long l = [|KL|];
    }
}
", @"
class C
{
    public const bool KB = true;
    public const char KC = '\n';
    public const int KI = int.MaxValue;
    public const long KL = 1;

    void M(string s)
    {
        bool b = true;
        char c = '\n';
        int i = 2147483647;
        long l = 1;
    }
}
", RefactoringId);
        }

        [Fact]
        public async Task TestCodeRefactoring_Local()
        {
            await VerifyRefactoringAsync(@"
class C
{
    string M(string s)
    {
        const string k = @""x"";
        const string k2 = k;
        const string k3 = k2;

        s += [|k|];
        s += [|k3|];

        return k3;
    }
}
", @"
class C
{
    string M(string s)
    {
        const string k = @""x"";
        const string k2 = k;
        const string k3 = k2;

        s += @""x"";
        s += @""x"";

        return k3;
    }
}
", RefactoringId);
        }

        [Fact]
        public async Task TestNoCodeRefactoring()
        {
            await VerifyNoRefactoringAsync(@"
using System;

class C
{
    public readonly string F = null;

    void M(string s)
    {
        s = [|""x""|];
        s = [|""x"" + ""x""|];
        s = [|F|];
        s = [|string.Empty|];
        var options = [|StringSplitOptions.None|];
    }
}
", RefactoringId);
        }
    }
}