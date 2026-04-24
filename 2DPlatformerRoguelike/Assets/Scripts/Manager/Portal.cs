using UnityEngine;

public class Portal : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            if (RewardManager.Instance != null && RoomManager.Instance != null)
            {
                RewardManager.Instance.isLevelPortal = true;
                RewardManager.Instance.ShowRewardUI();
                Destroy(gameObject);
            }
        }
    }
}