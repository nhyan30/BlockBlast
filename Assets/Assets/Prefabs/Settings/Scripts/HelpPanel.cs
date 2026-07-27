using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Controls the Help panel that shows "How To Play" instructions.
/// </summary>
public class HelpPanel : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private CanvasGroup helpPanel;
    [SerializeField] private Button cancelButton;

    [Header("Help Text")]
    [SerializeField] private TMP_Text helpText;

    [TextArea(10, 20)]
    [SerializeField] private string howToPlayText = @"HOW TO PLAY

OBJECTIVE:
Place blocks on the 8x8 grid to clear rows and columns for points!

GAMEPLAY:

1. PLACING BLOCKS
   - Drag and drop the available blocks onto the grid.
   - Blocks cannot be rotated, so plan carefully!
   - You must place all three blocks to get a new set.

2. CLEARING LINES
   - Fill an entire row or column to clear it.
   - Clear multiple lines at once for bonus points!

3. GAME OVER
   - The game ends when there is no space left on the board for the available blocks.

TIPS:
   - Think ahead before placing blocks.
   - Keep the middle of the board clear.
   - Avoid leaving tiny gaps that are hard to fill.
   - Try to clear multiple lines at once for a higher score!";

    private void Awake()
    {
        SetupButtonListeners();
    }

    private void Start()
    {
        if (helpText != null && !string.IsNullOrEmpty(howToPlayText))
        {
            helpText.text = howToPlayText;
        }
        Hide();
    }

    private void SetupButtonListeners()
    {
        if (cancelButton != null)
        {
            cancelButton.onClick.AddListener(OnCancelClicked);
        }
    }

    private void OnCancelClicked()
    {
        if (AudioManager.Instance != null) AudioManager.Instance.Play(SoundType.ButtonClick);
        Hide();
    }

    public void Show()
    {
        if (helpPanel != null)
        {
            UIManager.Instance.Fade(helpPanel, true);
        }
    }

    public void Hide()
    {
        if (helpPanel != null)
        {
            UIManager.Instance.Fade(helpPanel, false);
        }
    }
}