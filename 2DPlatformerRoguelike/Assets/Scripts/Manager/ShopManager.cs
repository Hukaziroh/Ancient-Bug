using UnityEngine;

public class ShopManager : MonoBehaviour
{
    public static ShopManager Instance;
    [Header("UI 연결")]
    public GameObject shopPanel;

    [Header("상품 가격")]
    public int healCost = 50;
    public int maxHpCost = 150;
    public int attackCost = 200;

    [Header("사운드 설정")]
    public AudioClip buySound; 
    public AudioClip failSound;

    private Player player;

    private bool isOpenedFromDoor = false;
    private bool isLevelTransition = false;

    private void Awake()
    {
        if (Instance == null) Instance = this;
    }
    private void Start()
    {
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null) player = playerObj.GetComponent<Player>();
    }

    public void OpenShopFromDoor(bool isLevel = false)
    {
        isOpenedFromDoor = true;
        isLevelTransition = isLevel;
        if (shopPanel != null) shopPanel.SetActive(true);
    }

    public void BuyHeal()
    {
        if (player.SpendGold(healCost))
        {
            if (buySound != null && SoundManager.Instance != null) SoundManager.Instance.PlaySFX(buySound);
            player.Heal(30f);
        }
        else
        {
            if (failSound != null && SoundManager.Instance != null) SoundManager.Instance.PlaySFX(failSound);
        }
    }

    public void BuyMaxHp()
    {
        if (player.SpendGold(maxHpCost))
        {
            if (buySound != null && SoundManager.Instance != null) SoundManager.Instance.PlaySFX(buySound);
            player.IncreaseMaxHp(20f);
        }
        else
        {
            if (failSound != null && SoundManager.Instance != null) SoundManager.Instance.PlaySFX(failSound);
        }
    }

    public void BuyAttackUp()
    {
        if (player.SpendGold(attackCost))
        {
            if (buySound != null && SoundManager.Instance != null) SoundManager.Instance.PlaySFX(buySound);
            PlayerAttack pAttack = player.GetComponent<PlayerAttack>();
            if (pAttack != null)
            {
                pAttack.AttackDamage += 5f;
            }
        }
        else
        {
            if (failSound != null && SoundManager.Instance != null) SoundManager.Instance.PlaySFX(failSound);
        }
    }

    public void CloseShop()
    {
        if (shopPanel != null) shopPanel.SetActive(false);

        if (isOpenedFromDoor)
        {
            isOpenedFromDoor = false;
            Time.timeScale = 1f;
            if (isLevelTransition)
            {
                RoomManager.Instance.GoToNextLevel();
                isLevelTransition = false;
            }
            else
            {
                RoomManager.Instance.LoadNextRoom();
            }
        }
        else
        {
            Time.timeScale = 1f;
        }
    }
}