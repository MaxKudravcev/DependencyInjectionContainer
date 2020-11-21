using System;

namespace DependencyInjectionContainer
{
    public enum LifeCycle
    {
        InstancePerDependency,
        Singleton
    }

    class Implementation
    {
        public Type TImplementation { get; }
        public LifeCycle LifeCycle { get; }

        public Implementation(Type implementation, LifeCycle lifeCycle)
        {
            TImplementation = implementation;
            LifeCycle = lifeCycle;
        }
    }
}
