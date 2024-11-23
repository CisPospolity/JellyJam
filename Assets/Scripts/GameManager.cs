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

    private Vector3 GetSpawnPositionOnGround()
    {
        // Get the camera's forward direction projected onto the XZ plane
        Vector3 cameraForward = mainCamera.transform.forward;
        cameraForward.y = 0;
        cameraForward.Normalize();

        // Get the camera's right direction projected onto the XZ plane
        Vector3 cameraRight = mainCamera.transform.right;
        cameraRight.y = 0;
        cameraRight.Normalize();

        // Get camera position projected onto XZ plane
        Vector3 cameraPos = mainCamera.transform.position;
        Vector3 cameraPosGround = new Vector3(cameraPos.x, 0, cameraPos.z);

        // Calculate visible area extents
        float verticalFov = mainCamera.fieldOfView;
        float horizontalFov = Camera.VerticalToHorizontalFieldOfView(verticalFov, mainCamera.aspect);

        // Convert FOV to radians
        float verticalFovRad = verticalFov * Mathf.Deg2Rad;
        float horizontalFovRad = horizontalFov * Mathf.Deg2Rad;

        // Calculate view distances based on camera height and angle
        float cameraHeight = mainCamera.transform.position.y;
        float visibleDistance = cameraHeight * Mathf.Tan(verticalFovRad);

        // Choose a random direction to spawn (0: front, 1: right, 2: back, 3: left)
        int spawnSide = Random.Range(0, 4);
        Vector3 spawnPos = cameraPosGround;

        switch (spawnSide)
        {
            case 0: // Front
                spawnPos += cameraForward * (visibleDistance + spawnOffset);
                spawnPos += cameraRight * Random.Range(-visibleDistance, visibleDistance);
                break;
            case 1: // Right
                spawnPos += cameraRight * (visibleDistance + spawnOffset);
                spawnPos += cameraForward * Random.Range(-visibleDistance, visibleDistance);
                break;
            case 2: // Back
                spawnPos -= cameraForward * (visibleDistance + spawnOffset);
                spawnPos += cameraRight * Random.Range(-visibleDistance, visibleDistance);
                break;
            case 3: // Left
                spawnPos -= cameraRight * (visibleDistance + spawnOffset);
                spawnPos += cameraForward * Random.Range(-visibleDistance, visibleDistance);
                break;
        }

        // Cast ray to find ground position
        Vector3 rayStart = spawnPos + Vector3.up * raycastHeight;
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

    private void OnDrawGizmos()
    {
        if (!Application.isPlaying && mainCamera != null)
        {
            // Draw spawn area visualization
            Vector3 cameraPos = mainCamera.transform.position;
            Vector3 cameraPosGround = new Vector3(cameraPos.x, 0, cameraPos.z);

            float verticalFov = mainCamera.fieldOfView;
            float visibleDistance = cameraPos.y * Mathf.Tan(verticalFov * Mathf.Deg2Rad);

            Gizmos.color = Color.yellow;
            // Draw inner rectangle (visible area)
            DrawRectangle(cameraPosGround, visibleDistance, mainCamera.transform.forward, mainCamera.transform.right);

            // Draw outer rectangle (spawn area)
            Gizmos.color = Color.red;
            DrawRectangle(cameraPosGround, visibleDistance + spawnOffset, mainCamera.transform.forward, mainCamera.transform.right);
        }
    }

    private void DrawRectangle(Vector3 center, float size, Vector3 forward, Vector3 right)
    {
        forward.y = 0;
        right.y = 0;
        forward.Normalize();
        right.Normalize();

        Vector3 topLeft = center + forward * size - right * size;
        Vector3 topRight = center + forward * size + right * size;
        Vector3 bottomLeft = center - forward * size - right * size;
        Vector3 bottomRight = center - forward * size + right * size;

        Gizmos.DrawLine(topLeft, topRight);
        Gizmos.DrawLine(topRight, bottomRight);
        Gizmos.DrawLine(bottomRight, bottomLeft);
        Gizmos.DrawLine(bottomLeft, topLeft);
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
