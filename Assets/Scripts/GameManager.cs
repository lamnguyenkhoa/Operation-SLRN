using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    [Header("Config")]
    public int score = 0;
    public int oreScoreGain = 100;
    public bool startedGame = false;

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
    public TextMeshProUGUI coordText;
    public TextMeshProUGUI helpText;
    public GameObject gameOverScreen;
    public GameObject startScreen;
    public Image blackFade;
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
        TextMeshProUGUI[] textMeshPros = startScreen.GetComponentsInChildren<TextMeshProUGUI>();
        foreach (TextMeshProUGUI textMeshPro in textMeshPros)
        {
            StartCoroutine(FadeInAndOutTextMesh(textMeshPro, 0.5f, 2f));
        }
    }

    void Update()
    {
        iframeTimer += Time.deltaTime;
        if (!startedGame && Input.GetKeyDown(KeyCode.E))
        {
            StartGame();
        }
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

    public void StartGame()
    {
        scoreText.text = score.ToString("000000000");
        scoreText.gameObject.SetActive(true);
        sideScreenText.gameObject.SetActive(true);
        coordText.gameObject.SetActive(true);
        helpText.gameObject.SetActive(true);
        startScreen.SetActive(false);
        startedGame = true;
        submarine.gameObject.SetActive(true);
        StartCoroutine(FadeOutImage(blackFade, 2f));
        sonarBgs.Play();

        // Spawn stuff
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
}
