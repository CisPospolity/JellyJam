using UnityEngine;

public class ExpMagnet : MonoBehaviour
{
    [SerializeField] private float pullRadius = 5f;
    [SerializeField] private LayerMask expLayer;

    [SerializeField] private float minPullSpeed = 5f;
    [SerializeField] private float maxPullSpeed = 15f;
    [SerializeField] private AnimationCurve pullCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);

    private void Update()
    {
        Collider[] expColliders = Physics.OverlapSphere(transform.position, pullRadius, expLayer);

        foreach (Collider col in expColliders)
        {
            Vector3 currentPos = col.transform.position;
            Vector3 directionToPlayer = transform.position - currentPos;
            float distance = directionToPlayer.magnitude;

            float distanceRatio = 1f - (distance / pullRadius);
            float speedMultiplier = pullCurve.Evaluate(distanceRatio);
            float currentPullSpeed = Mathf.Lerp(minPullSpeed, maxPullSpeed, speedMultiplier);

            Vector3 newPosition = currentPos + (directionToPlayer.normalized * currentPullSpeed * Time.deltaTime);
            col.transform.position = newPosition;
        }
    }
}
