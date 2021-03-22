using System;
using System.Collections;
using System.Collections.Generic;
using Networking;
using UnityEngine;
using UnityEngine.SceneManagement;
public class GameManager : MonoBehaviour
{
    
    
    public static GameManager instance;
    public PlayerData PlayerData;
    private void Awake()
    {
        if (instance == null)
            instance = this;
        else if(instance != this)
            Destroy(gameObject);
        
        DontDestroyOnLoad(this.gameObject);

        Cursor.lockState = CursorLockMode.Confined;
    }

    public void LoadScene(GameData data)
    {
        var asyncOP = SceneManager.LoadSceneAsync(data.MapSelection, LoadSceneMode.Additive);
        asyncOP.completed += (operation => SceneManager.GetSceneByBuildIndex(data.MapSelection));
        
    }
    

    public void ReturnToLobby(int currentMap)
    {
        SceneManager.SetActiveScene(SceneManager.GetSceneAt(0)); 
        var asyncOP = SceneManager.UnloadSceneAsync(SceneManager.GetSceneAt(1));
    }

    private void FightSceneReady()
    {
        
    }
 
}
