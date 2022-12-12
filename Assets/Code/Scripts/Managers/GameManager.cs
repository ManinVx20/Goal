using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using FishNet.Object;
using FishNet.Object.Synchronizing;
using UnityEngine;

namespace StormDreams
{
    public class GameManager : NetworkBehaviour
    {
        public static GameManager Instance;

        [field: SyncObject()]
        public readonly SyncList<Player> ConnectedPlayers = new();

        [field: SyncVar]
        public bool CanStart { get; private set; }

        private const int MAX_PLAYERS = 2;

        private bool _gameStarted = false;

        private void Awake()
        {
            Instance = this;
        }

        private void Update()
        {
            if (!IsServer)
            {
                return;
            }

            if (ConnectedPlayers.Count == 0)
            {
                CanStart = false;
            }
            else
            {
                CanStart = ConnectedPlayers.All(player => player.Ready);
            }

            if (!_gameStarted)
            {
                if (ConnectedPlayers.Count == 2 && CanStart)
                {
                    StartGame();

                    _gameStarted = true;
                }
            }
        }

        [Server]
        public void StartGame()
        {
            for (int i = 0; i < ConnectedPlayers.Count; i++)
            {
                ConnectedPlayers[i].StartGame();
            }

            MatchManager.Instance.BeginMatch();
        }

        [Server]
        public void StopGame()
        {
            for (int i = 0; i < ConnectedPlayers.Count; i++)
            {
                ConnectedPlayers[i].StopGame();
            }
        }
    }
}
