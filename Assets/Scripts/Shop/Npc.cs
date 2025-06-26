using System;
using System.Linq;
using UnityEngine;

public class Npc : MonoBehaviour
{
    [Header("Rotation Settings")]
    [SerializeField] private float rotationSpeed = 5f;

    public NpcData npcData;
    
    private Animator animator;
    private Quaternion targetRotation;
    private bool shouldRotate;
    private void Start()
    {
        npcData.affection = 1;
        npcData.questId = 1;
        npcData.requiredItemId = UnityEngine.Random.Range(16, 26);
        npcData.requiredAmount = UnityEngine.Random.Range(1, 4);
        
        animator = GetComponent<Animator>();
    }

    private void OnEnable()
    {
        NPCInteractionManager.OnQuestCompleted += HandleQuestCompleted;
    }
    private void OnDisable()
    {
        NPCInteractionManager.OnQuestCompleted -= HandleQuestCompleted;
    }
    
    private void Update()
    {
        // 부드러운 회전 처리
        if (shouldRotate)
        {
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * rotationSpeed);
            if (Quaternion.Angle(transform.rotation, targetRotation) < 0.5f)
                shouldRotate = false;
        }
    }
    
    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        animator.SetTrigger("Hello");
        
        Vector3 direction = other.transform.position - transform.position;
        direction.y = 0f; // 수직 성분 제거

        if (direction.sqrMagnitude > 0.001f)
        {
            targetRotation = Quaternion.LookRotation(direction);
            shouldRotate = true;
        }
        
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