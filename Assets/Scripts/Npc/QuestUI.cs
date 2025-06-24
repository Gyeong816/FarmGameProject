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
    
    public void Init(NpcData data)
    {
        npcData = data;

        
        status = QuestStatus.InProgress;
        questDescription.text = $"Deliver {npcData.requiredAmount} {npcData.requiredItemName} to {npcData.npcName}";
    }
}
