using System;

namespace Services.VisibleValues
{
    public abstract class VisibleObjectValues
    {
        public Guid Id { get; set; }
        public abstract int Rate(string propertyName);
    }
}
