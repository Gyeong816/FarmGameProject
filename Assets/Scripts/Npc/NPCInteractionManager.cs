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
    [SerializeField] private TMP_Text npcAffectionText;
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
    [SerializeField] private Image currentNpcImage;
    [SerializeField] private List<Sprite> npcImagelist;
    private Dictionary<string, Sprite> npcIconDic;
    private List<DialogueLine> dialogueDatabase;
    private Dictionary<int, NpcData> questNpcDict;
    private Dictionary<int, DialogueLine> dialogueDict;
    private Dictionary<int, QuestUI> activeQuestUIs;
    private DialogueLine currentLine;
    
    private NpcData questData;
    private int currentNpcId;
    public static event System.Action<int, int> OnQuestCompleted;

    private void Awake()
    {
        dialogueDict = new Dictionary<int, DialogueLine>();
        questNpcDict = new Dictionary<int, NpcData>();
        activeQuestUIs = new Dictionary<int, QuestUI>();
        npcIconDic = new Dictionary<string, Sprite>();
        
        foreach (var sprite in npcImagelist)
        {
            if (sprite != null)
                npcIconDic[sprite.name] = sprite;
        }
        
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
        currentNpcId = npcId;
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
        
        npcAffectionText.text = $"Affection : {npcData.affection}";
        npcNameText.text = npcData.npcName;
        var image = GetNpcImage(dialogueDict[npcId].imageKey);
        currentNpcImage.sprite = image;
        string itemName = InventoryManager.Instance.GetItemName(npcData.requiredItemId);
        
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
                npcCurrentText.text = $"Could you bring me {npcData.requiredAmount} {itemName}?";
                optionButtonObj.SetActive(false);
                OnButtons(acceptButtonObj, denyButtonObj);
                acceptButton.onClick.RemoveAllListeners();
                acceptButton.onClick.AddListener(() =>AcceptQuest(npcData,itemName));
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
                giveItemButton.onClick.AddListener(() =>GiveItem());
                break;
            
   
        }
        
        
    }
    

    private void AcceptQuest(NpcData npcData, string itemName)
    { 
        questNpcDict[npcData.npcId] = npcData;
        
        var questPrefab = Instantiate(questUIPrefab, questPanel);
        var questUI = questPrefab.GetComponent<QuestUI>();
        questUI.Init(npcData, itemName);
        activeQuestUIs[npcData.npcId] = questUI;
        uiManager.CloseDialoguePanel();
    }

    private void GiveItem()
    {
        
        OffButtons(giveItemButtonObj, denyButtonObj);
        confirmButtonObj.SetActive(true);
        
        if (!InventoryManager.Instance.HasItem(questData.requiredItemId, questData.requiredAmount))
        {
            npcCurrentText.text = "I think You don't have enough items.";
            confirmButton.onClick.RemoveAllListeners();
            confirmButton.onClick.AddListener(uiManager.CloseDialoguePanel);
            return;
        }
        questNpcDict.Remove(currentNpcId);
        InventoryManager.Instance.SubtractItemFromSmallInventory(questData.requiredItemId, questData.requiredAmount);
        InventoryManager.Instance.SubtractItemFromBigInventory(questData.requiredItemId, questData.requiredAmount);

        int coin = 100 * questData.affection;
        TradeManager.Instance.AddRewardCoin(coin);
        
        npcCurrentText.text = $"Here, take {coin} coins as your reward.";
        
        if (activeQuestUIs.TryGetValue(questData.npcId, out var questUI))
        {
            questUI.status = QuestStatus.Completed;
            questUI.OnQuestCompleted();
            OnQuestCompleted?.Invoke(currentNpcId, questData.questId);
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
    
    public Sprite GetNpcImage(string imageKey)
    {
        if (npcIconDic.TryGetValue(imageKey, out var sprite))
            return sprite;
        
        Debug.LogWarning($" '{imageKey}'를 찾을 수 없습니다.");
        return null;
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

