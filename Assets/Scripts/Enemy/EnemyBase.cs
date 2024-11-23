using UnityEngine;
using UnityEngine.InputSystem.XR;

public abstract class EnemyBase : MonoBehaviour, IDamagable
{
    [SerializeField] private int health = 5;
    [SerializeField] private float moveSpeed = 3;
    [SerializeField] private int damage = 5;
    [SerializeField] private float damageRate = 0.5f;
    private float nextDamageTime;
    private PlayerScript player;
    private CharacterController controller;

    protected virtual void Start()
    {
        player = FindFirstObjectByType<PlayerScript>();
        controller = GetComponent<CharacterController>();
    }

    protected virtual void Update()
    {
        if (player == null) return;
        Vector3 direction = (player.transform.position - transform.position).normalized;
        direction.y = 0;
        controller.Move(direction * moveSpeed * Time.deltaTime);

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

    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        if (Time.time < nextDamageTime) return;
        if (!hit.gameObject.CompareTag("Player")) return;

        var playerScript = hit.gameObject.GetComponent<PlayerScript>();
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
