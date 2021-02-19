using Domain.Entities;
using Moq;
using NUnit.Framework;
using Repo.Interfaces;
using Services.Implementation.Indexing;
using Services.VisibleValues;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Services.UnitTests.Indexing
{
    [TestFixture]
    public class IndexedObjectsObserverTests
    {
        [Test]
        public async Task OnUpdatedEntityAsync_GetsAFullyModifiedArticle_DeletesOldInfo()
        {
            Article article = new Article { Id = Guid.NewGuid(), Header = "something something_else", User = new User() };
            IEnumerable<FoundWord> foundWords1 = null, foundWords2 = null, foundWords3 = null;
            Mock<IRepository> _repository = new Mock<IRepository>();
            _repository.SetupSequence(r => r.FoundWords.Find(It.IsAny<Func<FoundWord, bool>>()))
                .Returns(foundWords1)
                .Returns(foundWords2);

            IndexObject indexObject = new IndexObject(_repository.Object);
            List<FoundWord> oldFoundWords = indexObject.Index((VisibleArticleValues)article);

            List<IndexInfo> indexInfos = new List<IndexInfo>();
            foreach (var foundWord in oldFoundWords)
            {
                foreach (var indexInfo in foundWord.IndexInfos)
                {
                    indexInfo.FoundWord = foundWord;
                }
                indexInfos.AddRange(foundWord.IndexInfos);
            }

            Mock<IRepository> _repository2 = new Mock<IRepository>();
            _repository2.SetupSequence(r => r.FoundWords.Find(It.IsAny<Func<FoundWord, bool>>()))
                .Returns(foundWords3)
                .Returns(new List<FoundWord> { oldFoundWords[0] });
            _repository2.Setup(r => r.IndexInfos.Find(It.IsAny<Func<IndexInfo, bool>>()))
                .Returns(indexInfos);

            article.Header = "something";
            IndexedObjectsObserver observer = new IndexedObjectsObserver(_repository2.Object);
            await observer.CheckUpdatedEntityAsync((VisibleArticleValues)article);

            _repository2.Verify(r => r.FoundWords.UpdateRangeAsync(It.IsNotNull<IEnumerable<FoundWord>>()));
            _repository2.Verify(r => r.FoundWords.DeleteRangeAsync(It.IsNotNull<IEnumerable<FoundWord>>()));
        }

        [Test]
        public async Task OnUpdatedEntityAsync_GetsModifiedArticleWithoutDeletedWords_UpdatesInfo()
        {
            Article article = new Article { Id = Guid.NewGuid(), Header = "something", User = new User() };

            Mock<IRepository> _repository = new Mock<IRepository>();
            _repository.Setup(r => r.FoundWords.Find(It.IsAny<Func<FoundWord, bool>>()))
                .Returns<IEnumerable<FoundWord>>(null);

            IndexObject indexObject = new IndexObject(_repository.Object);
            List<FoundWord> oldFoundWords = indexObject.Index((VisibleArticleValues)article);

            var indexInfos = oldFoundWords.ElementAt(0).IndexInfos;
            foreach (var indexInfo in indexInfos)
                indexInfo.FoundWord = oldFoundWords.ElementAt(0);

            _repository.Setup(r => r.FoundWords.Find(It.IsAny<Func<FoundWord, bool>>()))
                .Returns(oldFoundWords);
            _repository.Setup(r => r.IndexInfos.Find(It.IsAny<Func<IndexInfo, bool>>()))
                .Returns(indexInfos);

            article.Header = "something something";
            IndexedObjectsObserver observer = new IndexedObjectsObserver(_repository.Object);
            await observer.CheckUpdatedEntityAsync((VisibleArticleValues)article);

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
            IndexObject indexObject = new IndexObject(_repository.Object);
            List<FoundWord> oldFoundWords = indexObject.Index((VisibleArticleValues)article);

            var indexInfos = oldFoundWords.ElementAt(0).IndexInfos;
            foreach (var indexInfo in indexInfos)
                indexInfo.FoundWord = oldFoundWords.ElementAt(0);

            _repository.Setup(r => r.IndexInfos.Find(It.IsAny<Func<IndexInfo, bool>>()))
                .Returns(indexInfos);
            _repository.Setup(r => r.FoundWords.DeleteRangeAsync(It.IsAny<IEnumerable<FoundWord>>()));

            IndexedObjectsObserver observer = new IndexedObjectsObserver(_repository.Object);
            await observer.CheckDeletedEntityAsync((VisibleArticleValues)article);

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

            IndexObject indexObject = new IndexObject(_repository.Object);
            List<FoundWord> articleFoundWords = indexObject.Index((VisibleArticleValues)article);
            List<FoundWord> articleToDeleteFoundWords = indexObject.Index((VisibleArticleValues)articleToDelete);
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
            await observer.CheckDeletedEntityAsync((VisibleArticleValues)articleToDelete);

            _repository.Verify(r => r.FoundWords.Find(It.IsAny<Func<FoundWord, bool>>()), Times.Exactly(2));
            _repository.Verify(r => r.IndexInfos.Find(It.IsAny<Func<IndexInfo, bool>>()));
            _repository.Verify(r => r.IndexInfos.DeleteRangeAsync(It.IsAny<IEnumerable<IndexInfo>>()));
            _repository.VerifyNoOtherCalls();
        }
    }
}
