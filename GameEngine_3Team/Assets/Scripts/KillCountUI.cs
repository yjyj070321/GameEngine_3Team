using UnityEngine;
using TMPro;

public class KillCountUI : MonoBehaviour
{
    public TMP_Text label;

    void Start()
    {
        Health.killCount = 0;   // Play 마다 0 부터
    }

    void Update()
    {
        label.text = "KILL " + Health.killCount;
    }
}