using System;
using System.Collections.Generic;
using FluentAssertions;
using NSubstitute.Exceptions;
using NUnit.Framework;

namespace NSubstitute.Equivalency.Tests
{
    public class ArgExTests
    {
        public class Person
        {
            public DateTime Birthday { get; set; }
            public string FirstName { get; set; }
            public string LastName { get; set; }
        }

        public interface ISomeInterface
        {
            void Use(Person person);
            void UseEnumerable(IEnumerable<Person> people);
            void UseArray(Person[] people);
        }

        [Test]
        public void If_equivalency_is_given()
        {
            var service = Substitute.For<ISomeInterface>();
            DoSomethingWith(service);

            service.Received()
                .Use(ArgEx.IsEquivalentTo(new Person
                {
                    FirstName = "John",
                    LastName = "Doe",
                    Birthday = new DateTime(1968, 6, 1)
                }));
        }

        [Test]
        public void If_equivalency_is_not_given()
        {
            var service = Substitute.For<ISomeInterface>();
            DoSomethingWith(service);

            ExpectFailure(@"Expected to receive a call matching:
	Use(NSubstitute.Core.Arguments.ArgumentMatcher+GenericToNonGenericMatcherProxyWithDescribe`1[NSubstitute.Equivalency.Tests.ArgExTests+Person])
Actually received no matching calls.
Received 1 non-matching call (non-matching arguments indicated with '*' characters):
	Use(*ArgExTests+Person*)
		arg[0]: Expected property argument.Birthday to be <1968-07-01>, but found <1968-06-01>.", () => service.Received()
                .Use(ArgEx.IsEquivalentTo(new Person
                {
                    FirstName = "John",
                    LastName = "Doe",
                    Birthday = new DateTime(1968, 7, 1)
                })));
        }

        static void ExpectFailure(string expectedFailureMessage, Action check)
        {
            var receivedMsg = string.Empty;
            try
            {
                check();
            }
            catch (ReceivedCallsException x)
            {
                receivedMsg = x.Message;
            }
            Console.WriteLine(receivedMsg);
            var expected = split(expectedFailureMessage);

            split(receivedMsg).Should().StartWith(expected);

            string[] split(string what) =>
                what.Split(new[]
                {
                    '\n',
                    '\r'
                }, StringSplitOptions.RemoveEmptyEntries);
        }

        [Test]
        public void With_custom_equivalency_configuration()
        {
            var service = Substitute.For<ISomeInterface>();
            DoSomethingWith(service);

            service.Received()
                .Use(ArgEx.IsEquivalentTo(new Person
                {
                    FirstName = "John",
                    LastName = "Doe"
                }, cfg => cfg.Excluding(p => p.Birthday)));
        }

        [Test]
        public void Without_ArgEx()
        {
            var service = Substitute.For<ISomeInterface>();
            Person received = null;
            service.WhenForAnyArgs(s => s.Use(null)).Do(ci => received = ci.Arg<Person>());

            DoSomethingWith(service);

            received.Should()
            .BeEquivalentTo(new Person
            {
                FirstName = "John",
                LastName = "Doe",
                Birthday = new DateTime(1968, 6, 1)
            });
        }

        [Test]
        public void Enumerable_if_equivalency_is_given()
        {
            var service = Substitute.For<ISomeInterface>();

            InvokeActionOn(service, (x, y) => x.UseEnumerable(y));

            service.Received().UseEnumerable(ArgEx.IsEnumerableEquivalentTo(new[]
            {
                new Person {FirstName = "Alice", LastName = "Wonderland", Birthday = new DateTime(1968, 6, 1)},
                new Person {FirstName = "Bob", LastName = "Peanut", Birthday = new DateTime(1972, 9, 13)},
            }));
        }

        [Test]
        public void Enumerable_with_HashSet_in_usage_if_equivalency_is_given()
        {
            var service = Substitute.For<ISomeInterface>();

            service.UseEnumerable(new HashSet<Person>(){
                new Person() { FirstName = "Alice", LastName = "Wonderland", Birthday = new DateTime(1968, 6, 1) },
                new Person() { FirstName = "Bob", LastName = "Peanut", Birthday = new DateTime(1972, 9, 13) },
            });

            service.Received().UseEnumerable(ArgEx.IsEnumerableEquivalentTo(
                new Person { FirstName = "Alice", LastName = "Wonderland", Birthday = new DateTime(1968, 6, 1) },
                new Person { FirstName = "Bob", LastName = "Peanut", Birthday = new DateTime(1972, 9, 13) }));
        }

        [Test]
        public void Array_if_equivalency_is_given()
        {
            var service = Substitute.For<ISomeInterface>();

            InvokeActionOn(service, (x, y) => x.UseArray(y));

            service.Received().UseArray(ArgEx.IsArrayEquivalentTo(new[]
            {
                new Person {FirstName = "Alice", LastName = "Wonderland", Birthday = new DateTime(1968, 6, 1)},
                new Person {FirstName = "Bob", LastName = "Peanut", Birthday = new DateTime(1972, 9, 13)},
            }));
        }

        [Test]
        public void Array_with_CollectionExpression_in_usage_if_equivalency_is_given()
        {
            var service = Substitute.For<ISomeInterface>();

            service.UseArray([
                new Person() { FirstName = "Alice", LastName = "Wonderland", Birthday = new DateTime(1968, 6, 1) },
                new Person() { FirstName = "Bob", LastName = "Peanut", Birthday = new DateTime(1972, 9, 13) },
            ]);

            service.Received().UseArray(ArgEx.IsArrayEquivalentTo(new [] {
                new Person {FirstName = "Alice", LastName = "Wonderland", Birthday = new DateTime(1968, 6, 1)},
                new Person {FirstName = "Bob", LastName = "Peanut", Birthday = new DateTime(1972, 9, 13)},
            }));
        }

        [Test]
        public void Array_with_CollectionExpression_in_equivalence_check_if_equivalency_is_given()
        {
            var service = Substitute.For<ISomeInterface>();

            service.UseArray(new []{
                new Person() { FirstName = "Alice", LastName = "Wonderland", Birthday = new DateTime(1968, 6, 1) },
                new Person() { FirstName = "Bob", LastName = "Peanut", Birthday = new DateTime(1972, 9, 13) },
            });

            service.Received().UseArray(ArgEx.IsArrayEquivalentTo([
                new Person {FirstName = "Alice", LastName = "Wonderland", Birthday = new DateTime(1968, 6, 1)},
                new Person {FirstName = "Bob", LastName = "Peanut", Birthday = new DateTime(1972, 9, 13)},
            ]));
        }

        [Test]
        public void Enumerable_if_equivalency_is_not_given()
        {
            var service = Substitute.For<ISomeInterface>();

            InvokeActionOn(service, (x, y) => x.UseEnumerable(y));

            ExpectFailure(@"Expected to receive a call matching:
	UseEnumerable(NSubstitute.Core.Arguments.ArgumentMatcher+GenericToNonGenericMatcherProxyWithDescribe`1[System.Collections.Generic.IEnumerable`1[NSubstitute.Equivalency.Tests.ArgExTests+Person]])
Actually received no matching calls.
Received 1 non-matching call (non-matching arguments indicated with '*' characters):
	UseEnumerable(*Person[]*)
		arg[0]: Expected property argument[1].Birthday to be <1972-09-14>, but found <1972-09-13>.", () => service.Received().UseEnumerable(ArgEx.IsEnumerableEquivalentTo(new []
            {
                new Person(){FirstName = "Alice", LastName = "Wonderland", Birthday = new DateTime(1968, 6, 1)},
                new Person(){FirstName = "Bob", LastName = "Peanut", Birthday = new DateTime(1972, 9, 14)},
            })));
        }

        [Test]
        public void Array_if_equivalency_is_not_given()
        {
            var service = Substitute.For<ISomeInterface>();

            InvokeActionOn(service, (x, y) => x.UseArray(y));

            ExpectFailure(@"Expected to receive a call matching:
	UseArray(NSubstitute.Core.Arguments.ArgumentMatcher+GenericToNonGenericMatcherProxyWithDescribe`1[NSubstitute.Equivalency.Tests.ArgExTests+Person[]])
Actually received no matching calls.
Received 1 non-matching call (non-matching arguments indicated with '*' characters):
	UseArray(*Person[]*)
		arg[0]: Expected property argument[1].Birthday to be <1972-09-14>, but found <1972-09-13>.", () => service.Received().UseArray(ArgEx.IsArrayEquivalentTo(new[]
            {
                new Person(){FirstName = "Alice", LastName = "Wonderland", Birthday = new DateTime(1968, 6, 1)},
                new Person(){FirstName = "Bob", LastName = "Peanut", Birthday = new DateTime(1972, 9, 14)},
            })));
        }

