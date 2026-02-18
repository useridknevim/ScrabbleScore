using System.Collections.Generic;

namespace ScrabbleScore
{
    public static class ScrabbleLogika
    {
        public static readonly Dictionary<char, int> BodyPismen = new Dictionary<char, int>
        {
            {'A', 1}, {'B', 3}, {'C', 2}, {'Č', 4}, {'D', 2}, {'Ď', 8}, {'E', 1}, {'Ě', 3},
            {'F', 5}, {'G', 5}, {'H', 2}, {'I', 1}, {'Í', 2}, {'J', 2}, {'K', 1},
            {'L', 1}, {'M', 2}, {'N', 1}, {'Ň', 10}, {'O', 1}, {'Ó', 7}, {'P', 1}, {'R', 1},
            {'Ř', 4}, {'S', 1}, {'Š', 4}, {'T', 1}, {'Ť', 3}, {'U', 1}, {'Ú', 5}, {'Ů', 2},
            {'V', 1}, {'W', 8}, {'X', 10}, {'Y', 2}, {'Ý', 4}, {'Z', 2}, {'Ž', 3}
        };

        public static int ZiskejBodovouHodnotu(string pismeno)
        {
            if (string.IsNullOrEmpty(pismeno)) return 0;
            char c = pismeno.ToUpper()[0];
            if (BodyPismen.ContainsKey(c)) return BodyPismen[c];
            return 0;
        }
    }
}