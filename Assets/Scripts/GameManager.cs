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
    public int accumulatedScore = 0;
    public int oreScoreGain = 100;
    public bool startedGame = false;
    public bool endedGame = false;
    public bool isInLeaderboard = false;
    public bool isInUpgradeMenu = false;
    private string guestName;

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
    public int sonarLevel = 1;
    public int excavatorLevel = 1;
    public float oxygenTimeLeft;
    public float oxygenMaxTime = 600f; // 10 minute
    public bool disableSubmarineControl = false;
    public int upgradeCount;
    public bool isUpgrading;

    [Header("Audio")]
    public AudioSource sfxAudioSource;
    public AudioSource bgmAudioSource;
    public AudioClip hullDamaged;
    public AudioClip oreCollected;
    public AudioClip sonarPing;
    public AudioClip bookFlipping;
    public AudioClip upgradeSfx;
    public float bgmVolume;
    public float sfxVolume;


    [Header("Component")]
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI sideScreenText;
    public TextMeshProUGUI coordText;
    public TextMeshProUGUI helpText;
    public TextMeshProUGUI oxygenText;
    public GameObject gameOverScreen;
    public GameObject startScreen;
    public Light2D hullLight;
    public Gradient oxygenLightGradient;
    public Light2D oxygenLight;
    public GameObject[] objectToEnableAfterStart;
    public GameObject bookPageHolder;
    public LootLockerManager lootLockerManager;
    public TextMeshProUGUI[] textMeshProsToFade; // Same as TextMeshProUGUI. I want to try.
    public TMP_InputField nameInputField;
    public TextMeshProUGUI operatorNameDisplay;
    public Image mainScreenFade;


    void Awake()
    {
        if (!instance)
        {
            instance = this;

            // For testing fps
            if (Application.isEditor)
            {
                QualitySettings.vSyncCount = 0;  // VSync must be disabled
                Application.targetFrameRate = 120;
            }
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        foreach (TextMeshProUGUI textMeshPro in textMeshProsToFade)
        {
            StartCoroutine(FadeInAndOutTextMesh(textMeshPro, 0.2f, 2f));
        }
        oxygenTimeLeft = oxygenMaxTime;
        if (PlayerPrefs.GetString("PlayerName", "") != "")
        {
            nameInputField.text = PlayerPrefs.GetString("PlayerName");
        }

    }

    void Update()
    {
        if (!startedGame && !endedGame)
        {
            // Prevent start game while typing name
            if (!nameInputField.isFocused)
            {
                if (Input.GetKeyDown(KeyCode.E) && !isInLeaderboard)
                {
                    StartGame();
                }
                CheckPressQForLeaderboard();
            }
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

            if (Input.GetKeyDown(KeyCode.Tab))
            {
                OnToggleJournalBook();
            }
        }

        if (startedGame && endedGame)
        {
            CheckPressQForLeaderboard();
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
        // For every excavatorLevel beyond 1, +25% value;
        float bonus = 1 + (float)((GameManager.instance.excavatorLevel - 1) * 0.25);

        score += (int)Mathf.Round(oreScoreGain * bonus);
        accumulatedScore += oreScoreGain;

        sfxAudioSource.PlayOneShot(oreCollected, 0.3f);
        RefreshScoreText();
        SpawnNewEntity(orePrefab);

        // gain 1000 for 1st upgrade, then gain another 2000 for 2nd upgrade, then 3000...
        if (accumulatedScore >= (upgradeCount + 1) * 1000)
        {
            accumulatedScore = 0;
            upgradeCount += 1;
            OpenUpgradeMenu();
        }


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
        coordText.gameObject.SetActive(false);
        isInUpgradeMenu = false;
        submarine.GetComponent<SubmarineController>().upgradeScreen.SetActive(false);

        StartCoroutine(lootLockerManager.SubmitScoreRoutine(score, PlayerPrefs.GetString("PlayerName")));
    }

    public void RefreshSideScreen()
    {
        string hullLvText = "".PadRight(currentSubmarineHp, '|');
        string engineLvText = "".PadRight(speedLevel, '|');
        string sonarLvText = "".PadRight(sonarLevel, '|');
        string excavatorLvText = "".PadRight(excavatorLevel, '|');

        sideScreenText.text = $"hull\n{hullLvText}\nengine\n{engineLvText}\nsonar" +
            $"\n{sonarLvText}\nexcavator\n{excavatorLvText}";
    }

    public void StartGame()
    {
        RefreshScoreText();
        operatorNameDisplay.text = "Operator: " + PlayerPrefs.GetString("PlayerName");
        scoreText.gameObject.SetActive(true);
        sideScreenText.gameObject.SetActive(true);
        coordText.gameObject.SetActive(true);
        helpText.gameObject.SetActive(true);
        oxygenText.gameObject.SetActive(true);
        startScreen.SetActive(false);
        startedGame = true;
        submarine.gameObject.SetActive(true);

        foreach (GameObject go in objectToEnableAfterStart)
        {
            go.SetActive(true);
        }

        StartCoroutine(FadeIntensityLight2D(hullLight, hullLight.intensity, 2f, 2f));

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
        StartCoroutine(FadeAlphaTextMesh(sideScreenText, 0f, 1f, 1f));
        StartCoroutine(FadeAlphaTextMesh(scoreText, 0f, 1f, 1f));
        StartCoroutine(FadeAlphaImage(mainScreenFade, 1f, 0f, 3f));

    }

    public IEnumerator FadeIntensityLight2D(Light2D light, float startIntensity, float endIntensity, float duration)
    {
        // Keep track of the elapsed time
        float elapsedTime = 0.0f;

        while (elapsedTime < duration)
        {
            // Calculate the current intensity value
            float currentIntensity = Mathf.Lerp(startIntensity, endIntensity, elapsedTime / duration);

            // Set the intensity value of the light
            light.intensity = currentIntensity;

            // Wait for a frame
            yield return null;

            // Update the elapsed time
            elapsedTime += Time.deltaTime;
        }

        // Set the final intensity value of the light
        light.intensity = endIntensity;
    }

    public IEnumerator FadeAlphaImage(Image image, float startAlpha, float endAlpha, float duration)
    {
        float elapsedTime = 0.0f;

        while (elapsedTime < duration)
        {
            float currentAlpha = Mathf.Lerp(startAlpha, endAlpha, elapsedTime / duration);
            Color updateColor = image.color;
            updateColor.a = currentAlpha;
            image.color = updateColor;

            yield return null;

            elapsedTime += Time.deltaTime;
        }

        Color finalColor = image.color;
        finalColor.a = endAlpha;
        image.color = finalColor;
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
        if (startedGame)
        {
            hullLight.gameObject.SetActive(!hullLight.gameObject.activeSelf);
        }
    }

    public void OnToggleJournalBook()
    {
        bookPageHolder.SetActive(!bookPageHolder.activeSelf);
        disableSubmarineControl = bookPageHolder.activeSelf;
    }

    public void ChangeBGMVolume(float value)
    {
        bgmVolume = value;
        bgmAudioSource.volume = bgmVolume;
    }

    public void ChangeSFXVolume(float value)
    {
        sfxVolume = value;
        sfxAudioSource.volume = sfxVolume;
    }

    public void PlaySonarPing()
    {
        sfxAudioSource.PlayOneShot(sonarPing);
    }

    public void PlayBookFlipping()
    {
        sfxAudioSource.PlayOneShot(bookFlipping);
    }

    public void OpenUpgradeMenu()
    {
        isUpgrading = true;
    }

    public void AssignGuestNameToInputField(string playerName, bool placeholder)
    {
        guestName = playerName;

        // Prevent update name while player is editing them
        if (!nameInputField.isFocused)
        {
            PlayerPrefs.SetString("PlayerName", playerName);
            if (placeholder)
                nameInputField.placeholder.GetComponent<TextMeshProUGUI>().text = playerName;
            else
                nameInputField.text = playerName;
        }
    }

    public void OnNameInputFieldChange(string playerName)
    {
        PlayerPrefs.SetString("PlayerName", playerName);
    }

    public void CheckPressQForLeaderboard()
    {
        if (Input.GetKeyDown(KeyCode.E) && isInLeaderboard)
        {
            isInLeaderboard = false;
            lootLockerManager.CloseLeaderboard();
            if (endedGame)
                gameOverScreen.SetActive(true);
            else
                startScreen.SetActive(true);
        }
        if (Input.GetKeyDown(KeyCode.Q) && !isInLeaderboard)
        {
            isInLeaderboard = true;
            lootLockerManager.OpenLeaderboard();
            if (endedGame)
                gameOverScreen.SetActive(false);
            else
                startScreen.SetActive(false);
        }
    }

    public void PlayUpgradeSound()
    {
        sfxAudioSource.PlayOneShot(upgradeSfx);
    }
}
