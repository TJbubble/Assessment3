using UnityEngine;

public class Cherry : MonoBehaviour
{
    [Header("Effects")]
    public AudioClip collectSound;
    public ParticleSystem collectEffect;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Dog"))
        {
            Collect();
        }
    }

    private void Collect()
    {
        // 播放效果
        if (collectSound != null)
            AudioSource.PlayClipAtPoint(collectSound, transform.position);
        
        if (collectEffect != null)
            Instantiate(collectEffect, transform.position, Quaternion.identity);

        // 通知 GameManager
        GameManager.Instance?.CollectCherry(); // 安全调用
        
        Destroy(gameObject);
    }
}