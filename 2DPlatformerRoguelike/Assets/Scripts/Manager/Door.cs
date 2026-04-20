using UnityEngine;

public class Door : MonoBehaviour
{
    public Sprite openDoorSprite;

    SpriteRenderer spriteRenderer;

    private bool isOpened = false;
    private bool wasUsed = false;
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
            if (isOpened && !wasUsed && !RoomManager.Instance.isTransitioning)
            {
                wasUsed = true; 

                if (RewardManager.Instance != null)
                {
                    RewardManager.Instance.ShowRewardUI();
                }
            }
        }
    }
}