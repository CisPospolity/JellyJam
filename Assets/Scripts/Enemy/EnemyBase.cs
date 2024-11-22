using UnityEngine;

public abstract class EnemyBase : MonoBehaviour, IDamagable
{
    [SerializeField] private int health = 5;
    [SerializeField] private float moveSpeed = 3;
    private PlayerScript player;

    protected virtual void Start()
    {
        player = FindFirstObjectByType<PlayerScript>();
    }

    protected virtual void Update()
    {
        if (player == null) return;
        Vector3 direction = (player.transform.position - transform.position).normalized;
        direction.y = 0;
        transform.position += direction * moveSpeed * Time.deltaTime;

        // Optional: Make enemy face the player
        transform.forward = direction;
    }

    public virtual void Damage(int damage)
    {
        health -= damage;
        if(health <= 0)
        {
            Destroy(gameObject);
        }
    }
}
