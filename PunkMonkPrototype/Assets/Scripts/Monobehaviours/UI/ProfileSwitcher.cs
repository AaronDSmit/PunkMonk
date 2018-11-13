using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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
    private Sprite lightningUnitBasic;

    [SerializeField]
    private Sprite lightningUnitSpecial;

    [SerializeField]
    private Sprite selectedProfileEarth;

    [SerializeField]
    private Sprite selectedProfileLightning;






    #endregion

    #region Local Fields

    //private Vector2 primaryPos;

    //private Vector2 secondaryPos;
    private Sprite normalProfileEarth, normalProfileLightning;

    private RectTransform earthProfile;

    private RectTransform lightningProfile;

    private Unit currentUnit;


    private Button earthProfileButton;

    private Button lightningProfileButton;

    private bool earthSelected = true;

    private Sprite earthUnitBasic;

    private Sprite earthUnitSpecial;
    private Image earthImage, lightningImage;


    #endregion

    #region Properties

    public Transform CurrentProfile
    {
        get
        {
            if (earthSelected)
            {
                return earthProfile.transform;
            }
            else
            {
                return lightningProfile.transform;
            }
        }
    }

    public Unit CurrentUnit
    {
        get
        {
            return currentUnit;
        }
    }

    #endregion

    #region Public Methods

    public void UnitProfileSelected(bool earthSelected)
    {
        if(earthSelected)
        {
            earthImage.sprite = selectedProfileEarth;
            lightningImage.sprite = normalProfileLightning;
        }
        else
        {
            earthImage.sprite = normalProfileEarth;
            lightningImage.sprite = selectedProfileLightning;
        }
    }

    public void Switch(bool a_earthSelected)
    {
        earthSelected = a_earthSelected;

        if (earthSelected)
        {
            earthProfile.transform.SetAsLastSibling();

            currentUnit = Manager.instance.PlayerController.EarthUnit;

            //StartCoroutine(MoveProfile(earthProfile, secondaryPos, primaryPos));
            StartCoroutine(Scale(earthProfile, secondaryScale, primaryScale));

            // StartCoroutine(MoveProfile(lightningProfile, primaryPos, secondaryPos));
            StartCoroutine(Scale(lightningProfile, primaryScale, secondaryScale));


            earthImage.raycastTarget = false;
            lightningImage.raycastTarget = true;
            UnitProfileSelected(true);
        }
        else
        {
            lightningProfile.transform.SetAsLastSibling();

            currentUnit = Manager.instance.PlayerController.LightningUnit;


            //StartCoroutine(MoveProfile(lightningProfile, secondaryPos, primaryPos));
            StartCoroutine(Scale(lightningProfile, secondaryScale, primaryScale));

            //StartCoroutine(MoveProfile(earthProfile, primaryPos, secondaryPos));
            StartCoroutine(Scale(earthProfile, primaryScale, secondaryScale));


            earthImage.raycastTarget = true;
            lightningImage.raycastTarget = false;

            UnitProfileSelected(false);
        }
    }

    #endregion

    #region Unity Life-cycle Methods




    private void Awake()
    {
        earthProfile = transform.GetChild(1).GetComponent<RectTransform>();
        lightningProfile = transform.GetChild(2).GetComponent<RectTransform>();

        earthProfileButton = earthProfile.GetComponent<Button>();
        lightningProfileButton = lightningProfile.GetComponent<Button>();

        //primaryPos = earthProfile.anchoredPosition;
        //secondaryPos = lightningProfile.anchoredPosition;

        lightningProfile.localScale = secondaryScale;

        earthImage = earthProfileButton.GetComponent<Image>();
        lightningImage = lightningProfileButton.GetComponent<Image>();

        earthImage.raycastTarget = false;
        lightningImage.raycastTarget = true;

        earthProfileButton.interactable = true;
        lightningProfileButton.interactable = true;

        earthImage.raycastTarget = false;
        lightningImage.raycastTarget = true;

        earthUnitBasic = transform.parent.GetChild(1).GetChild(1).GetComponent<Image>().sprite;
        earthUnitSpecial = transform.parent.GetChild(5).GetChild(0).GetComponent<Image>().sprite;

    

        normalProfileEarth = earthImage.sprite;
        normalProfileLightning = lightningImage.sprite;

    }

    private void Start()
    {
        currentUnit = Manager.instance.PlayerController.EarthUnit;
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

    public void SwitchAbilityButtons()
    {
        if (earthSelected)
        {
            transform.parent.GetChild(1).GetChild(1).GetComponent<Image>().sprite = lightningUnitBasic;
            transform.parent.GetChild(5).GetChild(0).GetComponent<Image>().sprite = lightningUnitSpecial;
        }
        else
        {
            transform.parent.GetChild(1).GetChild(1).GetComponent<Image>().sprite = earthUnitBasic;
            transform.parent.GetChild(5).GetChild(0).GetComponent<Image>().sprite = earthUnitSpecial;
        }
    }

    #endregion
}