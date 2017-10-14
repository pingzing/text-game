using Optional.Unsafe;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using TextGameExperiment.Core.Services;
using Optional;
using BattleTextTokenizer;
using BattleTextTokenizer.Models;

namespace TextGameExperiment.Core.Models
{
    public class Room
    {
        private static readonly Tokenizer _headerTokenizer;

        public string Title { get; set; }
        public string Body { get; set; }
        public string RelativeDialoguePath { get; set; }

        static Room()
        {
            Option<StreamReader> readerResult = FileService.LoadFromResources("Assets.HeaderRules.txt");
            if (readerResult.HasValue)
            {
                using (var reader = readerResult.ValueOrFailure())
                {
                    string rulesString = reader.ReadToEnd();
                    IEnumerable<TokenDefinition> tokenDefs = RulesParser.ParseFromString(rulesString);
                    _headerTokenizer = new Tokenizer(tokenDefs);
                }
            }
        }

        public async Task LoadAsync()
        {
            // TODO: actual DI, or at least Service Location            
            var fileStream = FileService.LoadFromResources(RelativeDialoguePath);            
            if (fileStream.HasValue)
            {
                using (StreamReader reader = fileStream.ValueOrFailure())
                {
                    Option<string> firstLineResult = await FileService.ReadFirstLineAsync(reader);
                    string headerString = firstLineResult.ValueOr("");
                    foreach(TextToken token in _headerTokenizer.TokenizeText(headerString))
                    {
                        if (token.TokenType == TokenType.Title)
                        {
                            Title = token.Value;
                        }
                    }

                    Option<string> bodyResult = await FileService.ReadFileAsync(reader);
                    Body = bodyResult.ValueOr("#BODY NOT FOUND#");
                }
            }
        }
    }
}
