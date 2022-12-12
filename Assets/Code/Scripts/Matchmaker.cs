// using System;
// using System.Collections;
// using System.Collections.Generic;
// using System.Threading.Tasks;
// using ParrelSync;
// using TMPro;
// using Unity.Netcode;
// using Unity.Netcode.Transports.UTP;
// using Unity.Services.Authentication;
// using Unity.Services.Core;
// using Unity.Services.Lobbies;
// using Unity.Services.Lobbies.Models;
// using Unity.Services.Relay;
// using Unity.Services.Relay.Models;
// using UnityEngine;

// namespace StormDreams
// {
//     public class Matchmaker : MonoBehaviour
//     {
//         public static Matchmaker Instance;

//         [SerializeField]
//         private int _maxPlayers = 2;
//         [SerializeField]
//         private string _lobbyName = "New Lobby";
//         [SerializeField]
//         private string _joinKey = "S";

//         private UnityTransport _transport;
//         private string _playerId;
//         private string _lobbyId;

//         private void Awake()
//         {
//             Instance = this;
//         }

//         private void OnDestroy()
//         {
//             try
//             {
//                 StopAllCoroutines();
//             }
//             catch
//             {
//                 Debug.LogFormat("Failed to destroy");
//             }

//         }

//         public async void StartMatchmaking()
//         {
//             ViewManager.Instance.Get<MatchmakingView>().SetLoadingText("Logging in");

//             _transport = FindObjectOfType<UnityTransport>();

//             await Login();

//             CheckForLobbies();
//         }

//         private async Task Login()
//         {
//             if (UnityServices.State == ServicesInitializationState.Uninitialized)
//             {
//                 var options = new InitializationOptions();
// #if UNITY_EDITOR
//                 string profile = ClonesManager.IsClone() ? ClonesManager.GetArgument() : "Primary";
//                 options.SetProfile(profile);
// #endif
//                 await UnityServices.InitializeAsync(options);
//             }

//             if (!AuthenticationService.Instance.IsSignedIn)
//             {
//                 await AuthenticationService.Instance.SignInAnonymouslyAsync();

//                 _playerId = AuthenticationService.Instance.PlayerId;
//             }
//         }

//         private async void CreateLobby()
//         {
//             ViewManager.Instance.Get<MatchmakingView>().SetLoadingText("Creating lobby");

//             try
//             {
//                 var a = await RelayService.Instance.CreateAllocationAsync(_maxPlayers);
//                 var joinCode = await RelayService.Instance.GetJoinCodeAsync(a.AllocationId);

//                 var options = new CreateLobbyOptions
//                 {
//                     Data = new Dictionary<string, DataObject> { { _joinKey, new DataObject(DataObject.VisibilityOptions.Public, joinCode) } }
//                 };

//                 var lobby = await LobbyService.Instance.CreateLobbyAsync(_lobbyName, _maxPlayers, options);

//                 _transport.SetHostRelayData(a.RelayServer.IpV4, (ushort)a.RelayServer.Port, a.AllocationIdBytes, a.Key, a.ConnectionData);

//                 _lobbyId = lobby.Id;
//                 StartCoroutine(HeartbeatLobbyCoroutine(_lobbyId, 15.0f));

//                 NetworkManager.Singleton.StartHost();

//                 ViewManager.Instance.Get<MatchmakingView>().SetLoadingText($"Hosted lobby {_lobbyId}");

//             }
//             catch (Exception e)
//             {
//                 Debug.Log(e);

//                 ViewManager.Instance.Get<MatchmakingView>().SetLoadingText("Failed to create lobby");
//             }
//         }

//         private async void CheckForLobbies()
//         {
//             ViewManager.Instance.Get<MatchmakingView>().SetLoadingText("Finding lobbies");

//             try
//             {
//                 var queryOptions = new QueryLobbiesOptions
//                 {
//                     Filters = new List<QueryFilter>
//                 {
//                     new QueryFilter(
//                         field: QueryFilter.FieldOptions.AvailableSlots,
//                         op: QueryFilter.OpOptions.GT,
//                         value: "0"
//                     )
//                 }
//                 };

//                 var response = await LobbyService.Instance.QueryLobbiesAsync(queryOptions);
//                 var lobbies = response.Results;

//                 if (lobbies.Count > 0)
//                 {
//                     foreach (var lobby in lobbies)
//                     {
//                         JoinLobby(lobby);

//                         return;
//                     }
//                 }
//                 else
//                 {
//                     CreateLobby();
//                 }
//             }
//             catch (Exception e)
//             {
//                 Debug.Log(e);
//             }
//         }

//         private async void JoinLobby(Lobby lobby)
//         {
//             try
//             {
//                 var a = await RelayService.Instance.JoinAllocationAsync(lobby.Data[_joinKey].Value);
//                 _lobbyId = lobby.Id;

//                 SetTransformAsClient(a);

//                 NetworkManager.Singleton.StartClient();

//                 ViewManager.Instance.Get<MatchmakingView>().SetLoadingText($"Joined lobby {_lobbyId}");
//             }
//             catch (Exception e)
//             {
//                 Debug.Log(e);
//             }
//         }

//         private IEnumerator HeartbeatLobbyCoroutine(string lobbyId, float waitTimeSeconds)
//         {
//             var delay = new WaitForSecondsRealtime(waitTimeSeconds);

//             while (true)
//             {
//                 LobbyService.Instance.SendHeartbeatPingAsync(lobbyId);

//                 yield return delay;
//             }
//         }

//         private void SetTransformAsClient(JoinAllocation a)
//         {
//             _transport.SetClientRelayData(a.RelayServer.IpV4, (ushort)a.RelayServer.Port, a.AllocationIdBytes, a.Key, a.ConnectionData, a.HostConnectionData);
//         }
//     }
// }
