using Domain.Entities;
using Domain.Entities.Enums;
using Repo.Interfaces;
using Services.Interfaces;
using Services.VisibleValues;
using System;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace Services.Implementation
{
    public class VerifyObjectsObserver : IVisibleObjectsObserver
    {
        private readonly CheckList controlList;
        private readonly CheckList blackList;
        private VisibleObjectValues CheckedObject { get; set; }
        private IRepository Repository { get; }
        public VerifyObjectsObserver(IRepository repository)
        {
            Repository = repository;
            controlList = Repository.CheckLists.Find(c => c.CheckAction is CheckAction.Verify).First();
            blackList = Repository.CheckLists.Find(c => c.CheckAction is CheckAction.Delete).First();
        }

        public Task CheckNewEntityAsync(VisibleObjectValues visibleValues)
            => CheckAsync(visibleValues);

        public Task CheckUpdatedEntityAsync(VisibleObjectValues visibleValues)
            => CheckAsync(visibleValues);

        public Task CheckDeletedEntityAsync(VisibleObjectValues visibleValues)
            => Task.CompletedTask;

        private async Task CheckAsync(VisibleObjectValues visibleValues)
        {
            CheckedObject = visibleValues;
            Entity entity = await Repository.Entities.GetByIdAsync(CheckedObject.Id);
            PropertyInfo[] propertyInfos = CheckedObject.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance);

            entity.Accepted = true;
            entity.VerifiedMatches = string.Empty;

            foreach (PropertyInfo propertyInfo in propertyInfos)
            {
                if (propertyInfo.GetValue(CheckedObject) is not string checkedValue)
                    continue;

                foreach (string verifyable in blackList)
                {
                    if (checkedValue.Contains(verifyable))
                    {
                        entity.Accepted = false;
                        entity.VerifiedMatches += verifyable + Environment.NewLine;
                        break;
                    }
                }

                foreach (string verifyable in controlList)
                {
                    if (checkedValue.Contains(verifyable))
                    {
                        if (entity.Accepted is not false)
                            entity.Accepted = null;
                        entity.VerifiedMatches += verifyable + Environment.NewLine;
                    }
                }
            }

            if (entity.Accepted is not true)
                await Repository.Entities.UpdateAsync(entity);
        }
    }
}
