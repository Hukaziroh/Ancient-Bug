using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using System.Runtime.CompilerServices;


public class RewardManager : MonoBehaviour
{

    public static RewardManager Instance;

    [Header("UI ¿¬°á")]
    public GameObject rewardPanel;
    public TextMeshProUGUI UpButtonText;
    public TextMeshProUGUI DownButtonText;

    [HideInInspector] public bool isLevelPortal = false;

    private enum RewardType { AttackUp, Heal, MaxHpUp, Shop}

    private RewardType upReward;
    private RewardType downReward;

    private void Awake()
    {
        if (Instance == null) Instance = this;
    }

    private void Start()
    {
        if (rewardPanel != null) rewardPanel.SetActive(false);
    }

    public void ShowRewardUI()
    {
        rewardPanel.SetActive(true);
        Time.timeScale = 0f;

        List<RewardType> pool = new List<RewardType> { RewardType.AttackUp, RewardType.Heal, RewardType.MaxHpUp, RewardType.Shop };

        int index1 = Random.Range(0, pool.Count);
        upReward = pool[index1];
        pool.RemoveAt(index1);

        int index2 = Random.Range(0, pool.Count);
        downReward = pool[index2];

        UpButtonText.text = GetRewardName(upReward);
        DownButtonText.text = GetRewardName(downReward);
    }

    private string GetRewardName(RewardType type)
    {
        switch(type)
        {
            case RewardType.AttackUp: return "ATK UP";
            case RewardType.Heal: return "HEAL";
            case RewardType.MaxHpUp: return "MAX HP UP";
            case RewardType.Shop: return "SHOP";

        }
        return "";
    }

    public void SelectUpReward()
    {
        ApplyReward(upReward);

    }
    public void SelectDownReward()
    {
        ApplyReward(downReward);
    }

    private void ApplyReward(RewardType type)
    {
        rewardPanel.SetActive(false);

        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj == null) return;

        Player player = playerObj.GetComponent<Player>();
        PlayerAttack playerAttack = playerObj.GetComponent<PlayerAttack>();

        if (type == RewardType.AttackUp)
        {
            playerAttack.AttackDamage += 10f;
        }
        else if(type == RewardType.Heal)
        {
            player.Heal(30f);
        }
        else if(type == RewardType.MaxHpUp)
        {
            player.IncreaseMaxHp(20f);
        }
   
        if (type == RewardType.Shop)
        {
            if (ShopManager.Instance != null)
            {            
                ShopManager.Instance.OpenShopFromDoor();
                isLevelPortal = false;
            }
        }
        else
        {
         
            Time.timeScale = 1f;
            if (isLevelPortal)
            {
                RoomManager.Instance.GoToNextLevel();
                isLevelPortal = false;
            }
            else
            {
                RoomManager.Instance.LoadNextRoom();
            }          
        }
    }   
}
