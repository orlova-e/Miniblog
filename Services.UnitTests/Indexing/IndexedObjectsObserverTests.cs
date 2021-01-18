using Moq;
using NUnit.Framework;
using Repo.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using Domain.Entities;
using Services.Implementation.Indexing;
using Services.IndexedValues;
using System.Threading.Tasks;

namespace Services.UnitTests.Indexing
{
    [TestFixture]
    public class IndexedObjectsObserverTests
    {
        [Test]
        public async Task OnUpdatedEntityAsync_GetsAFullyModifiedArticle_DeletesOldInfo()
        {
            Article article = new Article { Id = Guid.NewGuid(), Header = "something", User = new User() };
            Mock<IRepository> _repository = new Mock<IRepository>();
            _repository.Setup(r => r.FoundWords.Find(It.IsAny<Func<FoundWord, bool>>()))
                .Returns<IEnumerable<FoundWord>>(null);
            IndexObject indexObject = new IndexObject(_repository.Object, new ArticleRateStrategy());
            List<FoundWord> oldFoundWords = indexObject.Index((ArticleIndexedValues)article);

            var indexInfos = oldFoundWords.ElementAt(0).IndexInfos;
            foreach (var indexInfo in indexInfos)
                indexInfo.FoundWord = oldFoundWords.ElementAt(0);

            _repository.Setup(r => r.IndexInfos.Find(It.IsAny<Func<IndexInfo, bool>>()))
                .Returns(indexInfos);
            _repository.Setup(r => r.FoundWords.CreateRangeAsync(It.IsNotNull<IEnumerable<FoundWord>>()));
            _repository.Setup(r => r.FoundWords.DeleteRangeAsync(It.IsNotNull<IEnumerable<FoundWord>>()));

            article.Header = "Anything";
            IndexedObjectsObserver observer = new IndexedObjectsObserver(_repository.Object);
            await observer.OnUpdatedEntityAsync((ArticleIndexedValues)article);

            _repository.Verify(r => r.FoundWords.Find(It.IsAny<Func<FoundWord, bool>>()), Times.AtLeast(2));
            _repository.Verify(r => r.IndexInfos.Find(It.IsAny<Func<IndexInfo, bool>>()));
            _repository.Verify(r => r.FoundWords.CreateRangeAsync(It.IsNotNull<IEnumerable<FoundWord>>()));
            _repository.Verify(r => r.FoundWords.DeleteRangeAsync(It.IsNotNull<IEnumerable<FoundWord>>()));
            _repository.VerifyNoOtherCalls();
        }

        [Test]
        public async Task OnUpdatedEntityAsync_GetsModifiedArticleWithoutDeletedWords_UpdatesInfo()
        {
            Article article = new Article { Id = Guid.NewGuid(), Header = "something", User = new User() };

            Mock<IRepository> _repository = new Mock<IRepository>();
            _repository.Setup(r => r.FoundWords.Find(It.IsAny<Func<FoundWord, bool>>()))
                .Returns<IEnumerable<FoundWord>>(null);

            IndexObject indexObject = new IndexObject(_repository.Object, new ArticleRateStrategy());
            List<FoundWord> oldFoundWords = indexObject.Index((ArticleIndexedValues)article);

            var indexInfos = oldFoundWords.ElementAt(0).IndexInfos;
            foreach (var indexInfo in indexInfos)
                indexInfo.FoundWord = oldFoundWords.ElementAt(0);

            _repository.Setup(r => r.FoundWords.Find(It.IsAny<Func<FoundWord, bool>>()))
                .Returns(oldFoundWords);
            _repository.Setup(r => r.IndexInfos.Find(It.IsAny<Func<IndexInfo, bool>>()))
                .Returns(indexInfos);
            _repository.Setup(r => r.FoundWords.UpdateRangeAsync(It.IsNotNull<IEnumerable<FoundWord>>()));

            article.Header = "something something";
            IndexedObjectsObserver observer = new IndexedObjectsObserver(_repository.Object);
            await observer.OnUpdatedEntityAsync((ArticleIndexedValues)article);

            _repository.Verify(r => r.FoundWords.Find(It.IsAny<Func<FoundWord, bool>>()), Times.AtLeast(2));
            _repository.Verify(r => r.IndexInfos.Find(It.IsAny<Func<IndexInfo, bool>>()));
            _repository.Verify(r => r.FoundWords.UpdateRangeAsync(It.IsNotNull<IEnumerable<FoundWord>>()));
            _repository.VerifyNoOtherCalls();
        }

