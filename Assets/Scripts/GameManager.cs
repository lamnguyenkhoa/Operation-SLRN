using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.Rendering.Universal;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    [Header("Config")]
    public int score = 0;
    public int oreScoreGain = 100;
    public bool startedGame = false;
    public bool endedGame = false;


    [Header("Entity")]
    public int startOreNumber = 20;
    public int startSmallEnemyNumber = 8;
    public int startProximityMine = 3;
    public int startEnemySubmarine = 1;

    public GameObject orePrefab;
    public GameObject smallEnemyPrefab;
    public GameObject proximityMinePrefab;
    public GameObject enemySubmarinePrefab;

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
    public float oxygenTimeLeft;
    public float oxygenMaxTime = 600f; // 10 minute

    [Header("SoundEffect")]
    public AudioSource sfxAudioSource;
    public AudioClip hullDamaged;
    public AudioClip oreCollected;

    [Header("Component")]
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI sideScreenText;
    public TextMeshProUGUI coordText;
    public TextMeshProUGUI helpText;
    public TextMeshProUGUI oxygenText;
    public GameObject gameOverScreen;
    public GameObject startScreen;
    public Image blackFade;
    public AudioSource bgm;
    public AudioSource sonarBgs;
    public GameObject hullLight;
    public Gradient oxygenLightGradient;
    public Light2D oxygenLight;


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
        TextMeshProUGUI[] textMeshPros = startScreen.GetComponentsInChildren<TextMeshProUGUI>();
        foreach (TextMeshProUGUI textMeshPro in textMeshPros)
        {
            StartCoroutine(FadeInAndOutTextMesh(textMeshPro, 0.5f, 2f));
        }
        oxygenTimeLeft = oxygenMaxTime;
    }

    void Update()
    {
        if (!startedGame && Input.GetKeyDown(KeyCode.E))
        {
            StartGame();
        }
        if (startedGame && !endedGame)
        {
            iframeTimer += Time.deltaTime;
            // Oxygen stuff
            oxygenTimeLeft -= Time.deltaTime;
            float t = 1 - (oxygenTimeLeft / oxygenMaxTime);
            oxygenLight.color = oxygenLightGradient.Evaluate(t);
            oxygenText.text = oxygenTimeLeft.ToString("F0");
            if (oxygenTimeLeft <= 0f)
            {
                GameOver(true);
            }
        }
    }

    public void RefreshScoreText()
    {
        if (score < 999999999)
        {
            scoreText.text = score.ToString("000000000");
        }
        else
        {
            scoreText.text = "err";
        }
    }

    public void CollectOre()
    {
        score += oreScoreGain;
        sfxAudioSource.PlayOneShot(oreCollected, 0.3f);
        RefreshScoreText();
        SpawnNewEntity(orePrefab);


        // 70% change to spawn a new enemy
        float spawnChance = Random.Range(0f, 1f);
        if (spawnChance < 0.7f)
        {
            SpawnRandomEnemy();
        }
    }

    public void SpawnRandomEnemy()
    {
        if (score >= 1000)
        {
            // 5% for enemy submarine
            // 25% for mine
            // 70% for small monster
            float enemyChance = Random.Range(0f, 1f);
            if (enemyChance < 0.05f)
                SpawnNewEntity(enemySubmarinePrefab);
            else if (enemyChance < 0.3f)
                SpawnNewEntity(proximityMinePrefab);
            else
                SpawnNewEntity(smallEnemyPrefab);
        }
        else
        {
            // 30% for mine
            // 70% for small monster
            float enemyChance = Random.Range(0f, 1f);
            if (enemyChance < 0.3f)
                SpawnNewEntity(proximityMinePrefab);
            else
                SpawnNewEntity(smallEnemyPrefab);
        }

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
                // Spawn not too close to submarine
                if (Vector2.Distance(new Vector2(randomX, randomY), submarine.transform.position) > 5f)
                {
                    goodSpawnLocation = true;
                }
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

    public void SubmarineDamaged(int damageAmount)
    {
        if (iframeTimer < iframeDuration)
        {
            return;
        }

        sfxAudioSource.PlayOneShot(hullDamaged);

        currentSubmarineHp -= damageAmount;
        currentSubmarineHp = Mathf.Clamp(currentSubmarineHp, 0, maxSubmarineHp);
        RefreshSideScreen();
        if (damageAmount >= 100)
        {
            CinemachineShake.instance.ShakeCamera(10f, 1f, false);
        }
        else
        {
            CinemachineShake.instance.ShakeCamera(5f, 0.5f, false);
        }
        if (currentSubmarineHp <= 0)
        {
            GameOver(false);
        }
        else
        {
            iframeTimer = 0;
        }
    }

    public void GameOver(bool hideCrack)
    {
        endedGame = true;
        gameOverScreen.SetActive(true);
        // If gameover not because of hull damaged, dont show crack
        if (hideCrack)
            gameOverScreen.GetComponent<GameOverMenu>().HideCracks();
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

    public void StartGame()
    {
        RefreshScoreText();
        scoreText.gameObject.SetActive(true);
        sideScreenText.gameObject.SetActive(true);
        coordText.gameObject.SetActive(true);
        helpText.gameObject.SetActive(true);
        oxygenText.gameObject.SetActive(true);
        startScreen.SetActive(false);
        startedGame = true;
        submarine.gameObject.SetActive(true);
        StartCoroutine(FadeOutImage(blackFade, 2f));
        sonarBgs.Play();

        // Spawn stuff
        for (int i = 0; i < startOreNumber; i++)
            SpawnNewEntity(orePrefab);
        for (int i = 0; i < startSmallEnemyNumber; i++)
            SpawnNewEntity(smallEnemyPrefab);
        for (int i = 0; i < startProximityMine; i++)
            SpawnNewEntity(proximityMinePrefab);
        for (int i = 0; i < startEnemySubmarine; i++)
            SpawnNewEntity(enemySubmarinePrefab);

        RefreshSideScreen();
    }

    public IEnumerator FadeOutImage(Image image, float fadeDuration)
    {
        // Get the initial alpha of the image
        float startingAlpha = image.color.a;

        // Keep track of the elapsed time
        float elapsedTime = 0.0f;

        while (elapsedTime < fadeDuration)
        {
            // Calculate the current alpha value
            float currentAlpha = Mathf.Lerp(startingAlpha, 0.0f, elapsedTime / fadeDuration);

            // Set the alpha value of the image
            Color newColor = image.color;
            newColor.a = currentAlpha;
            image.color = newColor;

            // Wait for a frame
            yield return null;

            // Update the elapsed time
            elapsedTime += Time.deltaTime;
        }

        // Disable the GameObject when the fade out is complete
        image.gameObject.SetActive(false);
    }

    public IEnumerator FadeInAndOutTextMesh(TextMeshProUGUI textMeshPro, float delayDuration, float fadeDuration)
    {
        while (true)
        {
            // Fade the text in
            yield return StartCoroutine(FadeAlphaTextMesh(textMeshPro, 0.1f, 1.0f, fadeDuration));

            // Wait for the delay duration
            yield return new WaitForSeconds(delayDuration);

            // Fade the text out
            yield return StartCoroutine(FadeAlphaTextMesh(textMeshPro, 1.0f, 0.1f, fadeDuration));

            // Wait for the delay duration
            yield return new WaitForSeconds(delayDuration);
        }
    }

    public IEnumerator FadeAlphaTextMesh(TextMeshProUGUI textMeshPro, float startAlpha, float endAlpha, float duration)
    {
        // Get the initial alpha of the text
        float startingAlpha = textMeshPro.alpha;

        // Keep track of the elapsed time
        float elapsedTime = 0.0f;

        while (elapsedTime < duration)
        {
            // Calculate the current alpha value
            float currentAlpha = Mathf.Lerp(startAlpha, endAlpha, elapsedTime / duration);

            // Set the alpha value of the text
            textMeshPro.alpha = currentAlpha;

            // Wait for a frame
            yield return null;

            // Update the elapsed time
            elapsedTime += Time.deltaTime;
        }

        // Set the final alpha value of the text
        textMeshPro.alpha = endAlpha;
    }

    public void OnToggleLightButton()
    {
        hullLight.SetActive(!hullLight.activeSelf);
    }

    public void OnToggleManualBook()
    {
        Debug.Log("Open manual book...");
    }
}
