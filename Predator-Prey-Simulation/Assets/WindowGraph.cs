using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;
using UnityEngine.UI;

public class WindowGraph : MonoBehaviour
{
    [SerializeField] private Sprite _dotSprite;

    private RectTransform _graphContainerPrey;
    private RectTransform _graphContainerPredator;
    private LineRenderer _lineRendererPrey;
    private LineRenderer _lineRendererPredator;
    private GameObject canvas;
    private CircularBuffer<float> preyBuffer;
    private CircularBuffer<float> predatorBuffer;
    private float timer;
    // Start is called before the first frame update
    void Awake()
    {
        _graphContainerPrey = transform.Find("PreyGraphContainer").GetComponent<RectTransform>();
        _graphContainerPredator = transform.Find("PredatorGraphContainer").GetComponent<RectTransform>();
        //_lineRenderer = transform.Find("PreyGraphContainer").gameObject.AddComponent<LineRenderer>();
        _lineRendererPrey = transform.Find("PreyGraphContainer").GetComponent<LineRenderer>();
        _lineRendererPredator = transform.Find("PredatorGraphContainer").GetComponent<LineRenderer>();
        _lineRendererPrey.useWorldSpace = false;
        _lineRendererPredator.useWorldSpace = false;
        // _lineRenderer.startColor = new Color(1,1,1,0f);
        // _lineRenderer.endColor = new Color(1,1,1,0f);
        _lineRendererPrey.startWidth = 0.1f;
        _lineRendererPrey.endWidth = 0.1f;
        _lineRendererPredator.startWidth = 0.1f;
        _lineRendererPredator.endWidth = 0.1f;
        //_lineRenderer.material.color = new Color(1,1,1,0f);
        //_lineRenderer.loop = true;
        //_lineRenderer.GetComponent<Renderer>().material.SetColor("_TintColor", new Color(1, 1, 1, 0.0f));
        canvas = gameObject.transform.parent.gameObject;
        _lineRendererPrey.transform.SetParent(transform.Find("PreyGraphContainer"), false);
        _lineRendererPredator.transform.SetParent(transform.Find("PredatorGraphContainer"), false);
        float[] valueList = new float[100];
        for (int i = 0; i < 100; i++)
        {
            valueList[i] = 0;
        }
        preyBuffer = new CircularBuffer<float>(valueList);
        predatorBuffer = new CircularBuffer<float>(valueList);
        timer = Time.time;
        //GetComponent<SpriteRenderer>().sprite = CreateSprite(valueList);
        //ShowGraph(valueList);
        // SpriteShapeController spriteShapeController = gameObject.AddComponent<SpriteShapeController>();
        // Vector3[] points = new Vector3[4];
        // points[0] = new Vector3(-1.0f, -1.0f, 0.0f);
        // points[1] = new Vector3(1.0f, -1.0f, 0.0f);
        // points[2] = new Vector3(1.0f, 1.0f, 0.0f);
        // points[3] = new Vector3(-1.0f, 1.0f, 0.0f);
    }

    // Update is called once per frame
    void Update()
    {
        if (Time.time - timer > 0.3)
        {
            timer = Time.time;
            float numberOfPreys = GameObject.FindGameObjectsWithTag("Prey").Length;
            float numberOfPredators = GameObject.FindGameObjectsWithTag("Predator").Length;
            preyBuffer.Add(numberOfPreys);
            predatorBuffer.Add(numberOfPredators);
            var valuesInBufferPrey = preyBuffer.Cycle();
            var valuesInBufferPredator = predatorBuffer.Cycle();
            List<float> tempPrey = new List<float>();
            List<float> tempPredator = new List<float>();
            foreach (float value in valuesInBufferPrey)
                tempPrey.Add(value);
            foreach (float value in valuesInBufferPredator)
                tempPredator.Add(value);

            // print("result:");
            // string resultString = "";
            // foreach (float value in result)
            //     resultString += value + ", ";
            // print(resultString);
            ShowGraph(tempPrey.ToArray(), _graphContainerPrey, _lineRendererPrey, true);
            ShowGraph(tempPredator.ToArray(), _graphContainerPredator, _lineRendererPredator, false);
        }
        // _lineRenderer.positionCount = 0;
        // //float canvasAlpha = canvas.GetComponent<CanvasGroup>().alpha;
        // print("canvasAlpha: " + canvasAlpha);

        //float numberOfPreys = GameObject.FindGameObjectsWithTag("Prey").Length;
        //preyBuffer.Enqueue(numberOfPreys);
        //List<float> valueList = new List<float>() { 0,Random.Range(2,90),9,8,7,6, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10,0 };
        //ShowGraph(preyBuffer.Cycle(-1));
    }

    // private Sprite CreateSprite(List<float> valueList) 
    // {
    //     Texture2D spriteTexture = new Texture2D(100, 100);
    //     //foreach (Vector2 point in new List<Vector2>() { new Vector2(0, 0), new Vector2(0, 100), new Vector2(100, 0), new Vector2(100, 100) })
    //     foreach (Vector2 point in new List<Vector2>() { new Vector2(0, 0), new Vector2(100, 100), new Vector2(100, 0) })
    //     {
    //         spriteTexture.SetPixel((int)point.x, (int)point.y, Color.red);
    //     }
    //     spriteTexture.Apply();
    //     Sprite sprite = Sprite.Create(spriteTexture, new Rect(0, 0, 100, 100), new Vector2(0.5f, 0.5f), 1);
    //     return sprite;

    // }

