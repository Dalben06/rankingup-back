using System.ComponentModel;
using System.Reflection;

namespace RankingUp.Core.Extensions
{
    public static class EnumsExtension
    {
        public static string GetDescription(this Enum enumValue)
        {

            FieldInfo fi = enumValue.GetType().GetField(enumValue.ToString());

            DescriptionAttribute[] attributes = fi.GetCustomAttributes(typeof(DescriptionAttribute), false) as DescriptionAttribute[];

            if (attributes != null && attributes.Any())
            {
                return attributes.First().Description;
            }

            return enumValue.ToString();
        }
    }
}
