using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [SerializeField] private Transform target;
    [SerializeField] private Vector3 offset = new Vector3(0, 0, -10);
    [SerializeField] private float smoothSpeed = 5f;
    [SerializeField] private bool useFixedUpdate = false;

    private void Start()
    {
        if (target == null)
        {
            target = GameObject.FindGameObjectWithTag("Player")?.transform;

            if (target == null)
            {
                enabled = false;
                return;
            }
        }

        transform.position = target.position + offset;
    }

    private void Update()
    {
        if (!useFixedUpdate)
        {
            FollowTarget();
        }
    }

    private void FixedUpdate()
    {
        if (useFixedUpdate)
        {
            FollowTarget();
        }
    }

    private void FollowTarget()
    {
        if (target == null) return;

        Vector3 desiredPosition = target.position + offset;
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed * Time.deltaTime);
        transform.position = smoothedPosition;
    }

    public void SetTarget(Transform newTarget)
    {
        target = newTarget;
    }
}
