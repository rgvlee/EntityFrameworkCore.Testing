using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoFixture;
using AutoMapper;
using KellermanSoftware.CompareNetObjects;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;

namespace EntityFrameworkCore.Testing.Common.Tests
{
    public abstract class BaseForQueryableTests<T> : BaseForTests where T : BaseTestEntity
    {
        protected List<T> ItemsAddedToQueryableSource;

        protected abstract IQueryable<T> Queryable { get; }

        protected abstract void SeedQueryableSource();

        [Test]
        public virtual void All_FalseCondition_ReturnsFalse()
        {
            SeedQueryableSource();

            var actualResult1 = Queryable.All(x => string.IsNullOrWhiteSpace(x.FullName));
            var actualResult2 = Queryable.All(x => string.IsNullOrWhiteSpace(x.FullName));

            Assert.Multiple(() =>
            {
                Assert.That(actualResult1, Is.False);
                Assert.That(actualResult2, Is.False);
            });
        }

        [Test]
        public virtual void All_TrueCondition_ReturnsTrue()
        {
            SeedQueryableSource();

            var actualResult1 = Queryable.All(x => !string.IsNullOrWhiteSpace(x.FullName));
            var actualResult2 = Queryable.All(x => !string.IsNullOrWhiteSpace(x.FullName));

            Assert.Multiple(() =>
            {
                Assert.That(actualResult1, Is.True);
                Assert.That(actualResult2, Is.True);
            });
        }

        [Test]
        public virtual async Task AllAsync_FalseCondition_ReturnsFalse()
        {
            SeedQueryableSource();

            var actualResult1 = await Queryable.AllAsync(x => string.IsNullOrWhiteSpace(x.FullName));
            var actualResult2 = await Queryable.AllAsync(x => string.IsNullOrWhiteSpace(x.FullName));

            Assert.Multiple(() =>
            {
                Assert.That(actualResult1, Is.False);
                Assert.That(actualResult2, Is.False);
            });
        }

        [Test]
        public virtual async Task AllAsync_TrueCondition_ReturnsTrue()
        {
            SeedQueryableSource();

            var actualResult1 = await Queryable.AllAsync(x => !string.IsNullOrWhiteSpace(x.FullName));
            var actualResult2 = await Queryable.AllAsync(x => !string.IsNullOrWhiteSpace(x.FullName));

            Assert.Multiple(() =>
            {
                Assert.That(actualResult1, Is.True);
                Assert.That(actualResult2, Is.True);
            });
        }

        [Test]
        public virtual void Any_ReturnsTrue()
        {
            SeedQueryableSource();

            var actualResult1 = Queryable.Any();
            var actualResult2 = Queryable.Any();

            Assert.Multiple(() =>
            {
                Assert.That(actualResult1, Is.True);
                Assert.That(actualResult2, Is.True);
            });
        }

        [Test]
        public virtual void Any_WithNoItemsAdded_ReturnsFalse()
        {
            var actualResult1 = Queryable.Any();
            var actualResult2 = Queryable.Any();

            Assert.Multiple(() =>
            {
                Assert.That(actualResult1, Is.False);
                Assert.That(actualResult2, Is.False);
            });
        }

        [Test]
        public virtual async Task AnyAsync_ReturnsTrue()
        {
            SeedQueryableSource();

            var actualResult1 = await Queryable.AnyAsync();
            var actualResult2 = await Queryable.AnyAsync();

            Assert.Multiple(() =>
            {
                Assert.That(actualResult1, Is.True);
                Assert.That(actualResult2, Is.True);
            });
        }

        [Test]
        public virtual async Task AnyAsyncWithCancellationToken_ReturnsTrue()
        {
            SeedQueryableSource();

            using (var cancellationTokenSource = new CancellationTokenSource(new TimeSpan(0, 1, 0)))
            {
                var actualResult1 = await Queryable.AnyAsync(cancellationTokenSource.Token);
                var actualResult2 = await Queryable.AnyAsync(cancellationTokenSource.Token);

                Assert.Multiple(() =>
                {
                    Assert.That(actualResult1, Is.True);
                    Assert.That(actualResult2, Is.True);
                });
            }
        }

