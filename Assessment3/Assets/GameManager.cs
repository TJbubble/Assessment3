using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    
    private Text collectionText;
    private GameObject congratsText;
    
    public int totalCherries = 4;
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
        
        FindUIElements();
        UpdateUI();
    }
    
    private void FindUIElements()
    {
        GameObject collectionGO = GameObject.Find("CollectionText");
        if (collectionGO != null)
        {
            collectionText = collectionGO.GetComponent<Text>();
        }
        else
        {
            Debug.LogError("找不到CollectionText对象！");
        }
        
        // 查找恭喜文本
        congratsText = GameObject.Find("CongratsText");
        if (congratsText == null)
        {
            Debug.LogError("找不到CongratsText对象！");
        }
        else
        {
            // 确保初始不可见
            congratsText.SetActive(false);
        }
    }
    
    public void CollectCherry()
    {
        collectedCherries++;
        UpdateUI();
        
        if (collectedCherries >= totalCherries && congratsText != null)
        {
            congratsText.SetActive(true);
        }
    }
    
    private void UpdateUI()
    {
        if (collectionText != null)
        {
            collectionText.text = $"{collectedCherries}/{totalCherries}";
        }
    }
}