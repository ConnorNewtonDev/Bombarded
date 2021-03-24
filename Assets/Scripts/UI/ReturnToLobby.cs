using Networking;
using UnityEngine;

namespace UI
{
    public class ReturnToLobby : MonoBehaviour
    {
        public void ButtonPressed()
        {
            FightManager.instance.networkObject.Destroy();
            Destroy(FightManager.instance.gameObject);
            LobbyManager.instance.ChangeState(GameState.Lobby);
        }
    }
}
