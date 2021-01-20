﻿using Domain.Entities;
using Repo.Interfaces;
using Services.IndexedValues;
using Services.Interfaces.Indexing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

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
            FoundWords = new List<FoundWord>();

            PropertyInfo[] propertyInfos = IndexedObject.GetType()
                .GetProperties(BindingFlags.Public | BindingFlags.Instance);

            foreach (PropertyInfo propertyInfo in propertyInfos)
            {
                if (propertyInfo.PropertyType != typeof(string))
                    continue;

                string propertyValue = (string)propertyInfo.GetValue(IndexedObject);

                List<string> words = new List<string>();
                try
                {
                    words = InputPreparation.PrepareWithoutDistinct(propertyValue);
                }
                catch (ArgumentNullException)
                {
                    continue;
                }

                foreach (string word in words)
                {
                    FoundWord foundWord = GetFoundWord(word);
                    IndexInfo indexInfo = foundWord.IndexInfos.Find(i => i.EntityId == IndexedObject.Id);
                    int indexInfoPosition = foundWord.IndexInfos.IndexOf(indexInfo);

                    if (indexInfo == default)
                    {
                        indexInfo = new IndexInfo
                        {
                            EntityId = IndexedObject.Id,
                            EntityType = IndexedObject.TypeOfIndexed.Name,
                            FoundWordId = foundWord.Id
                        };
                    }

                    int count = Count(words, word);
                    int propertyRank = Rate(propertyInfo.Name);
                    indexInfo.Rank += Summup(propertyRank, count);

                    if (indexInfoPosition != -1)
                        foundWord.IndexInfos[indexInfoPosition] = indexInfo;
                    else
                        foundWord.IndexInfos.Add(indexInfo);

                    AddOrUpdateIndexes(foundWord);
                }
            }

            return FoundWords;
        }

        private FoundWord GetFoundWord(string word)
        {
            FoundWord foundWord = FoundWords.Find(f => f.Word.Equals(word));
            if (foundWord == default)
            {
                foundWord = Repository.FoundWords
                    .Find(f => f.Word.Equals(word))?
                    .FirstOrDefault();
                if (foundWord == default)
                    foundWord = new FoundWord { Word = word, IndexInfos = new List<IndexInfo>() };
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

        private int Count(List<string> words, string word)
        {
            return words.Count(v => v.Equals(word));
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