using System;
using System.Collections.Generic;
using System.Linq;
using BeardedManStudios.Forge.Networking;
using BeardedManStudios.Forge.Networking.Generated;
using BeardedManStudios.Forge.Networking.Unity;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using Utilities;

namespace Networking
{
    public class LobbyManager : LobbyBehavior
    {
        public static LobbyManager instance;
        public int PlayersInLobby = 0;
        private GameState gameState;
        private GameData activeData;
        public List<PlayerData> players = new List<PlayerData>();

        public UnityEvent lobbyToBackground;
        public UnityEvent lobbyReturned;
        
        public GameObject lobbyUI;
        private void Awake()
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
            

            
            if (NetworkManager.Instance.IsServer)
            {
                networkObject.gameState = (int) GameState.Lobby;
                NetworkManager.Instance.Networker.playerConnected += (player, sender) =>
                {
                    Debug.Log($"Player Joined - count:{NetworkManager.Instance.Networker.Players.Count}");
                };
                NetworkManager.Instance.Networker.playerDisconnected += (player, sender) =>
                {
                    Debug.Log($"Player Joined - count:{NetworkManager.Instance.Networker.Players.Count}");
                };
            }
            else
            {
                networkObject.SendRpc(RPC_SYNC_PLAYER_LIST, Receivers.Server, JsonUtility.ToJson(GameManager.instance.PlayerData));
            }

        }

        public void StartFight()
        {
            //For testing
            activeData.MapSelection = (int)SceneEnums.Map1;
            ChangeState(GameState.Fighting);
        }

        public override void ChangeState(RpcArgs args)
        {
            var stateID = args.GetNext<int>();
            var json = args.GetNext<string>();
            var stateData = JsonUtility.FromJson<GameData>(json);
            
            activeData = stateData;
            var newState = gameState = (GameState) stateID;
            if (NetworkManager.Instance.IsServer)
            {
                Debug.Log($"Changing To State: {(GameState)stateID}");
                networkObject.SendRpc(RPC_CHANGE_STATE, Receivers.Others, stateID, json);
                switch (newState)
                {
                    case GameState.Lobby:
                        GameManager.instance.ReturnToLobby(activeData.MapSelection);
                        lobbyReturned?.Invoke();
                        break;
                    case GameState.Fighting:
                        lobbyToBackground?.Invoke();
                        GameManager.instance.LoadScene(activeData);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException(nameof(newState), newState, null);
               }
            }
            else
            {
                switch (newState)
                {
                    case GameState.Lobby:
                        GameManager.instance.ReturnToLobby(activeData.MapSelection);
                        lobbyReturned?.Invoke();
                        lobbyUI.SetActive(true);
                        break;
                    case GameState.Fighting:
                        lobbyToBackground?.Invoke();
                        lobbyUI.SetActive(false);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException(nameof(newState), newState, null);
                }
            }
            
            
            
            

        }

        public override void SyncPlayerList(RpcArgs args)
        {
            if (NetworkManager.Instance.IsServer)
            {
                var playerjson = args.GetNext<string>();
                Debug.Log($"Server json {playerjson}");
                if (!string.IsNullOrEmpty(playerjson))
                {
                    var playerData = JsonUtility.FromJson<PlayerData>(playerjson);
                    
                    if (!players.Contains(playerData))
                        players.Add(playerData);
    
                }
                
                var json = JsonHelper.ToJson(players.ToArray());
                networkObject.SendRpc(RPC_SYNC_PLAYER_LIST, Receivers.Others, json);
            }
            else
            {
                var json = args.GetNext<string>();
                var serverList = JsonHelper.FromJson<PlayerData>(json);

                players = new List<PlayerData>();
                players.AddRange(serverList);
                
            }
        }

        public void ChangeState(GameState newState)
        {
            var gameDataJson = JsonUtility.ToJson(activeData);
            Debug.Log($"GhangeState - ActiveData {gameDataJson}");
            networkObject.SendRpc(RPC_CHANGE_STATE, Receivers.Server, (int)newState, gameDataJson);
        }
        
        
        public override void UpdatePlayer(RpcArgs args)
        {
            var json = args.GetNext<string>();
            var player = JsonUtility.FromJson<PlayerData>(json);
            for (int i = 0; i < players.Count; i++ )
            {
                if (players[i].netID == player.netID)
                {
                    players[i] = new PlayerData(player);
                    return;
                }
            }
            networkObject.SendRpc(RPC_SYNC_PLAYER_LIST, Receivers.Server, json);
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

    [Serializable]
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
        
        public static bool operator ==(PlayerData lhs, PlayerData rhs)
        {
            if (lhs.netID == rhs.netID && lhs.username == rhs.username && lhs.combatType == rhs.combatType && lhs.customization == rhs.customization)
                return true;
            else
                return false;
        }
        
        public static bool operator !=(PlayerData lhs, PlayerData rhs)
        {
            if (lhs.netID == rhs.netID && lhs.username == rhs.username && lhs.combatType == rhs.combatType && lhs.customization == rhs.customization)
                return false;
            else
                return true;
        }
    }
}
