namespace RankingUp.Core.Extensions
{
    public static class IEnumerableExtension
    {
        public static void Add<TSource>(this IEnumerable<TSource> value, TSource item)
        {
            if (item is null)
                return;


            var temp = value.Concat(new List<TSource>() { item });

            if (value?.Any() ?? true)
                value = temp;
            else
                value = value.Concat(temp);
        }
    }
}
