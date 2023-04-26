using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;
using System;

public class LoadingScreenController : MonoBehaviour
{
    public Slider progressBar;

    public void UpdateProgress(float progress)
    {
        progressBar.value = progress;
    }
}