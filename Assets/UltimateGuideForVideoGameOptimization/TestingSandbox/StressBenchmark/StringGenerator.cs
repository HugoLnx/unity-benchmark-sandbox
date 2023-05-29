using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace StressBenchmark
{
    public static class StringGenerator
    {
        private static IEnumerable<char> Alphabet => Enumerable.Range(0, 26).Select(x => (char)('a' + x));
        private static IEnumerable<char> Digits => Enumerable.Range(0, 10).Select(x => (char)('0' + x));
        private static readonly string[] ValidCharacters = 
            Alphabet.Union(Digits)
            .Select(c => c.ToString())
            .ToArray();
        public static string Build(int size, string suffix = "", int seed = 127)
        {
            var rand = new System.Random(seed);
            StringBuilder str = new();
            int missingChars = size - suffix.Length;
            for (int i = 0; i < missingChars; i++)
            {
                str.Append(ValidCharacters[rand.Next(0, ValidCharacters.Length)]);
            }
            return str.Append(suffix).ToString();
        }
    }
}