using Domain.Entities;
using Moq;
using NUnit.Framework;
using Repo.Interfaces;
using Services.FoundValues;
using Services.Implementation.Search;
using Services.Interfaces.Search;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Services.UnitTests.Search
{
    public class AggregateSearchTests
    {
        public string Query { get; private set; }
        public string[] QueryWords { get; private set; }
        public List<FoundObject> UsersFoundObjects { get; private set; }

        [SetUp]
        public void SetUpUnitTest()
        {
            QueryWords = new string[] { "first", "second", "third" };
            Query = string.Join(' ', QueryWords);

            UsersFoundObjects = new List<FoundObject>();

            foreach (var word in QueryWords)
            {
                User user = new User { Id = Guid.NewGuid() };

                FoundObject foundObject = new FoundObject
                {
                    Entity = user,
                    MatchedWords = new List<string> { word },
                    TotalRating = word.Length * 2
                };

                UsersFoundObjects.Add(foundObject);
            }
        }

        [Test]
        public async Task FindAsync_AccurateSearchReturnsEmptyCollection_ReturnsCollectionFromSearchByWordsClass()
        {
            List<FoundObject> found = new List<FoundObject>();
            Mock<IRepository> _repository = new Mock<IRepository>();
            Mock<ISearch<User>> _accurateSearch = new Mock<ISearch<User>>();
            _accurateSearch.Setup(a => a.FindAsync(Query))
                .ReturnsAsync(found);
            Mock<ISearch<User>> _searchByWords = new Mock<ISearch<User>>();
            _searchByWords.Setup(a => a.FindAsync(Query))
                .ReturnsAsync(UsersFoundObjects);

            TestingAggregateSearch<User> testing = new TestingAggregateSearch<User>(_repository.Object, _accurateSearch.Object, _searchByWords.Object);
            List<FoundObject> foundObjects = await testing.FindAsync(Query);

            CollectionAssert.AreEquivalent(UsersFoundObjects, foundObjects);
        }

        [Test]
        public async Task FindAsync_SearchByWordsReturnsEmptyCollection_ReturnsCollectionFromAccurateSearchClass()
        {
            List<FoundObject> found = new List<FoundObject>();
            Mock<IRepository> _repository = new Mock<IRepository>();
            Mock<ISearch<User>> _accurateSearch = new Mock<ISearch<User>>();

            UsersFoundObjects = UsersFoundObjects
                .OrderByDescending(f => f.TotalRating)
                .ThenByDescending(f => f.MatchedWords.Count)
                .ToList();

            _accurateSearch.Setup(a => a.FindAsync(Query))
                .ReturnsAsync(UsersFoundObjects);
            Mock<ISearch<User>> _searchByWords = new Mock<ISearch<User>>();
            _searchByWords.Setup(a => a.FindAsync(Query))
                .ReturnsAsync(found);

            TestingAggregateSearch<User> testing = new TestingAggregateSearch<User>(_repository.Object, _accurateSearch.Object, _searchByWords.Object);
            List<FoundObject> foundObjects = await testing.FindAsync(Query);

            CollectionAssert.AreEqual(UsersFoundObjects, foundObjects);
        }

        [Test]
        public async Task FindAsync_SearchClassesReturnsEmptyCollection_ReturnsEmptyCollection()
        {
            List<FoundObject> accurateSearchFound = new List<FoundObject>();
            List<FoundObject> searchByWordsFound = new List<FoundObject>();

            Mock<IRepository> _repository = new Mock<IRepository>();
            Mock<ISearch<User>> _accurateSearch = new Mock<ISearch<User>>();

            _accurateSearch.Setup(a => a.FindAsync(Query))
                .ReturnsAsync(accurateSearchFound);
            Mock<ISearch<User>> _searchByWords = new Mock<ISearch<User>>();
            _searchByWords.Setup(a => a.FindAsync(Query))
                .ReturnsAsync(searchByWordsFound);

            TestingAggregateSearch<User> testing = new TestingAggregateSearch<User>(_repository.Object, _accurateSearch.Object, _searchByWords.Object);
            List<FoundObject> foundObjects = await testing.FindAsync(Query);

            CollectionAssert.IsEmpty(foundObjects);
        }

        [Test]
        public async Task FindAsync_SearchClassesReturnsSameCollections_ReturnsCollectionWithUniqueItems()
        {
            Mock<IRepository> _repository = new Mock<IRepository>();
            Mock<ISearch<User>> _accurateSearch = new Mock<ISearch<User>>();
            _accurateSearch.Setup(a => a.FindAsync(Query))
                .ReturnsAsync(UsersFoundObjects);
            Mock<ISearch<User>> _searchByWords = new Mock<ISearch<User>>();
            _searchByWords.Setup(a => a.FindAsync(Query))
                .ReturnsAsync(UsersFoundObjects);

            TestingAggregateSearch<User> testing = new TestingAggregateSearch<User>(_repository.Object, _accurateSearch.Object, _searchByWords.Object);
            List<FoundObject> foundObjects = await testing.FindAsync(Query);

            CollectionAssert.AllItemsAreUnique(foundObjects);
        }
    }

    internal class TestingAggregateSearch<T> : AggregateSearch<T>
        where T : Entity, new()
    {
        protected override ISearch<T> AccurateSearch { get; set; }
        protected override ISearch<T> SearchByWords { get; set; }
        public TestingAggregateSearch(IRepository repository, ISearch<T> accurateSearch, ISearch<T> searchByWords) : base(repository)
        {
            AccurateSearch = accurateSearch;
            SearchByWords = searchByWords;
        }
    }
}
