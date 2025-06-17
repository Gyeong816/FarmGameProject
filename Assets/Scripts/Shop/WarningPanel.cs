using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WarningPanel : MonoBehaviour
{
   public void CloseWarningPanel()
   {
      gameObject.SetActive(false);
   }
}
