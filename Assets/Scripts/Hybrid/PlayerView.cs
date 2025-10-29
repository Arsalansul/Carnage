using System.Collections.Generic;
using UnityEngine;

public interface IArmed
{
    void ShowWeapon(int id);
}

public class PlayerView : UnitView, IArmed
{
    [SerializeField] private List<MeshRenderer> weaponsMeshs;

    private void OnEnable()
    {
        ShowWeapon(0);
    }

    public void ShowWeapon(int id)
    {
        for (var i = 0; i < weaponsMeshs.Count; i++) weaponsMeshs[i].enabled = i == id;
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