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
        var index = LobbyManager.instance.players.IndexOf(GameManager.instance.PlayerData);
        Transform target = playerSpawns[index].transform;
        var spawn = NetworkManager.Instance.InstantiatePlayer(0, target.position, target.rotation);
    }
}
