using System;
using System.Collections.Generic;
using UnityEngine;

namespace Hybrid
{
    public interface IArmed
    {
        void ShowWeapon(WeaponType weaponType);
    }

    [Serializable]
    public struct WeaponMeshMap
    {
        public MeshRenderer mesh;
        public WeaponType weaponType;
    }

    public class PlayerView : UnitView, IArmed
    {
        [SerializeField] private List<WeaponMeshMap> weaponsMeshMap;

        public void ShowWeapon(WeaponType weaponType)
        {
            for (var i = 0; i < weaponsMeshMap.Count; i++)
            {
                weaponsMeshMap[i].mesh.enabled = weaponsMeshMap[i].weaponType == weaponType;
            }
        }

        public override void Move(float speed)
        {
            base.Move(speed);
            animator.SetBool("aim", speed > 0.1f);
        }

        public override void Attack()
        {
            animator.SetBool("aim", true);
            animator.SetTrigger("shoot");
        }
    }
}