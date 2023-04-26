using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;
using System;

public class LoadingScreenController : MonoBehaviour
{
    public GameObject progressBar;

    public void UpdateProgress(float progress)
    {
        progressBar.GetComponent<Slider>().value = progress;
    }
}