using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProfileSwitcher : MonoBehaviour
{
    private RectTransform earthProfile;

    private RectTransform lightningProfile;

    private Vector2 primaryPos;

    private Vector2 secondaryPos;

    private bool earthSelected = true;

    private void Awake()
    {
        earthProfile = transform.GetChild(0).GetComponent<RectTransform>();
        lightningProfile = transform.GetChild(1).GetComponent<RectTransform>();

        primaryPos = earthProfile.anchoredPosition;
        secondaryPos = lightningProfile.anchoredPosition;
    }

    public void Switch()
    {
        earthSelected = !earthSelected;

        if (earthSelected)
        {
            earthProfile.anchoredPosition = primaryPos;
            lightningProfile.anchoredPosition = secondaryPos;
        }
        else
        {
            earthProfile.anchoredPosition = secondaryPos;
            lightningProfile.anchoredPosition = primaryPos;
        }
    }
}