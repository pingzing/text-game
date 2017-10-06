using System.Collections.Generic;
using BattleTextTokenizer.Models;
using System.Linq;

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
            foreach(var tokenDef in TokenDefinitions)
            {
                matches.AddRange(tokenDef.FindMatches(text).ToList());
            }

            return matches;
        }        
    }
}
