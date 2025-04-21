using UnityEngine;

public class Cherry : MonoBehaviour
{
    public AudioClip collectSound; // 收集音效
    
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Dog"))
        {
            // 播放收集音效
            if (collectSound != null)
            {
                AudioSource.PlayClipAtPoint(collectSound, transform.position);
            }
            
            // 通知GameManager收集了一个Cherry
            GameManager.Instance.CollectCherry();
            
            // 销毁Cherry
            Destroy(gameObject);
        }
    }
}