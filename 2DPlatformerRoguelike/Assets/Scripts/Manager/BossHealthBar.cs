using UnityEngine;
using UnityEngine.UI; 

public class BossHealthBar : MonoBehaviour
{
    public static BossHealthBar Instance;

    [Header("UI ¿¬°á")]
    public GameObject uiContainer; 
    public Image fillImage;

    private void Awake()
    {
        Instance = this;
        if (uiContainer != null) uiContainer.SetActive(false);
    }

    public void ShowBossUI()
    {
        if (uiContainer != null) uiContainer.SetActive(true);
    }

    public void UpdateHP(float currentHp, float maxHp)
    {
        if (fillImage != null)
        {
            fillImage.fillAmount = currentHp / maxHp;
        }
    }

    public void HideBossUI()
    {
        if (uiContainer != null) uiContainer.SetActive(false);
    }
}