    // private GameObject CreateDot(Vector2 anchoredPosition) {
    //     GameObject gameObject = new GameObject("dot", typeof(Image));
    //     gameObject.transform.SetParent(_graphContainer, false);
    //     gameObject.GetComponent<Image>().sprite = _dotSprite;
    //     RectTransform rectTransform = gameObject.GetComponent<RectTransform>();
    //     rectTransform.anchoredPosition = anchoredPosition;
    //     rectTransform.sizeDelta = new Vector2(2, 2);
    //     rectTransform.anchorMin = new Vector2(0, 0);
    //     rectTransform.anchorMax = new Vector2(0, 0);
    //     return gameObject;
    // }

    private void ShowGraph(float[] valueList, RectTransform graphContainer, LineRenderer lineRenderer, bool prey)
    {
        float xSize = graphContainer.sizeDelta.x / (valueList.Length + 1);
        float yMax = prey == true ? Prey.MaxPrey + 5 : Predator.MaxPredator + 5;
        float graphHeight = graphContainer.sizeDelta.y;

        //GameObject lastDot = null;
        lineRenderer.positionCount = valueList.Length;
        float canvasAlpha = canvas.GetComponent<CanvasGroup>().alpha;
        if (canvasAlpha == 0)
        {
            //print("canvasAlpha == 0");
            lineRenderer.startColor = new Color(1, 1, 1, 0.0f);
            lineRenderer.endColor = new Color(1, 1, 1, 0.0f);
            //_lineRenderer.material.color =  new Color(1,1,1,0.5f);
            //_lineRenderer.GetComponent<Renderer>().material.SetColor("_TintColor", new Color(1, 1, 1, 0.0f));
            //_lineRenderer.GetComponent<Material>().color = new Color(1, 1, 1, 0.0f);
            //_lineRenderer.material.color = new Color(1, 1, 1, 0.0f);


            //_lineRenderer.sharedMaterial.color = new Color(1,1,1,0f);

        }
        else
        {
            if (prey)
            {
                lineRenderer.startColor = new Color(0, 1, 0, canvasAlpha);
                lineRenderer.endColor = new Color(0, 1, 0, canvasAlpha);
            }
            else
            {
                lineRenderer.startColor = new Color(1, 0, 0, canvasAlpha);
                lineRenderer.endColor = new Color(1, 0, 0, canvasAlpha);
            }
            //_lineRenderer.material.color = new Color(1,1,1,canvasAlpha);
            //_lineRenderer.GetComponent<Renderer>().material.SetColor("_TintColor", new Color(1, 1, 1, 1.0f));
            //_lineRenderer.GetComponent<Material>().color = new Color(1, 1, 1, 0.0f);
            //_lineRenderer.material.color = new Color(1, 1, 1, 1.0f);


            //_lineRenderer.sharedMaterial.color = new Color(1,1,1,1f);

        }
        for (int i = 0; i < valueList.Length; i++)
        {
            float xPosition = xSize + i * xSize;
            float yPosition = valueList[i] / yMax * graphHeight;
            //GameObject currentDot = CreateDot(new Vector2(xPosition+3, yPosition+3));
            lineRenderer.SetPosition(i, new Vector3(xPosition + 3, yPosition + 3, 0));
            // if (lastDot != null) {
            //     CreateDotConnection(lastDot.GetComponent<RectTransform>().anchoredPosition, currentDot.GetComponent<RectTransform>().anchoredPosition);
            // }
            // lastDot = currentDot;
        }
    }

    // private void CreateDotConnection(Vector2 dotPositionA, Vector2 dotPositionB) {
    //     GameObject gameObject = new GameObject("dotConnection", typeof(Image));
    //     gameObject.transform.SetParent(_graphContainer, false);
    //     gameObject.GetComponent<Image>().color = new Color(1, 1, 1, .5f);
    //     RectTransform rectTransform = gameObject.GetComponent<RectTransform>();
    //     Vector2 dir = (dotPositionB - dotPositionA).normalized;
    //     float distance = Vector2.Distance(dotPositionA, dotPositionB);
    //     rectTransform.anchorMin = new Vector2(0, 0);
    //     rectTransform.anchorMax = new Vector2(0, 0);
    //     rectTransform.sizeDelta = new Vector2(distance, 3f);
    //     rectTransform.anchoredPosition = dotPositionA + dir * distance * .5f;
    //     // UtilsClass.GetAngleFromVectorFloat(dir)
    //     float angle = Mathf.Acos(Vector2.Dot(dir, Vector2.right) / (dir.magnitude * Vector2.right.magnitude));
    //     angle = angle * Mathf.Rad2Deg;
    //     rectTransform.localEulerAngles = new Vector3(0, 0, angle);
    // }
}


public class CircularBuffer<T>
{
    private T[] buffer;
    private int head;
    private int tail;
    private int count;

    public CircularBuffer(int capacity)
    {
        buffer = new T[capacity];
        head = 0;
        tail = 0;
        count = 0;
    }

    public CircularBuffer(T[] array)
    {
        buffer = new T[array.Length];
        Array.Copy(array, buffer, array.Length);
        head = 0;
        tail = array.Length % buffer.Length;
        count = array.Length;
    }

    public void Add(T item)
    {
        buffer[tail] = item;
        tail = (tail + 1) % buffer.Length;
        if (count < buffer.Length)
        {
            count++;
        }
        else
        {
            head = (head + 1) % buffer.Length;
        }
    }

    public IEnumerable<T> Cycle()
    {
        if (count == 0)
        {
            yield break;
        }

        for (int i = 0; i < count; i++)
        {
            yield return buffer[(head + i) % buffer.Length];
        }
    }
}
