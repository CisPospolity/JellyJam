using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public abstract class EnemyBase : MonoBehaviour, IDamagable
{
    [SerializeField] private int health = 5;
    [SerializeField] private float moveSpeed = 3;
    [SerializeField] private int damage = 5;
    [SerializeField] private float damageRate = 0.5f;
    private float nextDamageTime;
    private PlayerScript player;
    private Rigidbody rb;
    [SerializeField] private Vector3 spawnOffset;
    [SerializeField] private int expValue = 1;
    [SerializeField] private ExpObj expObject;
    [SerializeField] private Vector3 coinSpawnOffset = Vector3.zero;
    public Vector3 SpawnOffset => spawnOffset;
    public float MoveSpeed => moveSpeed;

    protected virtual void Start()
    {
        player = FindFirstObjectByType<PlayerScript>();
        rb = GetComponent<Rigidbody>();
    }

    protected virtual void FixedUpdate()
    {
        if (player == null) return;
        Vector3 direction = (player.transform.position - transform.position).normalized;
        direction.y = 0;
        rb.MovePosition(rb.position + moveSpeed * Time.fixedDeltaTime * direction);

        if (direction != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            rb.MoveRotation(targetRotation);
        }
    }

    public virtual void Damage(int damage)
    {
        health -= damage;
        if(health <= 0)
        {
            if(expObject != null)
            {
                var spawnedExp = Instantiate(expObject, transform.position + coinSpawnOffset, Quaternion.identity);
                spawnedExp.SetExpValue(expValue);
            }
            Destroy(gameObject);
        }
    }

    protected virtual void OnCollisionStay(Collision collision)
    {
        if (Time.time < nextDamageTime) return;
        if (!collision.gameObject.CompareTag("Player")) return;

        var playerScript = collision.gameObject.GetComponent<PlayerScript>();
        if (playerScript != null)
        {
            playerScript.GetHit(damage);
            nextDamageTime = Time.time + damageRate;
        }
    }

    protected virtual void OnCollisionEnter(Collision collision)
    {
        
    }
}