        [Test]
        public virtual void Average_Decimal_ReturnsAverage()
        {
            SeedQueryableSource();

            var actualResult1 = Queryable.Average(x => x.Weight);
            var actualResult2 = Queryable.Average(x => x.Weight);

            Assert.Multiple(() =>
            {
                var average = 0m;
                for (var i = 0; i < ItemsAddedToQueryableSource.Count; i++)
                {
                    average += ItemsAddedToQueryableSource[i].Weight;
                }

                average = average / ItemsAddedToQueryableSource.Count;

                Assert.That(actualResult1, Is.EqualTo(average));
                Assert.That(actualResult2, Is.EqualTo(average));
            });
        }

        [Test]
        public virtual async Task AverageAsync_Decimal_ReturnsAverage()
        {
            SeedQueryableSource();

            var actualResult1 = await Queryable.AverageAsync(x => x.Weight);
            var actualResult2 = await Queryable.AverageAsync(x => x.Weight);

            Assert.Multiple(() =>
            {
                var average = 0m;
                for (var i = 0; i < ItemsAddedToQueryableSource.Count; i++)
                {
                    average += ItemsAddedToQueryableSource[i].Weight;
                }

                average = average / ItemsAddedToQueryableSource.Count;

                Assert.That(actualResult1, Is.EqualTo(average));
                Assert.That(actualResult2, Is.EqualTo(average));
            });
        }

        [Test]
        public virtual void Contains_FalseCondition_ReturnsFalse()
        {
            SeedQueryableSource();

            var itemToFind = Fixture.Create<T>();

            var actualResult1 = Queryable.Contains(itemToFind);
            var actualResult2 = Queryable.Contains(itemToFind);

            Assert.Multiple(() =>
            {
                Assert.That(actualResult1, Is.False);
                Assert.That(actualResult2, Is.False);
            });
        }

        [Test]
        public virtual void Contains_TrueCondition_ReturnsTrue()
        {
            SeedQueryableSource();

            var itemToFind = Queryable.First();

            var actualResult1 = Queryable.Contains(itemToFind);
            var actualResult2 = Queryable.Contains(itemToFind);

            Assert.Multiple(() =>
            {
                Assert.That(actualResult1, Is.True);
                Assert.That(actualResult2, Is.True);
            });
        }

        [Test]
        public virtual async Task ContainsAsync_FalseCondition_ReturnsFalse()
        {
            SeedQueryableSource();

            var itemToFind = Fixture.Create<T>();

            var actualResult1 = await Queryable.ContainsAsync(itemToFind);
            var actualResult2 = await Queryable.ContainsAsync(itemToFind);

            Assert.Multiple(() =>
            {
                Assert.That(actualResult1, Is.False);
                Assert.That(actualResult2, Is.False);
            });
        }

        [Test]
        public virtual async Task ContainsAsync_TrueCondition_ReturnsTrue()
        {
            SeedQueryableSource();

            var itemToFind = Queryable.First();

            var actualResult1 = await Queryable.ContainsAsync(itemToFind);
            var actualResult2 = await Queryable.ContainsAsync(itemToFind);

            Assert.Multiple(() =>
            {
                Assert.That(actualResult1, Is.True);
                Assert.That(actualResult2, Is.True);
            });
        }

        [Test]
        public virtual void ElementAt_ThrowsException()
        {
            SeedQueryableSource();

            Assert.Multiple(() =>
            {
                var ex1 = Assert.Throws<NotSupportedException>(() =>
                {
                    var firstElement = Queryable.ElementAt(0);
                });

                //Assert.That(ex1.Message, Is.EqualTo(string.Format(ExceptionMessages.TranslationFailedExceptionMessage, $"DbSet<{typeof(T).Name}>()\r\n    .ElementAt(0)")));

                var ex2 = Assert.Throws<NotSupportedException>(() =>
                {
                    var lastElement = Queryable.ElementAt(Queryable.ToList().Count - 1);
                });

                //Assert.That(ex2.Message, Is.EqualTo(string.Format(ExceptionMessages.TranslationFailedExceptionMessage, $"DbSet<{typeof(T).Name}>()\r\n    .ElementAt({Queryable.ToList().Count - 1})")));
            });
        }

