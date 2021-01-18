using Domain.Entities;
using Repo.Interfaces;
using Services.Interfaces.Search;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Services.Implementation.Search
{
    public class UserSearchStrategy : ISearchStrategy<User>
    {
        public List<Func<User, bool>> Predicates { get; }
        //public Func<Func<User, bool>, IEnumerable<User>> Find { get; }
        public Func<Func<User, bool>, Task<IEnumerable<User>>> FindAsync { get; }
        public UserSearchStrategy(IRepository repository,
            string query)
        {
            //Find = repository.Users.Find;
            Predicates = new List<Func<User, bool>>();
            Predicates.Add(u => u.Username.Contains(query));
            Predicates.Add(u => u.Description.Contains(query));
            FindAsync = repository.Users.FindAsync;
        }
    }
}
