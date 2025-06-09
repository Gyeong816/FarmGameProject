using UnityEngine;

public class LandTile : MonoBehaviour
{
    [Header("타일 Prefab")]
    [SerializeField] private GameObject grassPrefab;   // 잔디 Prefab 에셋
    [SerializeField] private GameObject plowedPrefab;  // 경작된 땅 Prefab 에셋

    // 실제 씬에 생성된 인스턴스를 담아둘 변수
    private GameObject grassInstance;
    private GameObject plowedInstance;

    private bool isPlowed = false;
    private int gridX, gridZ;
    private MapManager mapManager;

    public void Initialize(int x, int z, MapManager manager)
    {
        gridX      = x;
        gridZ      = z;
        mapManager = manager;

        // Prefab 으로부터 씬 오브젝트를 생성하고, 이 LandTile의 자식으로 둬서
        // 위치/회전/스케일을 이 트랜스폼과 함께 관리하게 합니다.
        grassInstance  = Instantiate(grassPrefab,  transform.position, Quaternion.identity, transform);
        plowedInstance = Instantiate(plowedPrefab, transform.position, Quaternion.identity, transform);

        // 시작할 때는 잔디만 보이게
        grassInstance .SetActive(true);
        plowedInstance.SetActive(false);
    }

    public void Hoe()
    {
        if (isPlowed) return;
        isPlowed = true;

        // 쟁기질 시 잔디는 숨기고, 경작 땅만 보여줍니다.
        grassInstance .SetActive(false);
        plowedInstance.SetActive(true);
    }

    public Vector2Int GetGridPosition()
    {
        return new Vector2Int(gridX, gridZ);
    }
}