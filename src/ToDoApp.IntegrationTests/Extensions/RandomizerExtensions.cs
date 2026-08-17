using Bogus;

namespace ToDoApp.IntegrationTests.Extensions;

public static class RandomizerExtensions
{
    extension(Randomizer thisRandom)
    {
        public string Substring(string str)
        {
            if(str.Length is 0 or 1)
            {
                return str;
            }
            int start = thisRandom.Number(0, str.Length - 1);
            int end = thisRandom.Number(start + 1, str.Length);
            return str[start..end];
        }
    }
}