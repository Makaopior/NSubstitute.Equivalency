using System;
using System.Collections.Generic;
using FluentAssertions.Equivalency;
using NSubstitute.Core.Arguments;

namespace NSubstitute.Equivalency
{
    public static class ArgEx
    {
        public static ref T IsEquivalentTo<T>(T value) => ref IsEquivalentTo(value, x => x);

        public static ref T IsEquivalentTo<T>(T value,
            Func<EquivalencyAssertionOptions<T>, EquivalencyAssertionOptions<T>> configure) =>
            ref ArgumentMatcher.Enqueue(new EquivalencyArgumentMatcher<T>(value, configure));

        public static ref TCollection IsCollectionEquivalentTo<TItem, TCollection>(
            TItem[] values,
            Func<EquivalencyAssertionOptions<TItem>, EquivalencyAssertionOptions<TItem>> configure)
            where TCollection : IEnumerable<TItem>
            => ref ArgumentMatcher.Enqueue(new CollectionEquivalencyMatcher<TItem, TCollection>(values, configure));


        public static ref T[] IsArrayEquivalentTo<T>(
            T[] values,
            Func<EquivalencyAssertionOptions<T>, EquivalencyAssertionOptions<T>> configure)
            => ref IsCollectionEquivalentTo<T, T[]>(values, configure);

        public static ref IEnumerable<T> IsEnumerableEquivalentTo<T>(
            T[] values,
            Func<EquivalencyAssertionOptions<T>, EquivalencyAssertionOptions<T>> configure)
            => ref IsCollectionEquivalentTo<T, IEnumerable<T>>(values, configure);


        public static ref T[] IsArrayEquivalentTo<T>(params T[] values)
            => ref IsCollectionEquivalentTo<T, T[]>(values, x => x);

        public static ref IEnumerable<T> IsEnumerableEquivalentTo<T>(params T[] values)
            => ref IsCollectionEquivalentTo<T, IEnumerable<T>>(values, x => x);
    }
}