using UnityEngine;
using DG.Tweening; 
public class LobbyTitleEffect : MonoBehaviour
{
    void Start()
    {      
        transform.localScale = Vector3.zero;
        transform.rotation = Quaternion.Euler(0, -180, 0);

        transform.DOScale(1f, 1.4f).SetEase(Ease.OutBack);
     
        transform.DORotate(new Vector3(0, 0, 0), 1.4f, RotateMode.FastBeyond360)
                 .SetEase(Ease.OutCubic);
    }
}