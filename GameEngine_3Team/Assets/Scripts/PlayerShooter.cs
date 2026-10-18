using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerShooter : MonoBehaviour
{
    public GameObject bulletPrefab;
    public Transform firePoint;
    public float bulletSpeed = 20f;
    public float fireCooldown = 0.2f;
    public float shootDuration = 0.3f;

    float lastFireTime = -999f;

    private float shootingTimer = 0f;
    public PlayerController playerController;

    void Update()
    {
        if (shootingTimer > 0f)
        {
            shootingTimer -= Time.deltaTime;

            if (shootingTimer <= 0f && playerController != null && playerController.Animator != null)
            {
                playerController.Animator.SetFloat("AttackWeight", 0f);
            }
        }
    }

    public void OnAttack(InputValue value)
    {
        if (Time.timeScale == 0f) return;
        if (Time.time < lastFireTime + fireCooldown) return;
        lastFireTime = Time.time;

        shootingTimer = shootDuration;
        if (playerController != null && playerController.Animator != null)
        {
            playerController.Animator.SetFloat("AttackWeight", 1f);
        }

        GameObject bullet = Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
        Rigidbody rb = bullet.GetComponent<Rigidbody>();
        rb.linearVelocity = firePoint.forward * bulletSpeed;
    }
}