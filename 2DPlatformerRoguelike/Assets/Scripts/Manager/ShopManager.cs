using UnityEngine;

public class ShopManager : MonoBehaviour
{
    [Header("UI 연결")]
    public GameObject shopPanel;

    [Header("상품 가격")]
    public int healCost = 50;
    public int maxHpCost = 150;
    public int attackCost = 200;

    private Player player;

    private void Start()
    {
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null) player = playerObj.GetComponent<Player>();
    }

    public void BuyHeal()
    {
        if (player.SpendGold(healCost))
        {
            player.Heal(30f);
            Debug.Log("체력");
        }
        else Debug.Log("돈없음");
    }

    public void BuyMaxHp()
    {
        if (player.SpendGold(maxHpCost))
        {
            player.IncreaseMaxHp(20f);
            Debug.Log("최체증");
        }
        else Debug.Log("돈없음");
    }

    public void BuyAttackUp()
    {
        if(player.SpendGold(attackCost))
        {
            PlayerAttack pAttack = player.GetComponent<PlayerAttack>();
            if(pAttack != null)
            {
                pAttack.AttackDamage += 10f;
                Debug.Log("공증완");
            }          
        }
        else Debug.Log("돈없음");
    }

    public void CloseShop()
    {
        if (shopPanel != null) shopPanel.SetActive(false);
    }
}
