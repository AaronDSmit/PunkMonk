using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SlidingUI : MonoBehaviour
{
    [SerializeField]
    private Vector2 startLocation;

    [SerializeField]
    private Vector2 targetLocation;

    [SerializeField]
    private bool overShootTarget;

    [SerializeField]
    private bool overShootOnReturn;

    [SerializeField]
    private bool disableButtonsDuringAnim;

    [SerializeField]
    private Vector2 overShootAmount;

    [SerializeField]
    private float animationTime;

    private RectTransform rect;

    private Button[] buttons;

    private bool[] buttonInitialState;

    private bool toggled = false;

    private void Awake()
    {
        rect = GetComponent<RectTransform>();

        buttons = GetComponentsInChildren<Button>();

        buttonInitialState = new bool[buttons.Length];

        for (int i = 0; i < buttons.Length; i++)
        {
            buttonInitialState[i] = buttons[i].interactable;
        }

        rect.anchoredPosition = startLocation;
    }

    public void Toggle()
    {
        if (!toggled)
        {
            if (overShootTarget)
            {
                StartCoroutine(MoveToTargetLocationWithOverShoot(startLocation, targetLocation, targetLocation + overShootAmount));
            }
            else
            {
                StartCoroutine(MoveToTargetLocation(startLocation, targetLocation));
            }
        }
        else
        {
            if (overShootOnReturn)
            {
                StartCoroutine(MoveToBackWithOverShoot(targetLocation, startLocation, targetLocation + overShootAmount));
            }
            else
            {
                StartCoroutine(MoveToBack(targetLocation, startLocation));
            }
        }
    }

    public bool Toggled
    {
        get { return toggled; }
    }

    private IEnumerator MoveToTargetLocation(Vector2 from, Vector2 target)
    {
        if (disableButtonsDuringAnim && buttons.Length > 0)
        {
            for (int i = 0; i < buttons.Length; i++)
            {
                buttons[i].interactable = false;
            }
        }

        float currentLerpTime = 0;
        float t = 0;

        while (t < 1)
        {
            currentLerpTime += Time.deltaTime;
            t = currentLerpTime / animationTime;
            t = t * t;

            rect.anchoredPosition = Vector3.Lerp(from, target, t);

            yield return null;
        }

        if (disableButtonsDuringAnim && buttons.Length > 0)
        {
            for (int i = 0; i < buttons.Length; i++)
            {
                buttons[i].interactable = buttonInitialState[i];
            }
        }

        toggled = true;
    }

    private IEnumerator MoveToTargetLocationWithOverShoot(Vector2 from, Vector2 target, Vector2 overshoot)
    {
        if (disableButtonsDuringAnim && buttons.Length > 0)
        {
            for (int i = 0; i < buttons.Length; i++)
            {
                buttons[i].interactable = false;
            }
        }

        float currentLerpTime = 0;
        float t = 0;

        while (t < 1)
        {
            currentLerpTime += Time.deltaTime;
            t = currentLerpTime / (animationTime * 0.7f);
            t = t * t;

            rect.anchoredPosition = Vector3.Lerp(from, overshoot, t);

            yield return null;
        }

        currentLerpTime = 0;
        t = 0;

        while (t < 1)
        {
            currentLerpTime += Time.deltaTime;
            t = currentLerpTime / (animationTime * 0.3f);

            rect.anchoredPosition = Vector3.Lerp(overshoot, target, t);

            yield return null;
        }

        if (disableButtonsDuringAnim && buttons.Length > 0)
        {
            for (int i = 0; i < buttons.Length; i++)
            {
                buttons[i].interactable = buttonInitialState[i];
            }
        }

        toggled = true;
    }

    private IEnumerator MoveToBack(Vector2 from, Vector2 target)
    {
        if (disableButtonsDuringAnim && buttons.Length > 0)
        {
            for (int i = 0; i < buttons.Length; i++)
            {
                buttons[i].interactable = false;
            }
        }

        float currentLerpTime = 0;
        float t = 0;

        while (t < 1)
        {
            currentLerpTime += Time.deltaTime;
            t = currentLerpTime / animationTime;
            t = t * t;

            rect.anchoredPosition = Vector3.Lerp(from, target, t);

            yield return null;
        }

        if (disableButtonsDuringAnim && buttons.Length > 0)
        {
            for (int i = 0; i < buttons.Length; i++)
            {
                buttons[i].interactable = buttonInitialState[i];
            }
        }

        toggled = false;
    }

    private IEnumerator MoveToBackWithOverShoot(Vector2 from, Vector2 target, Vector2 overshoot)
    {
        if (disableButtonsDuringAnim && buttons.Length > 0)
        {
            for (int i = 0; i < buttons.Length; i++)
            {
                buttons[i].interactable = false;
            }
        }

        float currentLerpTime = 0;
        float t = 0;

        while (t < 1)
        {
            currentLerpTime += Time.deltaTime;
            t = currentLerpTime / (animationTime * 0.3f);
            t = 1f - Mathf.Cos(t * Mathf.PI * 0.5f);

            rect.anchoredPosition = Vector3.Lerp(from, overshoot, t);

            yield return null;
        }

        currentLerpTime = 0;
        t = 0;

        while (t < 1)
        {
            currentLerpTime += Time.deltaTime;
            t = currentLerpTime / (animationTime * 0.7f);
            t = t * t;

            rect.anchoredPosition = Vector3.Lerp(overshoot, target, t);

            yield return null;
        }

        if (disableButtonsDuringAnim && buttons.Length > 0)
        {
            for (int i = 0; i < buttons.Length; i++)
            {
                buttons[i].interactable = buttonInitialState[i];
            }
        }

        toggled = false;
    }
}