        [Test]
        public virtual void ElementAt_AfterAsEnumerable_DoesNotThrowException()
        {
            SeedQueryableSource();

            Console.WriteLine("Assert start");

            Assert.Multiple(() =>
            {
                Assert.DoesNotThrow(() =>
                {
                    var firstElement = Queryable.AsEnumerable().ElementAt(0);
                });

                Assert.DoesNotThrow(() =>
                {
                    var lastElement = Queryable.AsEnumerable().ElementAt(Queryable.ToList().Count - 1);
                });
            });
        }

        [Test]
        public virtual void ElementAt_AfterToList_DoesNotThrowException()
        {
            SeedQueryableSource();

            Assert.Multiple(() =>
            {
                Assert.DoesNotThrow(() =>
                {
                    var firstElement = Queryable.ToList().ElementAt(0);
                });

                Assert.DoesNotThrow(() =>
                {
                    var lastElement = Queryable.ToList().ElementAt(Queryable.ToList().Count - 1);
                });
            });
        }

        [Test]
        public virtual void ElementAtOrDefault_ThrowsException()
        {
            SeedQueryableSource();

            Assert.Multiple(() =>
            {
                var ex1 = Assert.Throws<NotSupportedException>(() =>
                {
                    var firstElement = Queryable.ElementAtOrDefault(0);
                });

                //Assert.That(ex1.Message, Is.EqualTo(string.Format(ExceptionMessages.TranslationFailedExceptionMessage, $"DbSet<{typeof(T).Name}>()\r\n    .ElementAtOrDefault(0)")));

                var ex2 = Assert.Throws<NotSupportedException>(() =>
                {
                    var lastElement = Queryable.ElementAtOrDefault(Queryable.ToList().Count - 1);
                });

                //Assert.That(ex2.Message, Is.EqualTo(string.Format(ExceptionMessages.TranslationFailedExceptionMessage, $"DbSet<{typeof(T).Name}>()\r\n    .ElementAtOrDefault({Queryable.ToList().Count - 1})")));
            });
        }

        [Test]
        public virtual void ElementAtOrDefault_WithNoItemsAdded_ThrowsException()
        {
            Assert.Multiple(() =>
            {
                var ex1 = Assert.Throws<NotSupportedException>(() =>
                {
                    var firstElement = Queryable.ElementAtOrDefault(0);
                });

                //Assert.That(ex1.Message, Is.EqualTo(string.Format(ExceptionMessages.TranslationFailedExceptionMessage, $"DbSet<{typeof(T).Name}>()\r\n    .ElementAtOrDefault(0)")));

                var ex2 = Assert.Throws<NotSupportedException>(() =>
                {
                    var lastElement = Queryable.ElementAtOrDefault(Queryable.ToList().Count - 1);
                });

                //Assert.That(ex2.Message, Is.EqualTo(string.Format(ExceptionMessages.TranslationFailedExceptionMessage, $"DbSet<{typeof(T).Name}>()\r\n    .ElementAtOrDefault({Queryable.ToList().Count - 1})")));
            });
        }

        [Test]
        public virtual void First_ReturnsFirstElement()
        {
            SeedQueryableSource();

            var actualResult1 = Queryable.First();
            var actualResult2 = Queryable.First();

            Assert.Multiple(() =>
            {
                Assert.That(actualResult1, Is.EqualTo(ItemsAddedToQueryableSource.First()));
                Assert.That(actualResult2, Is.EqualTo(ItemsAddedToQueryableSource.First()));
            });
        }

