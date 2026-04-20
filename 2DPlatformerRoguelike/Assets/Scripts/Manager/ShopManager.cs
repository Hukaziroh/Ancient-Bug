using UnityEngine;

public class ShopManager : MonoBehaviour
{


    public static ShopManager Instance;
    [Header("UI ¿¬°á")]
    public GameObject shopPanel;

    [Header("»óÇ° °¡°Ý")]
    public int healCost = 50;
    public int maxHpCost = 150;
    public int attackCost = 200;

    private Player player;

    private bool isOpenedFromDoor = false;

    private void Awake()
    {
        if (Instance == null) Instance = this;
    }
    private void Start()
    {
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null) player = playerObj.GetComponent<Player>();
    }

    public void OpenShopFromDoor()
    {
        isOpenedFromDoor = true;
        if (shopPanel != null) shopPanel.SetActive(true);
    }

    public void BuyHeal()
    {
        if (player.SpendGold(healCost))
        {
            player.Heal(30f);
        }
        else Debug.Log("µ·¾øÀ½");
    }

    public void BuyMaxHp()
    {
        if (player.SpendGold(maxHpCost))
        {
            player.IncreaseMaxHp(20f);
        }
        else Debug.Log("µ·¾øÀ½");
    }

    public void BuyAttackUp()
    {
        if(player.SpendGold(attackCost))
        {
            PlayerAttack pAttack = player.GetComponent<PlayerAttack>();
            if(pAttack != null)
            {
                pAttack.AttackDamage += 10f;
            }          
        }
        else Debug.Log("µ·¾øÀ½");
    }

    public void CloseShop()
    {
        if (shopPanel != null) shopPanel.SetActive(false);

        if(isOpenedFromDoor)
        {
            isOpenedFromDoor = false;
            Time.timeScale = 1f;
            RoomManager.Instance.LoadNextRoom();
        }
        else
        {
            Time.timeScale = 1f;
        }
    }
}
