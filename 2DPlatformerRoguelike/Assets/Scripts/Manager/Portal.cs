using UnityEngine;

public class Portal : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
 
        if (other.CompareTag("Player"))
        {
            if (RoomManager.Instance != null)
            {
                RoomManager.Instance.GoToNextLevel();
                Destroy(gameObject); 
            }
        }
    }
}