using UnityEngine;

public class RotationScript : MonoBehaviour
{
    [SerializeField] private Vector3 rotationSpeed = new Vector3(0, 90, 0); // Degrees per second
    [SerializeField] private bool useLocalRotation = false;

    private void Update()
    {
        if (useLocalRotation)
        {
            transform.Rotate(rotationSpeed * Time.deltaTime, Space.Self);
        }
        else
        {
            transform.Rotate(rotationSpeed * Time.deltaTime, Space.World);
        }
    }
}
