using UnityEngine;

public class TestProjectile : MonoBehaviour
{
    private Vector3 targetPosition;
    private float damage;
    private float speed;
    private bool isInitialized = false;

    public void Initialize(Vector3 targetPosition, float damage, float speed, float area)
    {
        this.targetPosition = targetPosition;
        this.damage = damage;
        this.speed = speed;

        transform.localScale *= area;

        Vector3 direction = (targetPosition - transform.position).normalized;
        transform.rotation = Quaternion.LookRotation(direction);

        isInitialized = true;
    }

    private void Update()
    {
        if (!isInitialized) return;

        transform.position += speed * Time.deltaTime * transform.forward;
    }

    private void OnTriggerEnter(Collider other)
    {
        IDamagable damagable = other.GetComponent<IDamagable>();
        if (damagable != null)
        {
            damagable.Damage((int)damage);
        }
    }
}
