using Domain.Entities;
using System.Collections.Generic;

namespace Services.FoundValues
{
    public class FoundObject<T>
        where T : class, new()
    {
        public int TotalRating { get; set; }
        public List<string> MatchedWords { get; set; }
        public Entity Entity { get; set; }
    }
}
