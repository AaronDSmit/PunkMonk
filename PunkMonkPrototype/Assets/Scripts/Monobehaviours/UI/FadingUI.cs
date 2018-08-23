using System.Collections;
using UnityEngine;
using UnityEngine.UI;

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

    private Text[] texts;

    private Button[] buttons;

    #endregion

    #region Local Fields

    private bool[] buttonInitialState;

    #endregion

    #region Properties

    public bool IsVisible { get; private set; }

    #endregion

    #region Public Methods

    public void FadeOut()
    {
        StartCoroutine(AnimateFade(true));
    }

    public void FadeIn()
    {
        StartCoroutine(AnimateFade(false));
    }

    #endregion

    #region Unity Life-cycle Methods

    private void Awake()
    {
        buttons = GetComponentsInChildren<Button>();

        images = GetComponentsInChildren<Image>();

        texts = GetComponentsInChildren<Text>();

        foreach (Image image in images)
        {
            image.color = new Color(image.color.r, image.color.g, image.color.b, 0.0f);
        }

        foreach (Text text in texts)
        {
            text.color = new Color(text.color.r, text.color.g, text.color.b, 0.0f);
        }
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
                // Save the previous button state before setting it to false
                buttonInitialState[i] = buttons[i].interactable;
                buttons[i].interactable = false;
            }
        }

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
                image.color = new Color(image.color.r, image.color.g, image.color.b, Mathf.Lerp(from, to, t));
            }

            foreach (Text text in texts)
            {
                text.color = new Color(text.color.r, text.color.g, text.color.b, Mathf.Lerp(from, to, t));
            }

            yield return null;
        }

        if (disableButtons)
        {
            for (int i = 0; i < buttons.Length; i++)
            {
                buttons[i].interactable = buttonInitialState[i];
            }
        }

        IsVisible = !a_fadeOut;
    }

    #endregion
}