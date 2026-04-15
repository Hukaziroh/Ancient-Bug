using UnityEngine;

public class BackgroundScroller : MonoBehaviour
{
    public float scrollSpeed = 5f;
    public float endX = -20f;
    public float startX = 20f;

    void Update()
    {
        transform.Translate(Vector3.left * scrollSpeed * Time.deltaTime);

        if (transform.position.x <= endX)
        {
            Vector2 newPos = new Vector2(startX, transform.position.y);
            transform.position = newPos;
        }
    }
}