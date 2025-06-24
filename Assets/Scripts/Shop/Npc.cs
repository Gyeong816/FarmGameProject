using UnityEngine;

public class Npc : MonoBehaviour
{

    public NpcData npcData; 
    
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

    // 나중에 필요하면 거래 데이터를 여기에 추가
}