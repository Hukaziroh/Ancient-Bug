using UnityEngine;
using System.Collections;
using UnityEditor.Experimental.GraphView;
using UnityEngineInternal;
public class Boss2 : MonoBehaviour, IDamageable
{
    public enum Boss2State
    {
        Idle, Walk, BasicAttack, RangedAttack, ChargeAttack, Dead
    }

    [Header("보스 상태 및 스탯")]
    public Boss2State currentState;
    public float maxHp = 1200f;
    public float currentHp;
    public float moveSpeed = 3.5f;

    [Header("패턴 사거리 설정")]
    public float meleeAttackRange = 2f;
    public float chargeAttackRange = 6f;
    public float rangedAttackRange = 10f;

    [Header("데미지 설정")]
    public float basicDamage = 15f;
    public float chargeMoveDamage = 20f;
    public float chargeDamage = 40f;

    [Header("타격  판정 설정(기본)")]
    public LayerMask playerLayer;
    public Vector2 attackHitBox = new Vector2(3f, 2f);
    public float hitOffset = 1.5f;

    [Header("원거리 공격 설정")]
    public GameObject projectilePrefab;
    public Transform firePoint;

    [Header("보상 설정")]
    public int dropGold = 2000;
    public GameObject portalPrefab;

    [Header("타이머")]
    public float idleDelay = 1.5f;

    [Space(10)]
    public float basicAnticipation = 0.2f; // 평타 선딜
    public float basicHitDelay = 0.3f;     // 평타 데미지 들어가는 타이밍
    public float basicRecoil = 0.5f;       // 평타 후딜

    [Space(10)]
    public float rangedAnticipation = 0.4f; // 발사 전 폼잡기
    public float rangedRecoil = 0.6f;       // 발사 후 대기시간

