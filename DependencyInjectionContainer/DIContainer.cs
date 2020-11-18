using System;
using System.Linq;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Reflection;

namespace DependencyInjectionContainer
{
    public class DIContainer
    {
        private Dictionary<Type, List<Implementation>> dependencies;
        private ConcurrentDictionary<Type, object> singletons = new ConcurrentDictionary<Type, object>();
        
        public TDependency Resolve<TDependency>(Enum namedDependency = null)
        {
            return (TDependency)Resolve(typeof(TDependency), Convert.ToInt32(namedDependency));
        }

        private object Resolve(Type tDependency, int namedDependency = 0)
        {
            //create list if IEnumerable
            if (tDependency.GetInterfaces().Any(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IEnumerable<>)))
            {
                tDependency = tDependency.GetGenericArguments().First();
                if (!dependencies.ContainsKey(tDependency))
                    throw new Exception("Specified dependency does not exist.");
                var container = Array.CreateInstance(tDependency, dependencies[tDependency].Count);
                for (int i = 0; i < dependencies[tDependency].Count; i++)                
                    container.SetValue(CreateObject(dependencies[tDependency][i]), i);
                return container;
            }


            //Closed or open generic type logic
            Implementation imp = null; 
            if(tDependency.GenericTypeArguments.Length != 0)
            {
                if (!dependencies.ContainsKey(tDependency) && dependencies.ContainsKey(tDependency.GetGenericTypeDefinition())) //Open (unbound) generic
                {
                    if (dependencies[tDependency.GetGenericTypeDefinition()].Count <= namedDependency)
                        throw new Exception("Such named dependency does not exist.");

                    Type constructedType = dependencies[tDependency.GetGenericTypeDefinition()][namedDependency]
                        .TImplementation
                        .MakeGenericType(tDependency.GetGenericArguments().First());


                    imp = new Implementation(
                            constructedType,
                            dependencies[tDependency.GetGenericTypeDefinition()][namedDependency]
                            .LifeCycle);
                    tDependency = tDependency.GetGenericArguments().First();
                }
            }

            if (!dependencies.ContainsKey(tDependency))
                throw new Exception("Specified dependency does not exist");

            if (imp == null)
            {
                if (dependencies[tDependency].Count <= namedDependency)
                    throw new Exception("Such named dependency does not exist.");
                imp = dependencies[tDependency][namedDependency];
            }
                

            //Create implementation
            return CreateObject(imp);
        }

        private object CreateObject(Implementation imp)
        {
            if (singletons.ContainsKey(imp.TImplementation))
                return singletons[imp.TImplementation];
            else
            {
                ConstructorInfo ctor = imp.TImplementation.GetConstructors().First();
                ParameterInfo[] ctorParams = ctor.GetParameters();
                object[] invokeArgs = new object[ctorParams.Length];

                for (int i = 0; i < ctorParams.Length; i++)
                {
                    if (ctorParams[i].ParameterType.IsValueType)
                        throw new Exception("Implementation constructor takes invalid parameters.");

                    //Check for constructor attribute
                    DependencyKeyAttribute atr = ctorParams[i].GetCustomAttribute<DependencyKeyAttribute>();
                    if(atr != null)
                        invokeArgs[i] = Resolve(ctorParams[i].ParameterType, Convert.ToInt32(atr.Name));
                    else
                        invokeArgs[i] = Resolve(ctorParams[i].ParameterType);
                }

                object res = Activator.CreateInstance(imp.TImplementation, invokeArgs);

                if (imp.LifeCycle == LifeCycle.Singleton)
                    singletons.TryAdd(imp.TImplementation, res);

                return res;
            }
        }
    }
}
