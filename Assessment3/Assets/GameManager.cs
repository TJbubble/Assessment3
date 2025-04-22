using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Collections; // 添加协程需要的命名空间

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI winText;
    public int maxCherries = 10;
    public AudioClip winSound; // 可选音效

    private int _currentScore = 0;
    private AudioSource _audioSource;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        _audioSource = GetComponent<AudioSource>();
        winText.gameObject.SetActive(false);
    }

    public void CollectCherry()
    {
        _currentScore++;
        UpdateScoreDisplay();

        if (_currentScore >= maxCherries)
        {
            ShowWinMessage();
        }
    }

    private void ShowWinMessage()
    {
        winText.gameObject.SetActive(true);

        // 使用协程动画
        StartCoroutine(ScaleInAnimation());

        // 播放音效
        if (winSound != null && _audioSource != null)
        {
            _audioSource.PlayOneShot(winSound);
        }
    }

    IEnumerator ScaleInAnimation()
    {
        float duration = 0.5f;
        float elapsed = 0f;
        winText.transform.localScale = Vector3.zero;

        while (elapsed < duration)
        {
            float t = elapsed / duration;
            // 弹性动画公式
            float scale = Mathf.Sin(t * Mathf.PI * 2) * 0.3f + 1f;
            winText.transform.localScale = new Vector3(scale, scale, scale);
            elapsed += Time.deltaTime;
            yield return null;
        }

        winText.transform.localScale = Vector3.one;
    }

    private void UpdateScoreDisplay()
    {
        scoreText.text = $"Cherries: {_currentScore}/{maxCherries}";
    }
}