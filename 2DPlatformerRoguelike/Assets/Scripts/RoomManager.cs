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

        float timer = 0f;
        while (timer < fadeDuration)
        {
            timer += Time.deltaTime;  
            fadeCanvasGroup.alpha = Mathf.Lerp(0, 1, timer / fadeDuration);
            yield return null;
        }
        fadeCanvasGroup.alpha = 1f;

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

//1.UI용 검은 화면 만들기 (유니티 에디터 세팅)
//먼저 화면을 덮어줄 검은색 천(UI)을 하나 만들어야 합니다.

//하이어라키(Hierarchy) 창 우클릭  UI  Image를 클릭합니다. (Canvas도 자동으로 생성됩니다.)

//방금 생성된 Image의 이름을 FadePanel로 바꿉니다.

//인스펙터 창에서 다음을 설정합니다:

//Color: 완전한 검은색(Black)으로 바꿉니다.

//Rect Transform: 앵커 모양 아이콘을 누르고 Alt(Mac은 Option) 키를 누른 상태로 **우측 하단의 꽉 찬 네모(Stretch)**를 클릭합니다. (이렇게 하면 해상도가 바뀌어도 검은 화면이 꽉 차게 덮어줍니다.)

//Raycast Target: 체크를 해제합니다. (이게 켜져 있으면 나중에 마우스 클릭을 막아버릴 수 있습니다.)

//FadePanel에 Canvas Group 컴포넌트를 추가합니다. (Add Component 검색창에 Canvas Group 검색)

//이 컴포넌트의 Alpha 값을 0 ~ 1로 조절하면서 투명도를 제어할 겁니다. 처음에는 Alpha 값을 0(완전 투명)으로 맞춰두세요