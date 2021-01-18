using Domain.Entities;
using Moq;
using NUnit.Framework;
using Repo.Interfaces;
using Services.FoundValues;
using Services.Implementation.Search;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Services.UnitTests.Search
{
    [TestFixture]
    public class AccurateSearchTests
    {
        [Test]
        public async Task FindAsync_Query_ReturnsListOfFoundObjects()
        {
            string query = "smth";

            User user1 = new User { Id = Guid.NewGuid() };
            User user2 = new User { Id = Guid.NewGuid() };
            User user3 = new User { Id = Guid.NewGuid() };
            User user4 = new User { Id = Guid.NewGuid() };
            List<User> users = new List<User> { user1, user2, user3, user4 };

            Mock<IRepository> _repository = new Mock<IRepository>();
            _repository.Setup(r => r.Users.FindAsync(It.IsAny<Func<User, bool>>()))
                .ReturnsAsync(users);

            AccurateSearch<User> accurateSearch = new AccurateSearch<User>(_repository.Object);

            List<FoundObject<User>> actual = await accurateSearch.FindAsync(query);

            var result = from obj in actual
                         where obj.TotalRating == int.MaxValue
                         where obj.MatchedWords.Count == 1
                         where obj.MatchedWords.Contains(query)
                         select obj;

            Assert.IsTrue(result.Count() == users.Count);
        }

        [Test]
        public async Task FindAsync_Query_FindsNothingAsync()
        {
            string query = "smth";
            Mock<IRepository> _repository = new Mock<IRepository>();
            IEnumerable<User> users = null;
            _repository.Setup(r => r.Users.FindAsync(It.IsAny<Func<User, bool>>()))
                .ReturnsAsync(users);

            AccurateSearch<User> accurateSearch = new AccurateSearch<User>(_repository.Object);
            var result = await accurateSearch.FindAsync(query);

            CollectionAssert.IsEmpty(result);
        }

        [Test]
        public void FindAsync_NotValidGenericParameter_ThrowsArgumentException()
        {
            string query = "smth";
            Mock<IRepository> _repository = new Mock<IRepository>();

            AccurateSearch<Topic> accurateSearch = new AccurateSearch<Topic>(_repository.Object);

            Assert.ThrowsAsync(typeof(ArgumentException), () => accurateSearch.FindAsync(query));
        }

        [Test]
        public void FindAsync_NullStringQuery_ThrowsArgumentNullException()
        {
            string query = null;
            IEnumerable<User> users = null;
            Mock<IRepository> _repository = new Mock<IRepository>();
            _repository.Setup(r => r.Users.FindAsync(It.IsAny<Func<User, bool>>()))
                .ReturnsAsync(users);
            AccurateSearch<User> accurateSearch = new AccurateSearch<User>(_repository.Object);

            Assert.ThrowsAsync(typeof(ArgumentNullException), () => accurateSearch.FindAsync(query));
        }

        [Test]
        public void FindAsync_EmptyStringQuery_ThrowsArgumentNullException()
        {
            string query = string.Empty;
            IEnumerable<User> users = null;
            Mock<IRepository> _repository = new Mock<IRepository>();
            _repository.Setup(r => r.Users.FindAsync(It.IsAny<Func<User, bool>>()))
                .ReturnsAsync(users);

            AccurateSearch<User> accurateSearch = new AccurateSearch<User>(_repository.Object);

            Assert.ThrowsAsync(typeof(ArgumentNullException), () => accurateSearch.FindAsync(query));
        }
    }
}
