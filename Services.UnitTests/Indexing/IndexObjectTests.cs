using Domain.Entities;
using Moq;
using NUnit.Framework;
using Repo.Interfaces;
using Services.Implementation.Indexing;
using Services.VisibleValues;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Services.UnitTests.Indexing
{
    [TestFixture]
    public class IndexObjectTests
    {
        [Test]
        public void Index_IndexingArticleWhenRepositoryReturnsNull_ReturnsListOfFoundWords()
        {
            string[] expected = new string[] { "article", "header", "login", "one_", "tag1", "topic1", "series1" };
            ArticleTag articleTag = new ArticleTag { Tag = new Tag { Name = expected[4] } };
            Article article = new Article
            {
                Id = Guid.NewGuid(),
                Header = $"{expected[0]} {expected[1]}",
                User = new User { Username = $"{expected[2]}" },
                Text = $"{expected[3]}",
                ArticleTags = new List<ArticleTag> { articleTag },
                Topic = new Topic { Name = $"{expected[5]}" },
                Series = new Series { Name = $"{expected[6]}" }
            };

            Mock<IRepository> _repository = new Mock<IRepository>();
            _repository.Setup(r => r.FoundWords.Find(It.IsAny<Func<FoundWord, bool>>()))
                .Returns<IEnumerable<FoundWord>>(null);

            IndexObject indexObject = new IndexObject(_repository.Object);
            List<FoundWord> foundWords = indexObject.Index((VisibleArticleValues)article);

            string[] actual = foundWords.Select(f => f.Word).ToArray();
            CollectionAssert.AreEquivalent(expected, actual);
        }

        [Test]
        public void Index_IndexingArticleWhenRepositoryReturnsNullAndPropertiesContainsSameWords_ReturnsListOfFoundWordsWith1IndexInfo()
        {
            string[] words = new string[] { "first-word", "second_word", "-third_word" };
            Article article = new Article
            {
                Id = Guid.NewGuid(),
                Header = $"{words[0]} {words[0]}",
                User = new User { Username = $"{words[1]}" },
                Text = $@"{words[0]}, {words[1]}, - {words[2]},
                {words[2]}. {words[2]}"
            };

            Mock<IRepository> _repository = new Mock<IRepository>();
            _repository.Setup(r => r.FoundWords.Find(It.IsAny<Func<FoundWord, bool>>()))
                .Returns<IEnumerable<FoundWord>>(null);

            IndexObject indexObject = new IndexObject(_repository.Object);

            List<FoundWord> foundWords = indexObject.Index((VisibleArticleValues)article);

            IEnumerable<FoundWord> result = from foundWord in foundWords
                                            from indexInfo in foundWord.IndexInfos
                                            where words.Contains(foundWord.Word)
                                            where foundWord.IndexInfos.Count == 1
                                            where indexInfo.EntityId == article.Id
                                            select foundWord;

            Assert.That(result.Count() == words.Length);
        }

        [Test]
        public void Index_IndexingArticleTwice_ReturnsListOfFoundWordsWithIdenticalFoundWords()
        {
            string word = "first-word";
            Article article = new Article { Id = Guid.NewGuid(), Header = word, User = new User() };
            Mock<IRepository> _repository1 = new Mock<IRepository>();
            _repository1.Setup(r => r.FoundWords.Find(It.IsAny<Func<FoundWord, bool>>()))
                .Returns<IEnumerable<FoundWord>>(null);
            IndexObject indexArticle1 = new IndexObject(_repository1.Object);

            List<FoundWord> firstFound = indexArticle1.Index((VisibleArticleValues)article);

            Mock<IRepository> _repository2 = new Mock<IRepository>();
            _repository2.Setup(r => r.FoundWords.Find(It.IsAny<Func<FoundWord, bool>>()))
                .Returns(firstFound);
            IndexObject indexArticle2 = new IndexObject(_repository2.Object);

            List<FoundWord> foundAfter = indexArticle2.Index((VisibleArticleValues)article);

            Assert.AreEqual(firstFound, foundAfter);
        }

        //[Test]
        //public void Index_IndexingUpdatedArticle_ReturnsListOfFoundWordsWithoutOldInfo()
        //{
        //    string[] words = new string[] { "first-word", "second_word" };
        //    Article article = new Article { Id = Guid.NewGuid(), Header = words[0], User = new User() };
        //    Mock<IRepository> _repository1 = new Mock<IRepository>();
        //    _repository1.Setup(r => r.FoundWords.Find(It.IsAny<Func<FoundWord, bool>>()))
        //        .Returns<IEnumerable<FoundWord>>(null);
        //    IndexObject indexArticle1 = new IndexObject(_repository1.Object, new ArticleRateStrategy());

        //    List<FoundWord> firstFound = indexArticle1.Index((ArticleIndexedValues)article);

        //    Mock<IRepository> _repository2 = new Mock<IRepository>();

        //    var parameter = It.Is<Func<FoundWord>>((f) => f.Word.Equals(""));

        //    _repository2.Setup(r => r.FoundWords.Find(It.IsAny<Func<FoundWord, bool>>()))
        //        .Returns(firstFound);
        //    IndexObject indexArticle2 = new IndexObject(_repository2.Object, new ArticleRateStrategy());
        //    article.Header = words[1];

        //    List<FoundWord> foundAfter = indexArticle2.Index((ArticleIndexedValues)article);

        //    Assert.AreNotEqual(firstFound, foundAfter);
        //}

        [Test]
        public void Index_IndexingArticleWhenThereIsNothingToIndex_ReturnsEmptyListOfFoundWords()
        {
            string[] words = new string[] { "fir", "sec", "-th" };
            Article article = new Article
            {
                Id = Guid.NewGuid(),
                Header = $@"{words[0]} {words[0]}",
                User = new User { Username = $"{words[2]}" },
                Text = $@"{words[0]}, {words[1]}, - {words[2]},
                {words[2]}. {words[2]}"
            };

            Mock<IRepository> _repository = new Mock<IRepository>();
            _repository.Setup(r => r.FoundWords.Find(It.IsAny<Func<FoundWord, bool>>()))
                .Returns<IEnumerable<FoundWord>>(null);
            IndexObject indexObject = new IndexObject(_repository.Object);

            List<FoundWord> foundWords = indexObject.Index((VisibleArticleValues)article);

            CollectionAssert.IsEmpty(foundWords);
        }

        [Test]
        public void Index_IndexingEmptyArticleWithUser_ReturnsEmptyListOfFoundWords()
        {
            Article article = new Article { User = new User() };
            Mock<IRepository> _repository = new Mock<IRepository>();
            _repository.Setup(r => r.FoundWords.Find(It.IsAny<Func<FoundWord, bool>>()))
                .Returns<IEnumerable<FoundWord>>(null);
            IndexObject indexObject = new IndexObject(_repository.Object);

            List<FoundWord> foundWords = indexObject.Index((VisibleArticleValues)article);

            CollectionAssert.IsEmpty(foundWords);
        }
    }
}
