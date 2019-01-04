using System.Collections.Generic;

namespace Synthesis
{
    using System;
    using System.Linq;

    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;

    internal sealed class Rewriter : CSharpSyntaxRewriter
    {
        public override SyntaxNode VisitExpressionStatement(ExpressionStatementSyntax node)

        {

            var invocationName = node.ChildNodes().OfType<InvocationExpressionSyntax>().FirstOrDefault()?.ChildNodes().OfType<IdentifierNameSyntax>().FirstOrDefault()?
                .ChildTokens()?.FirstOrDefault().Text;
            if (invocationName == null)
            {
                invocationName = node.ChildNodes().OfType<InvocationExpressionSyntax>().FirstOrDefault()?
                    .ChildNodes().OfType<GenericNameSyntax>().FirstOrDefault()?
                    .ChildTokens()?.FirstOrDefault().Text;
            } 
            if (invocationName!=null && invocationName.Equals("YIELD",StringComparison.OrdinalIgnoreCase))
            {
                var genericName = node.ChildNodes().OfType<InvocationExpressionSyntax>().FirstOrDefault()?
                    .ChildNodes().OfType<GenericNameSyntax>().FirstOrDefault()?
                    .ChildNodes().OfType<TypeArgumentListSyntax>().FirstOrDefault()?
                    .ChildNodes().OfType<PredefinedTypeSyntax>().FirstOrDefault()?.Keyword.Text;
                var varName = node.ChildNodes().OfType<InvocationExpressionSyntax>().FirstOrDefault()?
                    .ChildNodes().OfType<ArgumentListSyntax>().FirstOrDefault()?
                    .ChildNodes().OfType<ArgumentSyntax>().FirstOrDefault()?
                    .ChildNodes().OfType<IdentifierNameSyntax>().FirstOrDefault()?.GetText().ToString();
                String type = "var";
                if (genericName != null)
                {
                   type = genericName;
                }

                var leadingTrivia = node.GetLeadingTrivia();
                var forEachExp = SyntaxFactory.ForEachStatement(
                        SyntaxFactory.Token(SyntaxKind.ForEachKeyword),
                        SyntaxFactory.Token(SyntaxKind.OpenParenToken),
                        SyntaxFactory.IdentifierName(type),
                        SyntaxFactory.Identifier("item"),
                        SyntaxFactory.Token(SyntaxKind.InKeyword),
                        SyntaxFactory.IdentifierName(varName),
                        SyntaxFactory.Token(SyntaxKind.CloseParenToken)
                            .WithTrailingTrivia(SyntaxFactory.EndOfLine(Environment.NewLine))
                            .WithTrailingTrivia(leadingTrivia),
                        SyntaxFactory.Block(
                            SyntaxFactory.YieldStatement(
                                SyntaxKind.YieldReturnStatement,
                                SyntaxFactory.IdentifierName("item").WithLeadingTrivia(leadingTrivia)
                            )
                        )).NormalizeWhitespace().WithLeadingTrivia(leadingTrivia)
                    .WithTrailingTrivia(SyntaxFactory.EndOfLine(Environment.NewLine));
                return forEachExp;
            }
            return base.VisitExpressionStatement(node);
        }
        
    }
}
