using System;
using System.Collections;
using System.Collections.Generic;
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

  private void GoToInGame()
  {
    SceneManager.LoadScene(2);
  }
}
