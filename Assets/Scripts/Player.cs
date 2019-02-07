using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : Blob
{
    public float startSize;
    [HideInInspector]public GameManager gameManager;
    protected Vector2 inputAxis;

    protected override void Awake()
    {
        base.Awake();
        SetSize(startSize, false);
    }
    // Update is called once per frame
    protected override void Update()
    {
        base.Update();
        HandleInput();
    }

    void HandleInput()
    {
        inputAxis = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        Move(inputAxis);

        if (GameManager.isGameOver)
        {
            if (Input.GetKey(KeyCode.Alpha0) || Input.GetKey(KeyCode.Space)) gameManager.Restart();
        }  
    }

    protected override void FindNearbyBlobs()
    {
        base.FindNearbyBlobs();
        BasicEnemyBlob e;
        for (int i = 0; i < nearbyBlobs.Count; i++)
        {
            e = nearbyBlobs[i].GetComponent<BasicEnemyBlob>();
            if (e != null) e.timeToRecal = 0;
        }
    }

    protected override void Eat(Blob b)
    {
        GameManager.SetScore(GameManager.score + b.size);
        base.Eat(b);
    }

    public override IEnumerator Die()
    {
        if (isDead) yield break;

        gameManager.GameOver(true);

        isDead = true;
        spriteRenderer.sortingOrder = -1;

        EnemyManager.activeBlobs.Remove(this);
        float t = 0;
        float smoothProgress = 0;
        float startRad = radius + radiusPadding;
        while (smoothProgress < 1)
        {
            t += GameManager.deltaTime;
            smoothProgress = Mathf.SmoothStep(0, 1, t / shrinkDur);
            transform.localScale = Vector2.one * Mathf.Lerp(startRad, 0.1f, smoothProgress);
            yield return new WaitForEndOfFrame();
        }

        spriteRenderer.enabled = false;
    }

}
