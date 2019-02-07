using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    public string gameScene;
    // Update is called once per frame

    private void Awake()
    {
        Screen.SetResolution(672, 384, Screen.fullScreen);
    }
    void Update()
    {
        if(Input.GetKey(KeyCode.Alpha0) || Input.GetKey(KeyCode.Space))
        {
            SceneManager.LoadScene(gameScene);
        }
    }
}
