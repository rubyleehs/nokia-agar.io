using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class GameManager : MonoBehaviour
{
    public static EnemyManager enemyManager;
    public static Player player;
    public static float deltaTime;
    public static float fixedDeltaTime;
    public static float timeScale = 1;
    public static bool isGameOver = false;
    private static TextMeshProUGUI scoreText;
    public static AudioSource musicPlayer;

    [Header("Gameplay")]
    public Vector2 I_gameRegionSize;
    public static Vector2 gameRegionSize;

    [Header("Audio")]
    public float normPitch;
    public float deathPitch;
    public float dangerPitch;

    [Header("Refrences")]
    public Transform floor;
    public Transform[] walls;
    public Transform[] gameOverScreenText;
    public TextMeshProUGUI I_scoreText;
    public Transform musicPlayerToDestroy;

    public static float score;

    private void Awake()
    {
        Screen.SetResolution(672, 384, Screen.fullScreen);
        if (scoreText == null) scoreText = I_scoreText;
        if (enemyManager == null) enemyManager = GetComponent<EnemyManager>();
        if (player == null) player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
        if (musicPlayer == null)
        {
            musicPlayer = GameObject.FindGameObjectWithTag("MusicPlayer").GetComponent<AudioSource>();
            DontDestroyOnLoad(musicPlayer.transform);
        }
        else musicPlayerToDestroy.gameObject.SetActive(false);

        player.gameManager = this;
        gameRegionSize = I_gameRegionSize;
        SetupGameRegion();
        SetScore(0);
    }

    // Update is called once per frame
    void Update()
    {
        deltaTime = Time.deltaTime * timeScale;
    }

    private void FixedUpdate()
    {
        fixedDeltaTime = Time.fixedDeltaTime * timeScale;
    }

    private void SetupGameRegion()
    {
        floor.position = Vector2.zero;
        floor.localScale = gameRegionSize;

        walls[0].position = Vector2.up * (gameRegionSize.y * 0.5f + 0.5f);
        walls[0].localScale = new Vector2(gameRegionSize.x + 2, 1);
        walls[1].position = Vector2.down * (gameRegionSize.y * 0.5f + 0.5f);
        walls[1].localScale = new Vector2(gameRegionSize.x + 2, 1);
        walls[2].position = Vector2.left * (gameRegionSize.x * 0.5f + 0.5f);
        walls[2].localScale = new Vector2(1, gameRegionSize.y + 2);
        walls[3].position = Vector2.right * (gameRegionSize.x * 0.5f + 0.5f);
        walls[3].localScale = new Vector2(1, gameRegionSize.y + 2);
    }

    public void GameOver(bool value)
    {
        isGameOver = value;
        musicPlayer.pitch = deathPitch;
        for (int i = 0; i < gameOverScreenText.Length; i++)
        {
            gameOverScreenText[i].gameObject.SetActive(value);
        }
    }

    public void Restart()
    {
        GameOver(false);
        musicPlayer.pitch = normPitch;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public static void SetScore(float value)
    {
        score = value;
        scoreText.text = " " + Mathf.Round(value);
    }
}
