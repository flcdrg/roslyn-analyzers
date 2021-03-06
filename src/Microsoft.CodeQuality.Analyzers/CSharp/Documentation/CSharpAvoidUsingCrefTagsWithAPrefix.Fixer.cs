// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

using System.Composition;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using XmlDocumentationComments.Analyzers;

namespace XmlDocumentationComments.CSharp.Analyzers
{
    /// <summary>
    /// RS0010: Avoid using cref tags with a prefix
    /// </summary>
    [ExportCodeFixProvider(LanguageNames.CSharp), Shared]
    public class CSharpAvoidUsingCrefTagsWithAPrefixFixer : AvoidUsingCrefTagsWithAPrefixFixer
    {
    }
}