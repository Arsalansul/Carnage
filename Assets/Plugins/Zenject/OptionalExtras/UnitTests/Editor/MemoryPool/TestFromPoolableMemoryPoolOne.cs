using System;
using NUnit.Framework;
using Assert = ModestTree.Assert;

namespace Zenject.Tests.Bindings
{
    [TestFixture]
    public class TestFromPoolableMemoryPoolOne : ZenjectUnitTestFixture
    {
        public class Foo : IPoolable<string, IMemoryPool>, IDisposable
        {
            public Foo(string initialData)
            {
                InitialData = initialData;
                SetDefaults();
            }

            public string InitialData { get; }

            public IMemoryPool Pool { get; private set; }

            public string Data { get; private set; }

            public void Dispose()
            {
                Pool.Despawn(this);
            }

            public void OnDespawned()
            {
                Data = null;
                Pool = null;
                SetDefaults();
            }

            public void OnSpawned(string data, IMemoryPool pool)
            {
                Pool = pool;
                Data = data;
            }

            private void SetDefaults()
            {
                Pool = null;
                Data = null;
            }

            public class Factory : PlaceholderFactory<string, Foo>
            {
            }
        }

        [Test]
        public void Test1()
        {
            Container.BindFactory<string, Foo, Foo.Factory>()
                .FromPoolableMemoryPool(x => x.WithInitialSize(2).WithArguments("blurg"));

            var factory = Container.Resolve<Foo.Factory>();

            var foo = factory.Create("asdf");

            Assert.IsEqual(foo.InitialData, "blurg");

            var pool = foo.Pool;

            Assert.IsEqual(pool.NumActive, 1);
            Assert.IsEqual(pool.NumTotal, 2);
            Assert.IsEqual(pool.NumInactive, 1);

            Assert.IsEqual(foo.Data, "asdf");

            foo.Dispose();

            Assert.IsEqual(pool.NumActive, 0);
            Assert.IsEqual(pool.NumTotal, 2);
            Assert.IsEqual(pool.NumInactive, 2);
            Assert.IsEqual(foo.Data, null);
        }
    }
}