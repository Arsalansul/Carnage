using System;
using NUnit.Framework;
using Assert = ModestTree.Assert;

namespace Zenject.Tests.Bindings
{
    [TestFixture]
    public class TestFromPoolableMemoryPoolSix : ZenjectUnitTestFixture
    {
        public class Foo : IPoolable<string, int, float, char, double, long, IMemoryPool>, IDisposable
        {
            public Foo()
            {
                SetDefaults();
            }

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

            public void OnSpawned(string p1, int p2, float p3, char p4, double p5, long p6, IMemoryPool pool)
            {
                Pool = pool;
                Data = p1;
            }

            private void SetDefaults()
            {
                Pool = null;
                Data = null;
            }

            public class Factory : PlaceholderFactory<string, int, float, char, double, long, Foo>
            {
            }
        }

        [Test]
        public void Test1()
        {
            Container.BindFactory<string, int, float, char, double, long, Foo, Foo.Factory>()
                .FromPoolableMemoryPool(x => x.WithInitialSize(2));

            var factory = Container.Resolve<Foo.Factory>();

            var foo = factory.Create("asdf", 1, 1.0f, 'u', 1.0, 1L);
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