using System;
using System.Collections.Generic;

namespace DependencyInjectionContainer
{
    class DependenciesConfig
    {
        internal Dictionary<Type, List<Implementation>> dependencies = new Dictionary<Type, List<Implementation>>();

        public bool Register<TDependency, TImplementation>(LifeCycle lifeCycle)
            where TImplementation : TDependency
        {
            if(typeof(TImplementation).GetConstructors().Length != 0)
            {
                if (dependencies.ContainsKey(typeof(TDependency)))
                    dependencies.Add(
                        typeof(TDependency),
                        new List<Implementation> { new Implementation(typeof(TImplementation), lifeCycle) });
                else
                    dependencies[typeof(TDependency)].Add(
                        new Implementation(typeof(TImplementation), lifeCycle));

                return true;
            }

            return false;
        }
    }
}
