using UnityEngine;

namespace Zenject.Tests.Factories.BindFactoryOne
{
    public interface IFoo
    {
        string Value { get; }
    }

    public class IFooFactory : PlaceholderFactory<string, IFoo>
    {
    }

    public class Foo : MonoBehaviour, IFoo
    {
        public string Value { get; private set; }

        [Inject]
        public void Init(string value)
        {
            Value = value;
        }

        public class Factory : PlaceholderFactory<string, Foo>
        {
        }
    }
}