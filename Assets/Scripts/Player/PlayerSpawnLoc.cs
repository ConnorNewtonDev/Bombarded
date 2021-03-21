using UnityEngine;

namespace Player
{
    public class PlayerSpawnLoc : MonoBehaviour
    {
        public bool taken = false;
        // Start is called before the first frame update
        void Start()
        {
        
        }

        public Vector3 GetPosition()
        {
            taken = true;
            return transform.position;
        }
    }
}