        [Test]
        public void Enumerable_Without_ArgEx()
        {
            var service = Substitute.For<ISomeInterface>();
            IEnumerable<Person> received = null;
            service.WhenForAnyArgs(s => s.UseEnumerable(null)).Do(ci => received= ci.Arg<IEnumerable<Person>>());

            InvokeActionOn(service, (x, y) => x.UseEnumerable(y));

            received.Should().BeEquivalentTo(new []
            {
                new Person(){FirstName = "Alice", LastName = "Wonderland", Birthday = new DateTime(1968, 6, 1)},
                new Person(){FirstName = "Bob", LastName = "Peanut", Birthday = new DateTime(1972, 9, 13)},
            });
        }

        static void InvokeActionOn(ISomeInterface service, Action<ISomeInterface, Person[]> action)
        {
            action.Invoke(service, new[]
            {
                new Person(){FirstName = "Alice", LastName = "Wonderland", Birthday = new DateTime(1968, 6, 1)},
                new Person(){FirstName = "Bob", LastName = "Peanut", Birthday = new DateTime(1972, 9, 13)},
            });
        }

        static void DoSomethingWith(ISomeInterface service)
        {
            service.Use(new Person
            {
                FirstName = "John",
                LastName = "Doe",
                Birthday = new DateTime(1968, 6, 1)
            });
        }
    }
}