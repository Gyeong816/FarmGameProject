using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public enum QuestStatus { InProgress, Completed }
public class QuestUI : MonoBehaviour
{
    public QuestStatus status;
    
    public TMP_Text questDescription;
    
    public NpcData npcData;
    
    [SerializeField] private GameObject InProgress;
    [SerializeField] private GameObject Completed;
    
    public void Init(NpcData data)
    {
        npcData = data;
        InProgress.SetActive(true);
        
        status = QuestStatus.InProgress;
        questDescription.text = $"Deliver {npcData.requiredAmount} {npcData.requiredItemName} to {npcData.npcName}";
    }
    
    public void OnQuestCompleted()
    {
        InProgress.SetActive(false);
        Completed.SetActive(true);
        status = QuestStatus.Completed;
 
    }
}
