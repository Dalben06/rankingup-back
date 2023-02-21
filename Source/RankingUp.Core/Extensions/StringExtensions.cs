namespace RankingUp.Core.Extensions
{
    public static class StringExtensions
    {
        public static Guid ToGuid(this string value)
        {
            if (string.IsNullOrEmpty(value) || value?.Length != 36)
                return Guid.Empty;

            return new Guid(value);
        }

        public static string OnlyNumbers(this string value)
        {
            if (string.IsNullOrEmpty(value) || value?.Length <= 0)
                return value;

            var regex = new System.Text.RegularExpressions.Regex("[^0-9a-zA-Z]+");
            value = regex.Replace(value ?? "", "");
            return value;
        }
    }
}
