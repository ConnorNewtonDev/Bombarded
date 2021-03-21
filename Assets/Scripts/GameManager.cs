using System;
using System.Collections;
using System.Collections.Generic;
using Networking;
using UnityEngine;
using UnityEngine.SceneManagement;
public class GameManager : MonoBehaviour
{
    
    
    public static GameManager instance;
    private void Awake()
    {
        if (instance == null)
            instance = this;
        else if(instance != this)
            Destroy(gameObject);
        
        DontDestroyOnLoad(this.gameObject);
    }

    public void LoadScene(GameData data)
    {
        var asyncOP = SceneManager.LoadSceneAsync(data.MapSelection, LoadSceneMode.Additive);
        asyncOP.completed += (operation => SceneManager.SetActiveScene(SceneManager.GetSceneAt(1)));
    }

    public void ReturnToLobby(int currentMap)
    {
        var asyncOP = SceneManager.UnloadSceneAsync(currentMap);
        SceneManager.SetActiveScene(SceneManager.GetSceneAt(0));

    }
    
 
}
