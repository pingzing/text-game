using System.Text.RegularExpressions;
using System.Linq;
using TextGameExperiment.Core.Extensions;
using System.Collections.Generic;

namespace BattleTextTokenizer.Models
{
    public class TokenDefinition
    {                
        private readonly uint _precedence;

        /// <summary>
        /// The regex used to extract the token from the surrounding text.
        /// </summary>
        public Regex Regex { get; }

        /// <summary>
        /// The type of token this definition will return.
        /// </summary>
        public TokenType ReturnsToken { get; }

        /// <summary>
        /// Whether or not the token this defines surrounds text with an opening and closing tag.
        /// </summary>
        public bool IsBracketed { get; set; }

        public TokenDefinition(TokenType returnsToken, string regexPattern, uint precendence, bool isBracketed = false)
        {
            Regex = new Regex(regexPattern, RegexOptions.Compiled | RegexOptions.IgnoreCase | RegexOptions.Singleline);
            ReturnsToken = returnsToken;
            _precedence = precendence;
            IsBracketed = isBracketed;
        }

        public IEnumerable<TokenMatch> FindMatches(string inputString)
        {
            MatchCollection matches = Regex.Matches(inputString);            
            for (int i = 0; i < matches.Count; i++)
            {
                Match currentMatch = matches[i];

                string value = null;
                if (currentMatch.Groups.Count == 2) // 2 is what we get when we have a single opening tag
                {
                    value = currentMatch.Groups[1].ToString();
                }
                else if (currentMatch.Groups.Count > 2) //More than 2 is what we get when have an opening and closing tag
                {                    
                    foreach (var group in currentMatch.Groups.OfType<object>()                        
                        .Skip(2) // Skip the actual match and the opening tag
                        .SkipLastN(1)) // And also skip the closing tag
                    {
                        value += $"{group} "; //trailing space to use as separator for later reading of args
                    }
                    value = value.Substring(0, value.Length - 1); // chop off the final space
                }
                else
                {
                    value = currentMatch.Value;
                }

                yield return new TokenMatch
                {
                    StartIndex = currentMatch.Index,
                    EndIndex = currentMatch.Index + currentMatch.Length,
                    TokenType = ReturnsToken,
                    Value = value,
                    Precedence = _precedence
                };
            }
        }
    }
}
