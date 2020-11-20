using System;
using System.Collections.Generic;

namespace DependencyInjectionContainer
{
    public class DependenciesConfig
    {
        internal Dictionary<Type, List<Implementation>> dependencies = new Dictionary<Type, List<Implementation>>();

        public bool Register<TDependency, TImplementation>(LifeCycle lifeCycle = LifeCycle.InstancePerDependency)
            where TImplementation : TDependency
            where TDependency : class
        {
            if (typeof(TImplementation).IsAbstract)
                throw new Exception("TImplementation can not be abstract.");

            if (typeof(TImplementation).GetConstructors().Length != 0)
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

            throw new Exception("TImplementation should have at least one public constructor.");
        }
    }
}
