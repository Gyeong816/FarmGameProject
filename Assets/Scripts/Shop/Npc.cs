using System;
using System.Linq;
using UnityEngine;

public class Npc : MonoBehaviour
{

    public NpcData npcData;
    
    private void Start()
    {
        npcData.affection = 1;
        npcData.questId = 1;
        npcData.requiredItemId = UnityEngine.Random.Range(16, 26);
        npcData.requiredAmount = UnityEngine.Random.Range(1, 4);
    }

    private void OnEnable()
    {
        NPCInteractionManager.OnQuestCompleted += HandleQuestCompleted;
    }
    private void OnDisable()
    {
        NPCInteractionManager.OnQuestCompleted -= HandleQuestCompleted;
    }
    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        UIManager.Instance.ShowPromptUI(gameObject.transform,UIManager.PromptType.Npc,npcData);
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        UIManager.Instance.HidePromptUI();
    }

    private void HandleQuestCompleted(int completedNpcId, int questId)
    {
        if (completedNpcId != npcData.npcId || questId != npcData.questId) return;

        npcData.questId++;
        npcData.affection++;
        npcData.requiredItemId = UnityEngine.Random.Range(16, 26);
        npcData.requiredAmount = UnityEngine.Random.Range(1, 4);
    }
}