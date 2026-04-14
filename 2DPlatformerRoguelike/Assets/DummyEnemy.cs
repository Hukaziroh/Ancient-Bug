using UnityEngine;
using System.Collections;

public class DummyEnemy : MonoBehaviour, IDamageable
{
    [Header("스탯 데이터")]
    public EnemyStatData statData; 

    private float currentHP;
    private SpriteRenderer spriteRenderer;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void Start()
    {      
        if (statData != null)
        {
            currentHP = statData.maxHP;
            Debug.Log($"{statData.enemyName} 생성됨! 체력: {currentHP}");
        }
        else
        {
            Debug.LogWarning("EnemyStatData가 할당되지 않았습니다!");
        }
    }


    public void TakeDamage(float damage)
    {
        currentHP -= damage;
        Debug.Log($"{statData.enemyName} 피격! 남은 체력: {currentHP} (받은 데미지: {damage})");
   
        StartCoroutine(HitFlashRoutine());

        if (currentHP <= 0)
        {
            Die();
        }
    }

    private IEnumerator HitFlashRoutine()
    {
        if (spriteRenderer != null)
        {
            spriteRenderer.color = Color.red;
            yield return new WaitForSeconds(0.1f);
            spriteRenderer.color = Color.white;
        }
    }

    private void Die()
    {
        Debug.Log($"{statData.enemyName} 처치됨!");
       
        Destroy(gameObject);
    }
}