        [Test]
        public async Task OnDeletedEntityAsync_GetsDeletedArticleWithUniqueFoundWords_DeletesAllInfo()
        {
            Article article = new Article { Id = Guid.NewGuid(), Header = "something", User = new User() };

            Mock<IRepository> _repository = new Mock<IRepository>();
            _repository.Setup(r => r.FoundWords.Find(It.IsAny<Func<FoundWord, bool>>()))
                .Returns<IEnumerable<FoundWord>>(null);
            IndexObject indexObject = new IndexObject(_repository.Object, new ArticleRateStrategy());
            List<FoundWord> oldFoundWords = indexObject.Index((ArticleIndexedValues)article);

            var indexInfos = oldFoundWords.ElementAt(0).IndexInfos;
            foreach (var indexInfo in indexInfos)
                indexInfo.FoundWord = oldFoundWords.ElementAt(0);

            _repository.Setup(r => r.IndexInfos.Find(It.IsAny<Func<IndexInfo, bool>>()))
                .Returns(indexInfos);
            _repository.Setup(r => r.FoundWords.DeleteRangeAsync(It.IsAny<IEnumerable<FoundWord>>()));

            IndexedObjectsObserver observer = new IndexedObjectsObserver(_repository.Object);
            await observer.OnDeletedEntityAsync((ArticleIndexedValues)article);

            _repository.Verify(r => r.FoundWords.Find(It.IsAny<Func<FoundWord, bool>>()));
            _repository.Verify(r => r.IndexInfos.Find(It.IsAny<Func<IndexInfo, bool>>()));
            _repository.Verify(r => r.FoundWords.DeleteRangeAsync(It.IsAny<IEnumerable<FoundWord>>()));
            _repository.VerifyNoOtherCalls();
        }

        [Test]
        public async Task OnDeletedEntityAsync_GetsDeletedArticleWithNonUniqueFoundWords_DeletesIndexInfo()
        {
            Article article = new Article { Id = Guid.NewGuid(), Header = "something", User = new User() };
            Article articleToDelete = new Article { Id = Guid.NewGuid(), Header = "something", User = new User() };

            Mock<IRepository> _repository = new Mock<IRepository>();
            _repository.Setup(r => r.FoundWords.Find(It.IsAny<Func<FoundWord, bool>>()))
                .Returns<IEnumerable<FoundWord>>(null);

            IndexObject indexObject = new IndexObject(_repository.Object, new ArticleRateStrategy());
            List<FoundWord> articleFoundWords = indexObject.Index((ArticleIndexedValues)article);
            List<FoundWord> articleToDeleteFoundWords = indexObject.Index((ArticleIndexedValues)articleToDelete);
            articleToDeleteFoundWords.ElementAt(0).IndexInfos.Add(articleFoundWords.ElementAt(0).IndexInfos[0]);
            List<IndexInfo> indexInfos = new List<IndexInfo>();
            foreach (var foundWord in articleToDeleteFoundWords)
                indexInfos.AddRange(foundWord.IndexInfos);
            foreach (var indexInfo in indexInfos)
                indexInfo.FoundWord = articleToDeleteFoundWords.ElementAt(0);

            _repository.Setup(r => r.IndexInfos.Find(It.IsAny<Func<IndexInfo, bool>>()))
                .Returns(indexInfos);
            _repository.Setup(r => r.IndexInfos.DeleteRangeAsync(It.IsAny<IEnumerable<IndexInfo>>()));

            IndexedObjectsObserver observer = new IndexedObjectsObserver(_repository.Object);
            await observer.OnDeletedEntityAsync((ArticleIndexedValues)articleToDelete);

            _repository.Verify(r => r.FoundWords.Find(It.IsAny<Func<FoundWord, bool>>()), Times.Exactly(2));
            _repository.Verify(r => r.IndexInfos.Find(It.IsAny<Func<IndexInfo, bool>>()));
            _repository.Verify(r => r.IndexInfos.DeleteRangeAsync(It.IsAny<IEnumerable<IndexInfo>>()));
            _repository.VerifyNoOtherCalls();
        }
    }
}
