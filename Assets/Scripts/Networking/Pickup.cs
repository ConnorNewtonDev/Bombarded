using BeardedManStudios.Forge.Networking;
using BeardedManStudios.Forge.Networking.Generated;
using Player;
using UnityEngine;

namespace Networking
{
    public class Pickup : PickupBehavior
    {
        public BombType bombType = BombType.Standard;
        private bool taken = false;
        
        private void OnTriggerEnter(Collider other)
        {
            if (taken)
                return;
            if (other.CompareTag("Player"))
            {
                var pStats = other.GetComponent<PlayerStats>();
                if (pStats.networkObject.IsOwner && !pStats.IsHoldingBomb())
                {
                    taken = true;
                    pStats.SetHeldBomb((int)bombType);
                    networkObject.SendRpc(RPC_DESTROY, Receivers.All);
                }
            }
        }

        public override void Destroy(RpcArgs args)
        {
            networkObject.Destroy();
        }
    }
}
