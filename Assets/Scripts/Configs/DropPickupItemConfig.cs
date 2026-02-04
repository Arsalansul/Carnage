using System.Collections.Generic;
using DOTS.Authoring;
using UnityEngine;
using Zenject;

namespace Configs
{
    [CreateAssetMenu(fileName = "DropPickupItemConfig", menuName = "Configs/DropPickupItemConfig")]
    public class DropPickupItemConfig : ScriptableObjectInstaller<GameConfig>
    {
        public List<PickupSettings> PickupSettings;
        public DropSettings DropSettings;
        public BombConsumableSettings BombSettings;
    
        public override void InstallBindings()
        {
            Container.BindInstance(PickupSettings);
            Container.BindInstance(DropSettings);
            Container.BindInstance(BombSettings);
        }
    }
}