        [Test]
        public virtual void First_WithNoItemsAdded_ThrowsException()
        {
            Assert.Throws<InvalidOperationException>(() =>
            {
                var actualResult1 = Queryable.First();
            });
        }

        [Test]
        public virtual async Task FirstAsync_ReturnsFirstElement()
        {
            SeedQueryableSource();

            var actualResult1 = await Queryable.FirstAsync();
            var actualResult2 = await Queryable.FirstAsync();

            Assert.Multiple(() =>
            {
                Assert.That(actualResult1, Is.EqualTo(ItemsAddedToQueryableSource.First()));
                Assert.That(actualResult2, Is.EqualTo(ItemsAddedToQueryableSource.First()));
            });
        }

        [Test]
        public virtual void FirstOrDefault_ReturnsFirstElement()
        {
            SeedQueryableSource();

            var actualResult1 = Queryable.FirstOrDefault();
            var actualResult2 = Queryable.FirstOrDefault();

            Assert.Multiple(() =>
            {
                Assert.That(actualResult1, Is.EqualTo(ItemsAddedToQueryableSource.First()));
                Assert.That(actualResult2, Is.EqualTo(ItemsAddedToQueryableSource.First()));
            });
        }

        [Test]
        public virtual void FirstOrDefault_WithNoItemsAdded_ReturnsFirstElement()
        {
            var actualResult1 = Queryable.FirstOrDefault();
            var actualResult2 = Queryable.FirstOrDefault();

            Assert.Multiple(() =>
            {
                Assert.That(actualResult1, Is.EqualTo(default(T)));
                Assert.That(actualResult2, Is.EqualTo(default(T)));
            });
        }

        [Test]
        public virtual async Task FirstOrDefaultAsync_ReturnsFirstElement()
        {
            SeedQueryableSource();

            var actualResult1 = await Queryable.FirstOrDefaultAsync();
            var actualResult2 = await Queryable.FirstOrDefaultAsync();

            Assert.Multiple(() =>
            {
                Assert.That(actualResult1, Is.EqualTo(ItemsAddedToQueryableSource.First()));
                Assert.That(actualResult2, Is.EqualTo(ItemsAddedToQueryableSource.First()));
            });
        }

        [Test]
        public virtual void Last_ReturnsLastElement()
        {
            SeedQueryableSource();

            var actualResult1 = Queryable.Last();
            var actualResult2 = Queryable.Last();

            Assert.Multiple(() =>
            {
                Assert.That(actualResult1, Is.EqualTo(ItemsAddedToQueryableSource.Last()));
                Assert.That(actualResult2, Is.EqualTo(ItemsAddedToQueryableSource.Last()));
            });
        }

        [Test]
        public virtual void Last_WithNoItemsAdded_ThrowsException()
        {
            Assert.Throws<InvalidOperationException>(() =>
            {
                var actualResult1 = Queryable.Last();
            });
        }

        [Test]
        public virtual async Task LastAsync_ReturnsLastElement()
        {
            SeedQueryableSource();

            var actualResult1 = await Queryable.LastAsync();
            var actualResult2 = await Queryable.LastAsync();

            Assert.Multiple(() =>
            {
                Assert.That(actualResult1, Is.EqualTo(ItemsAddedToQueryableSource.Last()));
                Assert.That(actualResult2, Is.EqualTo(ItemsAddedToQueryableSource.Last()));
            });
        }

        [Test]
        public virtual void LastOrDefault_ReturnsLastElement()
        {
            SeedQueryableSource();

            var actualResult1 = Queryable.LastOrDefault();
            var actualResult2 = Queryable.LastOrDefault();

            Assert.Multiple(() =>
            {
                Assert.That(actualResult1, Is.EqualTo(ItemsAddedToQueryableSource.Last()));
                Assert.That(actualResult2, Is.EqualTo(ItemsAddedToQueryableSource.Last()));
            });
        }

