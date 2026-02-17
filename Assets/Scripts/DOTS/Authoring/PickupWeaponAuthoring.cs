using Unity.Entities;
using UnityEngine;

namespace DOTS.Authoring
{
    public class PickupWeaponAuthoring : MonoBehaviour
    {
        public WeaponType weaponType;
        private class Baker : Baker<PickupWeaponAuthoring>
        {
            public override void Bake(PickupWeaponAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.Dynamic);
                AddComponent(entity, new PickupWeapon()
                {
                    weaponType = authoring.weaponType
                });
            }
        }
    }

    public struct PickupWeapon : IComponentData
    {
        public WeaponType weaponType;
    }
}