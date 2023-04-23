using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    public int score = 0;
    public int oreScoreGain = 100;
    public int startOreAmount = 10;
    public GameObject orePrefab;
    public Transform entityHolder;


    public TextMeshProUGUI scoreText;


    void Awake()
    {
        if (!instance)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        scoreText.text = score.ToString();
        for (int i = 0; i < startOreAmount; i++)
        {
            SpawnNewOre();
        }
    }

    public void CollectOre()
    {
        score += oreScoreGain;
        scoreText.text = score.ToString();
        SpawnNewOre();
    }

    public void SpawnNewOre()
    {
        float randomX = 0;
        float randomY = 0;
        bool goodSpawnLocation = false;
        int retryCount = 0;

        while (!goodSpawnLocation)
        {
            randomX = Random.Range(-114, -75);
            randomY = Random.Range(21, -21);

            Collider2D hitColliders = Physics2D.OverlapCircle(new Vector2(randomX, randomY), 0.5f);
            if (!hitColliders)
            {
                goodSpawnLocation = true;
            }
            retryCount += 1;
            if (retryCount > 10)
            {
                Debug.Log("Retry too many time");
            }
        }

        GameObject go = Instantiate(orePrefab, entityHolder);
        go.transform.position = new Vector3(randomX, randomY, 0);
    }
}
