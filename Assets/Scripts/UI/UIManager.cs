using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [Header("참조")]
    [SerializeField] private GameObject InventoryUI;
    [SerializeField] private CameraController cameraController;

    private bool isInventoryOpen;

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked; 
        Cursor.visible = false;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            if (!isInventoryOpen)
            {
                cameraController.IsInventoryOpen();
                isInventoryOpen = true;
                InventoryUI.SetActive(true);
                Time.timeScale = 0;
                Cursor.lockState = CursorLockMode.Confined; 
                Cursor.visible = true;
            }
            else
            {
                cameraController.IsInventoryClose();
                isInventoryOpen = false;
                InventoryUI.SetActive(false);
                Time.timeScale = 1;
                Cursor.lockState = CursorLockMode.Locked; 
                Cursor.visible = false;
            }
        }
    }
}
