namespace RankingUp.Core.Extensions
{
    public static class IEnumerableExtension
    {
        public static void Add<TSource>(this IEnumerable<TSource> value, TSource item)
        {
            if (item is null)
                return;

            if(!value?.Any() ?? false)
                value = new List<TSource>();

            value.Concat( new List<TSource>() { item });
        }
    }
}
