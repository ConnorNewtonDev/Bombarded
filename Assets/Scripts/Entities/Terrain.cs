using System.Collections.Generic;
using BeardedManStudios.Forge.Networking;
using BeardedManStudios.Forge.Networking.Generated;
using BeardedManStudios.Forge.Networking.Unity;
using UnityEngine;
using UnityEngine.XR;

public class Terrain : WorldObjectBehavior, IDamageReceiver
{
    public float Health
    {
        get { return networkObject.health;}
        private set { networkObject.health = value; }
    }
    public TerrainState[] healthStates;
    public MeshRenderer meshRenderer;
    public GameObject destroyEffect;
    private float _maxHealth = 100;
    private int _currentState = 0;
    
    
    // Start is called before the first frame update
    void Start()
    {
        Health = _maxHealth;
     
        if (!NetworkManager.Instance.IsServer)
            networkObject.onDestroy += DestroyEffect;

        networkObject.healthChanged += CheckState;
    }
    

    public void TakeDamage(float value)
    {
        Health -= value;
    }

    private void CheckState(float value, ulong timestep)
    {
        if (value <= 0)
        {
            DestroyTerrain();
            return;
        }
        
        var percent = (value / _maxHealth) * 100;
        if (percent < healthStates[_currentState].minThreshhold)
        {
            networkObject.SendRpc(RPC_CHANGE_STATE, Receivers.All, _currentState+1);
            return;
        }
    }

    private void ChangeState(TerrainState state)
    {
        meshRenderer.material = state.material;
        if (healthStates[_currentState].transitionOutEffect != null)
        {
            MainThreadManager.Run(() =>
            {
                var effect = Instantiate(healthStates[_currentState].transitionOutEffect, transform.position + new Vector3(2.5f, 2.5f, 2.5f), transform.rotation, null);
                Destroy(effect, 5f);
            });
        }

        
    }
    
    public override void ChangeState(RpcArgs args)
    {
        var newState = args.GetNext<int>();

        if (!NetworkManager.Instance.IsServer)
        {
            ChangeState(healthStates[newState]);
        }

        _currentState = newState;
    }
    private void DestroyTerrain()
    {
        networkObject.Destroy();
    }

    private void DestroyEffect(NetWorker netWorker)
    {
        // if (destroyEffect != null)
        // {
        //     var effect = Instantiate(destroyEffect, transform.position, transform.rotation, null);
        //     effect.transform.localPosition += Vector3.one * 2.5f;
        //     Destroy(effect, 3f);
        // }
        networkObject.onDestroy -= DestroyEffect;
        networkObject.healthChanged -= CheckState;
        
        MainThreadManager.Run(() =>
        {
            Destroy(gameObject);    
        });
        
    }
    
    [System.Serializable]
    public struct TerrainState
    {
        public float minThreshhold;
        public Material material;
        public GameObject transitionOutEffect;
    }


}
