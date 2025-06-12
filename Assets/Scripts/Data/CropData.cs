      // SimpleCropData.cs
using UnityEngine;

[CreateAssetMenu(fileName = "CropData", menuName = "ScriptableObjects/CropData")]
public class CropData : ScriptableObject
{
    [Header("데이터 매핑")]
    public int seedId;          
    public int itemId;   
    
    [Header("성장 단계별 프리팹")]
    public GameObject[] stagePrefabs = new GameObject[5];
}