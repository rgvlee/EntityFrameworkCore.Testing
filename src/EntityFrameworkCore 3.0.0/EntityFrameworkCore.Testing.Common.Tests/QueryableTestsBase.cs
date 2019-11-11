using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoFixture;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;

namespace EntityFrameworkCore.Testing.Common.Tests
{
    [TestFixture]
    public abstract class QueryableTestsBase<T> : TestBase
        where T : TestEntityBase
    {
        protected abstract IQueryable<T> Queryable { get; }
        protected abstract void SeedQueryableSource();
        protected List<T> ItemsAddedToQueryableSource;

        [Test]
        public virtual void All_FalseCondition_ReturnsFalse()
        {
            SeedQueryableSource();

            var actualResult1 = Queryable.All(x => string.IsNullOrWhiteSpace(x.String));
            var actualResult2 = Queryable.All(x => string.IsNullOrWhiteSpace(x.String));

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

            var actualResult1 = Queryable.All(x => !string.IsNullOrWhiteSpace(x.String));
            var actualResult2 = Queryable.All(x => !string.IsNullOrWhiteSpace(x.String));

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

            var actualResult1 = await Queryable.AllAsync(x => string.IsNullOrWhiteSpace(x.String));
            var actualResult2 = await Queryable.AllAsync(x => string.IsNullOrWhiteSpace(x.String));

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

            var actualResult1 = await Queryable.AllAsync(x => !string.IsNullOrWhiteSpace(x.String));
            var actualResult2 = await Queryable.AllAsync(x => !string.IsNullOrWhiteSpace(x.String));

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
        public virtual void Average_Int_ReturnsAverage()
        {
            SeedQueryableSource();

            var actualResult1 = Queryable.Average(x => x.Int);
            var actualResult2 = Queryable.Average(x => x.Int);

            Assert.Multiple(() =>
            {
                var average = 0d;
                for (var i = 0; i < ItemsAddedToQueryableSource.Count; i++)
                {
                    average += ItemsAddedToQueryableSource[i].Int;
                }

                average = average / ItemsAddedToQueryableSource.Count;

                Assert.That(actualResult1, Is.EqualTo(average));
                Assert.That(actualResult2, Is.EqualTo(average));
            });
        }

        [Test]
        public virtual async Task AverageAsync_Int_ReturnsAverage()
        {
            SeedQueryableSource();

            var actualResult1 = await Queryable.AverageAsync(x => x.Int);
            var actualResult2 = await Queryable.AverageAsync(x => x.Int);

            Assert.Multiple(() =>
            {
                var average = 0d;
                for (var i = 0; i < ItemsAddedToQueryableSource.Count; i++)
                {
                    average += ItemsAddedToQueryableSource[i].Int;
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
        public virtual void ElementAt_ReturnsElementAtSpecifiedIndex()
        {
            SeedQueryableSource();

            var firstElement = Queryable.ElementAt(0);
            var lastElement = Queryable.ElementAt(Queryable.ToList().Count - 1);

            Assert.Multiple(() =>
            {
                Assert.That(firstElement, Is.EqualTo(ItemsAddedToQueryableSource[0]));
                Assert.That(lastElement, Is.EqualTo(ItemsAddedToQueryableSource[ItemsAddedToQueryableSource.Count - 1]));
            });
        }

        [Test]
        public virtual void ElementAtOrDefault_ReturnsElementAtSpecifiedIndex()
        {
            SeedQueryableSource();

            var firstElement = Queryable.ElementAtOrDefault(0);
            var lastElement = Queryable.ElementAtOrDefault(Queryable.ToList().Count - 1);

            Assert.Multiple(() =>
            {
                Assert.That(firstElement, Is.EqualTo(ItemsAddedToQueryableSource[0]));
                Assert.That(lastElement, Is.EqualTo(ItemsAddedToQueryableSource[ItemsAddedToQueryableSource.Count - 1]));
            });
        }

        [Test]
        public virtual void ElementAtOrDefault_WithNoItemsAdded_ReturnsDefault()
        {
            var firstElement = Queryable.ElementAtOrDefault(0);
            var lastElement = Queryable.ElementAtOrDefault(Queryable.ToList().Count - 1);

            Assert.Multiple(() =>
            {
                Assert.That(firstElement, Is.EqualTo(default(T)));
                Assert.That(lastElement, Is.EqualTo(default(T)));
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
        public virtual void IndexedSelectThenWhereThenAny_TrueCondition_ReturnsTrue()
        {
            SeedQueryableSource();

            var actualResult1 = Queryable.Select((x, i) => new {Index = i, Item = x}).Where(x => !x.Index.Equals(0)).Any();
            var actualResult2 = Queryable.Select((x, i) => new {Index = i, Item = x}).Where(x => !x.Index.Equals(0)).Any();

            Assert.Multiple(() =>
            {
                Assert.That(actualResult1, Is.True);
                Assert.That(actualResult2, Is.True);
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

            var actualResult1 = Queryable.Max(x => x.DateTime);
            var actualResult2 = Queryable.Max(x => x.DateTime);

            Assert.Multiple(() =>
            {
                var maxDateTime = ItemsAddedToQueryableSource[0].DateTime;
                for (var i = 1; i < ItemsAddedToQueryableSource.Count; i++)
                {
                    if (ItemsAddedToQueryableSource[i].DateTime > maxDateTime)
                    {
                        maxDateTime = ItemsAddedToQueryableSource[i].DateTime;
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

            var actualResult1 = await Queryable.MaxAsync(x => x.DateTime);
            var actualResult2 = await Queryable.MaxAsync(x => x.DateTime);

            Assert.Multiple(() =>
            {
                var maxDateTime = ItemsAddedToQueryableSource[0].DateTime;
                for (var i = 1; i < ItemsAddedToQueryableSource.Count; i++)
                {
                    if (ItemsAddedToQueryableSource[i].DateTime > maxDateTime)
                    {
                        maxDateTime = ItemsAddedToQueryableSource[i].DateTime;
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

            var actualResult1 = Queryable.Min(x => x.DateTime);
            var actualResult2 = Queryable.Min(x => x.DateTime);

            Assert.Multiple(() =>
            {
                var minDateTime = ItemsAddedToQueryableSource[0].DateTime;
                for (var i = 1; i < ItemsAddedToQueryableSource.Count; i++)
                {
                    if (ItemsAddedToQueryableSource[i].DateTime < minDateTime)
                    {
                        minDateTime = ItemsAddedToQueryableSource[i].DateTime;
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

            var actualResult1 = await Queryable.MinAsync(x => x.DateTime);
            var actualResult2 = await Queryable.MinAsync(x => x.DateTime);

            Assert.Multiple(() =>
            {
                var minDateTime = ItemsAddedToQueryableSource[0].DateTime;
                for (var i = 1; i < ItemsAddedToQueryableSource.Count; i++)
                {
                    if (ItemsAddedToQueryableSource[i].DateTime < minDateTime)
                    {
                        minDateTime = ItemsAddedToQueryableSource[i].DateTime;
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

            var actualResult1 = Queryable.OrderBy(x => x.DateTime).ToList();
            var actualResult2 = Queryable.OrderBy(x => x.DateTime).ToList();

            Assert.Multiple(() =>
            {
                for (var i = 1; i < actualResult1.Count; i++)
                {
                    Assert.That(actualResult1[i].DateTime, Is.GreaterThanOrEqualTo(actualResult1[i - 1].DateTime));
                }

                for (var i = 1; i < actualResult2.Count; i++)
                {
                    Assert.That(actualResult2[i].DateTime, Is.GreaterThanOrEqualTo(actualResult2[i - 1].DateTime));
                }
            });
        }

        [Test]
        public virtual void OrderByDescending_DateTime_ReturnsItemsInDescendingOrder()
        {
            SeedQueryableSource();

            var actualResult1 = Queryable.OrderByDescending(x => x.DateTime).ToList();
            var actualResult2 = Queryable.OrderByDescending(x => x.DateTime).ToList();

            Assert.Multiple(() =>
            {
                for (var i = 1; i < actualResult1.Count; i++)
                {
                    Assert.That(actualResult1[i].DateTime, Is.LessThanOrEqualTo(actualResult1[i - 1].DateTime));
                }

                for (var i = 1; i < actualResult2.Count; i++)
                {
                    Assert.That(actualResult2[i].DateTime, Is.LessThanOrEqualTo(actualResult2[i - 1].DateTime));
                }
            });
        }

        [Test]
        public virtual void OrderByThenOrderBy_FixedDateTimeAndInt_ReturnsItemsInAscendingOrder()
        {
            SeedQueryableSource();

            var actualResult1 = Queryable.OrderBy(x => x.FixedDateTime).ThenBy(x => x.Int).ToList();
            var actualResult2 = Queryable.OrderBy(x => x.FixedDateTime).ThenBy(x => x.Int).ToList();

            Assert.Multiple(() =>
            {
                for (var i = 1; i < actualResult1.Count; i++)
                {
                    Assert.That(actualResult1[i].Int, Is.GreaterThanOrEqualTo(actualResult1[i - 1].Int));
                }

                for (var i = 1; i < actualResult2.Count; i++)
                {
                    Assert.That(actualResult2[i].Int, Is.GreaterThanOrEqualTo(actualResult2[i - 1].Int));
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
        public virtual void Select_WithIndex_ReturnsIndexedSequence()
        {
            SeedQueryableSource();

            var actualResult1 = Queryable.Select((x, i) => new {Index = i, Item = x}).ToList();
            var actualResult2 = Queryable.Select((x, i) => new {Index = i, Item = x}).ToList();

            Assert.Multiple(() =>
            {
                for (var i = 0; i < ItemsAddedToQueryableSource.Count; i++)
                {
                    Assert.That(actualResult1[i].Index, Is.EqualTo(i));
                    Assert.That(actualResult1[i].Item, Is.EqualTo(ItemsAddedToQueryableSource[i]));

                    Assert.That(actualResult2[i].Index, Is.EqualTo(i));
                    Assert.That(actualResult2[i].Item, Is.EqualTo(ItemsAddedToQueryableSource[i]));
                }
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
        public virtual void SkipWhile_SkipFirstItem_ReturnsSequenceThatDoesNotIncludeFirstItem()
        {
            SeedQueryableSource();

            var firstItem = Queryable.First();

            var actualResult1 = Queryable.SkipWhile(x => x.Equals(firstItem));
            var actualResult2 = Queryable.SkipWhile(x => x.Equals(firstItem));

            Assert.Multiple(() =>
            {
                Assert.That(actualResult1.Contains(firstItem), Is.False);
                Assert.That(actualResult1.Count(), Is.EqualTo(Queryable.Count() - 1));

                Assert.That(actualResult2.Contains(firstItem), Is.False);
                Assert.That(actualResult2.Count(), Is.EqualTo(Queryable.Count() - 1));
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
        public virtual void TakeWhile_TakeFirstItem_ReturnsFirstItem()
        {
            SeedQueryableSource();

            var firstItem = Queryable.First();

            var actualResult1 = Queryable.TakeWhile(x => x.Equals(firstItem));
            var actualResult2 = Queryable.TakeWhile(x => x.Equals(firstItem));

            Assert.Multiple(() =>
            {
                Assert.That(actualResult1.Count(), Is.EqualTo(1));
                Assert.That(actualResult1.First(), Is.EqualTo(firstItem));

                Assert.That(actualResult2.Count(), Is.EqualTo(1));
                Assert.That(actualResult2.First(), Is.EqualTo(firstItem));
            });
        }

        [Test]
        public virtual void TakeWhile_TakeFirstItemUsingIndex_ReturnsFirstItem()
        {
            SeedQueryableSource();

            var firstItem = Queryable.First();

            var actualResult1 = Queryable.TakeWhile((x, i) => i.Equals(0));
            var actualResult2 = Queryable.TakeWhile((x, i) => i.Equals(0));

            Assert.Multiple(() =>
            {
                Assert.That(actualResult1.Count(), Is.EqualTo(1));
                Assert.That(actualResult1.First(), Is.EqualTo(firstItem));

                Assert.That(actualResult2.Count(), Is.EqualTo(1));
                Assert.That(actualResult2.First(), Is.EqualTo(firstItem));
            });
        }

        [Test]
        public virtual void Where_Condition_ReturnsItemsThatSatisfyCondition()
        {
            SeedQueryableSource();

            var actualResult1 = Queryable.Where(x => !x.Guid.Equals(default)).ToList();
            var actualResult2 = Queryable.Where(x => !x.Guid.Equals(default)).ToList();

            Assert.Multiple(() =>
            {
                for (var i = 0; i < ItemsAddedToQueryableSource.Count; i++)
                {
                    var item = ItemsAddedToQueryableSource[i];
                    Assert.That(item.Guid, Is.Not.EqualTo(default(Guid)));

                    Assert.That(actualResult1[i], Is.EqualTo(item));
                    Assert.That(actualResult1[i].Guid, Is.Not.EqualTo(default(Guid)));

                    Assert.That(actualResult2[i], Is.EqualTo(item));
                    Assert.That(actualResult2[i].Guid, Is.Not.EqualTo(default(Guid)));
                }
            });
        }

        [Test]
        public void ContainsListCollection_ReturnsFalse()
        {
            var containsListCollection = ((IListSource)Queryable).ContainsListCollection;
            Assert.That(containsListCollection, Is.False);
        }
    }
}