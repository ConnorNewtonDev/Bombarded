using System;
using System.Collections.Generic;
using System.Linq;
using BeardedManStudios.Forge.Networking;
using BeardedManStudios.Forge.Networking.Generated;
using BeardedManStudios.Forge.Networking.Unity;
using UnityEngine;

namespace Networking
{
    public class LobbyManager : LobbyBehavior
    {
        public static LobbyManager instance;
        public int PlayersInLobby = 0;
        private GameState gameState;
        private GameData activeData;
        private List<PlayerData> players = new List<PlayerData>();

        public GameObject lobbyUI;
        private void Start()
        {
            if (instance == null)
                instance = this;
            else if(instance != this)
                Destroy(gameObject);
            
        }

        // Start is called before the first frame update
        protected override void NetworkStart()
        {
            base.NetworkStart();

            NetworkManager.Instance.automaticScenes = false;
            if (NetworkManager.Instance.IsServer)
            {
                networkObject.gameState = (int)GameState.Lobby;

                NetworkManager.Instance.Networker.playerConnected += (player, sender) =>
                {
                    RegisterPlayer(player.NetworkId);
                    PlayersInLobby++;
                    Debug.Log($"Player Joined - count:{NetworkManager.Instance.Networker.Players.Count}");
                };
                NetworkManager.Instance.Networker.playerDisconnected += (player, sender) =>
                {
                    RemovePlayer(player.NetworkId);
                    PlayersInLobby--;
                    Debug.Log($"Player Joined - count:{NetworkManager.Instance.Networker.Players.Count}");
                };
                
            }
            else
            {
                NetworkManager.Instance.Networker.playerConnected += (player, sender) =>
                {
                    RegisterPlayer(player.NetworkId);
                    Debug.Log($"Player Joined - count:{NetworkManager.Instance.Networker.Players.Count}");
                };
                NetworkManager.Instance.Networker.playerDisconnected += (player, sender) =>
                {
                    RemovePlayer(player.NetworkId);
                    Debug.Log($"Player Joined - count:{NetworkManager.Instance.Networker.Players.Count}");
                };
            }
        }

        public void StartFight()
        {
            //For testing
            activeData.MapSelection = (int) SceneEnums.Map1;
            ChangeState(GameState.Fighting);
        }

        public override void ChangeState(RpcArgs args)
        {
            var stateID = args.GetNext<int>();
            if (NetworkManager.Instance.IsServer)
            {
                networkObject.SendRpc(RPC_CHANGE_STATE, Receivers.OthersBuffered, stateID);
            }
            else
            {
                var newState = (GameState) stateID;
                switch (newState)
                {
                    case GameState.Lobby:
                        GameManager.instance.ReturnToLobby(activeData.MapSelection);
                        lobbyUI.SetActive(true);
                        break;
                    case GameState.Fighting:
                        GameManager.instance.LoadScene(activeData);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException(nameof(newState), newState, null);
                }
            }

            
        }

        public void ChangeState(GameState newState)
        {
            networkObject.SendRpc(RPC_CHANGE_STATE, Receivers.Server, (int)newState);
        }
        
        private void RegisterPlayer(uint id)
        {
            players.Add(new PlayerData(id));
        }

        private void RemovePlayer(uint id)
        {
            foreach (var player in players.Where(player => player.netID == id))
            {
                players.Remove(player);
                break;
            }
        }

        private void UpdatePlayer(PlayerData data)
        {
            var playerJson = JsonUtility.ToJson(data);
            networkObject.SendRpc(RPC_UPDATE_PLAYER, Receivers.Others, data);
        }
        
        
        public override void UpdatePlayer(RpcArgs args)
        {
            var player = JsonUtility.FromJson<PlayerData>(args.GetNext<string>());
            for (int i = 0; i < players.Count; i++ )
            {
                if(players[i].netID == player.netID)
                    players[i] = new PlayerData(player);
                return;
            }
        }
    }

    public enum GameState
    {
        Lobby = 0,
        Fighting = 1
    }

    public enum SceneEnums
    {
        Title = 0,
        Lobby = 1,
        Map1 = 2
    }

    public struct GameData
    {
        public int MaxRounds;
        public int MapSelection;
        public bool RepeatMap;
    }

    public struct PlayerData
    {
        public uint netID;
        public string username;
        public int combatType;
        public int customization;

        public PlayerData(uint netID)
        {
            this.netID = netID;
            username = "";
            combatType = 0;
            customization = 0;
        }

        public PlayerData(uint netID, string username, int combatType, int customization)
        {
            this.netID = netID;
            this.username = username;
            this.combatType = combatType;
            this.customization = customization;
        }
        
        public PlayerData(PlayerData data)
        {
            this.netID = data.netID;
            this.username = data.username;
            this.combatType = data.combatType;
            this.customization = data.customization;
        }
    }
}
