
using System.Collections;
using UnityEngine;

public class EnemyChaser : MonoBehaviour
{
    [Header("이동 설정")]
    public float moveSpeed = 2f;
    public float detectRange = 15f;

    [Header("공격 설정")]
    public int damage = 1;
    public float attackCooldown = 1f;
    public float windupTime = 0.8f;

    [Header("공격 범위 표시")]
    public AttackConeIndicator attackIndicator;

    [Tooltip("공격 표시가 없을 때 사용하는 사거리")]
    public float fallbackAttackRange = 1.5f;

    [Tooltip("공격 표시가 없을 때 사용하는 각도")]
    [Range(1f, 180f)]
    public float fallbackAttackAngle = 90f;

    private Transform player;
    private float lastAttackTime = -999f;
    private bool isAttacking;

    void Start()
    {
        GameObject playerObject = GameObject.FindWithTag("Player");

        if (playerObject != null)
        {
            player = playerObject.transform;
        }

        if (attackIndicator == null)
        {
            attackIndicator =
                GetComponentInChildren<AttackConeIndicator>(true);
        }

        if (attackIndicator != null)
        {
            attackIndicator.Hide();
        }
        else
        {
            Debug.LogWarning(
                gameObject.name +
                ": AttackConeIndicator를 찾을 수 없습니다."
            );
        }
    }

    void Update()
    {
        if (player == null || isAttacking)
        {
            return;
        }

        Vector3 direction = player.position - transform.position;
        direction.y = 0f;

        float distance = direction.magnitude;

        if (distance > detectRange)
        {
            return;
        }

        // 플레이어 방향을 바라본다.
        if (direction.sqrMagnitude > 0.001f)
        {
            transform.forward = direction.normalized;
        }

        // 부채꼴 범위 안에 들어왔을 때만 공격을 시작한다.
        if (IsPlayerInAttackCone())
        {
            if (Time.time >= lastAttackTime + attackCooldown)
            {
                lastAttackTime = Time.time;
                StartCoroutine(AttackRoutine());
            }

            return;
        }

        // 공격 범위 밖이면 플레이어를 추적한다.
        transform.position +=
            transform.forward * moveSpeed * Time.deltaTime;
    }

    private bool IsPlayerInAttackCone()
    {
        if (player == null)
        {
            return false;
        }

        Vector3 direction = player.position - transform.position;
        direction.y = 0f;

        float distance = direction.magnitude;

        float attackRange = attackIndicator != null
            ? attackIndicator.range
            : fallbackAttackRange;

        float attackAngle = attackIndicator != null
            ? attackIndicator.angle
            : fallbackAttackAngle;

        // 부채꼴의 사거리 밖이면 공격할 수 없다.
        if (distance > attackRange)
        {
            return false;
        }

        // 적과 플레이어가 거의 같은 위치라면 각도 검사를 생략한다.
        if (distance < 0.001f)
        {
            return true;
        }

        Vector3 forward = transform.forward;
        forward.y = 0f;
        forward.Normalize();

        direction.Normalize();

        // 적이 바라보는 방향과 플레이어 방향의 각도를 계산한다.
        float angleToPlayer =
            Vector3.Angle(forward, direction);

        // 부채꼴의 절반 각도 안에 있는지 확인한다.
        return angleToPlayer <= attackAngle * 0.5f;
    }

    private IEnumerator AttackRoutine()
    {
        isAttacking = true;

        // 공격 예고 부채꼴을 펼친다.
        if (attackIndicator != null)
        {
            attackIndicator.PlayWarning(windupTime);
        }

        // 공격 예고 시간 동안 기다린다.
        yield return new WaitForSeconds(windupTime);

        // 예고가 끝난 순간에도 부채꼴 안에 있어야 피해를 입는다.
        if (IsPlayerInAttackCone())
        {
            Health playerHealth = player.GetComponent<Health>();

            if (playerHealth != null)
            {
                playerHealth.TakeDamage(damage);
            }
        }

        // 공격 표시를 숨긴다.
        if (attackIndicator != null)
        {
            attackIndicator.Hide();
        }

        isAttacking = false;
    }
}
