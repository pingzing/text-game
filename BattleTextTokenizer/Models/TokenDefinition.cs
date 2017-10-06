using System.Text.RegularExpressions;
using System.Linq;
using TextGameExperiment.Core.Extensions;
using System.Collections.Generic;

namespace BattleTextTokenizer.Models
{
    public class TokenDefinition
    {
        private Regex _regex;
        private readonly TokenType _returnsToken;
        private readonly uint _precedence;

        public TokenDefinition(TokenType returnsToken, string regexPattern, uint precendence)
        {
            _regex = new Regex(regexPattern, RegexOptions.Compiled | RegexOptions.IgnoreCase | RegexOptions.Singleline);
            _returnsToken = returnsToken;
            _precedence = precendence;
        }

        public IEnumerable<TokenMatch> FindMatches(string inputString)
        {
            MatchCollection matches = _regex.Matches(inputString);
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
                    TokenType = _returnsToken,
                    Value = value,
                    Precedence = _precedence
                };
            }
        }
    }
}
