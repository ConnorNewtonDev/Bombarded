using Networking;
using UnityEngine;

namespace UI
{
    public class ReturnToLobby : MonoBehaviour
    {
        public void ButtonPressed()
        {
            LobbyManager.instance.ChangeState(GameState.Lobby);
        }
    }
}