        [Test]
        public virtual void LastOrDefault_WithNoItemsAdded_ReturnsLastElement()
        {
            var actualResult1 = Queryable.LastOrDefault();
            var actualResult2 = Queryable.LastOrDefault();

            Assert.Multiple(() =>
            {
                Assert.That(actualResult1, Is.EqualTo(default(T)));
                Assert.That(actualResult2, Is.EqualTo(default(T)));
            });
        }

        [Test]
        public virtual async Task LastOrDefaultAsync_ReturnsLastElement()
        {
            SeedQueryableSource();

            var actualResult1 = await Queryable.LastOrDefaultAsync();
            var actualResult2 = await Queryable.LastOrDefaultAsync();

            Assert.Multiple(() =>
            {
                Assert.That(actualResult1, Is.EqualTo(ItemsAddedToQueryableSource.Last()));
                Assert.That(actualResult2, Is.EqualTo(ItemsAddedToQueryableSource.Last()));
            });
        }

        [Test]
        public virtual void Max_DateTime_ReturnsMaxDateTime()
        {
            SeedQueryableSource();

            var actualResult1 = Queryable.Max(x => x.DateOfBirth);
            var actualResult2 = Queryable.Max(x => x.DateOfBirth);

            Assert.Multiple(() =>
            {
                var maxDateTime = ItemsAddedToQueryableSource[0].DateOfBirth;
                for (var i = 1; i < ItemsAddedToQueryableSource.Count; i++)
                {
                    if (ItemsAddedToQueryableSource[i].DateOfBirth > maxDateTime)
                    {
                        maxDateTime = ItemsAddedToQueryableSource[i].DateOfBirth;
                    }
                }

                Assert.That(actualResult1, Is.EqualTo(maxDateTime));
                Assert.That(actualResult2, Is.EqualTo(maxDateTime));
            });
        }

        [Test]
        public virtual async Task MaxAsync_DateTime_ReturnsMaxDateTime()
        {
            SeedQueryableSource();

            var actualResult1 = await Queryable.MaxAsync(x => x.DateOfBirth);
            var actualResult2 = await Queryable.MaxAsync(x => x.DateOfBirth);

            Assert.Multiple(() =>
            {
                var maxDateTime = ItemsAddedToQueryableSource[0].DateOfBirth;
                for (var i = 1; i < ItemsAddedToQueryableSource.Count; i++)
                {
                    if (ItemsAddedToQueryableSource[i].DateOfBirth > maxDateTime)
                    {
                        maxDateTime = ItemsAddedToQueryableSource[i].DateOfBirth;
                    }
                }

                Assert.That(actualResult1, Is.EqualTo(maxDateTime));
                Assert.That(actualResult2, Is.EqualTo(maxDateTime));
            });
        }

        [Test]
        public virtual void Min_DateTime_ReturnsMinDateTime()
        {
            SeedQueryableSource();

            var actualResult1 = Queryable.Min(x => x.DateOfBirth);
            var actualResult2 = Queryable.Min(x => x.DateOfBirth);

            Assert.Multiple(() =>
            {
                var minDateTime = ItemsAddedToQueryableSource[0].DateOfBirth;
                for (var i = 1; i < ItemsAddedToQueryableSource.Count; i++)
                {
                    if (ItemsAddedToQueryableSource[i].DateOfBirth < minDateTime)
                    {
                        minDateTime = ItemsAddedToQueryableSource[i].DateOfBirth;
                    }
                }

                Assert.That(actualResult1, Is.EqualTo(minDateTime));
                Assert.That(actualResult2, Is.EqualTo(minDateTime));
            });
        }

        [Test]
        public virtual async Task MinAsync_DateTime_ReturnsMinDateTime()
        {
            SeedQueryableSource();

            var actualResult1 = await Queryable.MinAsync(x => x.DateOfBirth);
            var actualResult2 = await Queryable.MinAsync(x => x.DateOfBirth);

            Assert.Multiple(() =>
            {
                var minDateTime = ItemsAddedToQueryableSource[0].DateOfBirth;
                for (var i = 1; i < ItemsAddedToQueryableSource.Count; i++)
                {
                    if (ItemsAddedToQueryableSource[i].DateOfBirth < minDateTime)
                    {
                        minDateTime = ItemsAddedToQueryableSource[i].DateOfBirth;
                    }
                }

                Assert.That(actualResult1, Is.EqualTo(minDateTime));
                Assert.That(actualResult2, Is.EqualTo(minDateTime));
            });
        }

