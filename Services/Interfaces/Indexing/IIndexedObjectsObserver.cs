using Services.IndexedValues;
using System.Threading.Tasks;

namespace Services.Interfaces.Indexing
{
    public interface IIndexedObjectsObserver
    {
        Task OnNewEntityAsync(IndexedObject indexedObject);
        Task OnUpdatedEntityAsync(IndexedObject indexedObject);
        Task OnDeletedEntityAsync(IndexedObject indexedObject);
    }
}
