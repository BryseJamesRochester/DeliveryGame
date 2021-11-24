using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneManagement : MonoBehaviour
{

    #region Singleton Pattern

    private static SceneManagement _instance;

    public static SceneManagement Instance { get { return _instance; } }


    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            _instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
    }

    #endregion

    private string[] scenes;

    void Start()
    {
        scenes = new string[SceneManager.sceneCountInBuildSettings];
        for (int i = 0; i < scenes.Length; i++)
            scenes[i] = System.IO.Path.GetFileNameWithoutExtension(SceneUtility.GetScenePathByBuildIndex(i));
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void goToScene(string scene)
    {
        SceneManager.LoadScene(scene);
    }

     public void goToLevel(string level)
    {
        foreach(string scene in scenes)
        {
            if (scene.Contains(level))
                goToScene(scene);
        }
    }

    public void goToWorldMap()
    {
        goToScene("WorldMap");
    }
}
