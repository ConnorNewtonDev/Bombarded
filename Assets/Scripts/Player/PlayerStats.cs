using System;
using BeardedManStudios.Forge.Networking;
using BeardedManStudios.Forge.Networking.Generated;
using BeardedManStudios.Forge.Networking.Unity;
using Bombs;
using Networking;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Player
{
    public class PlayerStats : PlayerBehavior
    {
        private PlayerMovement _movement;
        private NetPlayerMovment _netMovement;
        private Rigidbody _rb3d;
        private bool knockedUp = false;

        public BombType activeBomb;
        public Transform bombAnchor;

        public Vector3 throwForce = new Vector3(0, 100, 500);
        private void Awake()
        {
            _movement = GetComponent<PlayerMovement>();
            _rb3d = GetComponent<Rigidbody>();
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
                if(!knockedUp)
                    _netMovement.UpdatePositionAndRotation(networkObject.position, networkObject.rotation);
            }
        }

        private void OnFire(InputValue args)
        {
            Debug.Log("FIRE");
            if (IsHoldingBomb())
            { 
                networkObject.SendRpc(RPC_THROW_BOMB, Receivers.All);
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
            var spawn = Instantiate(bomb, bombAnchor);
            spawn.GetComponent<Collider>().enabled = false;

        }

        public override void ThrowBomb(RpcArgs args)
        {
            Destroy(bombAnchor.GetChild(0).gameObject);
            
            var bomb = Instantiate(BombCollection.instance.GetBomb(activeBomb), bombAnchor.position, bombAnchor.rotation);
            var rb3d = bomb.AddComponent<Rigidbody>();
            rb3d.useGravity = true;
            rb3d.AddForce(transform.TransformDirection(throwForce));

        }

        public void Knockback(float force, Vector3 position, float radius)
        {
            networkObject.SendRpc(RPC_KNOCKBACK, Receivers.Owner, force, position, radius);
        }
        
        public override void Knockback(RpcArgs args)
        {
            var force = args.GetNext<float>();
            var origin = args.GetNext<Vector3>();
            var radius = args.GetNext<float>();

            if (networkObject.IsOwner)
            {
                _movement.ToggleRigidbodyMode(true);
                var dir = transform.position - origin;
                dir.Normalize();
                dir.y = 0.5f;
                _rb3d.AddForce(dir * force, ForceMode.VelocityChange);
              
                networkObject.SendRpc(RPC_KNOCKBACK, Receivers.Others, force, origin, radius);
            }
            else if (!networkObject.IsServer)
            {
                _movement.ToggleRigidbodyMode(true);
                var dir = transform.position - origin;
                dir.Normalize();
                dir.y = 0.5f;
                _rb3d.AddForce(dir * force, ForceMode.VelocityChange);
            }

            knockedUp = true;

            // Set knockback animation
        }

        private void OnCollisionEnter(Collision other)
        {
            if (knockedUp)
            {
                _movement.ToggleRigidbodyMode(false);
                knockedUp = false;
            }
        }
        
    }
}
