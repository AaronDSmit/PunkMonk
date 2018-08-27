using UnityEngine;
using UnityEngine.UI;

public class SegmentedHealthBar : MonoBehaviour
{
    [SerializeField] float borderWidth = 0.1f;
    [SerializeField] Color healthColour = Color.green;
    [SerializeField] Color missingHealthColour = new Color(0, 0, 0, 0);

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
        fade = GetComponent<FadingUI>();
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

    private void Refresh()
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
            Color newColour = Color.black;
            if (i < currentHealth)
            {
                newColour = healthColour;
            }
            else
            {
                newColour = missingHealthColour;
            }
            segments.GetChild(i).GetComponent<Image>().color = newColour;
        }

        // Set the position and scale of each segment using the Horizontal Layout Group
        groupLayout.spacing = borderWidth / 2.0f;
        Vector3 newSize = bg.sizeDelta;
        newSize.x -= borderWidth;
        newSize.y -= borderWidth;
        segments.sizeDelta = newSize;

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