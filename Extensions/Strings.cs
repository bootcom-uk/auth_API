using System.Security.Cryptography;

namespace Extensions
{
    public static class Strings
    {

        public static string PerformMerge(this string message, Dictionary<string, string> mergeValues)
        {
            foreach (KeyValuePair<string, string> mergeValue in mergeValues)
            {
                message = message.Replace(mergeValue.Key, mergeValue.Value);
                message = message.Replace(mergeValue.Key.Replace("<", "&lt;"), mergeValue.Value);
                message = message.Replace(mergeValue.Key.Replace(">", "&gt;"), mergeValue.Value);
            }

            return message;
        }

        public static string GenerateRefreshToken(this string input)
        {
            byte[] array = new byte[32];
            using RandomNumberGenerator randomNumberGenerator = RandomNumberGenerator.Create();
            randomNumberGenerator.GetBytes(array);
            return Convert.ToBase64String(array);
        }

        private static System.Random random = new System.Random();

        public static string RandomString(int length, bool useNumbers = true, bool useUpperCaseChars = true, bool useLowerCaseChars = true)
        {
            string text = "";
            if (useNumbers)
            {
                text += "0123456789";
            }

            if (useUpperCaseChars)
            {
                text += "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            }

            if (useLowerCaseChars)
            {
                text += "abcdefghijklmnopqrstuvwxyz";
            }

            if (!useNumbers && !useUpperCaseChars && !useLowerCaseChars)
            {
                throw new ArgumentException("You must specify at least one of the items you can use");
            }

            return new string((from s in Enumerable.Repeat(text, length)
                               select s[random.Next(s.Length)]).ToArray());
        }

    }
}
