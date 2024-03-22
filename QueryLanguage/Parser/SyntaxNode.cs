using Wix_Technical_Test.QueryLanguage.Lexer;

namespace Wix_Technical_Test.QueryLanguage.Parser
{
    public abstract class SyntaxNode
    {
        public abstract SyntaxKind Kind { get; }
    }
}
