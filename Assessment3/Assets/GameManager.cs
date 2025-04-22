using UnityEngine;
using TMPro; // 必须引入 TMPro 命名空间

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    
    [Header("UI References (TMP)")]
    [SerializeField] private TextMeshProUGUI collectionText; // TMP 文本组件
    [SerializeField] private GameObject congratsText; // 普通 GameObject（如果恭喜文字也用TMP则改为 TextMeshProUGUI）
    
    [Header("Game Settings")]
    public int totalCherries = 8;
    private int collectedCherries = 0;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
        
        // 初始化UI状态
        UpdateCollectionText();
        if (congratsText != null) 
            congratsText.SetActive(false);
    }

    public void CollectCherry()
    {
        collectedCherries++;
        UpdateCollectionText();
        
        if (collectedCherries >= totalCherries)
        {
            ShowCongratulations();
        }
    }

    private void UpdateCollectionText()
    {
        if (collectionText != null)
        {
            collectionText.text = $"<color=#FFD700>{collectedCherries}</color>/<size=20>{totalCherries}</size>"; // 使用TMP富文本
        }
    }

    private void ShowCongratulations()
    {
        if (congratsText != null)
        {
            congratsText.SetActive(true);
            // 可选：添加动画效果
        }
    }
}