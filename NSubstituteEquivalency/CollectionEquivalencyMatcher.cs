using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using FluentAssertions.Equivalency;
using FluentAssertions.Execution;
using NSubstitute.Core;
using NSubstitute.Core.Arguments;

namespace NSubstitute.Equivalency
{
    public class CollectionEquivalencyMatcher<TItem, TCollection> : IArgumentMatcher<TCollection>, IDescribeNonMatches
        where TCollection : IEnumerable<TItem>
    {
        readonly Func<EquivalencyAssertionOptions<TItem>, EquivalencyAssertionOptions<TItem>> _configure;
        readonly TItem[] _expectation;
        string _failedExpectations = string.Empty;
        public CollectionEquivalencyMatcher(TItem[] expectation,
            Func<EquivalencyAssertionOptions<TItem>, EquivalencyAssertionOptions<TItem>> configure)
        {
            _configure = configure;
            _expectation = expectation;
        }
        public string DescribeFor(object argument) => _failedExpectations;

        public bool IsSatisfiedBy(TCollection argument)
        {
            using (var scope = new AssertionScope())
            {
                argument.Should().BeEquivalentTo(_expectation, _configure);
                _failedExpectations = string.Join(Environment.NewLine, scope.Discard());
                return !_failedExpectations.Any();
            }
        }
    }
}