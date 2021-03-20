using System;
using System.Collections;
using System.Collections.Generic;
using BeardedManStudios.Forge.Networking;
using BeardedManStudios.Forge.Networking.Generated;
using BeardedManStudios.Forge.Networking.Unity;
using UnityEngine;

public class SceneStart : MonoBehaviour
{
    public GameObject spawners;
    private void Start()
    {
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
        InitNetwork();
    }

    private void InitNetwork()
    {
        NetworkManager.Instance.objectInitialized += ObjectInitialized;
        NetworkManager.Instance.Networker.playerConnected += (player, sender) =>
        {
            Debug.Log($"Player Joined - count:{NetworkManager.Instance.Networker.Players.Count}");
        };
        NetworkManager.Instance.Networker.playerDisconnected += (player, sender) =>
        {
            Debug.Log($"Player Left - count:{NetworkManager.Instance.Networker.Players.Count}");
        };
    }

    private void ObjectInitialized(INetworkBehavior behavior, NetworkObject obj)
    {
        if (!(obj is PlayerNetworkObject))
            return;
        
        if (NetworkManager.Instance.Networker is IServer)
        {
            //Bind destroy cross network
            Debug.Log($"Player object initalised for user {obj.Owner.NetworkId}");
            obj.Owner.disconnected += (sender) =>
            {
                obj.Destroy();
                Debug.Log($"Player object destroyed for user {obj.Owner.NetworkId}");
            };
        }
    }

    private void SpawnPlayer()
    {
        var spawn = NetworkManager.Instance.InstantiatePlayer(0, transform.position, transform.rotation);
    }
}
