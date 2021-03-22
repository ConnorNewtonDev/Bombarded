using System.Collections;
using System.Collections.Generic;
using BeardedManStudios.Forge.Networking.Unity;
using UnityEngine;

public class WorldObjectSpawner : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        if (NetworkManager.Instance.IsServer)
        {
            var offset = new Vector3(0, 90 * Random.Range(0, 4), 0) ;
            var rot = transform.rotation.eulerAngles + offset;
            NetworkManager.Instance.InstantiateWorldObject(0, transform.position, Quaternion.Euler(rot), true);
            Debug.Log($"SPAWN {offset}");            
        }
    }

    
}
