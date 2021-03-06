﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;
using UnityEngine.UI;

public class EndScene : MonoBehaviour
{

    [SerializeField]
    private Image fadeImage = null;

    [SerializeField]
    private float fadeInTime = 1f;

    [SerializeField]
    private float fadeOutTime = 1f;

    private bool fading = false;

    [SerializeField]
    private AK.Wwise.Event stopMusicEvent = null;

    private void Awake()
    {
        if (GameObject.FindGameObjectWithTag("Manager") != null)
        {
            Destroy(GameObject.FindGameObjectWithTag("Manager"));
        }
    }

    private void Start()
    {
        fadeImage.gameObject.SetActive(true);
        fadeImage.enabled = true;
        StartCoroutine(Fade(Color.black, Color.clear, fadeInTime, fadeImage));
        StartCoroutine(DoFadePlaneRaycast(fadeInTime));
    }

    public void Quit()
    {
        stopMusicEvent.Post(gameObject);
        fadeImage.raycastTarget = true;
        StartCoroutine(Fade(Color.clear, Color.black, fadeOutTime, fadeImage, Application.Quit));

    }

    public void PlayAgain()
    {
        stopMusicEvent.Post(gameObject);
        fadeImage.raycastTarget = true;
        StartCoroutine(ChangeSceneAfterFade(1));
    }

    public void MainMenu()
    {
        stopMusicEvent.Post(gameObject);
        fadeImage.raycastTarget = true;
        StartCoroutine(ChangeSceneAfterFade(0));
    }

    private IEnumerator ChangeSceneAfterFade(int a_sceneIndex)
    {
        StartCoroutine(Fade(Color.clear, Color.black, fadeOutTime, fadeImage));

        yield return new WaitUntil(() => fading == false);

        SceneManager.LoadScene(a_sceneIndex);
    }


    private IEnumerator Fade(Color a_from, Color a_to, float a_time, Image a_plane, System.Action a_finishedAction = null)
    {
        fading = true;

        float speed = 1 / a_time;
        float percent = 0;

        while (percent < 1)
        {
            percent += Time.deltaTime * speed;
            a_plane.color = Color.Lerp(a_from, a_to, percent);

            yield return null;
        }

        if (a_finishedAction != null)
            a_finishedAction();

        fading = false;
    }

    private IEnumerator DoFadePlaneRaycast(float a_time)
    {
        fadeImage.raycastTarget = true;

        yield return new WaitForSeconds(a_time);

        fadeImage.raycastTarget = false;

    }
}