using System.Collections;
using UnityEngine;

public class DartsWeapon : WeaponBase
{
    [SerializeField] private GameObject projectile;
    [SerializeField] private float projectileDelayBetween = 0.1f;
    private PlayerInputManager playerInputManager;
    Vector3 target;
    private Camera mainCamera;
    protected override void Attack()
    {
        int count = GetTotalProjectileCount();
        if (count <= 0) return;

        StartCoroutine(FireProjectiles(count));
    }

    private void Start()
    {
        playerInputManager = GetComponentInParent<PlayerInputManager>();
        playerInputManager.OnLookEvent += HandleMouse;
        mainCamera = Camera.main;
    }

    private void HandleMouse(Vector2 pos)
    {
        Vector3 mousePos = new Vector3(pos.x, pos.y, 0);

        Ray ray = mainCamera.ScreenPointToRay(mousePos);
        Plane groundPlane = new Plane(Vector3.up, transform.position);

        if (groundPlane.Raycast(ray, out float rayDistance))
        {
            target = ray.GetPoint(rayDistance);
            
        }
    }

    private IEnumerator FireProjectiles(int count)
    {
        for (int i = 0; i < count; i++)
        {
            SpawnProjectile();
            if (i < count - 1)
            {
                yield return new WaitForSeconds(projectileDelayBetween);
            }
        }
    }

    private void SpawnProjectile()
    {
        var spawnedProjectile = Instantiate(projectile, transform.position, Quaternion.identity);
        TestProjectile proj = spawnedProjectile.GetComponent<TestProjectile>();
        
        Vector3 newPos = new Vector3(target.x, transform.position.y, target.z);
        if (proj != null)
        {
            proj.Initialize(
                newPos,
                GetModifiedDamage(),
                GetModifiedProjectileSpeed(),
                GetModifiedArea());
        }
    }
}
