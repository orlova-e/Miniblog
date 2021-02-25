using Domain.Entities;
using Repo.Interfaces;
using Services.FoundValues;
using Services.Interfaces.Search;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Services.Implementation.Search
{
    public class AccurateSearch<T> : ISearch<T>
        where T : Entity, new()
    {
        protected virtual List<Func<T, bool>> Predicates { get; set; }
        protected virtual IPlainRepository<T> PlainRepository { get; set; }
        protected IRepository Repository { get; }
        public AccurateSearch(IRepository repository)
        {
            Repository = repository;
        }

        public async Task<List<FoundObject>> FindAsync(string query)
        {
            if (string.IsNullOrWhiteSpace(query))
                throw new ArgumentNullException();

            Match(Repository, query);

            List<FoundObject> foundObjects = new List<FoundObject>();
            List<T> foundEntities = new List<T>();

            foreach (var predicate in Predicates)
            {
                IEnumerable<T> list = await PlainRepository.FindAsync(predicate);
                if (list is not null)
                    foundEntities.AddRange(list);
            }

            foundEntities = foundEntities.DistinctBy(f => f.Id).ToList();

            foundObjects = foundEntities
                .Select(f => new FoundObject
                {
                    Entity = f,
                    MatchedWords = new List<string> { query },
                    TotalRating = int.MaxValue
                })
                .ToList();

            return foundObjects;
        }

        protected virtual void Match(IRepository repository, string query)
        {
            Predicates = new T() switch
            {
                User => new List<Func<T, bool>>
                {
                    _ => (_ as User).Username.Contains(query),
                    _ => (_ as User).Description?.Contains(query) ?? false
                },
                Article => new List<Func<T, bool>>
                {
                    _ => (_ as Article).Header.Contains(query)
                },
                Comment => new List<Func<T, bool>>
                {
                    _ => (_ as Comment).Author.Username.Contains(query),
                    _ => (_ as Comment).Text.Contains(query)
                },
                Topic => new List<Func<T, bool>>
                {
                    _ => (_ as Topic).Name.Contains(query)
                },
                Tag => new List<Func<T, bool>>
                {
                    _ => (_ as Tag).Name.Contains(query)
                },
                Series => new List<Func<T, bool>>
                {
                    _ => (_ as Series).Name.Contains(query)
                },
                { } => throw new NotImplementedException()
            };

            PlainRepository = new T() switch
            {
                User => repository.Users as IPlainRepository<T>,
                Article => repository.Articles as IPlainRepository<T>,
                Comment => repository.Comments as IPlainRepository<T>,
                Topic => repository.Topics as IPlainRepository<T>,
                Tag => repository.Tags as IPlainRepository<T>,
                Series => repository.Series as IPlainRepository<T>,
                { } => throw new NotImplementedException()
            };
        }
    }
}
