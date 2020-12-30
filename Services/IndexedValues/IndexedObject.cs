using System;

namespace Services.IndexedValues
{
    public abstract class IndexedObject
    {
        public Guid Id { get; set; }
        public Type TypeOfIndexed { get; set; }
    }
}
