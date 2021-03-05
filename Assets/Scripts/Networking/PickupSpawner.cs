using System.Collections;
using System.Collections.Generic;
using BeardedManStudios.Forge.Networking;
using BeardedManStudios.Forge.Networking.Unity;
using UnityEngine;

namespace Networking
{
    public class PickupSpawner : MonoBehaviour
    {
        private float _cooldown;
    
        public BombType bombType;
        public float spawnCooldown;

        public bool PickupReady { get; private set; } = true;
    
        // Start is called before the first frame update
        void Start()
        {
            if (!NetworkManager.Instance.IsServer)
                enabled = false;
            else
                SetPickup();
        }

        // Update is called once per frame
        void Update()
        {
            if (!PickupReady)
            {
                _cooldown -= Time.deltaTime;
                if (_cooldown <= 0)
                    SetPickup();
            }
        }

        private void SetPickup()
        {
            PickupReady = true;
            _cooldown = spawnCooldown;
        
            //Instatiate pickup Object
            var obj = NetworkManager.Instance.InstantiatePickup(0, transform.position, transform.rotation);
            obj.networkObject.pickupType = (int) bombType;
            obj.networkObject.onDestroy += PickupTaken;
        }

        private void PickupTaken(NetWorker sender)
        {
            PickupReady = false;
        }
    }
    [System.Serializable]
    public enum BombType
    {
        Standard = 0,
        Scatter = 1,
        KaBoom = 2
    }
}