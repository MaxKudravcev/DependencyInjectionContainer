using System;

namespace DependencyInjectionContainer
{
    public class DependencyKeyAttribute : Attribute
    {
        public Enum Name { get; }
        public DependencyKeyAttribute(Enum name) => Name = name;
    }
}
