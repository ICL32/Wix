namespace Wix_Technical_Test.QueryLanguage.Lexer
{
    public enum SyntaxKind
    {
        EqualToken,
        AndToken,
        OrToken,
        NotToken,
        GreaterThanToken,
        LessThanToken,
        CommaToken,
        PropertyToken,
        StringLiteralToken,
        NumberLiteralToken,
        WhiteSpaceToken,
        OpenParenthesisToken,
        CloseParenthesisToken,
        BadToken,
        EndOfFileToken,
    }
}
