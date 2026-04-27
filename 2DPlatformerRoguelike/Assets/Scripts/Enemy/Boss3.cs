using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss3 : MonoBehaviour, IDamageable
{
    public enum Boss3State
    {
        Idle,
        Walk,
        Attack1,
        Attack2,
        Attack3,
        RunAttack,
        Dead
    }

    [Header("보스 상태")]
    public Boss3State currentState;
    public float maxHp = 1500f;
    public float currentHp;
    public float walkSpeed = 3f;
    public float runSpeed = 10f;

    [Header("사거리 설정")]
    public float meleeAttackRange = 2f;
    public float runAttackRange = 4f;

    [Header("데미지 설정")]
    public float attack1Damage = 15f;
    public float attack2Damage = 20f;
    public float attack3Damage = 25f;
    public float runDamage = 30f;

    [Header("타격 판정 범위")]
    public LayerMask playerLayer;
    public Vector2 attack1Box = new Vector2(3f, 3f);
    public float attack2Radius = 3f;
    public Vector2 attack3Box = new Vector2(2.5f, 2f);
    public Vector2 runHitBox = new Vector2(2f, 2f);

    [Header("돌진(Run) 벽 반사 설정")]
    public LayerMask wallLayer;
    public float wallCheckDistance = 1.5f;
    public int maxBounces = 3;

    [Header("보상 및 기타")]
    public int dropGold = 1000;
    public GameObject portalPrefab;
    public float idleToWalkDelay = 1f;

    [Header("사운드 설정")]
    public AudioClip deathSound;
    public AudioClip attack1Sound;
    public AudioClip attack2Sound;
    public AudioClip attack3Sound;
    public AudioClip runAttackSound;
    public AudioSource loopAudioSource;

    private Animator anim;
    private Rigidbody2D rb;
    private Transform player;
    private SpriteRenderer sr;

    private void Awake()
    {
        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();

        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null) player = playerObj.transform;
    }

    private void Start()
    {
        currentHp = maxHp;

        if (BossHealthBar.Instance != null)
        {
            BossHealthBar.Instance.ShowBossUI();
            BossHealthBar.Instance.UpdateHP(currentHp, maxHp);
        }
        ChangeState(Boss3State.Idle);
        StartCoroutine(ThinkRoutine());
    }

    private void Update()
    {
        if (currentState == Boss3State.Dead || player == null) return;

        if (currentState == Boss3State.Walk || currentState == Boss3State.Idle)
        {
            LookAtPlayer();
        }
    }

    private void FixedUpdate()
    {
        if (currentState == Boss3State.Dead || player == null) return;

        if (currentState == Boss3State.Walk)
        {
            float moveDir = (player.position.x > transform.position.x) ? 1f : -1f;
            rb.linearVelocity = new Vector2(moveDir * walkSpeed, rb.linearVelocity.y);
        }
        else if (currentState != Boss3State.RunAttack)
        {
            rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
        }
    }

    public void ChangeState(Boss3State newState)
    {
        if (currentState == Boss3State.Dead) return;
        currentState = newState;

        if (currentState == Boss3State.Walk) anim.Play("Walk");
        else if (currentState == Boss3State.Idle) anim.Play("Idle");
    }

    private void LookAtPlayer()
    {
        float baseScaleX = Mathf.Abs(transform.localScale.x);
        float scaleY = transform.localScale.y;
        float scaleZ = transform.localScale.z;
        if (player.position.x > transform.position.x)
        {
            transform.localScale = new Vector3(baseScaleX, scaleY, scaleZ);
        }
        else
        {
            transform.localScale = new Vector3(-baseScaleX, scaleY, scaleZ);
        }
    }

    private IEnumerator ThinkRoutine()
    {
        while (currentState != Boss3State.Dead && player != null)
        {
            if (currentState == Boss3State.Idle)
            {
                yield return new WaitForSeconds(idleToWalkDelay);
                ChangeState(Boss3State.Walk);
            }
            else if (currentState == Boss3State.Walk)
            {
                float distance = Vector2.Distance(transform.position, player.position);

                if (distance <= meleeAttackRange)
                {
                    int rand = Random.Range(0, 3);
                    if (rand == 0) StartCoroutine(Attack1Routine());
                    else if (rand == 1) StartCoroutine(Attack2Routine());
                    else StartCoroutine(Attack3Routine());
                }
                else if (distance >= runAttackRange)
                {
                    int rand = Random.Range(0, 100);
                    if (rand < 30)
                    {
                        StartCoroutine(RunAttackRoutine());
                    }
                }
            }
            yield return null;
        }
    }

    private IEnumerator Attack1Routine()
    {
        ChangeState(Boss3State.Attack1);
        anim.Play("Attack1");

        if (attack1Sound != null && SoundManager.Instance != null)
        {
            SoundManager.Instance.PlaySFX(attack1Sound);
        }

        yield return new WaitForSeconds(0.4f);

        float currentFacingDir = Mathf.Sign(transform.localScale.x);
        Vector2 hitCenter = (Vector2)transform.position + new Vector2(currentFacingDir * 1.5f, 0f);

        Collider2D hit = Physics2D.OverlapBox(hitCenter, attack1Box, 0f, playerLayer);
        if (hit != null) CheckAndDamage(hit, attack1Damage);

        yield return new WaitForSeconds(0.6f);
        ChangeState(Boss3State.Idle);
    }

    private IEnumerator Attack2Routine()
    {
        ChangeState(Boss3State.Attack2);
        anim.Play("Attack2");

        if (attack2Sound != null && SoundManager.Instance != null)
        {
            SoundManager.Instance.PlaySFX(attack2Sound);
        }

        yield return new WaitForSeconds(0.5f);

        Collider2D hit = Physics2D.OverlapCircle(transform.position, attack2Radius, playerLayer);
        if (hit != null) CheckAndDamage(hit, attack2Damage);

        yield return new WaitForSeconds(0.5f);
        ChangeState(Boss3State.Idle);
    }

    private IEnumerator Attack3Routine()
    {
        ChangeState(Boss3State.Attack3);
        anim.Play("Attack3");

        if (attack3Sound != null && SoundManager.Instance != null)
        {
            SoundManager.Instance.PlaySFX(attack3Sound);
        }

        yield return new WaitForSeconds(0.3f);

        float currentFacingDir = Mathf.Sign(transform.localScale.x);
        Vector2 hitCenter = (Vector2)transform.position + new Vector2(currentFacingDir * 2f, 0f);

        Collider2D hit = Physics2D.OverlapBox(hitCenter, attack3Box, 0f, playerLayer);
        if (hit != null) CheckAndDamage(hit, attack3Damage);

        yield return new WaitForSeconds(0.5f);
        ChangeState(Boss3State.Idle);
    }

    private IEnumerator RunAttackRoutine()
    {
        ChangeState(Boss3State.RunAttack);
        anim.Play("Idle");
        sr.color = Color.red;
        yield return new WaitForSeconds(0.5f);
        sr.color = Color.white;

        anim.Play("Run");

        if (runAttackSound != null && loopAudioSource != null)
        {
            loopAudioSource.clip = runAttackSound;
            loopAudioSource.loop = true;
            loopAudioSource.Play();
        }

        float runDir = Mathf.Sign(transform.localScale.x);
        int currentBounces = 0;
        bool hasHitPlayer = false;

        while (currentBounces < maxBounces)
        {
            rb.linearVelocity = new Vector2(runDir * runSpeed, rb.linearVelocity.y);

            RaycastHit2D wallHit = Physics2D.Raycast(transform.position, Vector2.right * runDir, wallCheckDistance, wallLayer);

            if (wallHit.collider != null)
            {
                runDir *= -1f;
                currentBounces++;
                hasHitPlayer = false;

                float baseScaleX = Mathf.Abs(transform.localScale.x);
                transform.localScale = new Vector3(baseScaleX * runDir, transform.localScale.y, transform.localScale.z);
            }

            if (!hasHitPlayer)
            {
                Collider2D hit = Physics2D.OverlapBox(transform.position, runHitBox, 0f, playerLayer);
                if (hit != null)
                {
                    CheckAndDamage(hit, runDamage);
                    hasHitPlayer = true;
                }
            }
            yield return null;
        }

        if (loopAudioSource != null) loopAudioSource.Stop();

        anim.Play("Walk");
        float targetCenterX = 7f;

        float returnDir = (targetCenterX > transform.position.x) ? 1f : -1f;
        float finalScaleX = Mathf.Abs(transform.localScale.x);
        transform.localScale = new Vector3(finalScaleX * returnDir, transform.localScale.y, transform.localScale.z);

        while (Mathf.Abs(transform.position.x - targetCenterX) > 0.5f)
        {
            rb.linearVelocity = new Vector2(returnDir * runSpeed, rb.linearVelocity.y);
            yield return null;
        }

        rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
        anim.Play("Idle");
        yield return new WaitForSeconds(1f);

        ChangeState(Boss3State.Idle);
    }

    private void CheckAndDamage(Collider2D hit, float damage)
    {
        IDamageable damageable = hit.GetComponentInParent<IDamageable>();
        if (damageable != null) damageable.TakeDamage(damage);
    }

    public void TakeDamage(float damage)
    {
        if (currentState == Boss3State.Dead) return;

        currentHp -= damage;
        if (BossHealthBar.Instance != null) BossHealthBar.Instance.UpdateHP(currentHp, maxHp);
        if (sr != null) StartCoroutine(HitFlashRoutine());

        if (currentHp <= 0) Die();
    }

    private IEnumerator HitFlashRoutine()
    {
        sr.color = Color.red;
        yield return new WaitForSeconds(0.1f);
        sr.color = Color.white;
    }

    private void Die()
    {
        if (currentState == Boss3State.Dead) return;
        ChangeState(Boss3State.Dead);

        if (loopAudioSource != null) loopAudioSource.Stop();

        if (deathSound != null && SoundManager.Instance != null)
        {
            SoundManager.Instance.PlaySFX(deathSound);
        }

        if (BossHealthBar.Instance != null) BossHealthBar.Instance.HideBossUI();
        if (sr != null) sr.color = Color.white;

        StopAllCoroutines();
        anim.Play("Dead");

        rb.linearVelocity = Vector2.zero;
        rb.gravityScale = 0;

        Collider2D[] allColliders = GetComponentsInChildren<Collider2D>();
        foreach (Collider2D coll in allColliders) coll.enabled = false;

        if (RoomManager.Instance != null) RoomManager.Instance.OnEnemyKilled();

        if (player != null)
        {
            Player playerScript = player.GetComponent<Player>();
            if (playerScript != null)
            {
                playerScript.AddGold(dropGold);
                playerScript.Heal(playerScript.maxHP * 0.2f);
            }
          
        }
        StartCoroutine(DeathRoutine());
    }

    private IEnumerator DeathRoutine()
    {
        yield return new WaitForSeconds(3f);
        if (portalPrefab != null) Instantiate(portalPrefab, transform.position, Quaternion.identity);
        Destroy(gameObject);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, runAttackRange);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, meleeAttackRange);

        float facingDir = Mathf.Sign(transform.localScale.x);

        Gizmos.color = Color.blue;
        Vector3 hitCenter1 = transform.position + new Vector3(facingDir * 1.5f, 0f, 0f);
        Gizmos.DrawWireCube(hitCenter1, attack1Box);

        Gizmos.color = Color.magenta;
        Gizmos.DrawWireSphere(transform.position, attack2Radius);

        Gizmos.color = Color.green;
        Vector3 hitCenter3 = transform.position + new Vector3(facingDir * 2f, 0f, 0f);
        Gizmos.DrawWireCube(hitCenter3, attack3Box);

        Gizmos.color = Color.cyan;
        Gizmos.DrawWireCube(transform.position, runHitBox);

        Gizmos.color = Color.white;
        Gizmos.DrawLine(transform.position, transform.position + new Vector3(facingDir * wallCheckDistance, 0f, 0f));
    }
}