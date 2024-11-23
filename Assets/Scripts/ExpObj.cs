using UnityEngine;

public class ExpObj : MonoBehaviour
{
    [SerializeField] private int expValue = 1;

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
