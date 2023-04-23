using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    [Header("Config")]
    public int score = 0;
    public int oreScoreGain = 100;

    [Header("Entity")]
    public int startOreNumber = 10;
    public int startSmallEnemyNumber = 10;
    public GameObject orePrefab;
    public GameObject smallEnemyPrefab;
    public Transform entityHolder;

    [Header("Submarine")]
    public int currentSubmarineHp = 3;
    public int maxSubmarineHp = 3;
    public Transform submarine;
    private float iframeTimer;
    public float iframeDuration = 1f;
    public int speedLevel = 1;
    public int sonarSpeedLevel = 1;
    public int sonarRangeLevel = 1;

    [Header("SoundEffect")]
    public AudioSource sfxAudioSource;
    public AudioClip hullDamaged;
    public AudioClip oreCollected;

    [Header("Component")]
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI sideScreenText;
    public GameObject gameOverScreen;
    public AudioSource bgm;
    public AudioSource sonarBgs;


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
        scoreText.text = score.ToString("000000000");
        for (int i = 0; i < startOreNumber; i++)
        {
            SpawnNewEntity(orePrefab);
        }
        for (int i = 0; i < startSmallEnemyNumber; i++)
        {
            SpawnNewEntity(smallEnemyPrefab);
        }
        RefreshSideScreen();
    }

    void Update()
    {
        iframeTimer += Time.deltaTime;
    }

    public void CollectOre()
    {
        score += oreScoreGain;
        if (score < 999999999)
        {
            scoreText.text = score.ToString("000000000");
        }
        else
        {
            scoreText.text = "err";
        }
        SpawnNewEntity(orePrefab);
    }

    public void SpawnNewEntity(GameObject prefab)
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

        GameObject go = Instantiate(prefab, entityHolder);
        go.transform.position = new Vector3(randomX, randomY, 0);
    }

    public void SubmarineDamaged()
    {
        if (iframeTimer < iframeDuration)
        {
            return;
        }

        sfxAudioSource.PlayOneShot(hullDamaged);

        currentSubmarineHp -= 1;
        RefreshSideScreen();
        CinemachineShake.instance.ShakeCamera(5f, 0.5f, false);
        if (currentSubmarineHp <= 0)
        {
            GameOver();
        }
        else
        {
            iframeTimer = 0;
        }
    }

    public void GameOver()
    {
        gameOverScreen.SetActive(true);
        submarine.gameObject.SetActive(false);
        sonarBgs.Stop();
    }

    public void RefreshSideScreen()
    {
        string hullText = "".PadRight(currentSubmarineHp, '|');
        string engineText = "".PadRight(speedLevel, '|');
        string sonarSpeedText = "".PadRight(sonarSpeedLevel, '|');
        string sonarRangeText = "".PadRight(sonarRangeLevel, '|');

        sideScreenText.text = $"hull\n{hullText}\nengine\n{engineText}\nsonar-speed\n{sonarSpeedText}\nsonar-range\n{sonarRangeText}";
    }
}
