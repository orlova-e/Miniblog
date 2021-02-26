using Repo.Interfaces;
using Services.Implementation.Indexing;
using Services.Interfaces;
using Services.VisibleValues;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Services.Implementation
{
    public class EntityObserver : IEntityObserver
    {
        readonly List<Func<VisibleObjectValues, Task>> onCreateList = new();
        readonly List<Func<VisibleObjectValues, Task>> onUpdateList = new();
        readonly List<Func<VisibleObjectValues, Task>> onDeleteList = new();

        public Task OnNewEntryAsync(VisibleObjectValues visible) => OnEntityAsync(onCreateList, visible);
        public Task OnUpdateAsync(VisibleObjectValues visible) => OnEntityAsync(onUpdateList, visible);
        public Task OnDeleteAsync(VisibleObjectValues visibleObjectValues) => OnEntityAsync(onDeleteList, visibleObjectValues);

        public EntityObserver()
        { }

        public EntityObserver(IRepository repository)
        {
            Add(new VerifyObjectsObserver(repository));
            Add(new IndexedObjectsObserver(repository));
        }

        private async Task OnEntityAsync(List<Func<VisibleObjectValues, Task>> funcs, VisibleObjectValues visible)
        {
            try
            {
                foreach (var func in funcs)
                {
                    await func(visible);
                }
            }
            catch (ArgumentNullException) { }
        }

        public void Add(IVisibleObjectsObserver newObserver)
        {
            onCreateList.Add(newObserver.CheckNewEntityAsync);
            onUpdateList.Add(newObserver.CheckUpdatedEntityAsync);
            onDeleteList.Add(newObserver.CheckDeletedEntityAsync);
        }
    }
}
