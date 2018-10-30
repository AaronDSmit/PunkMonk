using UnityEngine;
using UnityEngine.UI;

public class SegmentedHealthBar : MonoBehaviour
{
    [SerializeField] private float borderWidth = 0.1f;
    [SerializeField] private Material healthMat = null;
    [SerializeField] private Material missingHealthMat = null;
    [SerializeField] private Material previewDamageHealthMat = null;
    [SerializeField] private bool automateLayoutGroup = false;

    private int maxHealth = 4;
    private int currentHealth = 4;

    private RectTransform bg = null;
    private RectTransform segments = null;
    private HorizontalLayoutGroup groupLayout = null;
    private GameObject mainHealthBar = null;
    private FadingUI fade;

    public int MaxHealth { get { return maxHealth; } set { maxHealth = Mathf.Clamp(value, 1, 100); Refresh(); } }

    public int CurrentHealth { get { return currentHealth; } set { currentHealth = Mathf.Clamp(value, 0, maxHealth); Refresh(); } }

    private void Awake()
    {
        bg = transform.GetChild(0).GetComponent<RectTransform>();
        segments = transform.GetChild(1).GetComponent<RectTransform>();
        groupLayout = segments.GetComponent<HorizontalLayoutGroup>();
        mainHealthBar = segments.transform.GetChild(0).gameObject;
        fade = transform.parent.GetComponent<FadingUI>();
    }

    public void Show()
    {
        if (Manager.instance.StateController.CurrentGameState == GameState.battle)
        {
            fade.FadeIn();
        }
    }

    public void Hide()
    {
        fade.FadeOut();
    }

    // returns whether or not the unit will die if they receive that amount of damage
    public bool PreviewDamage(int a_damage)
    {
        for (int i = 0; i < currentHealth; i++)
        {
            if (i < (currentHealth - a_damage))
            {
                //segments.GetChild(i).GetComponent<Material>().color = currentHealth;
                segments.GetChild(i).GetComponent<Image>().material = healthMat;
            }
            else
            {
                //segments.GetChild(i).GetComponent<Material>().color = previewDamageHealthColour;
                //Color segColour = segments.GetChild(i).GetComponent<Material>().GetColor("_Colour");
                segments.GetChild(i).GetComponent<Image>().material = previewDamageHealthMat;
            }
        }

        return (currentHealth - a_damage <= 0);
    }

    private void Refresh()
    {
        if (automateLayoutGroup)
        {

            // First insure there is the correct amount of health bar segments
            if (segments.childCount < maxHealth)
            {
                // Add health bars
                for (int i = segments.childCount; i < maxHealth; ++i)
                {
                    Instantiate(mainHealthBar, segments);
                }
            }
            else if (segments.childCount > maxHealth)
            {
                // Remove unnecessary health segments
                for (int i = segments.childCount - 1; i >= maxHealth; --i)
                {
                    Destroy(segments.GetChild(i).gameObject);
                }
            }

            // Set the segments invisible for the missing health
            for (int i = 0; i < maxHealth; i++)
            {
                //segments.GetChild(i).GetComponent<Image>().enabled = (i < currentHealth);
                Material newMat = null;
                if (i < currentHealth)
                {
                    newMat = healthMat;
                }
                else
                {
                    newMat = missingHealthMat;
                }
                segments.GetChild(i).GetComponent<Image>().material = newMat;
            }

            // Set the position and scale of each segment using the Horizontal Layout Group
            groupLayout.spacing = borderWidth / 2.0f;
            Vector3 newSize = bg.sizeDelta;
            newSize.x -= borderWidth;
            newSize.y -= borderWidth;
            segments.sizeDelta = newSize;

        }

        if (Manager.instance.StateController.CurrentGameState == GameState.battle)
        {
            fade.UpdateReferences(false);
        }
        else
        {
            fade.UpdateReferences(true);
        }
    }
}