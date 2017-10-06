namespace BattleTextTokenizer.Models
{
    public class TokenMatch
    {        
        public TokenType TokenType { get; set; }
        public string Value { get; set; }
        public int StartIndex { get; set; }
        public int EndIndex { get; set; }
        public uint Precedence { get; set; }
    }
}
