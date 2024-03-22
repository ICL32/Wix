namespace Wix_Technical_Test.QueryLanguage.Lexer
{
    public class SyntaxToken
    {
        public SyntaxKind Kind { get; }
        public string Text { get; }
        public int Position { get; }
        public object Value { get; }


        public SyntaxToken(SyntaxKind kind, int position, string text, object value)
        {
            Text = text;
            Kind = kind;
            Position = position;
            Value = value;
        }
    }
}
