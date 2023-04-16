using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ElapsedTime : MonoBehaviour
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
        this.GetComponent<TMPro.TextMeshProUGUI>().text = "Time: " + (Time.time - _startTime).ToString("F2");
    }
}
