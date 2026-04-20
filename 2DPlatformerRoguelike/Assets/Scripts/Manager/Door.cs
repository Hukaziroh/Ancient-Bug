using UnityEngine;

public class Door : MonoBehaviour
{
    public Sprite openDoorSprite;

    SpriteRenderer spriteRenderer;

    private bool isOpened = false;
    private void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void Update()
    {
        if (!isOpened && RoomManager.Instance.remainingEnemies <= 0)
        {
            OpenDoor();
        }
    }

    private void OpenDoor()
    {
        isOpened = true;    
        if (openDoorSprite != null)
        {
            spriteRenderer.sprite = openDoorSprite;
        }

    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            if (RoomManager.Instance != null && RoomManager.Instance.remainingEnemies <= 0)
            {
                RoomManager.Instance.LoadNextRoom();
            }
            else
            {
                Debug.Log("아직 몬스터가 남아있어 문이 열리지 않습니다!");
            }
        }
    }
}