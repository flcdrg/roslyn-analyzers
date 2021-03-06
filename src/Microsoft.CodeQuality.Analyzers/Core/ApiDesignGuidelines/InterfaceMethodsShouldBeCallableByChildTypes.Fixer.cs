﻿// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

using System;
using System.Collections.Immutable;
using System.Composition;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Editing;
using System.Collections.Generic;
using Microsoft.CodeAnalysis;
using Analyzer.Utilities;
using Analyzer.Utilities.Extensions;

namespace Microsoft.ApiDesignGuidelines.Analyzers
{
    /// <summary>
    /// CA1033: Interface methods should be callable by child types
    /// </summary>
    [ExportCodeFixProvider(LanguageNames.CSharp, LanguageNames.VisualBasic), Shared]
    public sealed class InterfaceMethodsShouldBeCallableByChildTypesFixer : CodeFixProvider
    {
        public sealed override ImmutableArray<string> FixableDiagnosticIds => ImmutableArray.Create(InterfaceMethodsShouldBeCallableByChildTypesAnalyzer.RuleId);

        public override FixAllProvider GetFixAllProvider()
        {
            return WellKnownFixAllProviders.BatchFixer;
        }

        public async override Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            SemanticModel semanticModel = await context.Document.GetSemanticModelAsync(context.CancellationToken).ConfigureAwait(false);
            SyntaxNode root = await context.Document.GetSyntaxRootAsync(context.CancellationToken).ConfigureAwait(false);
            SyntaxNode nodeToFix = root.FindNode(context.Span);
            if (nodeToFix == null)
            {
                return;
            }

            var methodSymbol = semanticModel.GetDeclaredSymbol(nodeToFix, context.CancellationToken) as IMethodSymbol;
            if (methodSymbol == null)
            {
                return;
            }

            SyntaxGenerator generator = SyntaxGenerator.GetGenerator(context.Document);
            SyntaxNode declaration = generator.GetDeclaration(nodeToFix);
            if (declaration == null)
            {
                return;
            }

            IMethodSymbol candidateToIncreaseVisibility = GetExistingNonVisibleAlternate(methodSymbol);
            if (candidateToIncreaseVisibility != null)
            {
                ISymbol symbolToChange = candidateToIncreaseVisibility.IsAccessorMethod() ? candidateToIncreaseVisibility.AssociatedSymbol : candidateToIncreaseVisibility;
                if (symbolToChange != null)
                {
                    string title = string.Format(MicrosoftApiDesignGuidelinesAnalyzersResources.InterfaceMethodsShouldBeCallableByChildTypesFix1, symbolToChange.Name);

                    context.RegisterCodeFix(new MyCodeAction(title,
                         async ct => await MakeProtected(context.Document, symbolToChange, ct).ConfigureAwait(false),
                         equivalenceKey: MicrosoftApiDesignGuidelinesAnalyzersResources.InterfaceMethodsShouldBeCallableByChildTypesFix1),
                    context.Diagnostics);
                }
            }
            else
            {
                ISymbol symbolToChange = methodSymbol.IsAccessorMethod() ? methodSymbol.AssociatedSymbol : methodSymbol;
                if (symbolToChange != null)
                {
                    string title = string.Format(MicrosoftApiDesignGuidelinesAnalyzersResources.InterfaceMethodsShouldBeCallableByChildTypesFix2, symbolToChange.Name);

                    context.RegisterCodeFix(new MyCodeAction(title,
                         async ct => await ChangeToPublicInterfaceImplementation(context.Document, symbolToChange, ct).ConfigureAwait(false),
                         equivalenceKey: MicrosoftApiDesignGuidelinesAnalyzersResources.InterfaceMethodsShouldBeCallableByChildTypesFix2),
                    context.Diagnostics);
                }
            }

