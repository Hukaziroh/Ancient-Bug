using UnityEngine;
using Unity.Cinemachine;

public class CameraController : MonoBehaviour
{
    public static CameraController Instance;
    private CinemachineConfiner2D confiner;

    private void Awake()
    {
        Instance = this;
        confiner = GetComponent<CinemachineConfiner2D>();
    }

    public void UpdateCameraBounds(Collider2D newBounds)
    {
        if (confiner != null)
        {
            confiner.BoundingShape2D = newBounds;
            confiner.InvalidateBoundingShapeCache();
        }
    }
}