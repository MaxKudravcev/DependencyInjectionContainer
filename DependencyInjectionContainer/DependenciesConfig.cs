using System;
using System.Collections.Generic;

namespace DependencyInjectionContainer
{
    public class DependenciesConfig
    {
        internal Dictionary<Type, List<Implementation>> dependencies = new Dictionary<Type, List<Implementation>>();

        public bool Register(Type tDependency, Type tImplementation, LifeCycle lifeCycle = LifeCycle.InstancePerDependency)
            
        {
            if (tImplementation.IsAbstract)
                throw new Exception("TImplementation can not be abstract.");

            if (tImplementation.GetConstructors().Length != 0)
            {
                if (!dependencies.ContainsKey(tDependency))
                    dependencies.Add(
                        tDependency,
                        new List<Implementation> { new Implementation(tImplementation, lifeCycle) });
                else
                    dependencies[tDependency].Add(
                        new Implementation(tImplementation, lifeCycle));

                return true;
            }

            throw new Exception("TImplementation should have at least one public constructor.");
        }

        public bool Register<TDependency, TImplementation>(LifeCycle lifeCycle = LifeCycle.InstancePerDependency) 
            where TImplementation : TDependency
            where TDependency : class
        {
            return Register(typeof(TDependency), typeof(TImplementation), lifeCycle);
        }
    }
}
