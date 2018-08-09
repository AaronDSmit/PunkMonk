using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FadingUI : MonoBehaviour
{
    [SerializeField] private float fadeTime;

    private Image[] images;

    private Text[] texts;

    private Button[] buttons;

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

    public bool IsVisible { get; private set; }

    public void FadeOut()
    {
        StartCoroutine(AnimateFade(true));
    }

    public void FadeIn()
    {
        StartCoroutine(AnimateFade(false));
    }

    private IEnumerator AnimateFade(bool a_fadeOut)
    {
        foreach (Button button in buttons)
        {
            button.interactable = false;
        }

        float currentLerpTime = 0;
        float t = 0;

        float from = (a_fadeOut) ? 1 : 0;
        float to = (a_fadeOut) ? 0 : 1;

        while (t < 1)
        {
            currentLerpTime += Time.deltaTime;
            t = currentLerpTime / fadeTime;

            foreach(Image image in images)
            {
                image.color = new Color(image.color.r, image.color.g, image.color.b, Mathf.Lerp(from, to, t));
            }

            foreach (Text text in texts)
            {
                text.color = new Color(text.color.r, text.color.g, text.color.b, Mathf.Lerp(from, to, t));
            }

            yield return null;
        }

        foreach (Button button in buttons)
        {
            button.interactable = true;
        }

        IsVisible = !a_fadeOut;
    }
}