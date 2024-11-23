using UnityEngine;

public class ExpObj : MonoBehaviour
{
    [SerializeField] private int expValue = 1;

    public void SetExpValue(int value)
    {
        expValue = value;
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            var playerScript = other.GetComponent<PlayerScript>();
            if (playerScript == null) return;
            playerScript.AddExp(expValue);
            Destroy(gameObject);
        }
    }
}
