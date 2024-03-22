namespace Wix_Technical_Test.QueryLanguage.Lexer
{
    public class SyntaxToken
    {
        public SyntaxKind Type { get; }
        public string Text { get; }
        public int Position { get; }
        public object Value { get; }


        public SyntaxToken(SyntaxKind type, int position, string text, object value)
        {
            Text = text;
            Type = type;
            Position = position;
            Value = value;
        }
    }
}
