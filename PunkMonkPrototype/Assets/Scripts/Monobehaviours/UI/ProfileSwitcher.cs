﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This script is used to animate the movement of the profile picture of the currently selected unit
/// </summary>

public class ProfileSwitcher : MonoBehaviour
{
    #region Unity Inspector Fields

    [SerializeField]
    private float animationTime = 1.0f;

    [SerializeField]
    private Vector2 primaryScale = Vector2.one;

    [SerializeField]
    private Vector2 secondaryScale = Vector2.one;

    [SerializeField]
    private RectTransform earthProfile;

    [SerializeField]
    private RectTransform lightningProfile;

    #endregion

    #region Local Fields

    private Vector2 primaryPos;

    private Vector2 secondaryPos;

    #endregion

    #region Public Methods

    public void Switch(bool a_earthSelected)
    {
        if (a_earthSelected)
        {
            earthProfile.transform.SetAsLastSibling();

            StartCoroutine(MoveProfile(earthProfile, secondaryPos, primaryPos));
            StartCoroutine(Scale(earthProfile, secondaryScale, primaryScale));

            StartCoroutine(MoveProfile(lightningProfile, primaryPos, secondaryPos));
            StartCoroutine(Scale(lightningProfile, primaryScale, secondaryScale));
        }
        else
        {
            lightningProfile.transform.SetAsLastSibling();

            StartCoroutine(MoveProfile(lightningProfile, secondaryPos, primaryPos));
            StartCoroutine(Scale(lightningProfile, secondaryScale, primaryScale));

            StartCoroutine(MoveProfile(earthProfile, primaryPos, secondaryPos));
            StartCoroutine(Scale(earthProfile, primaryScale, secondaryScale));
        }
    }

    #endregion

    #region Unity Life-cycle Methods

    private void Awake()
    {
        earthProfile = transform.GetChild(0).GetComponent<RectTransform>();
        lightningProfile = transform.GetChild(1).GetComponent<RectTransform>();

        primaryPos = earthProfile.anchoredPosition;
        secondaryPos = lightningProfile.anchoredPosition;

        lightningProfile.localScale = secondaryScale;
    }

    #endregion

    #region Local Methods

    private IEnumerator MoveProfile(RectTransform a_rect, Vector2 a_from, Vector2 a_target)
    {
        float currentLerpTime = 0;
        float t = 0;

        while (t < 1)
        {
            currentLerpTime += Time.deltaTime;
            t = currentLerpTime / animationTime;
            t = t * t;

            a_rect.anchoredPosition = Vector3.Lerp(a_from, a_target, t);

            yield return null;
        }
    }

    private IEnumerator Scale(RectTransform a_rect, Vector2 a_from, Vector2 a_target)
    {
        float currentLerpTime = 0;
        float t = 0;

        while (t < 1)
        {
            currentLerpTime += Time.deltaTime;
            t = currentLerpTime / animationTime;
            t = t * t;

            a_rect.localScale = Vector3.Lerp(a_from, a_target, t);

            yield return null;
        }
    }

    #endregion
}