using Networking;
using UnityEngine;

namespace UI
{
    public class ReturnToLobby : MonoBehaviour
    {
        public void ButtonPressed()
        {
            FightManager.instance.networkObject.Destroy();
            LobbyManager.instance.ChangeState(GameState.Lobby);
        }
    }
}