        [Test]
        public virtual void OrderBy_DateTime_ReturnsItemsInAscendingOrder()
        {
            SeedQueryableSource();

            var actualResult1 = Queryable.OrderBy(x => x.DateOfBirth).ToList();
            var actualResult2 = Queryable.OrderBy(x => x.DateOfBirth).ToList();

            Assert.Multiple(() =>
            {
                for (var i = 1; i < actualResult1.Count; i++)
                {
                    Assert.That(actualResult1[i].DateOfBirth, Is.GreaterThanOrEqualTo(actualResult1[i - 1].DateOfBirth));
                }

                for (var i = 1; i < actualResult2.Count; i++)
                {
                    Assert.That(actualResult2[i].DateOfBirth, Is.GreaterThanOrEqualTo(actualResult2[i - 1].DateOfBirth));
                }
            });
        }

        [Test]
        public virtual void OrderByDescending_DateTime_ReturnsItemsInDescendingOrder()
        {
            SeedQueryableSource();

            var actualResult1 = Queryable.OrderByDescending(x => x.DateOfBirth).ToList();
            var actualResult2 = Queryable.OrderByDescending(x => x.DateOfBirth).ToList();

            Assert.Multiple(() =>
            {
                for (var i = 1; i < actualResult1.Count; i++)
                {
                    Assert.That(actualResult1[i].DateOfBirth, Is.LessThanOrEqualTo(actualResult1[i - 1].DateOfBirth));
                }

                for (var i = 1; i < actualResult2.Count; i++)
                {
                    Assert.That(actualResult2[i].DateOfBirth, Is.LessThanOrEqualTo(actualResult2[i - 1].DateOfBirth));
                }
            });
        }

        [Test]
        public virtual void OrderByThenOrderBy_FixedDateTimeAndDecimal_ReturnsItemsInAscendingOrder()
        {
            SeedQueryableSource();

            var actualResult1 = Queryable.OrderBy(x => x.CreatedAt).ThenBy(x => x.Weight).ToList();
            var actualResult2 = Queryable.OrderBy(x => x.CreatedAt).ThenBy(x => x.Weight).ToList();

            Assert.Multiple(() =>
            {
                for (var i = 1; i < actualResult1.Count; i++)
                {
                    Assert.That(actualResult1[i].Weight, Is.GreaterThanOrEqualTo(actualResult1[i - 1].Weight));
                }

                for (var i = 1; i < actualResult2.Count; i++)
                {
                    Assert.That(actualResult2[i].Weight, Is.GreaterThanOrEqualTo(actualResult2[i - 1].Weight));
                }
            });
        }

        [Test]
        public virtual void Select_ReturnsSequence()
        {
            SeedQueryableSource();

            var actualResult1 = Queryable.Select(x => x).ToList();
            var actualResult2 = Queryable.Select(x => x).ToList();

            Assert.Multiple(() =>
            {
                for (var i = 0; i < ItemsAddedToQueryableSource.Count; i++)
                {
                    Assert.That(actualResult1[i], Is.EqualTo(ItemsAddedToQueryableSource[i]));
                    Assert.That(actualResult2[i], Is.EqualTo(ItemsAddedToQueryableSource[i]));
                }
            });
        }

        [Test]
        public virtual async Task SelectAnonymousObjectThenToListAsync_ReturnsList()
        {
            SeedQueryableSource();

            var expectedResult = Queryable.Select(x => new { x.Id }).ToList();

            var actualResult1 = await Queryable.Select(x => new { x.Id }).ToListAsync();
            var actualResult2 = await Queryable.Select(x => new { x.Id }).ToListAsync();

            Assert.Multiple(() =>
            {
                Assert.That(actualResult1, Is.EquivalentTo(expectedResult));
                Assert.That(actualResult2, Is.EquivalentTo(expectedResult));
            });
        }

