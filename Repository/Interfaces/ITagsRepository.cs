using Domain.Entities;
using System.Collections.Generic;

namespace Repo.Interfaces
{
    public interface ITagsRepository : IWorkingWithRange<Tag>
    {
        IEnumerable<Tag> FindRange(params string[] query);
    }
}
