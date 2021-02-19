using Services.VisibleValues;
using System.Threading.Tasks;

namespace Services.Interfaces.Indexing
{
    public interface IIndexedObjectsObserver
    {
        Task OnNewEntityAsync(VisibleObjectValues indexedObject);
        Task OnUpdatedEntityAsync(VisibleObjectValues indexedObject);
        Task OnDeletedEntityAsync(VisibleObjectValues indexedObject);
    }
}
