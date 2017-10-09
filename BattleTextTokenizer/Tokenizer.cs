using System.Collections.Generic;
using BattleTextTokenizer.Models;
using System.Linq;
using System.Text.RegularExpressions;

namespace BattleTextTokenizer
{
    public class Tokenizer
    {
        public List<TokenDefinition> TokenDefinitions {get;set;}

        public Tokenizer()
        {
            TokenDefinitions = new List<TokenDefinition>();            
        }

        public Tokenizer(IEnumerable<TokenDefinition> tokenDefinitions)
        {
            TokenDefinitions = new List<TokenDefinition>(tokenDefinitions);
        }

        public IEnumerable<TextToken> TokenizeText(string text)
        {
            List<TokenMatch> tokenMatches = FindTokenMatches(text);

            List<IGrouping<int, TokenMatch>> groupedByIndex = tokenMatches.GroupBy(x => x.StartIndex)
                .OrderBy(x => x.Key)
                .ToList();

            TokenMatch lastMatch = null;
            for(int i = 0; i < groupedByIndex.Count; i++)
            {
                TokenMatch bestMatch = groupedByIndex[i].OrderBy(x => x.Precedence).First();
                if (lastMatch != null && bestMatch.StartIndex < lastMatch.EndIndex)
                {
                    continue;
                }

                yield return new TextToken(bestMatch.TokenType, bestMatch.Value);

                lastMatch = bestMatch;
            }            
        }

        private List<TokenMatch> FindTokenMatches(string text)
        {
            var matches = new List<TokenMatch>();
            foreach (var tokenDef in TokenDefinitions)
            {
                matches.AddRange(tokenDef.FindMatches(text).ToList());
            }

            return matches;
        }

        /// <summary>
        /// Returns the given string, stripped of all tokens defined by this Tokenizer.
        /// </summary>
        /// <param name="text">The text to strip of tokens.</param>
        /// <returns>The stripped text.</returns>
        public string StripUnprintableTokens(string text)
        {
            // Strip out unary tokens
            var regexPatterns = TokenDefinitions
                .Where(x => x.ReturnsToken != TokenType.Character && x.ReturnsToken != TokenType.String)
                .Select(x => x.Regex.ToString());            
            var newString = Regex.Replace(text, string.Join("|", regexPatterns), "");

            // Find all bracketed tokens
            var matches = new List<Match>();
            foreach (var bracketingRegex in TokenDefinitions.Where(x => x.IsBracketed).Select(x => x.Regex))
            {
                if (bracketingRegex != null)
                {
                    matches.AddRange(bracketingRegex.Matches(newString).Cast<Match>()); //Matches() returns a NON-generic IEnumerable, so we cast it.                    
                }
            }

            // Strip out bracketed tokens
            foreach (var bracketedMatch in matches)
            {
                // Using Groups[2] because index 0 is always the whole match, and index 1 should be the opening tag, and index 2 should be the content.
                newString = newString.Replace(bracketedMatch.Value, bracketedMatch.Groups[2].Value);
            }

            return newString;
        }
    }
}
