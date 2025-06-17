using UnityEngine;

public class Merchant : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        UIManager.Instance.ShowPromptUI(gameObject.transform,UIManager.PromptType.Shop);
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        UIManager.Instance.HidePromptUI();
    }

    // 나중에 필요하면 거래 데이터를 여기에 추가
}