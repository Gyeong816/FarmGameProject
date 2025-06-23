using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using TMPro;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class DialogueManager : MonoBehaviour
{
    
    [Header("Dialogue UI")]
    [SerializeField] private TMP_Text npcCurrentText;


    [Header("Choice UI")]
    [SerializeField] private Button optionButton1;
    [SerializeField] private Button optionButton2;
    [SerializeField] private Button optionButton3;
    
    [SerializeField] private TMP_Text option1Text;
    [SerializeField] private TMP_Text option2Text;
    [SerializeField] private TMP_Text option3Text;
    
    [SerializeField] private GameObject dialoguePanel;
    [SerializeField] private GameObject confirmButtonObj;
    [SerializeField] private GameObject optionButtonObj;
    [SerializeField] private GameObject shopButtonObj;
    
    public List<DialogueLine> dialogueDatabase;
    
    private Dictionary<int, DialogueLine> dialogueDict;
    
    private DialogueLine currentLine;

    private int npcId;

    private void Awake()
    {
        dialogueDict = new Dictionary<int, DialogueLine>();
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

    
    
    public void ShowNpcDialogue(int npcId, int textId)
    {
        shopButtonObj.SetActive(false);
        confirmButtonObj.SetActive(false);
        optionButtonObj.SetActive(true);
        
        optionButton1.onClick.RemoveAllListeners();
        optionButton2.onClick.RemoveAllListeners();
        optionButton3.onClick.RemoveAllListeners();

        optionButton1.onClick.AddListener(() => ShowNpcDialogue(npcId,1));
        optionButton2.onClick.AddListener(() => ShowNpcDialogue(npcId,2));
        optionButton3.onClick.AddListener(() => ShowNpcDialogue(npcId,3));
        
        switch (textId)
        {
            case 0:
                npcCurrentText.text = dialogueDict[npcId].startText;
                option1Text.text = dialogueDict[npcId].option1;
                option2Text.text = dialogueDict[npcId].option2;
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
                npcCurrentText.text = dialogueDict[npcId].answer2;
                optionButtonObj.SetActive(false);
                confirmButtonObj.SetActive(true);
                break;
            case 3:
                npcCurrentText.text = dialogueDict[npcId].answer3;
                optionButtonObj.SetActive(false);
                confirmButtonObj.SetActive(true);
                break;
   
        }
    }


    


 
    
   
}

