
using UnityEngine;
using UnityEngine.UI;

public class Health : MonoBehaviour
{
    public int maxHp = 10;
    public int currentHp;

    public Image fillImage;

    public static int killCount;

    public float invincibleTime = 0.5f;
    private float lastHitTime = -999f;

    public GameObject dropPrefab;
    public float dropChance = 0.3f;

    public GameObject hitEffect;

    void Start()
    {
        maxHp = Mathf.Max(1, maxHp);
        currentHp = maxHp;
        UpdateBar();
    }

    public void TakeDamage(int damage)
    {
        if (currentHp <= 0) return;
        if (Time.time < lastHitTime + invincibleTime) return;
        if (damage <= 0) return;

        lastHitTime = Time.time;
        currentHp = Mathf.Max(0, currentHp - damage);

        Debug.Log(name + " HP: " + currentHp + "/" + maxHp);

        UpdateBar();

        Vector3 hitPos = transform.position + Vector3.up;

        if (hitEffect != null)
        {
            Instantiate(hitEffect, hitPos, Quaternion.identity);
        }

        if (currentHp <= 0)
        {
            Die();
        }
        else
        {
            Blink();
        }
    }

    void UpdateBar()
    {
        if (fillImage == null) return;

        float hpRatio = (float)currentHp / maxHp;
        fillImage.fillAmount = Mathf.Clamp01(hpRatio);

        // 체력이 0이면 빨간 게이지를 완전히 숨김
        fillImage.enabled = currentHp > 0;
    }

    void Die()
    {
        if (CompareTag("Player"))
        {
            GameManager.Instance.GameOver();
        }
        else
        {
            if (dropPrefab != null && Random.value < dropChance)
            {
                Vector3 pos = transform.position + Vector3.up * 0.5f;
                Instantiate(dropPrefab, pos, Quaternion.identity);
            }

            killCount++;
            Destroy(gameObject);
        }
    }

    void Blink()
    {
        SetVisible(false);
        Invoke(nameof(ShowAgain), 0.1f);
    }

    void ShowAgain()
    {
        SetVisible(true);
    }

    void SetVisible(bool on)
    {
        foreach (Renderer r in GetComponentsInChildren<Renderer>())
        {
            r.enabled = on;
        }
    }

    public void Heal(int amount)
    {
        if (currentHp <= 0) return;

        currentHp = Mathf.Min(currentHp + amount, maxHp);
        UpdateBar();
    }
}
