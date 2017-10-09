using BattleTextTokenizer.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Text.RegularExpressions;

namespace BattleTextTokenizer
{
    public class RulesParser
    {
        public static IEnumerable<TokenDefinition> ParseFromString(string rulesString)
        {
            // Split the string into lines
            foreach (string line in rulesString.Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries))
            {
                // And split each line into values separated by commas
                string[] argArray = line.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                
                //throw out entries without at least the first three args
                if (argArray.Length < 3)
                {
                    Debug.WriteLine($"ERROR: Skipping parsing of entry '{line}'. Not enough arguments were provided.");
                    continue;
                }

                TokenType type;
                if (argArray.Length >= 1 && Enum.TryParse(argArray[0], out TokenType parsedType))
                {
                    type = parsedType;
                }
                else
                {
                    Debug.WriteLine($"ERROR: Skipping parsing of entry '{line}'. Unable to interpret first argument as a TokenType. It was either misspelled or missing.");
                    continue;
                }

                string regexString;
                if (argArray.Length >= 2 && IsValidRegex(argArray[1]))
                {
                    regexString = argArray[1].TrimStart();
                }
                else
                {
                    Debug.WriteLine($"ERROR: Skipping parsing of entry '{line}'. Unable to interpret second argument as a valid regex string. It was either invalid regex syntax, or missing.");
                    continue;
                }
                uint precedence = 1;
                if (argArray.Length >= 3 && UInt32.TryParse(argArray[2], out uint parsedPrecedence))
                {
                    precedence = parsedPrecedence;
                }
                else
                {
                    Debug.WriteLine($"ERROR: Skipping parsing of entry '{line}'. Unable to third argument as positive, integer value.");
                }
                bool? isBracketed = null;
                if (argArray.Length >= 4 && bool.TryParse(argArray[3], out bool parsedIsBracketed))
                {
                    isBracketed = parsedIsBracketed;
                }
                yield return new TokenDefinition(type, regexString, precedence, isBracketed ?? false);
            }
        }

        private static bool IsValidRegex(string pattern)
        {
            if (string.IsNullOrEmpty(pattern))
            {
                return false;
            }

            try
            {
                Regex.IsMatch("", pattern);
            }
            catch (ArgumentException)
            {
                return false;
            }

            return true;
        }
    }
}
