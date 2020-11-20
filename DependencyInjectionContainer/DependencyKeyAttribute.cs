using System;

namespace DependencyInjectionContainer
{
    public class DependencyKeyAttribute : Attribute
    {
        public Enum Name { get; }
        public DependencyKeyAttribute(object name) => Name = (Enum)name;
    }
}
