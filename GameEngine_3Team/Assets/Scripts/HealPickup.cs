using UnityEngine;

public class HealPickup : MonoBehaviour
{
    public int amount = 3;

    void Update()
    {
        transform.Rotate(0, 90 * Time.deltaTime, 0);
    }
    void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;
        other.GetComponent<Health>().Heal(amount);
        Destroy(gameObject);
    }
}