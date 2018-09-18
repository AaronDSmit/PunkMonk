using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// This script is used to fade in or out any sprite or text UI. Can disable buttons during fade animation.
/// </summary>

public class FadingUI : MonoBehaviour
{
    #region Unity Inspector Fields

    [SerializeField]
    private float fadeTime = 1.0f;

    [SerializeField]
    private bool disableButtons = true;

    #endregion

    #region Reference Fields

    private Image[] images;

    private TextMeshProUGUI[] texts;

    private Button[] buttons;

    #endregion

    #region Local Fields

    private bool[] buttonInitialState;

    private System.Action finishedFading;

    #endregion

    #region Properties

    public bool IsVisible { get; private set; }

    public bool IsFading { get; private set; }

    #endregion

    #region Public Methods

    public void FadeOut(System.Action a_finishedFading = null)
    {
        finishedFading = a_finishedFading;
        StartCoroutine(AnimateFade(true));
    }

    public void FadeIn(System.Action a_finishedFading = null)
    {
        finishedFading = a_finishedFading;
        StartCoroutine(AnimateFade(false));
    }

    public void ShowInstant()
    {
        foreach (Image image in images)
        {
            image.color = new Color(image.color.r, image.color.g, image.color.b, 1.0f);
        }

        foreach (TextMeshProUGUI text in texts)
        {
            text.color = new Color(text.color.r, text.color.g, text.color.b, 1.0f);
        }
    }

    public void HideInstant()
    {
        foreach (Image image in images)
        {
            image.color = new Color(image.color.r, image.color.g, image.color.b, 0.0f);
        }

        foreach (TextMeshProUGUI text in texts)
        {
            text.color = new Color(text.color.r, text.color.g, text.color.b, 0.0f);
        }
    }

    public void UpdateReferences(bool a_hide)
    {
        buttons = GetComponentsInChildren<Button>();

        images = GetComponentsInChildren<Image>();

        texts = GetComponentsInChildren<TextMeshProUGUI>();

        foreach (Image image in images)
        {
            image.color = new Color(image.color.r, image.color.g, image.color.b, (a_hide) ? 0.0f : 1.0f);
        }

        foreach (TextMeshProUGUI text in texts)
        {
            text.color = new Color(text.color.r, text.color.g, text.color.b, (a_hide) ? 0.0f : 1.0f);
        }
    }

    public void SetButtonsInteractable()
    {
        foreach (Button button in buttons)
        {
            button.interactable = true;
        }
    }

    #endregion

    #region Unity Life-cycle Methods

    private void Awake()
    {
        UpdateReferences(true);
    }

    #endregion

    #region Local Methods

    private IEnumerator AnimateFade(bool a_fadeOut)
    {
        if (disableButtons)
        {
            buttonInitialState = new bool[buttons.Length];

            for (int i = 0; i < buttons.Length; i++)
            {
                if (buttons[i])
                {
                    // Save the previous button state before setting it to false
                    buttonInitialState[i] = buttons[i].interactable;
                    buttons[i].interactable = false;
                }
            }
        }

        IsFading = true;

        float currentLerpTime = 0;
        float t = 0;

        float from = (a_fadeOut) ? 1 : 0;
        float to = (a_fadeOut) ? 0 : 1;

        while (t < 1)
        {
            currentLerpTime += Time.deltaTime;
            t = currentLerpTime / fadeTime;

            foreach (Image image in images)
            {
                if (image)
                {
                    image.color = new Color(image.color.r, image.color.g, image.color.b, Mathf.Lerp(from, to, t));
                }
            }

            foreach (TextMeshProUGUI text in texts)
            {
                if (text)
                {
                    text.color = new Color(text.color.r, text.color.g, text.color.b, Mathf.Lerp(from, to, t));
                }
            }

            yield return null;
        }

        if (disableButtons)
        {
            for (int i = 0; i < buttons.Length; i++)
            {
                if (buttons[i])
                {
                    buttons[i].interactable = buttonInitialState[i];
                }
            }
        }

        IsVisible = !a_fadeOut;
        IsFading = false;

        if (finishedFading != null)
        {
            finishedFading();
        }
    }



    #endregion
}