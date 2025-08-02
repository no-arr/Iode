namespace Iode.Core.Utils
{
    public static class StringVerifier
    {
        public static bool IsValid(string verificationString, int maxLength = 100) =>
            !string.IsNullOrWhiteSpace(verificationString) && verificationString.Length <= maxLength;

        public static bool IsOnlyEnglishLetters(string input)
        {
            for (int i = 0; i < input.Length; i++)
            {
                char c = input[i];
                if ((c < 'A' || c > 'Z') && (c < 'a' || c > 'z'))
                    return false;
            }

            return true;
        }

    }
}