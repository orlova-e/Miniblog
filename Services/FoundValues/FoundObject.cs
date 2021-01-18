using System;
using System.Collections.Generic;

namespace Services.FoundValues
{
    public class FoundObject<T>
        where T : class, new()
    {
        public int TotalRating { get; set; }
        public List<string> MatchedWords { get; set; }
        public Guid EntityId { get; set; }
        public T Entity { get; set; }
    }
}
