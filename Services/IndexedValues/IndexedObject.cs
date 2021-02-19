using System;

namespace Services.IndexedValues
{
    public abstract class IndexedObject
    {
        public Guid Id { get; set; }
        public abstract int Rate(string propertyName);
    }
}
