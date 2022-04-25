namespace BlazingRecept.Shared.Extensions;

public static class ListExtensions
{
    public static IList<Type> Swap<Type>(this IList<Type> list, int firstIndex, int secondIndex)
    {
        Type tmp = list[firstIndex];
        list[firstIndex] = list[secondIndex];
        list[secondIndex] = tmp;
        return list;
    }
}