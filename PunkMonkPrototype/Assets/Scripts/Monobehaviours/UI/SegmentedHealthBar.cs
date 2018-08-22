using UnityEngine;
using UnityEngine.UI;

public class SegmentedHealthBar : MonoBehaviour
{
    [SerializeField] float borderWidth = 0.1f;

    private int maxHealth = 4;
    private int currentHealth = 4;

    private RectTransform bg = null;
    private RectTransform segments = null;
    private HorizontalLayoutGroup groupLayout = null;
    private GameObject mainHealthBar = null;

    public int MaxHealth { get { return maxHealth; } set { maxHealth = Mathf.Clamp(value, 1, 100); Refresh(); } }

    public int CurrentHealth { get { return currentHealth; } set { currentHealth = Mathf.Clamp(value, 0, maxHealth); Refresh(); } }

    private void Awake()
    {
        bg = transform.GetChild(0).GetComponent<RectTransform>();
        segments = transform.GetChild(1).GetComponent<RectTransform>();
        groupLayout = segments.GetComponent<HorizontalLayoutGroup>();
        mainHealthBar = segments.transform.GetChild(0).gameObject;
    }

    private void Refresh()
    {
        // First insure there is the correct amount of health bar segments
        if (bg.childCount > maxHealth)
        {
            // Add health bars
            for (int i = bg.childCount; i < maxHealth; ++i)
            {
                Instantiate(mainHealthBar);
            }
        }
        else if (bg.childCount < maxHealth)
        {
            // Remove unnecessary health segments
            for (int i = bg.childCount; i < maxHealth; --i)
            {
                Destroy(bg.GetChild(i));
            }
        }

        // Set the position and scale of each segment using the Horizontal Layout Group

        groupLayout.spacing = borderWidth / 2.0f;
        Vector3 newScale = bg.localScale;
        newScale.x -= borderWidth;
        newScale.y -= borderWidth;
        segments.localScale = newScale;

    }
}
