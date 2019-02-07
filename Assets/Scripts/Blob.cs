using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(SpriteRenderer))]
public class Blob : MonoBehaviour
{
    [Header("Refrences")]
    new Transform transform;
    protected SpriteRenderer spriteRenderer;
    protected Rigidbody2D rb;
    protected EnemyManager enemyManager;

    [Header("Gameplay")]
    public float speedGain;
    public float maxSpeedRatio;
    public float radiusPadding;
    public float growthLerpRate;

    [Header("Returning to Pool")]
    public float shrinkDur;

    [Header("Meiosis and Pathfinding")]
    public float checkRadiusRatio;
    public float checkUpdateInterval = 1;
    protected List<Blob> nearbyBlobs;
    protected float timeToNextCheckUpdate;

    [Header("Runtime Values")]
    public float radius;
    public float size;
    protected bool isDead = false;

    protected virtual void Awake()
    {
        Initialize();
    }
    
    public virtual void Initialize()
    {
        if (enemyManager == null) enemyManager = GameManager.enemyManager;
        if (transform == null) transform = GetComponent<Transform>();
        if (spriteRenderer == null) spriteRenderer = GetComponent<SpriteRenderer>();
        if (rb == null) rb = GetComponent<Rigidbody2D>();
        //if (size == 0) SetSize(Mathf.Pow(transform.lossyScale.x * 0.5f, 2) * Mathf.PI, true);
        spriteRenderer.enabled = true;
        spriteRenderer.sortingOrder = 1;
        isDead = false;
        gameObject.SetActive(true);
    }

    protected virtual void Update()
    {
        if (!isDead)
        {
            timeToNextCheckUpdate -= GameManager.deltaTime;
            if (timeToNextCheckUpdate <= 0) FindNearbyBlobs();
            EatNearbyBlobs();
        }
    }

    private void FixedUpdate()
    {
        if(!isDead) transform.localScale = Vector3.one * Mathf.Lerp(transform.localScale.x, (radius + radiusPadding) * 2, growthLerpRate);
    }

    public virtual void Move(Vector2 direction)
    {
        if (isDead) return;

        rb.velocity += direction.normalized * speedGain * Mathf.Max(1, radius + radiusPadding) * GameManager.deltaTime;
        rb.velocity = direction.normalized * Mathf.Min(maxSpeedRatio * (radius + radiusPadding), rb.velocity.magnitude);
    }

    public virtual IEnumerator Die()
    {
        if (isDead) yield break;
        isDead = true;
        spriteRenderer.sortingOrder = -1;

        EnemyManager.activeBlobs.Remove(this);
        float t = 0;
        float smoothProgress = 0;
        float startRad = radius + radiusPadding;
        while(smoothProgress < 1)
        {
            t += GameManager.deltaTime;
            smoothProgress = Mathf.SmoothStep(0, 1, t / shrinkDur);
            transform.localScale = Vector2.one * Mathf.Lerp(startRad, 0.1f, smoothProgress);
            yield return new WaitForEndOfFrame();
        }

        EnemyManager.inactiveBlobs.Add(this);
        gameObject.SetActive(false);
    }

    public void SetSize(float value, bool instantChange)
    {
        size = value;
        radius = Mathf.Sqrt(value * 0.31830988618f);
        if(instantChange) transform.localScale = Vector3.one * (radius + radiusPadding) * 2;
    }

    protected virtual void FindNearbyBlobs()
    {
        timeToNextCheckUpdate = checkUpdateInterval;

        if (nearbyBlobs == null) nearbyBlobs = new List<Blob>();
        else nearbyBlobs.Clear();

        float dCheckSqr = Mathf.Pow(checkRadiusRatio * (radius + radiusPadding), 2);

        for (int i = 0; i < EnemyManager.activeBlobs.Count; i++)
        {
            if (EnemyManager.activeBlobs[i] == this) continue;
            if ((EnemyManager.activeBlobs[i].transform.position - transform.position).sqrMagnitude <= dCheckSqr) nearbyBlobs.Add(EnemyManager.activeBlobs[i]);
        }
    }

    protected void EatNearbyBlobs()
    {
        for (int i = 0; i < nearbyBlobs.Count; i++)
        {
            if (nearbyBlobs[i].size  < size)
            {
                if ((nearbyBlobs[i].transform.position - transform.position).sqrMagnitude <= radius * radius)
                {
                    Eat(nearbyBlobs[i]);
                    nearbyBlobs.RemoveAt(i);
                    i--;
                }
            }
        }
    }

    protected virtual void Eat(Blob b)
    {
        if (b.isDead) return;

        StartCoroutine(b.Die());
        size += b.size;
        SetSize(size, false);
        timeToNextCheckUpdate = 0;
    }
}
