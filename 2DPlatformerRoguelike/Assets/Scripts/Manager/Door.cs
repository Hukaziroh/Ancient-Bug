using UnityEngine;

public class Door : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {     
        if (collision.CompareTag("Player"))
        {          
            RoomManager.Instance.LoadNextRoom();
        }
    }
}