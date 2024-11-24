using UnityEngine;

public class FloatScript : MonoBehaviour
{
    [SerializeField] private Vector3 relativeOffset = new Vector3(0f, 0.75f, 0f);
    [SerializeField] private float smoothSpeed = 5f;
    [SerializeField] private float maxMoveSpeed = 10f;
    [SerializeField] private float floatAmplitude = 0.2f;
    [SerializeField] private float floatSpeed = 2f;

    private Vector3 initialLocalPosition;
    private Vector3 initialOffset;
    private float startTime;
    private Vector3 currentVelocity;

    void Start()
    {
        startTime = Time.time;
        initialOffset = relativeOffset;
        initialLocalPosition = transform.localPosition;
    }

    void Update()
    {
        float floatOffset = Mathf.Sin((Time.time - startTime) * floatSpeed) * floatAmplitude;

        Vector3 currentOffset = initialOffset + Vector3.up * floatOffset;

        // Calculate target position in local space
        Vector3 targetLocalPosition = initialLocalPosition + currentOffset;

        // Smooth damp for local position
        transform.localPosition = Vector3.SmoothDamp(
            transform.localPosition,
            targetLocalPosition,
            ref currentVelocity,
            1f / smoothSpeed,
            maxMoveSpeed
        );
    }
}
