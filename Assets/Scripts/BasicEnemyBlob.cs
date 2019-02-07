using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicEnemyBlob : Blob
{
    [Header("Enemy AI")]
    public float reactRadiusRatio;
    public float optimalMoveRecalInterval;
    private Vector2 optimalDirToMove;
    public float optimalDirOverrideTreshold;
    [HideInInspector]public float timeToRecal;

    protected override void Awake()
    {
        base.Awake();
        FindNearbyBlobs();
    }


    protected override void Update()
    {
        base.Update();
        timeToRecal -= GameManager.deltaTime;
        SmartMove();
    }

    void SmartMove()
    {
        if (timeToRecal <= 0)
        {
            timeToRecal = optimalMoveRecalInterval;
            optimalDirToMove = Vector2.zero;
            for (int i = 0; i < nearbyBlobs.Count; i++)
            {
                optimalDirToMove += CalculateDirectionalWeightage(nearbyBlobs[i]);
            }
        }

        if (optimalDirToMove.sqrMagnitude < optimalDirOverrideTreshold * optimalDirOverrideTreshold) optimalDirToMove = Random.insideUnitCircle; 
        Move(optimalDirToMove);
    }

    private Vector2 CalculateDirectionalWeightage(Blob blob)
    {
        float netChangePerUnitTime;

        Vector2 delta = blob.transform.position - transform.position;
        if (delta.magnitude > reactRadiusRatio * (radius + radiusPadding)) return Vector2.zero;
        //
        float dt = delta.magnitude / (maxSpeedRatio * (radius + radiusPadding));
        if (dt == 0) return Vector2.zero;

        if(blob.size < size) netChangePerUnitTime = blob.size / dt;
        else netChangePerUnitTime = -(blob.size - size -0.2f) / dt;


        return delta * netChangePerUnitTime;
    }
}
