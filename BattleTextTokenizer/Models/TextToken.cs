namespace BattleTextTokenizer.Models
{
    public class TextToken
    {
        public TokenType TokenType { get; set; }
        public string Value { get; set; }

        public TextToken(TokenType tokenType)
        {
            TokenType = tokenType;
            Value = string.Empty;
        }

        public TextToken(TokenType tokenType, string value)
        {
            TokenType = tokenType;
            Value = value;
        }

        public TextToken Clone()
        {
            return new TextToken(TokenType, Value);
        }
    }
}
