using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.Rendering;
using System.Collections;

public class PlayerAttack : MonoBehaviour
{
    [Header("공격 설정")]
    public float AttackDamage = 10f;

    [Header("타격 판정 설정")]
    public Transform attackPoint;
    public Vector2 attackBoxSize = new Vector2(2f, 1f);
    public LayerMask enemyLayer;

    [Header("번개 스킬 설정")]
    public GameObject lightningPrefab;
    public float skillDamage = 20f;
    public float skillRange = 10f;
    public int maxTargetCount = 3;
    public float lightingYOffset = 1.5f;
    public float lightningStrikeDelay = 0.3f;

    [Header("사운드 설정")]
    public AudioClip attackSwingSound; 
    public AudioClip attackHitSound;  
    public AudioClip skillSound;      

    Animator anim;
    Vector2 moveInput;

    private void Awake()
    {
        anim = GetComponent<Animator>();
    }

    public void OnMove(InputValue value)
    {
        moveInput = value.Get<Vector2>();
    }

    public void OnAttack(InputValue value)
    {
        if (Time.timeScale == 0f) return;

        if (value.isPressed)
        {
            if (attackSwingSound != null && SoundManager.Instance != null)
            {
                SoundManager.Instance.PlaySFX(attackSwingSound);
            }

            if (moveInput.y > 0.5f)
            {
                anim.SetTrigger("UpAttack");
            }
            else
            {
                anim.SetTrigger("Attack");
            }
        }
    }

    public void OnSkill(InputValue value)
    {
        if (Time.timeScale == 0f) return;
        if (UIManager.Instance.skillUI.isCooldown) return;

        if (value.isPressed)
        {
            if (skillSound != null && SoundManager.Instance != null)
            {
                SoundManager.Instance.PlaySFX(skillSound);
            }

            anim.SetTrigger("IsSkill");
            UIManager.Instance.skillUI.UseSkill(5f);
            PerformSkill();
        }
    }

    public void PerformAttack()
    {
        if (attackPoint == null) return;

        Collider2D[] hitEnemies = Physics2D.OverlapBoxAll(attackPoint.position, attackBoxSize, 0f, enemyLayer);
        bool hasHitTarget = false;

        foreach (Collider2D enemy in hitEnemies)
        {
            IDamageable damageable = enemy.GetComponent<IDamageable>();
            if (damageable != null)
            {
                damageable.TakeDamage(AttackDamage);
                hasHitTarget = true; 
            }
        }

        if (hasHitTarget && attackHitSound != null && SoundManager.Instance != null)
        {
            SoundManager.Instance.PlaySFX(attackHitSound);
        }
    }

    public void PerformSkill()
    {
        if (lightningPrefab == null) return;

        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(transform.position, skillRange, enemyLayer);
        if (hitEnemies.Length == 0) return;

        List<Collider2D> sortedEnemies = hitEnemies.OrderBy(enemy =>
            Vector2.Distance(transform.position, enemy.transform.position)).ToList();

        int targetCount = Mathf.Min(maxTargetCount, sortedEnemies.Count);

        for (int i = 0; i < targetCount; i++)
        {
            Transform target = sortedEnemies[i].transform;
            Vector3 spawnPosition = new Vector3(target.position.x, target.position.y + lightingYOffset, 0f);

            GameObject lightning = LightningPool.Instance.Get();
            lightning.transform.position = spawnPosition;
            lightning.transform.rotation = Quaternion.identity;

            float finalDamage = AttackDamage + skillDamage;

            StartCoroutine(DelayedDamageRoutine(target, lightning, finalDamage, lightningStrikeDelay));
        }
    }

    private System.Collections.IEnumerator DelayedDamageRoutine(Transform target, GameObject lightning, float damage, float delay)
    {
        float timer = 0f;

        while (timer < delay)
        {
            yield return new WaitForFixedUpdate();
            timer += Time.fixedDeltaTime;

            if (target != null && lightning != null && lightning.activeSelf)
            {
                lightning.transform.position = new Vector3(target.position.x, target.position.y + lightingYOffset, 0f);
            }
        }

        if (target != null)
        {
            IDamageable damageable = target.GetComponent<IDamageable>();
            if (damageable != null)
            {
                damageable.TakeDamage(damage);
            }
        }
    }
    private System.Collections.IEnumerator ReleaseLightningRoutine(GameObject lightning, float delay)
    {
        yield return new WaitForSeconds(delay);

        if (lightning != null && lightning.activeSelf && LightningPool.Instance != null)
        {
            LightningPool.Instance.Release(lightning);
        }
    }
}