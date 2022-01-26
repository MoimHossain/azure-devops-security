
using System.Text.RegularExpressions;

namespace Kdoctl.CliServices.Supports
{
    public class PatternMatchAssistant
    {
        // Starts with S, ends with X, contains "me" and "a" (in that order) 
        // Boolean complex = Regex.IsMatch(test, WildCardToRegular("S*me*a*X"));
        // Copied from here for now: https://stackoverflow.com/questions/30299671/matching-strings-with-wildcard
        public bool IsMatch(string patternWithWildcards, string testKey)
        {
            return Regex.IsMatch(testKey, WildCardToRegular(patternWithWildcards));
        }

        private string WildCardToRegular(string patternWithWildcards)
        {
            return "^" + Regex.Escape(patternWithWildcards).Replace("\\*", ".*") + "$";
        }
    }
}
