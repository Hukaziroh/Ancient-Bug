using UnityEngine;
using UnityEngine.UI; 
public class UIButtonSound : MonoBehaviour
{
    [Header("버튼 클릭 사운드")]
    public AudioClip clickSound;

    private void Start()
    {    
        Button btn = GetComponent<Button>();
  
        if (btn != null && clickSound != null)
        {
            btn.onClick.AddListener(PlaySound);
        }
    }

    private void PlaySound()
    {
        if (SoundManager.Instance != null && clickSound != null)
        {
            SoundManager.Instance.PlaySFX(clickSound);
        }
    }
}