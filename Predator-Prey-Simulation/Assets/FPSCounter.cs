using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FPSCounter : MonoBehaviour
{
    private float _startTime;
    // Start is called before the first frame update
    void Start()
    {
        _startTime = Time.time;
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Time.time - _startTime > 0.3f)
        {
            _startTime = Time.time;
            this.GetComponent<TMPro.TextMeshProUGUI>().text = "FPS: " + (1.0f / Time.deltaTime).ToString("0");
        }
    }
}
