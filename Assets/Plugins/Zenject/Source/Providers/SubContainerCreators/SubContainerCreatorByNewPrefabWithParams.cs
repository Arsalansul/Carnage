#if !NOT_UNITY3D

using System;
using System.Collections.Generic;
using System.Linq;
using ModestTree;
using Zenject.Internal;

namespace Zenject
{
    [NoReflectionBaking]
    public class SubContainerCreatorByNewPrefabWithParams : ISubContainerCreator
    {
        private readonly GameObjectCreationParameters _gameObjectBindInfo;
        private readonly Type _installerType;
        private readonly IPrefabProvider _prefabProvider;

        public SubContainerCreatorByNewPrefabWithParams(
            Type installerType, DiContainer container, IPrefabProvider prefabProvider,
            GameObjectCreationParameters gameObjectBindInfo)
        {
            _gameObjectBindInfo = gameObjectBindInfo;
            _prefabProvider = prefabProvider;
            Container = container;
            _installerType = installerType;
        }

        protected DiContainer Container { get; }

        public DiContainer CreateSubContainer(List<TypeValuePair> args, InjectContext parentContext,
            out Action injectAction)
        {
            Assert.That(!args.IsEmpty());

            var prefab = _prefabProvider.GetPrefab(parentContext);
            var tempContainer = CreateTempContainer(args);

            bool shouldMakeActive;
            var gameObject = tempContainer.CreateAndParentPrefab(
                prefab, _gameObjectBindInfo, null, out shouldMakeActive);

            var context = gameObject.GetComponent<GameObjectContext>();

            Assert.That(context != null,
                "Expected prefab with name '{0}' to container a component of type 'GameObjectContext'", prefab.name);

            context.Install(tempContainer);

            injectAction = () =>
            {
                // Note: We don't need to call ResolveRoots here because GameObjectContext does this for us
                tempContainer.Inject(context);

                if (shouldMakeActive && !Container.IsValidating)
                {
#if ZEN_INTERNAL_PROFILING
                    using (ProfileTimers.CreateTimedBlock("User Code"))
#endif
                    {
                        gameObject.SetActive(true);
                    }
                }
            };

            return context.Container;
        }

        private IEnumerable<InjectableInfo> GetAllInjectableIncludingBaseTypes()
        {
            var info = TypeAnalyzer.GetInfo(_installerType);

            while (info != null)
            {
                foreach (var injectable in info.AllInjectables) yield return injectable;

                info = info.BaseTypeInfo;
            }
        }

        private DiContainer CreateTempContainer(List<TypeValuePair> args)
        {
            var tempSubContainer = Container.CreateSubContainer();

            var allInjectables = GetAllInjectableIncludingBaseTypes();

            foreach (var argPair in args)
            {
                // We need to intelligently match on the exact parameters here to avoid the issue
                // brought up in github issue #217
                var match = allInjectables
                    .Where(x => argPair.Type.DerivesFromOrEqual(x.MemberType))
                    .OrderBy(x => ZenUtilInternal.GetInheritanceDelta(argPair.Type, x.MemberType)).FirstOrDefault();

                Assert.That(match != null,
                    "Could not find match for argument type '{0}' when injecting into sub container installer '{1}'",
                    argPair.Type, _installerType);

                tempSubContainer.Bind(match.MemberType)
                    .FromInstance(argPair.Value).WhenInjectedInto(_installerType);
            }

            return tempSubContainer;
        }
    }
}

#endif