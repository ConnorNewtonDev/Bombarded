using System;
using BeardedManStudios.Forge.Networking;
using Player;
using UnityEngine;

namespace Bombs
{
    public class Bomb : MonoBehaviour
    {
        public float radius = 3f;
        public float knockbackForce = 1000f;
        public uint spawnerID;
        public float damage = 10f;
        public GameObject destroySpawn;
        private AudioSource _source;
        
        // Start is called before the first frame update
        void Awake()
        {
            _source = GetComponent<AudioSource>();
        }

        // Update is called once per frame
        void Update()
        {
        
        }

        private void OnTriggerEnter(Collider other)
        {
            GetComponent<Collider>().enabled = false;
            // Avoid owner collision
            if (other.TryGetComponent<PlayerStats>(out var p))
            {
                if(p.networkObject.NetworkId == spawnerID)
                    return;
            }

            var hits = Physics.SphereCastAll(transform.position, radius, Vector3.up);

            foreach (var hit in hits)
            {
                if (hit.transform.TryGetComponent<IDamageReceiver>(out var receiver))
                {
                    receiver.TakeDamage(damage);
                }
                if (hit.transform.TryGetComponent<PlayerStats>(out var player))
                {
                    player.Knockback(knockbackForce, transform.position, radius);
                }
            }

            SpawnEffect();

            _source.PlayOneShot(_source.clip);
            GetComponent<MeshRenderer>().enabled = false;
            Destroy(gameObject, 1f);
        }

        private void SpawnEffect()
        {
            var effect = Instantiate(destroySpawn, transform.position , transform.rotation, null);
            Destroy(effect, 1.5f);
        }
        
    }
}
