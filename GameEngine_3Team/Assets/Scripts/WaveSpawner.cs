using UnityEngine;
using TMPro;

public class WaveSpawner : MonoBehaviour
{
    public GameObject[] enemyPrefabs;
    public Transform[] spawnPoints;
    public int firstCount = 3;
    public int addPerWave = 2;
    public float spawnGap = 0.5f;
    public TMP_Text waveText;

    int wave = 0;
    int toSpawn = 0;
    float timer;

    void SpawnOne()
    {
        int e = Random.Range(0, enemyPrefabs.Length);
        int s = Random.Range(0, spawnPoints.Length);
        Vector3 pos = spawnPoints[s].position;
        Instantiate(enemyPrefabs[e], pos,
            Quaternion.identity);
    }

    void Update()
    {
        if (toSpawn > 0)
        {
            timer += Time.deltaTime;
            if (timer < spawnGap) return;
            timer = 0f;
            SpawnOne();
            toSpawn--;
            return;
        }

        int alive = GameObject.FindGameObjectsWithTag("Enemy").Length;
        if (alive > 0) return;

        wave++;
        toSpawn = firstCount + addPerWave * (wave - 1);
        if (waveText != null) waveText.text = "WAVE " + wave;
    }
}