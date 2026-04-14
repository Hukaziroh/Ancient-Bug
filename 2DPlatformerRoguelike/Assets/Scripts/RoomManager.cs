using System.Collections;
using UnityEngine;

public class RoomManager : MonoBehaviour
{
    public static RoomManager Instance;

    [Header("방 프리팹 설정")]
    public GameObject startRoomPrefab;
    public GameObject[] randomRoomPrefabs;
    public GameObject bossRoomPrefab;

    [Header("던전 진행도 설정")]
    public int totalRooms = 5;
    public Transform player;

    [Header("페이드 연출 설정")]
    public CanvasGroup fadeCanvasGroup; 
    public float fadeDuration = 0.5f; 

    private GameObject currentRoom;
    private int currentRoomCount = 0;
    private bool isTransitioning = false; 

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        if (fadeCanvasGroup != null) fadeCanvasGroup.alpha = 1f;
        LoadNextRoom();
    }

    public void LoadNextRoom()
    {
       
        if (isTransitioning || currentRoomCount >= totalRooms) return;

       
        StartCoroutine(TransitionRoomRoutine());
    }

    IEnumerator TransitionRoomRoutine()
    {
        isTransitioning = true;

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

        if (currentRoomCount == 1) roomToLoad = startRoomPrefab;
        else if (currentRoomCount < totalRooms) roomToLoad = randomRoomPrefabs[Random.Range(0, randomRoomPrefabs.Length)];
        else if (currentRoomCount == totalRooms) roomToLoad = bossRoomPrefab;

        currentRoom = Instantiate(roomToLoad, Vector3.zero, Quaternion.identity);

        Transform spawnPoint = currentRoom.transform.Find("SpawnPoint");
        if (spawnPoint != null) player.position = spawnPoint.position;
       
        yield return new WaitForSeconds(0.1f);


        float timer = 0f;
        timer = 0f;
        while (timer < fadeDuration)
        {
            timer += Time.deltaTime;
 
            fadeCanvasGroup.alpha = Mathf.Lerp(1, 0, timer / fadeDuration);
            yield return null;
        }
        fadeCanvasGroup.alpha = 0f;

        isTransitioning = false; 
    }
}
