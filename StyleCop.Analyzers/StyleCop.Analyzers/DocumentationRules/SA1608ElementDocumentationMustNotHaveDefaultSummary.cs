﻿// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.DocumentationRules
{
    using System;
    using System.Collections.Immutable;
    using Helpers;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using Microsoft.CodeAnalysis.Diagnostics;

    /// <summary>
    /// The <c>&lt;summary&gt;</c> tag within an element's XML header documentation contains the default text generated
    /// by Visual Studio during the creation of the element.
    /// </summary>
    /// <remarks>
    /// <para>C# syntax provides a mechanism for inserting documentation for classes and elements directly into the
    /// code, through the use of XML documentation headers. For an introduction to these headers and a description of
    /// the header syntax, see the following article:
    /// <see href="http://msdn.microsoft.com/en-us/magazine/cc302121.aspx">XML Comments Let You Build Documentation
    /// Directly From Your Visual Studio .NET Source Files</see>.</para>
    ///
    /// <para>Visual Studio provides helper functionality for adding new elements such as classes to a project. Visual
    /// Studio will create a default documentation header for the new class and fill in this header with default
    /// documentation text.</para>
    ///
    /// <para>A violation of this rule occurs when the <c>&lt;summary&gt;</c> tag for a code element still contains the
    /// default documentation text generated by Visual Studio.</para>
    /// </remarks>
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    internal class SA1608ElementDocumentationMustNotHaveDefaultSummary : DiagnosticAnalyzer
    {
        /// <summary>
        /// The ID for diagnostics produced by the <see cref="SA1608ElementDocumentationMustNotHaveDefaultSummary"/>
        /// analyzer.
        /// </summary>
        public const string DiagnosticId = "SA1608";
        private const string Title = "Element documentation must not have default summary";
        private const string MessageFormat = "Element documentation must not have default summary";
        private const string Description = "The <summary> tag within an element's XML header documentation contains the default text generated by Visual Studio during the creation of the element.";
        private const string HelpLink = "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/blob/master/documentation/SA1608.md";

        private const string DefaultText = "Summary description for";

        private static readonly DiagnosticDescriptor Descriptor =
            new DiagnosticDescriptor(DiagnosticId, Title, MessageFormat, AnalyzerCategory.DocumentationRules, DiagnosticSeverity.Warning, AnalyzerConstants.EnabledByDefault, Description, HelpLink);

        private static readonly Action<CompilationStartAnalysisContext> CompilationStartAction = HandleCompilationStart;
        private static readonly Action<SyntaxNodeAnalysisContext> DocumentationAction = HandleDocumentation;

        /// <inheritdoc/>
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; } =
            ImmutableArray.Create(Descriptor);

        /// <inheritdoc/>
        public override void Initialize(AnalysisContext context)
        {
            context.RegisterCompilationStartAction(CompilationStartAction);
        }

        private static void HandleCompilationStart(CompilationStartAnalysisContext context)
        {
            context.RegisterSyntaxNodeActionHonorExclusions(
                DocumentationAction,
                SyntaxKind.SingleLineDocumentationCommentTrivia,
                SyntaxKind.MultiLineDocumentationCommentTrivia);
        }

        private static void HandleDocumentation(SyntaxNodeAnalysisContext context)
        {
            var documentationTrivia = context.Node as DocumentationCommentTriviaSyntax;

            if (documentationTrivia != null)
            {
                var summaryElement = documentationTrivia.Content.GetFirstXmlElement(XmlCommentHelper.SummaryXmlTag) as XmlElementSyntax;

                if (summaryElement != null)
                {
                    var textElement = summaryElement.Content.FirstOrDefault() as XmlTextSyntax;

                    if (textElement != null)
                    {
                        string text = XmlCommentHelper.GetText(textElement, true);

                        if (!string.IsNullOrEmpty(text))
                        {
                            if (text.TrimStart().StartsWith(DefaultText, StringComparison.Ordinal))
                            {
                                context.ReportDiagnostic(Diagnostic.Create(Descriptor, summaryElement.GetLocation()));
                            }
                        }
                    }
                }
            }
        }
    }
}