    [Space(10)]
    public float chargeWarningTime = 0.6f;  // 빨갛게 번쩍이는 경고 시간
    public float chargeSpeed = 12f;         // 돌진 속도
    public float chargeDuration = 0.4f;     // 돌진 유지 시간
    public float chargeSlamDelay = 0.2f;    // 도착 후 내리찍기(Attack2) 데미지 타이밍
    public float chargeRecoil = 1f;         // 큰 기술 쓴 후 숨고르기 (후딜)

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
        ChangeState(Boss2State.Idle);
        StartCoroutine(ThinkRoutine());
    }

    private void Update()
    {
        if (currentState == Boss2State.Dead || player == null) return;

        if(currentState == Boss2State.Walk || currentState == Boss2State.Idle)
        {
            LookAtPlayer();
        }
    }

    private void FixedUpdate()
    {
        if (currentState == Boss2State.Dead || player == null) return;

        if(currentState == Boss2State.Walk)
        {
            Vector2 direction = (player.position - transform.position).normalized;
            rb.linearVelocity = new Vector2(direction.x * moveSpeed, rb.linearVelocity.y);
        }
        else if(currentState != Boss2State.ChargeAttack)
        {
            rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
        }
    }

    public void ChangeState(Boss2State newState)
    {
        if (currentState == Boss2State.Dead) return;
        currentState = newState;

        if (currentState == Boss2State.Walk) anim.Play("Run");
        else if (currentState == Boss2State.Idle) anim.Play("Idle");
    }    

    private void LookAtPlayer()
    {
        float currentSizeX = Mathf.Abs(transform.localScale.x);
        if (player.position.x > transform.position.x)
        {
            transform.localScale = new Vector3(currentSizeX, transform.localScale.y, transform.localScale.z);
        }
        else
        {
            transform.localScale = new Vector3(-currentSizeX, transform.localScale.y, transform.localScale.z);
        }
    }

    private IEnumerator ThinkRoutine()
    {
        while (currentState != Boss2State.Dead && player != null)
        {
            if (currentState == Boss2State.Idle)
            {
                yield return new WaitForSeconds(idleDelay);
                ChangeState(Boss2State.Walk);
            }
            else if(currentState == Boss2State.Walk)
            {
                float distance = Vector2.Distance(transform.position, player.position);

                if(distance <= meleeAttackRange)
                {
                    if (Random.Range(0, 100) > 70)
                        StartCoroutine(BasicAttackRoutine());
                    else StartCoroutine(ChargeAttackRoutine());
                }
                else if(distance <= chargeAttackRange)
                {
                    if (Random.Range(0, 100) < 60) StartCoroutine(ChargeAttackRoutine());
                    else StartCoroutine(RangedAttackRoutine());
                }
                else if (distance <= rangedAttackRange)
                {
                    StartCoroutine(RangedAttackRoutine());
                }
            }
            yield return null;
        }      
    }

    private IEnumerator BasicAttackRoutine()
    {
        ChangeState(Boss2State.BasicAttack);
        anim.Play("Idle");
        yield return new WaitForSeconds(basicAnticipation);

        anim.Play("Attack1");
        yield return new WaitForSeconds(basicHitDelay);

        ApplyMeleeDamage(basicDamage);

        yield return new WaitForSeconds(basicRecoil);
        ChangeState(Boss2State.Idle);
    }

    private IEnumerator RangedAttackRoutine()
    {
        ChangeState(Boss2State.RangedAttack);
        anim.Play("Idle");
        yield return new WaitForSeconds(rangedAnticipation);

        anim.Play("Attack1");

        if(projectilePrefab != null && firePoint != null)
        {
            GameObject projObj = EnemyProjectilePool.Instance.GetProjectile(projectilePrefab);
            if (projObj != null)
            {
                projObj.transform.position = firePoint.position;
                projObj.transform.rotation = Quaternion.identity;

                Vector2 shootDir = transform.localScale.x > 0 ? Vector2.right : Vector2.left;
                IProjectile projectile = projObj.GetComponent<IProjectile>();
                if (projectile != null) projectile.Setup(shootDir, basicDamage);
            }
        }
        yield return new WaitForSeconds(rangedRecoil);
        ChangeState(Boss2State.Idle);
    }

    private IEnumerator ChargeAttackRoutine()
    {
        ChangeState(Boss2State.ChargeAttack);
        anim.Play("Idle");

        float timer = 0;
        while (timer < chargeWarningTime)
        {
            sr.color = Color.red;
            yield return new WaitForSeconds(0.1f);
            sr.color = Color.white;
            yield return new WaitForSeconds(0.1f);
            timer += 0.2f;
        }

        anim.Play("Run");
        float chargeDir = transform.localScale.x > 0 ? 1f : -1f;

        float chargeTimer = 0f;
        bool hasHitDuringCharge = false;


        while (chargeTimer < chargeDuration)
        {
            chargeTimer += Time.deltaTime;
            rb.linearVelocity = new Vector2(chargeDir * chargeSpeed, rb.linearVelocity.y);
            if (!hasHitDuringCharge)
            {
                Vector2 hitCenter = (Vector2)transform.position + new Vector2(chargeDir * hitOffset, 0f);
                Collider2D hit = Physics2D.OverlapBox(hitCenter, attackHitBox, 0f, playerLayer);
                if (hit != null)
                {
                    IDamageable damageable = hit.GetComponent<IDamageable>();
                    if (damageable != null)
                    {
                        damageable.TakeDamage(chargeMoveDamage); 
                        hasHitDuringCharge = true;
                    }
                }
            }
            yield return null;
        }

        rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);

        anim.Play("Attack2");
        yield return new WaitForSeconds(chargeSlamDelay);

        ApplyMeleeDamage(chargeDamage);

        yield return new WaitForSeconds(chargeRecoil);
        ChangeState(Boss2State.Idle);
    }

    private void ApplyMeleeDamage(float damageAmount)
    {
        float attackDir = transform.localScale.x > 0 ? 1f : -1f;
        Vector2 hitCenter = (Vector2)transform.position + new Vector2(attackDir * hitOffset, 0f);

        Collider2D hit = Physics2D.OverlapBox(hitCenter, attackHitBox, 0f, playerLayer);
        if(hit != null)
        {
            IDamageable damageable = hit.GetComponent<IDamageable>();
            if (damageable != null) damageable.TakeDamage(damageAmount);
        }
    }

    public void TakeDamage(float damage)
    {
        if(currentState == Boss2State.Dead) return;

        currentHp -= damage;
        if (BossHealthBar.Instance != null) BossHealthBar.Instance.UpdateHP(currentHp, maxHp);
       
        if (sr != null) StartCoroutine(HitFlashRoutine());

        if (currentHp <= 0) Die();
    }

    private IEnumerator HitFlashRoutine()
    {
        sr.color = Color.red;
        yield return new WaitForSeconds(0.1f);
        if (currentState != Boss2State.ChargeAttack)
            sr.color = Color.white;
    }

    private void Die()
    {
        if (currentState == Boss2State.Dead) return;
        ChangeState(Boss2State.Dead);

        if (BossHealthBar.Instance != null) BossHealthBar.Instance.HideBossUI();

        sr.color = Color.white;
        StopAllCoroutines();

        anim.Play("Dead");

        rb.linearVelocity = Vector2.zero;
        rb.gravityScale = 0;

        Collider2D[] allColliders = GetComponentsInChildren<Collider2D>();
        foreach (Collider2D coll in allColliders) coll.enabled = false;

        if (RoomManager.Instance != null) RoomManager.Instance.OnEnemyKilled();

        GameObject activePlayer = GameObject.FindGameObjectWithTag("Player");
        if (activePlayer != null)
        {
            Player playerScript = activePlayer.GetComponent<Player>();
            if (playerScript != null) playerScript.AddGold(dropGold);
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
        // 1. 실제 데미지가 들어가는 물리 판정 박스 (가장 중요!)
        // 시선 방향(Scale X)에 맞춰서 위치를 계산합니다.
        float attackDir = (transform.localScale.x > 0) ? 1f : -1f;
        Vector3 hitCenter = transform.position + new Vector3(attackDir * hitOffset, 0f, 0f);

        // 빨간색 테두리와 반투명한 빨간색 면으로 공격 범위를 보여줍니다.
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(hitCenter, attackHitBox);
        Gizmos.color = new Color(1f, 0f, 0f, 0.2f); // 20% 투명도
        Gizmos.DrawCube(hitCenter, attackHitBox);


        // 2. 보스의 인공지능이 "공격할까?"라고 판단하는 인식 사거리 (원형)

        // 근접 공격 사거리 (노란색)
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, meleeAttackRange);

        // 돌진 공격 사거리 (파란색)
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, chargeAttackRange);

        // 원거리 공격 사거리 (하늘색)
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, rangedAttackRange);
    }


}
