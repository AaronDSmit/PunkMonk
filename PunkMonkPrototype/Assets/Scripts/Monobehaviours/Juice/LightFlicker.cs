using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Light))]
public class LightFlicker : MonoBehaviour
{
    [SerializeField] private float maxReduction;
    [SerializeField] private float maxIncrease;
    [SerializeField] private float rateDamping;
    [SerializeField] private float strength;
    [SerializeField] private float minFlickerTime;
    [SerializeField] private float maxFlickerTime;
    [SerializeField] private float minWaitTime;
    [SerializeField] private float maxWaitTime;
    [SerializeField] private bool stopFlickering;

    private new Light light;
    private float baseIntensity;
    private bool flickering;

    private bool waiting = false;

    public void Reset()
    {
        maxReduction = 0.2f;
        maxIncrease = 0.2f;
        rateDamping = 0.1f;
        strength = 20;
        minFlickerTime = 0.3f;
        maxFlickerTime = 1.0f;
        minWaitTime = 2.0f;
        maxWaitTime = 4.0f;
    }

    public void Start()
    {
        light = GetComponent<Light>();
        baseIntensity = light.intensity;
        StartCoroutine(DoFlicker());
    }

    void Update()
    {
        if (!stopFlickering && !flickering)
        {
            StartCoroutine(DoFlicker());
        }
    }

    private IEnumerator DoFlicker()
    {
        float flickerTimer = 0;
        flickering = true;
        while (!stopFlickering)
        {
            if (waiting)
            {
                light.intensity = baseIntensity;
                yield return new WaitForSeconds(Random.Range(minWaitTime, maxWaitTime));
                waiting = false;
                flickerTimer = Random.Range(minFlickerTime, maxFlickerTime);
            }
            else
            {
                if (flickerTimer > 0)
                    flickerTimer -= Time.deltaTime;
                else
                    waiting = true;

                light.intensity = Mathf.Lerp(light.intensity, Random.Range(baseIntensity - maxReduction, baseIntensity + maxIncrease), strength * Time.deltaTime);

                yield return new WaitForSeconds(rateDamping);

            }
        }
        flickering = false;
    }
}