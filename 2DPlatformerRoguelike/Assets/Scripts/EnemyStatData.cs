using UnityEngine;

[CreateAssetMenu(fileName = "EnemyStatData", menuName = "Scriptable Objects/EnemyStatData")]
public class EnemyStatData : ScriptableObject
{
    [Header("기본 정보")]
    public string enemyName = "Dummy";

    [Header("전투 스탯")]
    public float maxHP = 50f;
    public float damage = 10f;
    public float moveSpeed = 3f;
}


public interface IDamageable
{
    // 데미지를 입었을 때 호출될 함수
    void TakeDamage(float damage);
}