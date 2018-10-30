using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class SetMaterialColorToImage : MonoBehaviour
{
    private Image image = null;

    private void OnEnable()
    {
        image = GetComponent<Image>();
    }

    private void LateUpdate()
    {
        Color color = image.material.GetColor("_Colour");
        color.a = image.color.a;
        image.material.SetColor("_Colour", color);
    }

}
