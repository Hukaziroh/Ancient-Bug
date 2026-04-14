using UnityEngine;
using DG.Tweening;

public class SimpleGroupScroll : MonoBehaviour
{
    public float moveDistance = 20f; 
    public float duration = 10f;     

    void Start()
    {       
        transform.DOMoveX(-moveDistance, duration)
            .SetEase(Ease.Linear)
            .SetLoops(-1, LoopType.Restart);
    }
}