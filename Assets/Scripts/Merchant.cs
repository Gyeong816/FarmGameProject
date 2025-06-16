using UnityEngine;

public class Merchant : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        UIManager.Instance.ShowMerchantPrompt(gameObject.transform);
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        UIManager.Instance.HideMerchantPrompt();
    }

    // 나중에 필요하면 거래 데이터를 여기에 추가
}