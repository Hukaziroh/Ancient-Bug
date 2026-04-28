using UnityEngine;
using System.Collections;

public class Door : MonoBehaviour
{
    public Sprite openDoorSprite;

    SpriteRenderer spriteRenderer;

    private bool isOpened = false;
    private bool wasUsed = false;

    private IEnumerator Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();

        yield return null;

 
        if (RoomManager.Instance != null && RoomManager.Instance.remainingEnemies <= 0)
        {
            OpenDoor();
        }
    }

    public void OpenDoor()
    {
        if (isOpened) return;

        isOpened = true;
        if (openDoorSprite != null)
        {
            spriteRenderer.sprite = openDoorSprite;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
      
        if (Player.Instance != null && collision.gameObject == Player.Instance.gameObject)
        {
            if (isOpened && !wasUsed && !RoomManager.Instance.isTransitioning)
            {
                wasUsed = true;

                if (RewardManager.Instance != null)
                {
                    RewardManager.Instance.isLevelPortal = false;
                    RewardManager.Instance.ShowRewardUI();
                }
            }
        }
    }
}