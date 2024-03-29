﻿using System.Collections.Generic;

namespace ChatBeet.Utilities;

public static class EnumerableExtensions
{
    public static T PickRandom<T>(this IEnumerable<T> source)
    {
        return source.PickRandom(1).Single();
    }

    public static IEnumerable<T> PickRandom<T>(this IEnumerable<T> source, int count)
    {
        return source.Shuffle().Take(count);
    }

    public static IEnumerable<T> Shuffle<T>(this IEnumerable<T> source)
    {
        return source.OrderBy(_ => Guid.NewGuid());
    }

    public static IEnumerable<T> ToSingleElementSequence<T>(this T item)
    {
        yield return item;
    }

    public static IAsyncEnumerable<T> ToAsyncSingleElementSequence<T>(this T item) =>
        item.ToSingleElementSequence().ToAsyncEnumerable();
}