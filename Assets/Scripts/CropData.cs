// SimpleCropData.cs
using UnityEngine;

[CreateAssetMenu(fileName = "CropData", menuName = "ScriptableObjects/CropData")]
public class CropData : ScriptableObject
{
    [Tooltip("0일→1단계, 1일→2단계, 2일→3단계(수확), 3일→4단계(시들음)")]
    public GameObject[] stagePrefabs = new GameObject[5];
    
    [Tooltip("수확 시 나올 아이템 프리팹")]
    public GameObject harvestPrefab;
}