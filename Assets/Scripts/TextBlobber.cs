using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TextBlobber : MonoBehaviour
{

    public Vector2 moveMagnitude;
    public Vector2 scaleMagnitude;
    public Vector2 moveFrequency;
    public Vector2 scaleFrequency;
    public Vector2 rotMagnitude;
    public float rotFrequency;

    public Vector2 oriPos;
    public Vector2 oriScale;

    new RectTransform transform;
    private float t;
    private void Awake()
    {
        transform = GetComponent<RectTransform>();
    }

    void Update()
    {
        t = Time.timeSinceLevelLoad;
        this.transform.localPosition = oriPos + new Vector2(Mathf.PingPong(moveFrequency.x * t, moveMagnitude.x), Mathf.PingPong(moveFrequency.y * t, moveMagnitude.y));
        this.transform.localScale = oriScale + new Vector2(Mathf.PingPong(scaleFrequency.x * t, scaleMagnitude.x), Mathf.PingPong(scaleFrequency.y * t, scaleMagnitude.y));
        this.transform.eulerAngles = Vector3.forward * Mathf.Lerp(rotMagnitude.x, rotMagnitude.y, Mathf.PingPong(rotFrequency * t, 1));
    }
}
