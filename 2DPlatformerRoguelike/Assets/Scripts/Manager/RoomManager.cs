using System.Collections;
using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class LevelData
{
    public string levelName;
    public GameObject startRoomPrefab;
    public GameObject[] randomRoomPrefabs;
    public GameObject bossRoomPrefab;
}

public class RoomManager : MonoBehaviour
{
    public static RoomManager Instance;
    public int remainingEnemies = 0;

    [Header("레벨(층) 설정")]
    public List<LevelData> levels = new List<LevelData>();
    public int currentLevelIndex = 0;

    [Header("사운드 설정")]
    public AudioClip[] stageBgms;
    public AudioClip[] bossBgms;

    [Header("던전 진행도 설정")]
    public int totalRooms = 6;
    public Transform player;

    [Header("페이드 연출 설정")]
    public CanvasGroup fadeCanvasGroup;
    public float fadeDuration = 0.5f;

    private GameObject currentRoom;
    private int currentRoomCount = 0;
    public bool isTransitioning = false;

    private List<GameObject> shuffledRooms = new List<GameObject>();

    private LevelData CurrentLevelData => levels[Mathf.Min(currentLevelIndex, levels.Count - 1)];

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        if (fadeCanvasGroup != null) fadeCanvasGroup.alpha = 1f;

        StartNewLevel();
    }

    public void GoToNextLevel()
    {
        currentLevelIndex++;
        StartNewLevel();
    }

    private void StartNewLevel()
    {
        currentRoomCount = 0;
        shuffledRooms.Clear();

        ShuffleRandomRooms();
        LoadNextRoom();
    }

    private void ShuffleRandomRooms()
    {
        shuffledRooms.Clear();
        shuffledRooms.AddRange(CurrentLevelData.randomRoomPrefabs);

        for (int i = 0; i < shuffledRooms.Count; i++)
        {
            GameObject temp = shuffledRooms[i];
            int randomIndex = Random.Range(i, shuffledRooms.Count);
            shuffledRooms[i] = shuffledRooms[randomIndex];
            shuffledRooms[randomIndex] = temp;
        }
    }

    public void LoadNextRoom()
    {
        if (isTransitioning) return;
        if (currentRoomCount >= totalRooms)
        {
            GoToNextLevel();
            return;
        }

        StartCoroutine(TransitionRoomRoutine());
    }

    IEnumerator TransitionRoomRoutine()
    {
        isTransitioning = true;

        if (EnemyProjectilePool.Instance != null)
        {
            foreach (Transform child in EnemyProjectilePool.Instance.transform)
            {
                if (child.gameObject.activeSelf)
                {
                    child.gameObject.SetActive(false);
                }
            }
        }

        if (SoundManager.Instance != null)
        {
            int nextRoomCount = currentRoomCount + 1;

            if (nextRoomCount >= totalRooms)
            {
                if (currentLevelIndex < bossBgms.Length && bossBgms[currentLevelIndex] != null)
                {
                    SoundManager.Instance.PlayBGM(bossBgms[currentLevelIndex]);
                }
            }
            else
            {
                if (currentLevelIndex < stageBgms.Length && stageBgms[currentLevelIndex] != null)
                {
                    SoundManager.Instance.PlayBGM(stageBgms[currentLevelIndex]);
                }
            }
        }

        if (currentRoomCount > 0)
        {
            float fadeTimer = 0f;
            while (fadeTimer < fadeDuration)
            {
                fadeTimer += Time.deltaTime;
                fadeCanvasGroup.alpha = Mathf.Lerp(0, 1, fadeTimer / fadeDuration);
                yield return null;
            }
            fadeCanvasGroup.alpha = 1f;
        }

        if (currentRoom != null) Destroy(currentRoom);

        GameObject roomToLoad = null;
        currentRoomCount++;

        if (currentRoomCount == 1)
        {
            roomToLoad = CurrentLevelData.startRoomPrefab;
        }
        else if (currentRoomCount < totalRooms)
        {
            if (shuffledRooms.Count > 0)
            {
                roomToLoad = shuffledRooms[0];
                shuffledRooms.RemoveAt(0);
            }
            else
            {
                roomToLoad = CurrentLevelData.randomRoomPrefabs[Random.Range(0, CurrentLevelData.randomRoomPrefabs.Length)];
            }
        }
        else
        {
            roomToLoad = CurrentLevelData.bossRoomPrefab;
        }

        currentRoom = Instantiate(roomToLoad, Vector3.zero, Quaternion.identity);

        int normalEnemies = currentRoom.GetComponentsInChildren<Enemy>().Length;
        int fireEnemies = currentRoom.GetComponentsInChildren<FREnemy>().Length;
        int bosses = currentRoom.GetComponentsInChildren<Boss>().Length;
        int boss2 = currentRoom.GetComponentsInChildren<Boss2>().Length;
        int boss3 = currentRoom.GetComponentsInChildren<Boss3>().Length;

        remainingEnemies = normalEnemies + fireEnemies + bosses + boss2 + boss3;

        if (UIManager.Instance != null)
        {
            UIManager.Instance.UpdateEnemy(remainingEnemies);
        }

        Transform spawnPoint = currentRoom.transform.Find("SpawnPoint");
        if (player != null && spawnPoint != null)
        {
            player.position = spawnPoint.position;
        }

        yield return new WaitForSeconds(0.1f);

        float timer = 0f;
        while (timer < fadeDuration)
        {
            timer += Time.deltaTime;
            fadeCanvasGroup.alpha = Mathf.Lerp(1, 0, timer / fadeDuration);
            yield return null;
        }
        fadeCanvasGroup.alpha = 0f;

        isTransitioning = false;
    }

    public void OnEnemyKilled()
    {
        remainingEnemies--;
        if (UIManager.Instance != null)
        {
            UIManager.Instance.UpdateEnemy(remainingEnemies);
        }
    }
}