            context.RegisterCodeFix(new MyCodeAction(string.Format(MicrosoftApiDesignGuidelinesAnalyzersResources.InterfaceMethodsShouldBeCallableByChildTypesFix3, methodSymbol.ContainingType.Name),
                     async ct => await MakeContainingTypeSealed(context.Document, methodSymbol, ct).ConfigureAwait(false),
                         equivalenceKey: MicrosoftApiDesignGuidelinesAnalyzersResources.InterfaceMethodsShouldBeCallableByChildTypesFix3),
                context.Diagnostics);
        }

        private static IMethodSymbol GetExistingNonVisibleAlternate(IMethodSymbol methodSymbol)
        {
            foreach (IMethodSymbol interfaceMethod in methodSymbol.ExplicitInterfaceImplementations)
            {
                foreach (INamedTypeSymbol type in methodSymbol.ContainingType.GetBaseTypesAndThis())
                {
                    IMethodSymbol candidate = type.GetMembers(interfaceMethod.Name).OfType<IMethodSymbol>().FirstOrDefault(m => !m.Equals(methodSymbol));
                    if (candidate != null)
                    {
                        return candidate;
                    }
                }
            }

            return null;
        }

        private async Task<Document> MakeProtected(Document document, ISymbol symbolToChange, CancellationToken cancellationToken)
        {
            SymbolEditor editor = SymbolEditor.Create(document);

            await editor.EditAllDeclarationsAsync(symbolToChange, (docEditor, declaration) =>
            {
                docEditor.SetAccessibility(declaration, Accessibility.Protected);
            }, cancellationToken).ConfigureAwait(false);

            return editor.GetChangedDocuments().First();
        }

        private async Task<Document> ChangeToPublicInterfaceImplementation(Document document, ISymbol symbolToChange, CancellationToken cancellationToken)
        {
            SymbolEditor editor = SymbolEditor.Create(document);

            IEnumerable<ISymbol> explicitImplementations = GetExplicitImplementations(symbolToChange);
            if (explicitImplementations == null)
            {
                return document;
            }

            await editor.EditAllDeclarationsAsync(symbolToChange, (docEditor, declaration) =>
            {
                SyntaxNode newDeclaration = declaration;
                foreach (ISymbol implementedMember in explicitImplementations)
                {
                    SyntaxNode interfaceTypeNode = docEditor.Generator.TypeExpression(implementedMember.ContainingType);
                    newDeclaration = docEditor.Generator.AsPublicInterfaceImplementation(newDeclaration, interfaceTypeNode);
                }

                docEditor.ReplaceNode(declaration, newDeclaration);
            }, cancellationToken).ConfigureAwait(false);

            return editor.GetChangedDocuments().First();
        }

        private static IEnumerable<ISymbol> GetExplicitImplementations(ISymbol symbol)
        {
            if (symbol == null)
            {
                return null;
            }

            switch (symbol.Kind)
            {
                case SymbolKind.Method:
                    return ((IMethodSymbol)symbol).ExplicitInterfaceImplementations;

                case SymbolKind.Event:
                    return ((IEventSymbol)symbol).ExplicitInterfaceImplementations;

                case SymbolKind.Property:
                    return ((IPropertySymbol)symbol).ExplicitInterfaceImplementations;

                default:
                    return null;
            }
        }

        private async Task<Document> MakeContainingTypeSealed(Document document, IMethodSymbol methodSymbol, CancellationToken cancellationToken)
        {
            SymbolEditor editor = SymbolEditor.Create(document);

            await editor.EditAllDeclarationsAsync(methodSymbol.ContainingType, (docEditor, declaration) =>
            {
                DeclarationModifiers modifiers = docEditor.Generator.GetModifiers(declaration);
                docEditor.SetModifiers(declaration, modifiers + DeclarationModifiers.Sealed);
            }, cancellationToken).ConfigureAwait(false);

            return editor.GetChangedDocuments().First();
        }

        private class MyCodeAction : DocumentChangeAction
        {
            private readonly string _equivalenceKey;

            public MyCodeAction(string title, Func<CancellationToken, Task<Document>> createChangedDocument, string equivalenceKey)
                : base(title, createChangedDocument)
            {
                _equivalenceKey = equivalenceKey;
            }

            public override string EquivalenceKey => _equivalenceKey;
        }
    }
}
