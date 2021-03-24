using Networking;
using TMPro;
using UnityEngine;

namespace UI
{
    public class PlayerUI : MonoBehaviour
    {
        public Transform deathBar;
        public GameObject deathImage;
        public TextMeshProUGUI playerLabel;
        private int _playerID;
        
        // Start is called before the first frame update
        void Start()
        {
            _playerID = transform.GetSiblingIndex();
            playerLabel.text = $"Player {_playerID + 1}";
            FightManager.instance.playerDeath += ONDeath;
        }

        private void ONDeath(int index, int deaths)
        {
            if (index == _playerID)
            {
                UpdateDeaths(deaths);
            }
        }
        
        public void UpdateDeaths(int deaths)
        {
            Debug.Log("UpdateUI");
            var count = deathBar.transform.childCount;
            for (int i = 0; i < deaths - count; i++)
            {
                Debug.Log("SkullCreated");
                Instantiate(deathImage, deathBar);
            }
        }
    }
}
