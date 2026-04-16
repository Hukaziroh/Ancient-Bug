using UnityEngine;
using UnityEngine.UI;
using TMPro;
using JetBrains.Annotations;
using Unity.VisualScripting;

public class SkillCooldownUI : MonoBehaviour
{
    public static SkillCooldownUI Instance;

    [Header("UI ¿¬°á")]
    public Image cooldownImage;

    float maxCooldown;
    float currentCooldown;
    public bool isCooldown { get; private set; }

    private void Awake()
    {
        Instance = this;
    }
    private void Start()
    {
        ResetCoolDownUI();
    }

    private void Update()
    {
        if(isCooldown)
        {
            currentCooldown -= Time.deltaTime;
            if(currentCooldown <=0f)
            {
                isCooldown = false;
                ResetCoolDownUI();
            }
            else
            {
                cooldownImage.fillAmount = currentCooldown/maxCooldown;
            }

            

        }
    }
    public void UseSkill(float cooldownTime)
    {
        //if (isCooldown) return;
        maxCooldown = cooldownTime;
        currentCooldown = cooldownTime;
        isCooldown = true;
        cooldownImage.fillAmount = 1f;
    }

    private void ResetCoolDownUI()
    {
        currentCooldown = 0f;
        cooldownImage.fillAmount = 0f;
    }

}