        [Test]
        public virtual void SelectWithIndex_ThrowsException()
        {
            SeedQueryableSource();

            Assert.Multiple(() =>
            {
                var ex1 = Assert.Throws<NotSupportedException>(() =>
                {
                    var actualResult1 = Queryable.Select((x, i) => new { Index = i, Item = x }).ToList();
                });

                //Assert.That(ex1.Message, Is.EqualTo(string.Format(ExceptionMessages.TranslationFailedExceptionMessage, typeof(T).Name)));

                var ex2 = Assert.Throws<NotSupportedException>(() =>
                {
                    var actualResult2 = Queryable.Select((x, i) => new { Index = i, Item = x }).ToList();
                });

                //Assert.That(ex2.Message, Is.EqualTo(string.Format(ExceptionMessages.TranslationFailedExceptionMessage, typeof(T).Name)));
            });
        }

        [Test]
        public virtual void SelectWithIndexThenWhereThenAny_TrueCondition_ThrowsException()
        {
            SeedQueryableSource();

            Assert.Multiple(() =>
            {
                var ex = Assert.Throws<NotSupportedException>(() =>
                {
                    var actualResult1 = Queryable.Select((x, i) => new { Index = i, Item = x }).Where(x => !x.Index.Equals(0)).Any();
                });

                //Assert.That(ex.Message, Is.EqualTo(string.Format(ExceptionMessages.TranslationFailedExceptionMessage, typeof(T).Name)));
            });
        }

        [Test]
        public virtual void Skip_One_ReturnsSequenceThatDoesNotIncludeFirstItem()
        {
            SeedQueryableSource();

            var firstItem = Queryable.First();

            var actualResult1 = Queryable.Skip(1);
            var actualResult2 = Queryable.Skip(1);

            Assert.Multiple(() =>
            {
                Assert.That(actualResult1.Contains(firstItem), Is.False);
                Assert.That(actualResult1.Count(), Is.EqualTo(Queryable.Count() - 1));

                Assert.That(actualResult2.Contains(firstItem), Is.False);
                Assert.That(actualResult2.Count(), Is.EqualTo(Queryable.Count() - 1));
            });
        }

        [Test]
        public virtual void SkipWhile_SkipFirstItem_ThrowsException()
        {
            SeedQueryableSource();

            var firstItem = Queryable.First();

            Assert.Multiple(() =>
            {
                var ex1 = Assert.Throws<NotSupportedException>(() =>
                {
                    var actualResult1 = Queryable.SkipWhile(x => x.Equals(firstItem)).ToList();
                });

                //Assert.That(ex1.Message, Is.EqualTo(string.Format(ExceptionMessages.TranslationFailedExceptionMessage, typeof(T).Name)));

                var ex2 = Assert.Throws<NotSupportedException>(() =>
                {
                    var actualResult2 = Queryable.SkipWhile(x => x.Equals(firstItem)).ToList();
                });

                //Assert.That(ex2.Message, Is.EqualTo(string.Format(ExceptionMessages.TranslationFailedExceptionMessage, typeof(T).Name)));
            });
        }

        [Test]
        public virtual void Take_One_ReturnsFirstItem()
        {
            SeedQueryableSource();

            var firstItem = Queryable.First();

            var actualResult1 = Queryable.Take(1);
            var actualResult2 = Queryable.Take(1);

            Assert.Multiple(() =>
            {
                Assert.That(actualResult1.Count(), Is.EqualTo(1));
                Assert.That(actualResult1.First(), Is.EqualTo(firstItem));

                Assert.That(actualResult2.Count(), Is.EqualTo(1));
                Assert.That(actualResult2.First(), Is.EqualTo(firstItem));
            });
        }

