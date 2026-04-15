using UnityEngine;

public class RoomBoundsUpdater : MonoBehaviour
{
    private void Start()
    {
        CompositeCollider2D myBounds = GetComponent<CompositeCollider2D>();

        if (CameraController.Instance != null && myBounds != null)
        {
            CameraController.Instance.UpdateCameraBounds(myBounds);          
        }
      
    }
}