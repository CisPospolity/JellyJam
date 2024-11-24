using UnityEngine;

public class ShurikenProjectile : MonoBehaviour
{
    private float damage;
    private float speed;
    private bool isInitialized = false;
    private bool returning = false;
    private Transform playerPos;

    public void Initialize(Vector3 targetPosition, float damage, float speed, float area, Transform player)
    {
        this.damage = damage;
        this.speed = speed;
        playerPos = player;
        transform.localScale *= area;

        Vector3 direction = (targetPosition - transform.position).normalized;
        transform.rotation = Quaternion.LookRotation(direction);

        isInitialized = true;

        Destroy(gameObject, 20f);

        Invoke("Return", 1f);
    }

    private void Return()
    {
        returning = true;
    }

    private void Update()
    {
        if (!isInitialized) return;

        if (!returning)
        {
            transform.position += speed * Time.deltaTime * transform.forward;
        } else
        {
            Vector3 direction = (playerPos.position - transform.position).normalized;
            direction.y = 0;
            transform.rotation = Quaternion.LookRotation(direction, Vector3.up);
            transform.position += speed * Time.deltaTime * transform.forward;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        IDamagable damagable = other.GetComponent<IDamagable>();
        if (damagable != null)
        {
            damagable.Damage((int)damage);
        }

        if(returning && other.CompareTag("Player"))
        {
            Destroy(gameObject);
        }
    }
}
