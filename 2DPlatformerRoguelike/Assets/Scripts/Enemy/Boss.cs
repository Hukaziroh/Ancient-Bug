using System.Collections;
using UnityEngine;

public class Boss : MonoBehaviour, IDamageable
{
    public enum BossState
    {
        Idle,
        Walk,
        RoarAttack,
        RollAttack,
        SpikeAttack,
        Dead
    }

    [Header("보스 상태")]
    public BossState currentState;
    public float maxHp = 1000f;
    public float currentHp;
    public float moveSpeed = 3f;

    [Header("공격 사거리 설정")]
    public float meleeAttackRange = 1f;
    public float rollAttackRange = 8f;

    [Header("데미지 설정")]
    public float spikeDamage = 20f;
    public float roarDamage = 15f;
    public float rollDamage = 30f;
    public bool isSpikeActive = false;
    public float reflectDamage = 10f;

    [Header("타격 판정 설정")]
    public LayerMask playerLayer;
    public float roarRadius = 6f;
    public float rollHitRadius = 3f;
    public Vector2 spikeHitBox = new Vector2(4f, 3f);
    public float spikeHitOffset = 2f;

    [Header("보상 설정")]
    public int dropGold = 1000;
    public GameObject portalPrefab;

    [Header("이펙트 설정")]
    public GameObject roarEffectPrefab;
    public Transform roarEffectPoint;

    [Header("사운드 설정")]
    public AudioClip deathSound;
    public AudioClip roarSound;
    public AudioClip rollSound;
    public AudioClip spikeSound;
    public AudioSource loopAudioSource;

    [Header("패턴 전환 딜레이")]
    public float idleToWalkDelay = 1.5f;

    [Header("구르기(Roll) 시간 설정")]
    public float rollAnticipation = 0.5f;
    public float rollDuration = 3f;
    public float rollRecoil = 0.6f;

    [Header("가시(Spike) 시간 설정")]
    public float spikeAnticipation = 0.2f;
    public float spikeHitDelay = 1f;
    public float spikePostHitDelay = 0.15f;
    public float spikeRecoil = 0.2f;

    [Header("포효(Roar) 시간 설정")]
    public float roarAnticipation = 0.5f;
    public float roarHitDelay = 0.2f;
    public float roarPostHitDelay = 0.8f;
    public float roarRecoil = 0.5f;

    private Animator anim;
    private Rigidbody2D rb;
    private Transform player;

    private SpriteRenderer sr;

    private void Awake()
    {
        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();
    }

    private void Start()
    {
        if (Player.Instance != null) player = Player.Instance.transform;

        currentHp = maxHp;

        if (BossHealthBar.Instance != null)
        {
            BossHealthBar.Instance.ShowBossUI();
            BossHealthBar.Instance.UpdateHP(currentHp, maxHp);
        }

        ChangeState(BossState.Idle);
        StartCoroutine(ThinkRoutine());
    }

    private void Update()
    {
        if (currentState == BossState.Dead || player == null) return;

        if (currentState == BossState.Walk || currentState == BossState.Idle)
        {
            LookAtPlayer();
        }
    }

    private void FixedUpdate()
    {
        if (currentState == BossState.Dead || player == null) return;

        if (currentState == BossState.Walk)
        {
            Vector2 direction = (player.position - transform.position).normalized;
            rb.linearVelocity = new Vector2(direction.x * moveSpeed, rb.linearVelocity.y);
        }
        else if (currentState != BossState.RollAttack)
        {
            rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
        }
    }

    public void ChangeState(BossState newState)
    {
        if (currentState == BossState.Dead) return;
        currentState = newState;

        if (currentState == BossState.Walk) anim.Play("Walk");
        else if (currentState == BossState.Idle) anim.Play("Idle");
    }

    private void LookAtPlayer()
    {
        float currentSizeX = Mathf.Abs(transform.localScale.x);
        float currentSizeY = transform.localScale.y;
        float currentSizeZ = transform.localScale.z;

        if (player.position.x > transform.position.x)
        {
            transform.localScale = new Vector3(-currentSizeX, currentSizeY, currentSizeZ);
        }
        else
        {
            transform.localScale = new Vector3(currentSizeX, currentSizeY, currentSizeZ);
        }
    }

    private IEnumerator ThinkRoutine()
    {
        while (currentState != BossState.Dead && player != null)
        {
            if (currentState == BossState.Idle)
            {
                yield return new WaitForSeconds(idleToWalkDelay);
                ChangeState(BossState.Walk);
            }
            else if (currentState == BossState.Walk)
            {
                float distance = Vector2.Distance(transform.position, player.position);

                if (distance <= meleeAttackRange)
                {
                    int rand = Random.Range(0, 100);
                    if (rand < 20)
                    {
                        StartCoroutine(RollAttackRoutine());
                    }
                    else if (rand < 60)
                    {
                        StartCoroutine(SpikeAttackRoutine());
                    }
                    else
                    {
                        StartCoroutine(RoarAttackRoutine());
                    }
                }
                else if (distance >= rollAttackRange)
                {
                    int rand = Random.Range(0, 100);
                    if (rand < 2)
                    {
                        StartCoroutine(RollAttackRoutine());
                    }
                }
            }
            yield return null;
        }
    }

