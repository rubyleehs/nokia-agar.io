using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    [Header("Pooling")]
    public static List<Blob> inactiveBlobs;
    public static List<Blob> activeBlobs;
    public GameObject fallbackBlob;
    public Transform blobParent;

    [Header("Enemy Spawning")]
    public float enemySpawnInterval;
    public Vector2 deltaSizeToPlayer;
    private float timeToNextEnemySpawn;

    [Header("Food")]
    public float foodSpawnInterval;
    public int foodAmountPerInterval;
    public int unconsumedFoodCap;
    public Vector2 foodSize;
    private float timeToNextFoodSpawn;

    [Header("DecayRate")]
    public float minSizeToStartDecay;
    public float decayInterval;
    public float decayRatio;
    private float timeToNextDecay;

    private void Start()
    {
        inactiveBlobs = new List<Blob>();
        activeBlobs = new List<Blob>();
        activeBlobs.Add(GameManager.player);

        SpawnEnemy();
        SpawnEnemy();
        SpawnEnemy();
        SpawnEnemy();
        SpawnEnemy();
        SpawnEnemy();

    }

    private void Update()
    {
        timeToNextEnemySpawn -= GameManager.deltaTime;
        timeToNextFoodSpawn -= GameManager.deltaTime;
        timeToNextDecay -= GameManager.deltaTime;
        if (timeToNextEnemySpawn <= 0) SpawnEnemy();
        if (timeToNextFoodSpawn <= 0) SpawnFood();
        if (timeToNextDecay <= 0) Decay();
    }

    public void Decay()
    {
        timeToNextDecay = decayInterval;
        for (int i = 0; i < activeBlobs.Count; i++)
        {
            if(activeBlobs[i].size > minSizeToStartDecay)//
            {
                activeBlobs[i].SetSize(activeBlobs[i].size * (1-decayRatio),false);
            }
        }
    }

    public void SpawnFood()
    {
        timeToNextFoodSpawn = foodSpawnInterval;
        Vector2 randPos;
        float randSize;
        if (activeBlobs.Count < unconsumedFoodCap)
        {
            for (int i = 0; i < foodAmountPerInterval; i++)
            {
                randPos = 0.5f * new Vector2(Random.Range(-GameManager.gameRegionSize.x, GameManager.gameRegionSize.x), Random.Range(-GameManager.gameRegionSize.y, GameManager.gameRegionSize.y));
                randSize = Random.Range(foodSize.x, foodSize.y);
                SpawnBlob(randPos, randSize, false);
            }
        }
    }
    public void SpawnEnemy()
    {
        timeToNextEnemySpawn = enemySpawnInterval;
        Vector2 randPos = 0.5f * new Vector2(Random.Range(-GameManager.gameRegionSize.x, GameManager.gameRegionSize.x), Random.Range(-GameManager.gameRegionSize.y, GameManager.gameRegionSize.y));
        float randSize = GameManager.player.size + Random.Range(deltaSizeToPlayer.x, deltaSizeToPlayer.y);
        randSize = Mathf.Max(2.5f, randSize);
        SpawnBlob(randPos, randSize,true);
    }
    public void SpawnBlob(Vector2 pos, float size, bool isLive)
    {
        if (inactiveBlobs == null) inactiveBlobs = new List<Blob>();
        if (inactiveBlobs.Count > 0)
        {
            inactiveBlobs[0].transform.position = pos;
            inactiveBlobs[0].SetSize(size, true);
            inactiveBlobs[0].Initialize();

            activeBlobs.Add(inactiveBlobs[0]);

            if (isLive) inactiveBlobs[0].enabled = true;
            else inactiveBlobs[0].enabled = false;

            inactiveBlobs.RemoveAt(0);
        }
        else
        {
            if (activeBlobs == null)
            {
                activeBlobs = new List<Blob>();
                activeBlobs.Add(GameManager.player);
            }

            Blob b = Instantiate(fallbackBlob, pos, Quaternion.identity, blobParent).GetComponent<Blob>();
            b.SetSize(size, true);

            activeBlobs.Add(b);

            if (isLive) b.enabled = true;
            else b.enabled = false;
        }
    }

}
