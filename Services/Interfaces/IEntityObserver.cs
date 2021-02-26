using Services.VisibleValues;
using System.Threading.Tasks;

namespace Services.Interfaces
{
    public interface IEntityObserver
    {
        Task OnNewEntryAsync(VisibleObjectValues visible);
        Task OnUpdateAsync(VisibleObjectValues visible);
        Task OnDeleteAsync(VisibleObjectValues visible);
        void Add(IVisibleObjectsObserver newObserver);
    }
}
