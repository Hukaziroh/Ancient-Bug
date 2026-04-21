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
        Tired,
        Dead
    }

    [Header("보스 상태")]
    public BossState currentState;
    public float currentHp = 1000f;
    public float moveSpeed = 3f;

    [Header("공격 사거리 설정")]
    public float meleeAttackRange = 1f;
    public float rollAttackRange = 8f;

    [Header("데미지 설정")]
    public float spikeDamage = 20f;
    public float roarDamage = 15f;
    public float rollDamage = 30f;

    [Header("타격 판정 설정")]
    public LayerMask playerLayer;
    public float roarRadius = 6f;
    public float rollHitRadius = 3f;
    public Vector2 spikeHitBox = new Vector2(4f, 3f);
    public float spikeHitOffset = 2f;

    [Header("보상 설정")]
    public int dropGold = 1000;
    public GameObject portalPrefab;

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
                yield return new WaitForSeconds(1.5f);
                ChangeState(BossState.Walk);
            }
            else if (currentState == BossState.Walk)
            {
                float distance = Vector2.Distance(transform.position, player.position);

                if (distance <= meleeAttackRange)
                {
                    int rand = Random.Range(0, 2);
                    if (rand == 0) StartCoroutine(SpikeAttackRoutine());
                    else StartCoroutine(RoarAttackRoutine());
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
        yield return new WaitForSeconds(0.5f);

        anim.Play("RollAttack");

        float rollDirection = transform.localScale.x < 0 ? 1f : -1f;
        float rollDuration = 3f;
        float timer = 0f;
        bool hasHitPlayer = false;

        while (timer < rollDuration)
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
        anim.Play("RollAttackRecoil");
        rb.linearVelocity = Vector2.zero;
        yield return new WaitForSeconds(0.6f);

        ChangeState(BossState.Idle);
    }

    private IEnumerator SpikeAttackRoutine()
    {
        ChangeState(BossState.SpikeAttack);
        anim.Play("SpikeAttackAnticipation");
        yield return new WaitForSeconds(0.2f);

        anim.Play("SpikeAttack");

        yield return new WaitForSeconds(1f);

        float attackDir = transform.localScale.x < 0 ? 1f : -1f;
        Vector2 hitCenter = (Vector2)transform.position + new Vector2(attackDir * spikeHitOffset, 0f);

        Collider2D hit = Physics2D.OverlapBox(hitCenter, spikeHitBox, 0f, playerLayer);
        if (hit != null)
        {
            IDamageable damageable = hit.GetComponent<IDamageable>();
            if (damageable != null) damageable.TakeDamage(spikeDamage);
        }

        yield return new WaitForSeconds(0.15f);

        anim.Play("SpikeAttackRecoil");
        yield return new WaitForSeconds(0.2f);

        ChangeState(BossState.Idle);
    }

    private IEnumerator RoarAttackRoutine()
    {
        ChangeState(BossState.RoarAttack);
        anim.Play("RoarAnticipation");
        yield return new WaitForSeconds(0.5f);

        anim.Play("Roar");

        yield return new WaitForSeconds(0.2f);

        Collider2D hit = Physics2D.OverlapCircle(transform.position, roarRadius, playerLayer);
        if (hit != null)
        {
            IDamageable damageable = hit.GetComponent<IDamageable>();
            if (damageable != null) damageable.TakeDamage(roarDamage);
        }

        yield return new WaitForSeconds(0.8f);

        anim.Play("RoarRecoil");
        yield return new WaitForSeconds(0.5f);

        ChangeState(BossState.Idle);
    }

    public void TakeDamage(float damage)
    {
        if (currentState == BossState.Dead) return;

        currentHp -= damage;

        if (sr != null) StartCoroutine(HitFlashRoutine());

        if (currentHp <= 0) Die();
        else if (currentHp == 300f) StartCoroutine(TiredRoutine());
    }

    private IEnumerator HitFlashRoutine()
    {
        sr.color = Color.red;
        yield return new WaitForSeconds(0.1f);
        sr.color = Color.white;
    }

    private IEnumerator TiredRoutine()
    {
        ChangeState(BossState.Tired);
        anim.Play("Tired");
        yield return new WaitForSeconds(5f);
        ChangeState(BossState.Idle);
    }

    private void Die()
    {
        if (currentState == BossState.Dead) return;
        ChangeState(BossState.Dead);

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

        GameObject activePlayer = GameObject.FindGameObjectWithTag("Player");
        if (activePlayer != null)
        {
            Player playerScript = activePlayer.GetComponent<Player>();
            if (playerScript != null)
            {
                playerScript.AddGold(dropGold);
            }
        }

        if (portalPrefab != null)
        {
            Instantiate(portalPrefab, transform.position, Quaternion.identity);
        }

        Destroy(gameObject, 3f);
    }

    private IEnumerator GoToLobbyRoutine()
    {
        yield return new WaitForSeconds(3f);
        UnityEngine.SceneManagement.SceneManager.LoadScene("Lobby");
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