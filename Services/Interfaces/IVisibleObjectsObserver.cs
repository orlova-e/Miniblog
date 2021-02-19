using Services.VisibleValues;
using System.Threading.Tasks;

namespace Services.Interfaces
{
    public interface IVisibleObjectsObserver
    {
        Task CheckNewEntityAsync(VisibleObjectValues visibleValues);
        Task CheckUpdatedEntityAsync(VisibleObjectValues visibleValues);
        Task CheckDeletedEntityAsync(VisibleObjectValues visibleValues);
    }
}
