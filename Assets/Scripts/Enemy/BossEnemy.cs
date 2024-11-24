using UnityEngine;

public class BossEnemy : EnemyBase
{
    [SerializeField] private GameObject winScreen;
    public override void Damage(int damage)
    {
        health -= damage;
        if (health <= 0)
        {
            Instantiate(winScreen);
            Destroy(gameObject);
            Time.timeScale = 0f;
        }
    }
}
