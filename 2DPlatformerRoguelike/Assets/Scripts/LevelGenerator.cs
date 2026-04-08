using System.Collections.Generic;
using UnityEngine;

public class LevelGenerator : MonoBehaviour
{
    public GameObject startRoom; // 시작 방
    public GameObject[] roomPrefabs; // 중간에 나올 방들의 배열
    public GameObject endRoom; // 마지막 방

    public int totalRooms = 5; // 생성할 총 방의 개수
    public float roomWidth = 20f; // 방의 가로 길이 (규격화된 값)

    void Start()
    {
        GenerateLevel();
    }

    void GenerateLevel()
    {
     
        Instantiate(startRoom, Vector3.zero, Quaternion.identity);

        for (int i = 1; i < totalRooms - 1; i++)
        {
            int randomIndex = Random.Range(0, roomPrefabs.Length);

            Vector3 spawnPos = new Vector3(i * roomWidth, 0, 0);
            Instantiate(roomPrefabs[randomIndex], spawnPos, Quaternion.identity);
        }

        Vector3 endPos = new Vector3((totalRooms - 1) * roomWidth, 0, 0);
        Instantiate(endRoom, endPos, Quaternion.identity);
    }
}