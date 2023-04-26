using UnityEngine;
using LootLocker.Requests;
using System.Collections;
using TMPro;
using System.Collections.Generic;

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
    private List<int> leaderboardScores = new List<int>();
    private List<string> leaderboardNames = new List<string>();

    private int pageIndex = 0;
    private Coroutine listScoreCoroutine;
    public GameObject leaderboardLoadingIndicator;


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
                Debug.Log($"Player was logged in as {response.player_id}");
                done = true;

                // If first time, create a random name
                if (PlayerPrefs.GetString("PlayerName", "") == "")
                {
                    // The character limit for the Name INput component is 15
                    // so it should be fine. If not, increase the limit.
                    GameManager.instance.AssignGuestNameToInputField($"Sailor#{response.player_id}", false);
                }
            }
            else
            {
                Debug.Log("Could not start session");
                GameManager.instance.AssignGuestNameToInputField("Anonymous", true);
            }
        });


        yield return new WaitWhile(() => done == false);
    }

    public IEnumerator SubmitScoreRoutine(int scoreToUpload, string playerName)
    {
        if (playerName.Trim() == "")
        {
            yield return null;
        }

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
            if (i > leaderboardScores.Count - 1)
            {
                entry.text = (i + 1).ToString().PadLeft(3) + " - ";
            }
            else
            {
                entry.text = (i + 1).ToString().PadLeft(3) + " - " + leaderboardNames[i] + " - " + leaderboardScores[i];
            }
        }
        yield return null;
    }

    public IEnumerator LoadLeaderboardScores()
    {
        bool done = false;
        leaderboardLoadingIndicator.SetActive(true);
        LootLockerSDKManager.GetScoreList(leaderboardKey, 50, (response) =>
        {
            if (response.success)
            {
                Debug.Log("Successfully get leaderboard score");
                leaderboardNames.Clear();
                leaderboardScores.Clear();
                foreach (var item in response.items)
                {
                    leaderboardNames.Add(item.member_id);
                    leaderboardScores.Add(item.score);
                }
                done = true;
            }
            else
            {
                Debug.Log("Failed to get leaderboard score");
                done = true;
            }
        });
        yield return new WaitWhile(() => done == false);
        leaderboardLoadingIndicator.SetActive(false);
        if (listScoreCoroutine != null)
            StopCoroutine(listScoreCoroutine);
        listScoreCoroutine = StartCoroutine(ListScoreEntries());
        yield return null;
    }
}