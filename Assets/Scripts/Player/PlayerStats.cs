using System;
using BeardedManStudios.Forge.Networking;
using BeardedManStudios.Forge.Networking.Generated;
using BeardedManStudios.Forge.Networking.Unity;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Player
{
    public class PlayerStats : PlayerBehavior
    {
        private PlayerMovement _movement;
        private NetPlayerMovment _netMovement;

        public Transform bombAnchor;
        private void Awake()
        {
            _movement = GetComponent<PlayerMovement>();
        }

        protected override void NetworkStart()
        {
            base.NetworkStart();

            if (networkObject.IsOwner)
            {
                _movement.enabled = true;
                GetComponent<PlayerInput>().enabled = true;
            }
            else
                _netMovement = gameObject.AddComponent<NetPlayerMovment>();
        }
        
        void Update()
        {
            UpdateNetworkObject();
        }

        private void UpdateNetworkObject()
        {
            if (networkObject.IsOwner)
            {
                networkObject.position = transform.position;
                networkObject.rotation = transform.rotation;
            }
            else if(_netMovement)
            {
                _netMovement.UpdatePositionAndRotation(networkObject.position, networkObject.rotation);
            }
        }

        public bool IsHoldingBomb() => bombAnchor.childCount != 0;

        public void SetHeldBomb(int bombType)
        {
            networkObject.SendRpc(RPC_SET_HELD_BOMB, Receivers.All, bombType);
        }

        public override void SetHeldBomb(RpcArgs args)
        {
            var bombType = args.GetNext<int>();
            var bomb = BombCollection.instance.GetBomb(bombType);
            Instantiate(bomb, bombAnchor);
        }
    }
}
