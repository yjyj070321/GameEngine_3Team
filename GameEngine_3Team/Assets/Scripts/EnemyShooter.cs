
using System.Collections;
using UnityEngine;

public class EnemyShooter : MonoBehaviour
{
    [Header("탄환")]
    public GameObject bulletPrefab;
    public Transform firePoint;
    public float bulletSpeed = 10f;

    [Header("이동")]
    public float moveSpeed = 2f;
    public float keepDistance = 8f;
    public float fireRange = 12f;

    [Header("공격")]
    public float fireInterval = 2f;
    public float warningTime = 1.5f;

    [Header("조준선")]
    public LineRenderer warningLine;

    private Transform player;
    private float lastFireTime;
    private bool isAttacking;

    void Start()
    {
        GameObject p = GameObject.FindWithTag("Player");

        if (p != null)
            player = p.transform;

        lastFireTime = Time.time;

        if (warningLine != null)
        {
            warningLine.positionCount = 2;
            warningLine.useWorldSpace = true;
            warningLine.startColor = Color.red;
            warningLine.endColor = Color.red;
            warningLine.enabled = false;
        }
    }

    void Update()
    {
        if (player == null || isAttacking)
            return;

        Vector3 toPlayer = player.position - transform.position;
        toPlayer.y = 0f;

        float distance = toPlayer.magnitude;

        // 플레이어를 바라본다.
        if (distance > 0.01f)
            transform.forward = toPlayer.normalized;

        // 플레이어와 적 사이의 거리를 유지한다.
        if (distance > keepDistance)
        {
            transform.position +=
                transform.forward * moveSpeed * Time.deltaTime;
        }

        bool inRange = distance <= fireRange;
        bool ready = Time.time >= lastFireTime + fireInterval;

        if (inRange && ready)
            StartCoroutine(ShootWithWarning());
    }

    IEnumerator ShootWithWarning()
    {
        if (player == null || firePoint == null ||
            bulletPrefab == null)
            yield break;

        isAttacking = true;
        lastFireTime = Time.time;

        float elapsed = 0f;
        Vector3 direction = firePoint.forward;

        // Warning Time 동안 플레이어를 계속 조준한다.
        while (elapsed < warningTime)
        {
            if (player == null)
                break;

            Vector3 target = player.position + Vector3.up;
            Vector3 toTarget = target - firePoint.position;

            if (toTarget.sqrMagnitude > 0.001f)
            {
                direction = toTarget.normalized;

                // 적의 몸은 수평 방향으로 회전시킨다.
                Vector3 flatDirection = direction;
                flatDirection.y = 0f;

                if (flatDirection.sqrMagnitude > 0.001f)
                    transform.forward = flatDirection.normalized;

                // 조준선을 매 프레임 갱신한다.
                if (warningLine != null)
                {
                    warningLine.SetPosition(
                        0, firePoint.position
                    );

                    warningLine.SetPosition(
                        1, firePoint.position +
                           direction * fireRange
                    );

                    warningLine.enabled = true;
                }
            }

            elapsed += Time.deltaTime;
            yield return null;
        }

        // 조준 종료: 이제 방향을 더 이상 갱신하지 않는다.
        if (warningLine != null)
            warningLine.enabled = false;

        // 마지막으로 저장한 방향으로 발사한다.
        GameObject bullet = Instantiate(
            bulletPrefab,
            firePoint.position,
            Quaternion.LookRotation(direction)
        );

        Rigidbody rb = bullet.GetComponent<Rigidbody>();

        if (rb != null)
            rb.linearVelocity = direction * bulletSpeed;

        isAttacking = false;
    }

    void OnDisable()
    {
        if (warningLine != null)
            warningLine.enabled = false;

        StopAllCoroutines();
        isAttacking = false;
    }
}
