using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }

    [SerializeField] private GameObject winPanel;
    [SerializeField] private GameObject losePanel;
    [SerializeField] private Button winTryAgainButton;
    [SerializeField] private Button loseTryAgainButton;

    private void Awake()
    {
        Time.timeScale = 1;
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        if (winTryAgainButton != null)
            winTryAgainButton.onClick.AddListener(TryAgain);
        if (loseTryAgainButton != null)
            loseTryAgainButton.onClick.AddListener(TryAgain);
    }

    public void ShowWinPanel()
    {
        Time.timeScale = 0;
        winPanel.SetActive(true);
        losePanel.SetActive(false);
    }

    public void ShowLosePanel()
    {
        Time.timeScale = 0;
        winPanel.SetActive(false);
        losePanel.SetActive(true);
    }

    public void TryAgain()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
} 