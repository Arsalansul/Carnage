using Unity.Mathematics;
using UnityEngine;

public interface IUnitView
{
    void Move(float speed);
    void Attack();
    void Dead(bool isDead);
    void TakeDamage();
    void SetTransform(float3 position, quaternion rotation);
    void SetLookAngle(float angle, bool lookRight);
}

public class UnitView : MonoBehaviour, IUnitView
{
    [SerializeField] protected Animator animator;
    private string poolName;
    private bool shouldReturnToPool;

    private float timer;

    private void Update()
    {
        if (!shouldReturnToPool) return;
        timer -= Time.deltaTime;
        if (timer <= 0)
        {
            shouldReturnToPool = false;

            PoolManager.Instance.ReturnUnitToPool(poolName, this);
        }
    }

    public virtual void Move(float speed)
    {
        animator.SetFloat("speed", speed);
    }

    public virtual void Attack()
    {
        animator.SetTrigger("attack");
    }

    public void Dead(bool isDead)
    {
        animator.SetBool("dead", isDead);
        if (isDead) ReturnToPoolAfterDelay(2f);
    }

    public void TakeDamage()
    {
        animator.SetTrigger("damaged");
    }

    public void SetTransform(float3 position, quaternion rotation)
    {
        transform.position = position;
        transform.rotation = rotation;
    }

    public void SetLookAngle(float angle, bool lookRight)
    {
        animator.SetFloat("lookAngle", angle);
        animator.SetBool("lookRight", lookRight);
    }

    private void ReturnToPoolAfterDelay(float delay)
    {
        timer = delay;
        shouldReturnToPool = true;
    }

    public void SetPoolName(string poolName)
    {
        this.poolName = poolName;
    }
}