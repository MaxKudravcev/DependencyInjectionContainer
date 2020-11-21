using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DependencyInjectionContainer.Tests
{
    public class DIContainer_Tests
    {

        #region Simple scenarios

        public interface ISimpleDependency { }

        public class SimpleDependency : ISimpleDependency
        {
            int a = 0;
            bool b = false;

            public override bool Equals(object obj)
            {
                if (obj is SimpleDependency sd)
                    return a == sd.a && b == sd.b;
                return false;
            }

            public override int GetHashCode()
            {
                return HashCode.Combine(a, b);
            }
        }

        public class SimpleWithInnerDependency : ISimpleDependency
        {
            int a = 1;
            bool b = true;
            ISimpleDependency dependency;

            public SimpleWithInnerDependency(ISimpleDependency dependency)
            {
                this.dependency = dependency;
            }

            public override bool Equals(object obj)
            {
                if (obj is SimpleWithInnerDependency sd)
                    return a == sd.a && b == sd.b && dependency.Equals(sd.dependency);
                return false;
            }

            public override int GetHashCode()
            {
                return HashCode.Combine(a, b, dependency);
            }
        }

        #endregion

        #region Singleton 

        public interface ISingleton { }

        public class SingletonDependency : ISingleton
        {
            public int a = 42;
        }

        #endregion

        #region Retrieving collections

        public interface ICollectionDependency { }

        public class CollectionClass1 : ICollectionDependency
        {
            int a = 1;
            char b = 'a';

            public override bool Equals(object obj)
            {
                if (obj is CollectionClass1 cc)
                    return a == cc.a && b == cc.b;
                return false;
            }

            public override int GetHashCode()
            {
                return HashCode.Combine(a, b);
            }
        }

        public class CollectionClass2 : ICollectionDependency
        {
            int a = 2;
            char b = 'b';

            public override bool Equals(object obj)
            {
                if (obj is CollectionClass2 cc)
                    return a == cc.a && b == cc.b;
                return false;
            }

            public override int GetHashCode()
            {
                return HashCode.Combine(a, b);
            }
        }

        public class CollectionClass3 : ICollectionDependency
        {
            int a = 3;
            char b = 'c';

            public override bool Equals(object obj)
            {
                if (obj is CollectionClass3 cc)
                    return a == cc.a && b == cc.b;
                return false;
            }

            public override int GetHashCode()
            {
                return HashCode.Combine(a, b);
            }
        }

        public interface IInnerCollection { }

        public class InnerCollectionClass : IInnerCollection
        {
            IEnumerable<ICollectionDependency> dependencies;

            public InnerCollectionClass(IEnumerable<ICollectionDependency> dependencies)
            {
                this.dependencies = dependencies;
            }

            public override bool Equals(object obj)
            {
                if (obj is InnerCollectionClass c)
                {
                    return Enumerable.SequenceEqual(c.dependencies, dependencies);
                }
                return false;
            }

            public override int GetHashCode()
            {
                return dependencies.GetHashCode();
            }
        }

        #endregion

        #region Generic dependencies

        public interface IConstrained { }

        public class ConstrainedClass : IConstrained
        {
            int a = 42;
            char b = 'g';

            public override bool Equals(object obj)
            {
                if (obj is ConstrainedClass cc)
                    return a == cc.a && b == cc.b;
                return false;
            }

            public override int GetHashCode()
            {
                return HashCode.Combine(a, b);
            }
        }

        public interface IGenericConstrained<TConstrained> where TConstrained : IConstrained { }

        public class GenericConstrainedClass<TConstrainedClass> : IGenericConstrained<TConstrainedClass> where TConstrainedClass : IConstrained
        {
            TConstrainedClass @class;
            public GenericConstrainedClass(TConstrainedClass constrained)
            {
                @class = constrained;
            }

            public override bool Equals(object obj)
            {
                if (obj is GenericConstrainedClass<TConstrainedClass> c)
                {
                    return c.@class.Equals(@class);
                }
                return false;
            }

            public override int GetHashCode()
            {
                return @class.GetHashCode();
            }
        }

        public interface IOpenConstrained { }

        public class OpenConstrainedClass : IOpenConstrained
        {
            int a = 43;
            char b = 'f';

            public override bool Equals(object obj)
            {
                if (obj is OpenConstrainedClass cc)
                    return a == cc.a && b == cc.b;
                return false;
            }

            public override int GetHashCode()
            {
                return HashCode.Combine(a, b);
            }
        }

        public interface IOpenGeneric<TOpenGenericConstrained> where TOpenGenericConstrained : IOpenConstrained { }

        public class OpenGenericConstrainedClass<TOpenGenericConstrained> : IOpenGeneric<TOpenGenericConstrained> where TOpenGenericConstrained : IOpenConstrained
        {
            TOpenGenericConstrained @class;
            public OpenGenericConstrainedClass(TOpenGenericConstrained constrained)
            {
                @class = constrained;
            }

            public override bool Equals(object obj)
            {
                if (obj is OpenGenericConstrainedClass<TOpenGenericConstrained> c)
                {
                    return c.@class.Equals(@class);
                }
                return false;
            }

            public override int GetHashCode()
            {
                return @class.GetHashCode();
            }
        }

        #endregion

        #region Named dependencies

        public enum Names
        {
            First,
            Second,
            Third
        }

        public class AnotherSimpleClass : ISimpleDependency
        {
            ICollectionDependency dependency;

            public AnotherSimpleClass([DependencyKey(Names.Second)]ICollectionDependency dependency)
            {
                this.dependency = dependency;
            }

            public override bool Equals(object obj)
            {
                if (obj is AnotherSimpleClass c)
                {
                    return c.dependency.Equals(dependency);
                }
                return false;
            }

            public override int GetHashCode()
            {
                return dependency.GetHashCode();
            }
        }

        #endregion

        DIContainer container;

        [SetUp]
        public void Setup()
        {
            DependenciesConfig config = new DependenciesConfig();
            config.Register<ISimpleDependency, SimpleDependency>();
            config.Register<ISimpleDependency, SimpleWithInnerDependency>();
            config.Register<ISingleton, SingletonDependency>(LifeCycle.Singleton);
            config.Register<ICollectionDependency, CollectionClass1>();
            config.Register<ICollectionDependency, CollectionClass2>();
            config.Register<ICollectionDependency, CollectionClass3>();
            config.Register<IInnerCollection, InnerCollectionClass>();
            config.Register<IConstrained, ConstrainedClass>();
            config.Register<IGenericConstrained<IConstrained>, GenericConstrainedClass<IConstrained>>();
            config.Register<IOpenConstrained, OpenConstrainedClass>();
            config.Register(typeof(IOpenGeneric<>), typeof(OpenGenericConstrainedClass<>));
            config.Register<ISimpleDependency, AnotherSimpleClass>();

            container = new DIContainer(config);
        }

        [Test]
        public void SimpleDependencyTest()
        {
            var actual = container.Resolve<ISimpleDependency>();
            var expected = new SimpleDependency();

            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void SimpleInnerDependencyTest()
        {
            var actual = container.Resolve<ISimpleDependency>(Names.Second);
            var expected = new SimpleWithInnerDependency(new SimpleDependency());

            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void SingletonTest()
        {
            var expected = Task.Run(() => container.Resolve<ISingleton>());
            var actual1 = Task.Run(() => container.Resolve<ISingleton>());
            var actual2 = Task.Run(() => container.Resolve<ISingleton>());
            var actual3 = Task.Run(() => container.Resolve<ISingleton>());

            Assert.AreEqual(expected.Result, actual1.Result);
            Assert.AreEqual(expected.Result, actual2.Result);
            Assert.AreEqual(expected.Result, actual3.Result);
        }

        [Test]
        public void CollectionTest()
        {
            var actual = container.Resolve<IEnumerable<ICollectionDependency>>();
            var expected = new ICollectionDependency[] { new CollectionClass1(), new CollectionClass2(), new CollectionClass3() };

            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void InnerCollectionTest()
        {
            var actual = container.Resolve<IInnerCollection>();
            var expected = new InnerCollectionClass(new ICollectionDependency[] { new CollectionClass1(), new CollectionClass2(), new CollectionClass3() });

            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void ConstrainedGenericTest()
        {
            var actual = container.Resolve<IGenericConstrained<IConstrained>>();
            var expected = new GenericConstrainedClass<IConstrained>(new ConstrainedClass());

            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void OpenGenericTest()
        {
            var actual = container.Resolve<IOpenGeneric<IOpenConstrained>>();
            var expected = new OpenGenericConstrainedClass<IOpenConstrained>(new OpenConstrainedClass());

            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void NamedDependencyTest()
        {
            var actual = container.Resolve<ISimpleDependency>(Names.Third);
            var expected = new AnotherSimpleClass(new CollectionClass2());

            Assert.AreEqual(expected, actual);
        }
    }
}