        [Test]
        public virtual void TakeWhile_TakeFirstItem_ThrowsException()
        {
            SeedQueryableSource();

            var firstItem = Queryable.First();

            Assert.Multiple(() =>
            {
                var ex1 = Assert.Throws<NotSupportedException>(() =>
                {
                    var actualResult1 = Queryable.TakeWhile(x => x.Equals(firstItem)).ToList();
                });

                //Assert.That(ex1.Message, Is.EqualTo(string.Format(ExceptionMessages.TranslationFailedExceptionMessage, typeof(T).Name)));

                var ex2 = Assert.Throws<NotSupportedException>(() =>
                {
                    var actualResult2 = Queryable.TakeWhile(x => x.Equals(firstItem)).ToList();
                });

                //Assert.That(ex2.Message, Is.EqualTo(string.Format(ExceptionMessages.TranslationFailedExceptionMessage, typeof(T).Name)));
            });
        }

        [Test]
        public virtual void TakeWhile_TakeFirstItemUsingIndex_ThrowsException()
        {
            SeedQueryableSource();

            Assert.Multiple(() =>
            {
                var ex1 = Assert.Throws<NotSupportedException>(() =>
                {
                    var actualResult1 = Queryable.TakeWhile((x, i) => i.Equals(0)).ToList();
                });

                //Assert.That(ex1.Message, Is.EqualTo(string.Format(ExceptionMessages.TranslationFailedExceptionMessage, typeof(T).Name)));

                var ex2 = Assert.Throws<NotSupportedException>(() =>
                {
                    var actualResult2 = Queryable.TakeWhile((x, i) => i.Equals(0)).ToList();
                });

                //Assert.That(ex2.Message, Is.EqualTo(string.Format(ExceptionMessages.TranslationFailedExceptionMessage, typeof(T).Name)));
            });
        }

        [Test]
        public virtual async Task ToListAsync_ReturnsList()
        {
            SeedQueryableSource();

            var expectedResult = Queryable.ToList();

            var actualResult1 = await Queryable.ToListAsync();
            var actualResult2 = await Queryable.ToListAsync();

            Assert.Multiple(() =>
            {
                Assert.That(actualResult1, Is.EquivalentTo(expectedResult));
                Assert.That(actualResult2, Is.EquivalentTo(expectedResult));
            });
        }

        [Test]
        public virtual void Where_Condition_ReturnsItemsThatSatisfyCondition()
        {
            SeedQueryableSource();

            var actualResult1 = Queryable.Where(x => !x.Id.Equals(default)).ToList();
            var actualResult2 = Queryable.Where(x => !x.Id.Equals(default)).ToList();

            Assert.Multiple(() =>
            {
                for (var i = 0; i < ItemsAddedToQueryableSource.Count; i++)
                {
                    var item = ItemsAddedToQueryableSource[i];
                    Assert.That(item.Id, Is.Not.EqualTo(default(Guid)));

                    Assert.That(actualResult1[i], Is.EqualTo(item));
                    Assert.That(actualResult1[i].Id, Is.Not.EqualTo(default(Guid)));

                    Assert.That(actualResult2[i], Is.EqualTo(item));
                    Assert.That(actualResult2[i].Id, Is.Not.EqualTo(default(Guid)));
                }
            });
        }

        [Test]
        public async Task ProjectToThenToListAsync_ReturnsExpectedResult()
        {
            SeedQueryableSource();

            var expectedResult = new AsyncEnumerable<TestViewModel>(Queryable.Select(x => new TestViewModel { id = x.Id, fullName = x.FullName }));

            var mapper = new Mapper(new MapperConfiguration(x => x.AddProfile(new MappingProfile())));

            Console.WriteLine("ProjectTo about to be invoked");

            var actualResult = await mapper.ProjectTo<TestViewModel>(Queryable, null).ToListAsync();

            var compareLogic = new CompareLogic { Config = { IgnoreObjectTypes = true, IgnoreCollectionOrder = true } };
            var comparisonResult = compareLogic.Compare(expectedResult, actualResult);

            Assert.That(comparisonResult.AreEqual, Is.True);
        }
    }
}