    private IEnumerator RollAttackRoutine()
    {
        ChangeState(BossState.RollAttack);
        anim.Play("RollAttackAnticipation");
        yield return new WaitForSeconds(rollAnticipation);

        anim.Play("RollAttack");

        if (rollSound != null && loopAudioSource != null)
        {
            loopAudioSource.clip = rollSound;
            loopAudioSource.loop = true;
            loopAudioSource.Play();
        }

        float rollDirection = transform.localScale.x < 0 ? 1f : -1f;
        float rollDurationTimer = 3f;
        float timer = 0f;
        bool hasHitPlayer = false;

        while (timer < rollDurationTimer)
        {
            timer += Time.deltaTime;
            rb.linearVelocity = new Vector2(rollDirection * moveSpeed * 3f, rb.linearVelocity.y);

            if (!hasHitPlayer)
            {
                Collider2D hit = Physics2D.OverlapCircle(transform.position, rollHitRadius, playerLayer);
                if (hit != null)
                {
                    IDamageable damageable = hit.GetComponent<IDamageable>();
                    if (damageable != null)
                    {
                        damageable.TakeDamage(rollDamage);
                        hasHitPlayer = true;
                    }
                }
            }
            yield return null;
        }

        if (loopAudioSource != null) loopAudioSource.Stop();

        anim.Play("RollAttackRecoil");
        rb.linearVelocity = Vector2.zero;
        yield return new WaitForSeconds(rollRecoil);

        ChangeState(BossState.Idle);
    }

    private IEnumerator SpikeAttackRoutine()
    {
        ChangeState(BossState.SpikeAttack);
        anim.Play("SpikeAttackAnticipation");
        yield return new WaitForSeconds(spikeAnticipation);

        anim.Play("SpikeAttack");

        if (spikeSound != null && loopAudioSource != null)
        {
            loopAudioSource.clip = spikeSound;
            loopAudioSource.loop = true;
            loopAudioSource.Play();
        }

        isSpikeActive = true;

        yield return new WaitForSeconds(spikeHitDelay);

        float attackDir = transform.localScale.x < 0 ? 1f : -1f;
        Vector2 hitCenter = (Vector2)transform.position + new Vector2(attackDir * spikeHitOffset, 0f);

        Collider2D hit = Physics2D.OverlapBox(hitCenter, spikeHitBox, 0f, playerLayer);
        if (hit != null)
        {
            IDamageable damageable = hit.GetComponent<IDamageable>();
            if (damageable != null) damageable.TakeDamage(spikeDamage);
        }

        yield return new WaitForSeconds(spikePostHitDelay);

        isSpikeActive = false;

        if (loopAudioSource != null) loopAudioSource.Stop();

        anim.Play("SpikeAttackRecoil");
        yield return new WaitForSeconds(spikeRecoil);

        ChangeState(BossState.Idle);
    }

    private IEnumerator RoarAttackRoutine()
    {
        ChangeState(BossState.RoarAttack);
        anim.Play("RoarAnticipation");
        yield return new WaitForSeconds(roarAnticipation);

        anim.Play("Roar");

        if (roarSound != null && SoundManager.Instance != null)
        {
            SoundManager.Instance.PlaySFX(roarSound);

            if (roarEffectPrefab != null)
            {
                Vector3 spawnPos = (roarEffectPoint != null) ? roarEffectPoint.position : transform.position;

                GameObject effect = Instantiate(roarEffectPrefab, spawnPos, Quaternion.identity);

                float currentFacingDir = Mathf.Sign(transform.localScale.x);
                Vector3 effectScale = effect.transform.localScale;
                effect.transform.localScale = new Vector3(effectScale.x * currentFacingDir, effectScale.y, effectScale.z);
                Destroy(effect, 2f);
            }

            yield return new WaitForSeconds(roarHitDelay);

            Collider2D hit = Physics2D.OverlapCircle(transform.position, roarRadius, playerLayer);
            if (hit != null)
            {
                IDamageable damageable = hit.GetComponent<IDamageable>();
                if (damageable != null) damageable.TakeDamage(roarDamage);
            }

            yield return new WaitForSeconds(roarPostHitDelay);

            anim.Play("RoarRecoil");
            yield return new WaitForSeconds(roarRecoil);

            ChangeState(BossState.Idle);
        }
    }

    public void TakeDamage(float damage)
    {
        if (currentState == BossState.Dead) return;

        if (isSpikeActive)
        {
            if (Player.Instance != null)
            {
                Player.Instance.TakeDamage(reflectDamage);
            }
            return;
        }

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
        if (currentState == BossState.Dead) return;
        ChangeState(BossState.Dead);

        if (loopAudioSource != null) loopAudioSource.Stop();

        if (deathSound != null && SoundManager.Instance != null)
        {
            SoundManager.Instance.PlaySFX(deathSound);
        }

        if (BossHealthBar.Instance != null) BossHealthBar.Instance.HideBossUI();

        if (sr != null)
        {
            sr.color = Color.white;
        }

        StopAllCoroutines();

        anim.Play("Dead");

        rb.linearVelocity = Vector2.zero;
        rb.gravityScale = 0;

        Collider2D[] allColliders = GetComponentsInChildren<Collider2D>();
        foreach (Collider2D coll in allColliders)
        {
            coll.enabled = false;
        }

        if (RoomManager.Instance != null)
        {
            RoomManager.Instance.OnEnemyKilled();
        }

        if (Player.Instance != null)
        {
            Player.Instance.AddGold(dropGold);
            Player.Instance.Heal(Player.Instance.maxHP * 0.2f);
            StartCoroutine(DeathRoutine());
        }
    }

    private IEnumerator DeathRoutine()
    {
        yield return new WaitForSeconds(3f);
        if (portalPrefab != null)
        {
            Instantiate(portalPrefab, transform.position, Quaternion.identity);
        }
        Destroy(gameObject);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, roarRadius);

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, rollHitRadius);

        Gizmos.color = Color.magenta;
        float attackDir = transform.localScale.x < 0 ? 1f : -1f;

        Vector3 hitCenter = transform.position + new Vector3(attackDir * spikeHitOffset, 0f, 0f);
        Gizmos.DrawWireCube(hitCenter, spikeHitBox);
    }
}