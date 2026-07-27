using System.Collections;
using TMPro;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;

    [Header("Panels")]
    public CanvasGroup menuPanel;
    public CanvasGroup gameplayPanel;
    public CanvasGroup gameOverPanel;
    public CanvasGroup settingsPanel;
    public CanvasGroup helpPanel;

    [Header("Text Effects")]
    public CanvasGroup noSpacesTextGroup; // "No Spaces Left" UI element
    public TextMeshProUGUI comboText;     // "Good", "Nice", "Excellent" text

    [Header("References")]
    public WipeController wipeController;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        SetPanelInstant(menuPanel, true);
        SetPanelInstant(gameplayPanel, false);
        SetPanelInstant(gameOverPanel, false);
        SetPanelInstant(settingsPanel, false);
        SetPanelInstant(helpPanel, false);
        SetPanelInstant(noSpacesTextGroup, false);

        if (comboText != null) comboText.gameObject.SetActive(false);
    }

    public void Fade(CanvasGroup group, bool fadeIn)
    {
        StartCoroutine(FadeRoutine(group, fadeIn));
    }

    private IEnumerator FadeRoutine(CanvasGroup group, bool fadeIn)
    {
        float target = fadeIn ? 1 : 0;
        float start = group.alpha;
        float time = 0f;
        float duration = 0.3f;

        group.blocksRaycasts = fadeIn;
        group.interactable = fadeIn;

        while (time < duration)
        {
            time += Time.deltaTime;
            group.alpha = Mathf.Lerp(start, target, time / duration);
            yield return null;
        }
        group.alpha = target;
    }

    private void SetPanelInstant(CanvasGroup group, bool isVisible)
    {
        if (group == null) return;
        group.alpha = isVisible ? 1 : 0;
        group.interactable = isVisible;
        group.blocksRaycasts = isVisible;
    }

    public void ShowNoSpacesLeft()
    {
        if (noSpacesTextGroup != null)
            Fade(noSpacesTextGroup, true);
    }

    public void HideNoSpacesLeft()
    {
        if (noSpacesTextGroup != null)
            Fade(noSpacesTextGroup, false);
    }

    public void ShowComboText(int lines)
    {
        if (comboText == null) return;
        StartCoroutine(ComboTextRoutine(lines));
    }

    private IEnumerator ComboTextRoutine(int lines)
    {
        string text = "Good!";
        if (lines == 2) text = "Nice!";
        else if (lines >= 3) text = "Excellent!";

        comboText.text = text;
        comboText.gameObject.SetActive(true);
        comboText.color = new Color(1, 1, 0, 1); // Yellow
        comboText.transform.localScale = Vector3.one * 0.5f;

        float t = 0;
        while (t < 1f)
        {
            t += Time.deltaTime * 2f; // Takes 0.5 seconds
            comboText.transform.localScale = Vector3.Lerp(Vector3.one * 0.5f, Vector3.one * 1.5f, t);
            comboText.color = new Color(1, 1, 0, 1 - t);
            yield return null;
        }
        comboText.gameObject.SetActive(false);
    }

    public void OnPlayButtonPressed()
    {
        if (AudioManager.Instance != null) AudioManager.Instance.Play(SoundType.ButtonClick);

        if (wipeController != null)
        {
            wipeController.PlayTransition(() =>
            {
                SetPanelInstant(menuPanel, false);
                SetPanelInstant(gameplayPanel, true);
            });
        }
        else
        {
            SetPanelInstant(menuPanel, false);
            SetPanelInstant(gameplayPanel, true);
        }
    }

    public void OnRestartButtonPressed()
    {
        if (AudioManager.Instance != null) AudioManager.Instance.Play(SoundType.ButtonClick);

        if (wipeController != null)
        {
            wipeController.PlayTransition(() =>
            {
                GameManager.Instance.ResetGameplay();
                SetPanelInstant(gameOverPanel, false);
            });
        }
        else
        {
            GameManager.Instance.ResetGameplay();
            SetPanelInstant(gameOverPanel, false);
        }
    }

    public void OnHomeButtonPressed()
    {
        if (AudioManager.Instance != null) AudioManager.Instance.Play(SoundType.ButtonClick);

        if (wipeController != null)
        {
            wipeController.PlayTransition(() =>
            {
                GameManager.Instance.ResetGameplay();
                SetPanelInstant(gameOverPanel, false);
                SetPanelInstant(gameplayPanel, false);
                SetPanelInstant(menuPanel, true);
            });
        }
        else
        {
            GameManager.Instance.ResetGameplay();
            SetPanelInstant(gameOverPanel, false);
            SetPanelInstant(gameplayPanel, false);
            SetPanelInstant(menuPanel, true);
        }
    }

    public void ShowGameOver()
    {
        Fade(gameOverPanel, true);
    }

    public void ShowSettings()
    {
        if (AudioManager.Instance != null) AudioManager.Instance.Play(SoundType.ButtonClick);
        Fade(settingsPanel, true);
    }

    public void ShowHelp()
    {
        if (AudioManager.Instance != null) AudioManager.Instance.Play(SoundType.ButtonClick);
        Fade(helpPanel, true);
    }

    public void OnLevelsSelectPressed()
    {
        if (AudioManager.Instance != null) AudioManager.Instance.Play(SoundType.ButtonClick);

        if (wipeController != null)
        {
            wipeController.PlayTransition(() =>
            {
                GameManager.Instance.ResetGameplay();
                SetPanelInstant(gameplayPanel, false);
                SetPanelInstant(gameOverPanel, false);
                SetPanelInstant(settingsPanel, false);
                SetPanelInstant(menuPanel, true);
            });
        }
        else
        {
            GameManager.Instance.ResetGameplay();
            SetPanelInstant(gameplayPanel, false);
            SetPanelInstant(gameOverPanel, false);
            SetPanelInstant(settingsPanel, false);
            SetPanelInstant(menuPanel, true);
        }
    }
}