using System.Collections;
using System.Collections.Generic;
using BeardedManStudios.Forge.Networking.Unity;
using UnityEngine;

public class WorldObjectSpawner : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        if(NetworkManager.Instance.IsServer)
            NetworkManager.Instance.InstantiateWorldObject(0, transform.position, transform.rotation);
    }

    
}
