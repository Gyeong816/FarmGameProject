using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

#if UNITY_EDITOR
using UnityEditor;
#endif
public class MainmenuUIController : MonoBehaviour
{
  [SerializeField] private Button inGameButton;
  [SerializeField] private Button exitButton; 
  private void Awake()
  {
    inGameButton.onClick.AddListener(GoToInGame);
    exitButton.onClick.AddListener(QuitGame);
  }

  private void Start()
  {
    SoundManager.Instance.PlayBgm("BGM_MenuBgm");
  }

  private void GoToInGame()
  {
    SceneManager.LoadScene(2);
  }
  
  private void QuitGame()
  {
    Application.Quit();
    
#if UNITY_EDITOR
    EditorApplication.isPlaying = false;
#endif
  }
}
