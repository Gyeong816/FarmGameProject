using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fence : MonoBehaviour
{
    private void OnTriggerEnter(Collider col)
    {
        if (col.gameObject.CompareTag("Hammer"))
        {
            Debug.Log(col.gameObject.name);
            Destroy(gameObject);
        }
    }
}
