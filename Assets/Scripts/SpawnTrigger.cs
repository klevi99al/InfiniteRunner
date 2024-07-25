using UnityEngine;

public class SpawnTrigger : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            Manager.Instance.SpawnNextArea();
        }
    }
}