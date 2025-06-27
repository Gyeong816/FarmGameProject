using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainmenuUIController : MonoBehaviour
{
  [SerializeField] private Button inGameButton;

  private void Awake()
  {
    inGameButton.onClick.AddListener(GoToInGame);
  }

  private void Start()
  {
    SoundManager.Instance.PlayBgm("BGM_MenuBgm");
  }

  private void GoToInGame()
  {
    SceneManager.LoadScene(2);
  }
}
