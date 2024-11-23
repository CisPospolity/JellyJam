using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private float timeSinceStart = 0;
    public float Timer => timeSinceStart;

    [SerializeField] private float maxTimer = 15f * 60;
    [SerializeField] private List<EnemyWave> waves;
    [SerializeField] private Camera mainCamera;
    [SerializeField] private float spawnOffset = 2f;
    [SerializeField] private float spawnInterval = 0.2f;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private float raycastHeight = 20f;

    private int currentWaveIndex = 0;
    private float nextSpawnTime = 0f;

    private void Start()
    {
        if(mainCamera == null)
        {
            mainCamera = Camera.main;
        }
    }

    private void Update()
    {
        if (timeSinceStart < maxTimer)
        {
            timeSinceStart += Time.deltaTime;
        } else
        {
            timeSinceStart = maxTimer;
        }
        CheckWaveProgresion();
        HandleSpawning();
    }

    private void CheckWaveProgresion()
    {
        if (currentWaveIndex >= waves.Count - 1) return;

        if (timeSinceStart >= waves[currentWaveIndex + 1].timeToActivate)
        {
            currentWaveIndex++;
        }
    }

    private void HandleSpawning()
    {
        if (currentWaveIndex >= waves.Count) return;

        if (Time.time >= nextSpawnTime)
        {
            SpawnRandomEnemyFromCurrentWave();
            nextSpawnTime = Time.time + spawnInterval;
        }
    }

    private void SpawnRandomEnemyFromCurrentWave()
    {
        EnemyWave currentWave = waves[currentWaveIndex];

        List<EnemySpawnData> availableOneTimeEnemies = currentWave.enemiesToSpawn.FindAll(
            e => e.isOneTimeSpawn && !e.hasBeenSpawned);

        if (availableOneTimeEnemies.Count > 0)
        {
            int randomIndex = Random.Range(0, availableOneTimeEnemies.Count);
            EnemySpawnData enemyData = availableOneTimeEnemies[randomIndex];
            Vector3 groundPoint = GetSpawnPositionOnGround();

            if (groundPoint != Vector3.zero)
            {
                SpawnEnemyWithOffset(enemyData.enemyPrefab, groundPoint);
                enemyData.hasBeenSpawned = true;
            }
            return;
        }

        List<EnemySpawnData> regularEnemies = currentWave.enemiesToSpawn.FindAll(e => !e.isOneTimeSpawn);
        if (regularEnemies.Count > 0)
        {
            int randomEnemyIndex = Random.Range(0, regularEnemies.Count);
            Vector3 groundPoint = GetSpawnPositionOnGround();
            if (groundPoint != Vector3.zero)
            {
                SpawnEnemyWithOffset(regularEnemies[randomEnemyIndex].enemyPrefab, groundPoint);
            }
        }
    }

    private void SpawnEnemyWithOffset(GameObject enemyPrefab, Vector3 groundPoint)
    {
        EnemyBase spawnPoint = enemyPrefab.GetComponent<EnemyBase>();
        Vector3 finalPosition;

        if (spawnPoint != null)
        {
            finalPosition = groundPoint + spawnPoint.SpawnOffset;
        }
        else
        {
            finalPosition = groundPoint;
        }

        GameObject enemy = Instantiate(enemyPrefab, finalPosition, Quaternion.identity);

        Vector3 directionToCenter = (mainCamera.transform.position - enemy.transform.position);
        directionToCenter.y = 0; // Keep rotation only around Y axis
        if (directionToCenter != Vector3.zero)
        {
            enemy.transform.rotation = Quaternion.LookRotation(directionToCenter);
        }
    }

    private float SelectFromTwoRanges()
    {
        var var1 = Random.Range(-1.5f, -0.5f);
        var var2 = Random.Range(1.5f, 2.5f);
        var var3 = Random.Range(0, 2);
        if(var3 == 0)
        {
            return var1;
        } else
        {
            return var2;
        }
    }

    private Vector3 GetSpawnPositionOnGround()
    {
        Vector3 spawnPos = mainCamera.transform.position;
        spawnPos.y = 0;

        Vector3 viewportPoint = new Vector3(SelectFromTwoRanges(), SelectFromTwoRanges(), Random.Range(-2f,10f));
        Vector3 worldPoint = Camera.main.ViewportToWorldPoint(viewportPoint);

        Vector3 rayStart = new Vector3(worldPoint.x, raycastHeight, worldPoint.z);
        Ray ray = new Ray(rayStart, Vector3.down);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, raycastHeight * 2f, groundLayer))
        {
            // Debug visualization
            Debug.DrawLine(rayStart, hit.point, Color.red, 1f);
            Debug.DrawLine(mainCamera.transform.position, hit.point, Color.blue, 1f);
            return hit.point;
        }

        Debug.LogWarning("Could not find ground to spawn enemy!");
        return Vector3.zero;
    }
}

[System.Serializable]
public class EnemyWave
{
    public float timeToActivate;
    public List<EnemySpawnData> enemiesToSpawn;
}

[System.Serializable]
public class EnemySpawnData
{
    public GameObject enemyPrefab;
    public bool isOneTimeSpawn;
    [HideInInspector]
    public bool hasBeenSpawned;
}
