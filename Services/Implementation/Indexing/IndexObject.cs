using Domain.Entities;
using Repo.Interfaces;
using Services.IndexedValues;
using Services.Interfaces.Indexing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;

namespace Services.Implementation.Indexing
{
    public class IndexObject
    {
        private IndexedObject IndexedObject { get; set; }
        private List<FoundWord> FoundWords { get; set; }
        private IRepository Repository { get; }
        private IRateStrategy RateStrategy { get; }
        public IndexObject(IRepository repository,
            IRateStrategy rateStrategy)
        {
            Repository = repository;
            RateStrategy = rateStrategy;
        }

        public List<FoundWord> Index(IndexedObject indexedObject)
        {
            IndexedObject = indexedObject;
            if (FoundWords.Any())
                FoundWords = new List<FoundWord>();

            PropertyInfo[] propertyInfos = IndexedObject.GetType()
                .GetProperties(BindingFlags.Public | BindingFlags.Instance);

            foreach (PropertyInfo propertyInfo in propertyInfos)
            {
                if (propertyInfo.GetType() == typeof(string))
                {
                    string propertyValue = (string)propertyInfo.GetValue(IndexedObject);
                    propertyValue = RemovePunctuation(propertyValue);
                    List<string> values = GetWords(propertyValue);

                    foreach (string value in values)
                    {
                        FoundWord foundWord = GetFoundWord(value);
                        IndexInfo indexInfo = foundWord.IndexInfos.Find(i => i.EntityId == IndexedObject.Id);
                        if (indexInfo == default)
                        {
                            indexInfo = new IndexInfo
                            {
                                EntityId = IndexedObject.Id,
                                EntityType = IndexedObject.TypeOfIndexed.ToString()
                            };
                        }

                        int count = Count(values, value);
                        int propertyRank = Rate(propertyInfo.Name);
                        indexInfo.Rank += Summup(propertyRank, count);
                        foundWord.IndexInfos.Add(indexInfo);

                        AddOrUpdateIndexes(foundWord);
                    }
                }
            }

            return FoundWords;
        }

        private string RemovePunctuation(string values)
        {
            return Regex.Replace(values, "[-.?!)(,:;/|*+@#$%^&*]", "");
        }

        private List<string> GetWords(string value)
        {
            return value.Split(new string[] { Environment.NewLine, " " }, StringSplitOptions.RemoveEmptyEntries).ToList();
        }

        private FoundWord GetFoundWord(string word)
        {
            FoundWord foundWord = FoundWords.Find(f => f.Word.Equals(word));
            if (foundWord == default)
            {
                foundWord = Repository.FoundWords
                    .Find(f => f.Word.Equals(word))
                    .FirstOrDefault();
                if (foundWord == default)
                    foundWord = new FoundWord { Word = word };
            }
            return foundWord;
        }

        private void AddOrUpdateIndexes(FoundWord foundWord)
        {
            if (!FoundWords.Contains(foundWord))
            {
                FoundWords.Add(foundWord);
            }
            else
            {
                int i = FoundWords.IndexOf(foundWord);
                FoundWords[i] = foundWord;
            }
        }

        private int Count(List<string> values, string value)
        {
            return values.Count(v => v.Equals(value));
        }

        protected virtual int Rate(string propertyName)
        {
            return RateStrategy.RateElement(propertyName);
        }

        protected virtual int Summup(int rate, int count)
        {
            return rate * count;
        }
    }
}
