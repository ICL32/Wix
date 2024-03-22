namespace Wix_Technical_Test.QueryLanguage.Lexer
{
    public class Lexer
    {
        private readonly string _text;
        private int _position;

        public Lexer(string text)
        {
            _text = text;
        }

        private char Current
        {
            get
            {
                if (_position >= _text.Length)
                    return '\0';

                return _text[_position];
            }
        }

        private void Next()
        {
            _position++;
        }


        public SyntaxToken NextToken()
        {
            if (_position >= _text.Length)
            {
                return new SyntaxToken(SyntaxKind.EndOfFileToken, _position, "\0", null);
            }

            if (char.IsDigit(Current))
            {
                var start = _position;

                while (char.IsDigit(Current))
                    Next();

                var length = _position - start;
                var text = _text.Substring(start, length);

                int.TryParse(text, out var value);
                return new SyntaxToken(SyntaxKind.NumberLiteralToken, start, text, value);
            }

            if (char.IsWhiteSpace(Current))
            {
                var start = _position;

                while (char.IsWhiteSpace(Current))
                    Next();

                var length = _position - start;
                var text = _text.Substring(start, length);

                return new SyntaxToken(SyntaxKind.WhiteSpaceToken, start, text, null);
            }

            // Operators and Properties
            if (char.IsLetter(Current))
            {
                var start = _position;

                while (!char.IsWhiteSpace(Current))
                    Next();

                var length = _position - start;
                var text = _text.Substring(start, length);


                if (text.Equals("EQUAL"))
                    return new SyntaxToken(SyntaxKind.EqualToken, start, text, null);

                else if (text.Equals("AND"))
                    return new SyntaxToken(SyntaxKind.AndToken, start, text, null);

                else if (text.Equals("OR"))
                    return new SyntaxToken(SyntaxKind.OrToken, start, text, null);

                else if (text.Equals("NOT"))
                    return new SyntaxToken(SyntaxKind.NotToken, start, text, null);

                else if (text.Equals("GREATER_THAN"))
                    return new SyntaxToken(SyntaxKind.GreaterThanToken, start, text, null);

                else if (text.Equals("LESS_THAN"))
                    return new SyntaxToken(SyntaxKind.LessThanToken, start, text, null);

                else
                    return new SyntaxToken(SyntaxKind.PropertyToken, start, text, null);
            }

            
            if (Current.Equals("\""))
            {
                var start = _position++;

            }


            return new SyntaxToken(SyntaxKind.BadToken, _position++, _text.Substring(_position - 1, 1), null);
        }
    }
}
