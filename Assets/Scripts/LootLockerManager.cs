using UnityEngine;
using LootLocker.Requests;
using System.Collections;
using TMPro;

/// <summary>
/// My class for connecting to LootLocker stuff. Not mistake with
/// LootLockerSDKManager.
/// </summary>
public class LootLockerManager : MonoBehaviour
{
    public string leaderboardKey = "Operation_SLRN";
    public GameObject leaderboardScreen;
    public GameObject entryPrefab;
    public Transform entryHolder;
    private int maxEntriesInPage = 10;
    private LootLockerLeaderboardMember[] leaderboardScores;
    private int pageIndex = 0;
    private Coroutine listScoreCoroutine;


    void Start()
    {
        StartCoroutine(LoginRoutine());
    }

    void Update()
    {
        if (GameManager.instance.isInLeaderboard)
        {
            if (Input.GetButtonDown("Vertical"))
            {
                float moveVertical = Input.GetAxis("Vertical");
                if (moveVertical > 0)
                {
                    moveVertical = 0;
                    if (pageIndex > 0)
                    {
                        pageIndex = Mathf.Clamp(pageIndex - 1, 0, 4);
                        if (listScoreCoroutine != null)
                            StopCoroutine(listScoreCoroutine);
                        listScoreCoroutine = StartCoroutine(ListScoreEntries());
                    }
                }
                else if (moveVertical < 0)
                {
                    moveVertical = 0;
                    if (pageIndex < 4)
                    {
                        pageIndex = Mathf.Clamp(pageIndex + 1, 0, 4);
                        if (listScoreCoroutine != null)
                            StopCoroutine(listScoreCoroutine);
                        listScoreCoroutine = StartCoroutine(ListScoreEntries());
                    }

                }
            }
        }
    }

    public void OpenLeaderboard()
    {
        foreach (Transform child in entryHolder)
        {
            Destroy(child.gameObject);
        }
        leaderboardScreen.SetActive(true);
        StartCoroutine(LoadLeaderboardScores());
    }

    public void CloseLeaderboard()
    {
        StopCoroutine(listScoreCoroutine);
        foreach (Transform child in entryHolder)
        {
            Destroy(child.gameObject);
        }
        leaderboardScreen.SetActive(false);
    }

    public IEnumerator LoginRoutine()
    {
        bool done = false;
        LootLockerSDKManager.StartGuestSession((response) =>
        {
            if (response.success)
            {
                Debug.Log("Player was logged in");
                done = true;
            }
            else
            {
                Debug.Log("Could not start session");
            }
        });
        yield return new WaitWhile(() => done == false);
    }

    public IEnumerator SubmitScoreRoutine(int scoreToUpload, string playerName)
    {
        yield return new WaitForSeconds(2f);
        bool done = false;
        LootLockerSDKManager.SubmitScore(playerName, scoreToUpload, leaderboardKey, (response) =>
        {
            if (response.success)
            {
                Debug.Log("Successfully upload score");
                done = true;
            }
            else
            {
                Debug.Log("Failed to upload score");
                done = true;
            }
        });
        yield return new WaitWhile(() => done == false);
    }

    public IEnumerator ListScoreEntries()
    {
        // Destroy old entries
        foreach (Transform child in entryHolder)
        {
            Destroy(child.gameObject);
        }

        // Create new entries
        int startIndex = pageIndex * maxEntriesInPage;
        for (int i = startIndex; i < startIndex + maxEntriesInPage; i++)
        {
            TextMeshProUGUI entry = Instantiate(entryPrefab, entryHolder).GetComponent<TextMeshProUGUI>();
            yield return new WaitForSeconds(0.1f);
            if (i > leaderboardScores.Length - 1)
            {
                entry.text = (i + 1).ToString().PadLeft(3) + " - ";
            }
            else
            {
                entry.text = leaderboardScores[i].rank.ToString().PadLeft(3) + " - " + leaderboardScores[i].member_id + " - " + leaderboardScores[i].score;
            }
        }
        yield return null;
    }

    public IEnumerator LoadLeaderboardScores()
    {
        bool done = false;
        LootLockerSDKManager.GetScoreList(leaderboardKey, 50, (response) =>
        {
            if (response.success)
            {
                Debug.Log("Successfully get leaderboard score");
                leaderboardScores = response.items;
                done = true;
            }
            else
            {
                Debug.Log("Failed to get leaderboard score");
                done = true;
            }
        });
        yield return new WaitWhile(() => done == false);
        if (listScoreCoroutine != null)
            StopCoroutine(listScoreCoroutine);
        listScoreCoroutine = StartCoroutine(ListScoreEntries());
        yield return null;
    }
}