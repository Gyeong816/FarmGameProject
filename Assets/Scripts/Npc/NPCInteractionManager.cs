using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using TMPro;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class NPCInteractionManager : MonoBehaviour
{
    public UIManager uiManager;
    
    
    [SerializeField] private Button optionButton1;
    [SerializeField] private Button optionButton2;
    [SerializeField] private Button optionButton3;
    [SerializeField] private Button acceptButton;
    [SerializeField] private Button denyButton;
    [SerializeField] private Button confirmButton;
    [SerializeField] private Button openShopButton;
    [SerializeField] private Button giveItemButton;
    [SerializeField] private Button removeCompletedButton;
    
    [SerializeField] private TMP_Text npcCurrentText;
    [SerializeField] private TMP_Text option1Text;
    [SerializeField] private TMP_Text option2Text;
    [SerializeField] private TMP_Text option3Text;
    [SerializeField] private TMP_Text npcNameText;
    
    [SerializeField] private GameObject dialoguePanel;
    [SerializeField] private GameObject confirmButtonObj;
    [SerializeField] private GameObject optionButtonObj;
    [SerializeField] private GameObject shopButtonObj;
    [SerializeField] private GameObject denyButtonObj;
    [SerializeField] private GameObject acceptButtonObj;
    [SerializeField] private GameObject giveItemButtonObj;
    
    
    [SerializeField] private GameObject questUIPrefab;
    [SerializeField] private Transform questPanel;
        
    public List<DialogueLine> dialogueDatabase;
    
    public Dictionary<int, NpcData> questNpcDict;
    private Dictionary<int, DialogueLine> dialogueDict;
    private Dictionary<int, QuestUI> activeQuestUIs;
    private DialogueLine currentLine;
    private int npcId;

    private NpcData questData;
    

    private void Awake()
    {
        dialogueDict = new Dictionary<int, DialogueLine>();
        questNpcDict = new Dictionary<int, NpcData>();
        activeQuestUIs = new Dictionary<int, QuestUI>();
        
        confirmButton.onClick.AddListener(uiManager.CloseDialoguePanel);
        openShopButton.onClick.AddListener(uiManager.OpenShopPanel);
        denyButton.onClick.AddListener(uiManager.CloseDialoguePanel);
        removeCompletedButton.onClick.AddListener(RemoveCompletedQuest);
    }

    
    private async void Start()
    {
        var allDialogues = await TsvLoader.LoadTableAsync<DialogueLine>("DialogueLine");
        
        dialogueDatabase = allDialogues;
        
        dialogueDict.Clear();
        foreach (var data in dialogueDatabase)
        {
            dialogueDict[data.npcId] = data;
        }
        
    }

    
    
    public void ShowNpcDialogue(NpcData npcData, int npcId, int textId)
    {
        questData = null;
        bool canGiveItem = false;
        foreach (var data in questNpcDict)
        {
            if (data.Key == npcId)
            {
                canGiveItem = true;
                questData = data.Value;
            }
        }
        

        OffButtons(giveItemButtonObj,shopButtonObj,confirmButtonObj,denyButtonObj,acceptButtonObj);
        
        optionButtonObj.SetActive(true);
        
        optionButton1.onClick.RemoveAllListeners();
        optionButton2.onClick.RemoveAllListeners();
        optionButton3.onClick.RemoveAllListeners();

        optionButton1.onClick.AddListener(() => ShowNpcDialogue(npcData, npcId,1));
        optionButton3.onClick.AddListener(() => ShowNpcDialogue(npcData, npcId,3));

        if (canGiveItem)
            optionButton2.onClick.AddListener(() => ShowNpcDialogue(npcData, npcId, 4));
        else
            optionButton2.onClick.AddListener(() => ShowNpcDialogue(npcData, npcId, 2));
  
        
        npcNameText.text = npcData.npcName;
        
        switch (textId)
        {
            case 0:
                npcCurrentText.text = dialogueDict[npcId].startText;
                option1Text.text = dialogueDict[npcId].option1;
                option2Text.text = canGiveItem ? "Regarding the task you asked of me…" : dialogueDict[npcId].option2;
                option3Text.text = dialogueDict[npcId].option3;
                break;
            case 1:
                npcCurrentText.text = dialogueDict[npcId].answer1;
                optionButtonObj.SetActive(false);
                confirmButtonObj.SetActive(true);
                if (npcId == 1)
                { 
                    shopButtonObj.SetActive(true);
                }
                break;
            case 2:
                npcCurrentText.text = $"Could you bring me {npcData.requiredAmount} {npcData.requiredItemName}?";
                optionButtonObj.SetActive(false);
                OnButtons(acceptButtonObj, denyButtonObj);
                acceptButton.onClick.RemoveAllListeners();
                acceptButton.onClick.AddListener(() =>AcceptQuest(npcData));
                break;
            case 3:
                npcCurrentText.text = dialogueDict[npcId].answer3;
                optionButtonObj.SetActive(false);
                confirmButtonObj.SetActive(true);
                break;
            
            case 4:
                npcCurrentText.text = "Oh, you already brought them?";
                optionButtonObj.SetActive(false);
                OnButtons(giveItemButtonObj, denyButtonObj);
                giveItemButton.onClick.RemoveAllListeners();
                giveItemButton.onClick.AddListener(() =>GiveItem(questData.requiredItemId, questData.requiredAmount,npcId));
                break;
            
   
        }
        
        
    }
    

    private void AcceptQuest(NpcData npcData)
    { 
        questNpcDict[npcData.npcId] = npcData;
        
        var questPrefab = Instantiate(questUIPrefab, questPanel);
        var questUI = questPrefab.GetComponent<QuestUI>();
        questUI.Init(npcData);
        activeQuestUIs[npcData.npcId] = questUI;
        uiManager.CloseDialoguePanel();
    }

    private void GiveItem(int itemId, int amount, int npcId)
    {
        
        OffButtons(giveItemButtonObj, denyButtonObj);
        confirmButtonObj.SetActive(true);
        
        if (!InventoryManager.Instance.HasItem(itemId, amount))
        {
            npcCurrentText.text = "I think You don't have enough items.";
            confirmButton.onClick.RemoveAllListeners();
            confirmButton.onClick.AddListener(uiManager.CloseDialoguePanel);
            return;
        }
        questNpcDict.Remove(npcId);
        InventoryManager.Instance.SubtractItemFromSmallInventory(itemId, amount);
        InventoryManager.Instance.SubtractItemFromBigInventory(itemId, amount);
        TradeManager.Instance.AddRewardCoin(100);
        
        npcCurrentText.text = "Here, take 100 coins as your reward.";
        
        if (activeQuestUIs.TryGetValue(questData.npcId, out var questUI))
        {
            questUI.status = QuestStatus.Completed;
            questUI.OnQuestCompleted();
        }
        
        confirmButton.onClick.RemoveAllListeners();
        confirmButton.onClick.AddListener(uiManager.CloseDialoguePanel);
    }

    private void RemoveCompletedQuest()
    {
        var completedKeys = activeQuestUIs
            .Where(pair => pair.Value.status == QuestStatus.Completed)
            .Select(pair => pair.Key)
            .ToList();


        foreach (var npcId in completedKeys)
        {
            if (activeQuestUIs.TryGetValue(npcId, out var questUI))
            {
             
                Destroy(questUI.gameObject);
          
                activeQuestUIs.Remove(npcId);
                
                if (questNpcDict.ContainsKey(npcId))
                    questNpcDict.Remove(npcId);
            }
        }
    }
    
    private void OffButtons(params GameObject[] buttons)
    {
        foreach (var p in buttons)
            p.SetActive(false);
    }
    private void OnButtons(params GameObject[] buttons)
    {
        foreach (var p in buttons)
            p.SetActive(true);
    }
   
    
}

