using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainCamera : CameraShake {

    public static Vector2 mousePos;
    public new static Camera camera;
    public new static Transform transform;
    public static Transform camHolder;
    public static float width;
    public static float height;

    [Header("Positioning")]
    public float z;
    public float moveLerpSpeed;
    public float maxDisplacementFromPlayer;

    [Header("Size")]
    public float minHeight;
    public float camHeightToPlayerSizeRatio;
    public float camHeightLerpSpeed;

    [Header("Kick")]
    public float kickDiminishSpeed;
    [HideInInspector]
    public Vector2 kick;//knockback

    [Header("Player")]
    public float playerToMouseRatio;

    [Header("Audio")]
    public float masterVolume;

    private void Awake()
    {
        if(camera == null) camera = GetComponent<Camera>();
        if(transform == null) transform = GetComponent<Transform>();
        if(camHolder == null) camHolder = transform.parent;

        if (height > 0) camera.orthographicSize = height;
        else height = camera.orthographicSize;

        width = height * camera.aspect;
    }

    private void Start()
    {
        camHolder.position = GameManager.player.transform.position + Vector3.forward * z;
        AudioListener.volume = masterVolume;
    }

    private void FixedUpdate()
    {
        camHolder.position = (Vector3)(Vector2.Lerp(camHolder.position, GameManager.player.transform.position, GameManager.fixedDeltaTime * moveLerpSpeed) + kick) + Vector3.forward * z;
        SetHeight(Mathf.Lerp(height, Mathf.Max(GameManager.player.transform.lossyScale.x * camHeightToPlayerSizeRatio, minHeight), camHeightLerpSpeed));
    }

    private void Update()
    {
        mousePos = GetMouseWorld2DPoint();
        kick -= kick * kickDiminishSpeed * Time.deltaTime;
    }

    public void SetHeight(float value)
    {
        height = value;
        width = height * camera.aspect;
        camera.orthographicSize = height;
    }

    public void SetPosition(Vector3 position)
    {
        camHolder.position = new Vector3(position.x, position.y, position.z);
        transform.localPosition = Vector3.zero;
    }
    public void SetPosition(Vector2 position)
    {
        camHolder.position = new Vector3(position.x, position.y, transform.position.z);
        transform.localPosition = Vector3.zero;
    }

    private Vector2 GetMouseWorld2DPoint()
    {
        return camera.ScreenToWorldPoint(Input.mousePosition);
    }
}
