using System;
using System.Collections;
using System.Collections.Generic;
using BeardedManStudios.Forge.Networking;
using BeardedManStudios.Forge.Networking.Generated;
using BeardedManStudios.Forge.Networking.Unity;
using Networking;
using Player;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneStart : MonoBehaviour
{
    public GameObject spawners;
    public PlayerSpawnLoc[] playerSpawns;
    private void Start()
    {
        SceneManager.SetActiveScene(SceneManager.GetSceneAt(1));
        if (!NetworkManager.Instance.IsServer)
        {
            SpawnPlayer();
            return;
        }
        else
        {
#if UNITY_EDITOR
            SpawnPlayer();
#endif
        }
        
        spawners.SetActive(true);
    }

    private void SpawnPlayer()
    {
        var data = GameManager.instance.PlayerData;
        // var index = LobbyManager.instance.players.IndexOf(GameManager.instance.PlayerData);
        var players = LobbyManager.instance.players;
        for (int i = 0; i < players.Count; i++)
        {
            Debug.Log($"Player : {players[i]} / {data}");
            if (players[i] == data)
            {
                Debug.Log($"Player Index : {i}");
                Transform target = playerSpawns[i].transform;
                var spawn = NetworkManager.Instance.InstantiatePlayer(0, target.position, target.rotation);
                return;
            }
        }
    }

}
