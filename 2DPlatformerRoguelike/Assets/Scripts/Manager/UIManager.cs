using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Unity.VisualScripting;
public class UIManager : MonoBehaviour
{
    public static UIManager Instance;

    [Header("스킬 UI")]
    public SkillCooldownUI dashSkillUI;
    public SkillCooldownUI skillUI;

    [Header("체력 UI")]
    public Image hpFillImage;

    [Header("재화 UI")]
    public TextMeshProUGUI goldText;
    public TextMeshProUGUI stoneText;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    public void UpdateHp(float currentHP, float maxHP)
    {
        if(hpFillImage != null)
        {
            hpFillImage.fillAmount = currentHP / maxHP;
        }
    }

    public void UpdateGold(int amount)
    {
        if (goldText != null) goldText.text = amount.ToString();
    }

    public void UpdateStone(int amount)
    {
        if (stoneText != null) stoneText.text = amount.ToString();
    }
}
