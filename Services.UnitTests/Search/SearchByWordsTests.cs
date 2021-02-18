using Domain.Entities;
using Moq;
using NUnit.Framework;
using Repo.Interfaces;
using Services.Implementation.Search;
using Services.Interfaces.Search;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Services.UnitTests.Search
{
    [TestFixture]
    public class SearchByWordsTests
    {
        public string OneWordQuery { get; private set; }
        public string ThreeWordsQuery { get; private set; }
        public string[] Queries { get; private set; }
        public List<FoundWord> FoundWordsWithUsersEntities { get; private set; }
        public List<FoundWord> FoundWordsWithArticlesEntities { get; private set; }

        [OneTimeSetUp]
        public void SetUpUnitTests()
        {
            OneWordQuery = "first";
            Queries = new string[] { OneWordQuery, "second", "third" };
            ThreeWordsQuery = string.Join(' ', Queries);

            FoundWordsWithUsersEntities = new List<FoundWord>();
            FoundWordsWithArticlesEntities = new List<FoundWord>();

            foreach (string query in Queries)
            {
                FoundWord foundWord1 = new FoundWord { Id = Guid.NewGuid(), Word = query };
                FoundWord foundWord2 = new FoundWord { Id = Guid.NewGuid(), Word = query };

                List<IndexInfo> indexInfos = new List<IndexInfo>(9);
                for (int i = 0; i < indexInfos.Capacity; i++)
                {
                    IndexInfo indexInfo = new IndexInfo { Id = Guid.NewGuid(), Rank = i * 5 + 1 };

                    if (i % 2 == 0)
                    {
                        User user = new User { Id = Guid.NewGuid() };
                        indexInfo.EntityId = user.Id;
                        indexInfo.Entity = user;
                        indexInfo.FoundWord = foundWord1;
                    }
                    else
                    {
                        Article article = new Article { Id = Guid.NewGuid() };
                        indexInfo.EntityId = article.Id;
                        indexInfo.Entity = article;
                        indexInfo.FoundWord = foundWord2;
                    }

                    indexInfos.Add(indexInfo);
                }

                foundWord1.IndexInfos = indexInfos
                    .Where(i => i.Entity is User)
                    .ToList();
                FoundWordsWithUsersEntities.Add(foundWord1);

                foundWord2.IndexInfos = indexInfos
                    .Where(i => i.Entity is Article)
                    .ToList();
                FoundWordsWithArticlesEntities.Add(foundWord2);
            }
        }

        [Test]
        public async Task FindAsync_RepositoryReturnsNull_ReturnsEmptyCollection()
        {
            IEnumerable<FoundWord> foundWords = null;
            Mock<IRepository> _repository = new Mock<IRepository>();
            _repository.Setup(r => r.FoundWords.FindAsync(It.IsNotNull<Func<FoundWord, bool>>()))
                .ReturnsAsync(foundWords);

            ISearch<User> search = new SearchByWords<User>(_repository.Object);
            var result = await search.FindAsync(OneWordQuery);

            CollectionAssert.IsEmpty(result);
        }

        [Test]
        public async Task FindAsync_RepositoryReturnsFoundWordWithOtherTypeObjects_ReturnsEmptyCollection()
        {
            Mock<IRepository> _repository = new Mock<IRepository>();
            _repository.SetupSequence(r => r.FoundWords.FindAsync(It.IsAny<Func<FoundWord, bool>>()))
                .ReturnsAsync(new List<FoundWord> { FoundWordsWithUsersEntities[0] })
                .ReturnsAsync(new List<FoundWord> { FoundWordsWithUsersEntities[1] })
                .ReturnsAsync(new List<FoundWord> { FoundWordsWithUsersEntities[2] });

            ISearch<Article> search = new SearchByWords<Article>(_repository.Object);
            var result = await search.FindAsync(ThreeWordsQuery);

            CollectionAssert.IsEmpty(result);
        }

        [Test]
        public async Task FindAsync_RepositoryReturnsFoundWordWithDifferentTypeObjects_ReturnsCollectionWithSearchByWordsParameterType()
        {
            Mock<IRepository> _repository = new Mock<IRepository>();
            _repository.SetupSequence(r => r.FoundWords.FindAsync(It.IsAny<Func<FoundWord, bool>>()))
                .ReturnsAsync(new List<FoundWord> { FoundWordsWithArticlesEntities[0] })
                .ReturnsAsync(new List<FoundWord> { FoundWordsWithUsersEntities[1] })
                .ReturnsAsync(new List<FoundWord> { FoundWordsWithArticlesEntities[2] });

            ISearch<Article> search = new SearchByWords<Article>(_repository.Object);
            var result = await search.FindAsync(ThreeWordsQuery);

            var checkingResult = result
                .Where(r => r.Entity is Article)
                .ToList();

            CollectionAssert.AreEquivalent(checkingResult, result);
        }

        [Test]
        public async Task FindAsync_AskSomething_ReturnsOrderedCollection()
        {
            Mock<IRepository> _repository = new Mock<IRepository>();
            _repository.SetupSequence(r => r.FoundWords.FindAsync(It.IsAny<Func<FoundWord, bool>>()))
                .ReturnsAsync(new List<FoundWord> { FoundWordsWithUsersEntities[0] })
                .ReturnsAsync(new List<FoundWord> { FoundWordsWithUsersEntities[1] })
                .ReturnsAsync(new List<FoundWord> { FoundWordsWithUsersEntities[2] });

            ISearch<User> search = new SearchByWords<User>(_repository.Object);
            var result = await search.FindAsync(ThreeWordsQuery);

            var orderedResult = result.OrderByDescending(r => r.TotalRating).ToList();
            CollectionAssert.AreEqual(orderedResult, result);
        }

        [Test]
        public async Task FindAsync_RepositoryReturnsFoundWordsWithTheSameEntitites_ReturnsCollectionWithUniqueEntities()
        {
            List<FoundWord> foundWords = new List<FoundWord>();
            foundWords.Add(FoundWordsWithUsersEntities[0]);
            foundWords.Add(FoundWordsWithUsersEntities[0]);
            foundWords.Add(FoundWordsWithUsersEntities[0]);

            Mock<IRepository> _repository = new Mock<IRepository>();
            _repository.SetupSequence(r => r.FoundWords.FindAsync(It.IsAny<Func<FoundWord, bool>>()))
                .ReturnsAsync(new List<FoundWord> { foundWords[0] })
                .ReturnsAsync(new List<FoundWord> { foundWords[1] })
                .ReturnsAsync(new List<FoundWord> { foundWords[2] });

            ISearch<User> search = new SearchByWords<User>(_repository.Object);
            var result = await search.FindAsync(ThreeWordsQuery);

            IEnumerable<Guid> ids = foundWords
                .SelectMany(f => f.IndexInfos)
                .Select(i => i.EntityId)
                .Distinct();
            IEnumerable<object> entities = foundWords
                .SelectMany(f => f.IndexInfos)
                .Select(i => i.Entity)
                .Distinct();

            CollectionAssert.AllItemsAreUnique(ids);
            CollectionAssert.AllItemsAreUnique(entities);
        }

        [Test]
        public void FindAsync_GetsNullQuery_ThrowsArgumentNullException()
        {
            string query = null;
            Mock<IRepository> _repository = new Mock<IRepository>();
            _repository.Setup(r => r.FoundWords.FindAsync(It.IsAny<Func<FoundWord, bool>>()))
                .ReturnsAsync(new List<FoundWord> { FoundWordsWithUsersEntities[0] });
            ISearch<User> search = new SearchByWords<User>(_repository.Object);

            Assert.ThrowsAsync(typeof(ArgumentNullException), () => search.FindAsync(query));
        }
    }
}
