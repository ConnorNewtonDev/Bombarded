using System;
using System.Collections;
using System.Collections.Generic;
using BeardedManStudios.Forge.Networking.Generated;
using BeardedManStudios.Forge.Networking.Unity;
using Networking;
using Player;
using UnityEngine;
using UnityEngine.InputSystem;

public class Killbox : MonoBehaviour
{
    
    private void OnTriggerEnter(Collider other)
    {
    
        if (other.CompareTag("LocalPlayer"))
        {
            FightManager.instance.PlayerDied(GameManager.instance.PlayerData.netID);
            Debug.Log("Respawn");                
            
        }
    }
}
