using System;
using System.Collections;
using System.Collections.Generic;
using FishNet.Connection;
using FishNet.Object;
using FishNet.Object.Synchronizing;
using TMPro;
using UnityEngine;

namespace StormDreams
{
    public class Player : NetworkBehaviour
    {
        public static Player Instance;

        [Header("Properties")]
        [SyncVar(OnChange = nameof(OnChangePlayerName))]
        public string Name = "Player";
        [SyncVar]
        public int Level = 0;
        [SyncVar]
        public int Rank = 0;

        [Header("Skills Related")]
        [SyncVar(OnChange = nameof(OnChangePlayerSkill))]
        public string SkillOneName;
        [SyncVar(OnChange = nameof(OnChangePlayerSkill))]
        public string SkillTwoName;

        [Header("Misc")]
        [SyncVar(OnChange = nameof(OnChangePlayerReady))]
        public bool Ready = false;
        [SyncVar]
        public Dummy ControlledDummy;

        private GameObject _playerListItem;
        private TMP_Text _playerNameText;
        private TMP_Text _playerReadyText;

        public override void OnStartServer()
        {
            base.OnStartServer();

            GameManager.Instance.ConnectedPlayers.Add(this);
        }

        public override void OnStopServer()
        {
            base.OnStopServer();

            GameManager.Instance.ConnectedPlayers.Remove(this);
        }

        public override void OnStartClient()
        {
            base.OnStartClient();

            GameObject playerList = GameObject.Find("PlayerList");
            if (!playerList)
            {
                Debug.LogError("Game object PlayerList is not found.");
            }
            else
            {
                
                _playerListItem = Instantiate(ResourceManager.Instance.PlayerListItemPrefab, playerList.transform.GetChild(0).GetChild(0));
                TMP_Text[] texts = _playerListItem.GetComponentsInChildren<TMP_Text>();
                if (texts == null || texts.Length < 2)
                {
                    Debug.LogError("Texts components in PlayerList game object are not found or not enough.");
                }
                else
                {
                    _playerNameText = texts[0];
                    _playerReadyText = texts[1];
                }
            }

            if (!IsOwner)
            {
                return;
            }

            Instance = this;

            RpcSetPlayerName($"Player_{OwnerId}");

            ViewManager.Instance.Initialize();
        }

        public override void OnStopClient()
        {
            base.OnStopClient();

            Destroy(_playerListItem);
        }

        public void StartGame()
        {
            GameObject dummy = Instantiate(ResourceManager.Instance.DummyPrefab);

            Spawn(dummy, Owner);

            ControlledDummy = dummy.GetComponent<Dummy>();

            ControlledDummy.ControllingPlayer = this;

            RpcDummySpawned(Owner);
        }

        public void StopGame()
        {
            if (ControlledDummy != null && ControlledDummy.IsSpawned)
            {
                ControlledDummy.Despawn();
            }
        }

        private void OnChangePlayerName(string prev, string next, bool asServer)
        {
            if (!asServer)
            {
                _playerNameText.text = Name;
            }
        }

        private void OnChangePlayerReady(bool prev, bool next, bool asServer)
        {
            if (!asServer)
            {
                _playerReadyText.color = Ready ? Color.green : Color.red;
            }
        }

        private void OnChangePlayerSkill(string prev, string next, bool asServer)
        {
            if (!asServer)
            {
                ViewManager.Instance.Get<LobbyView>().UpdateSkillSlots();
            }
        }

        [ServerRpc]
        public void RpcToggleReadyState()
        {
            Ready = !Ready;
        }

        [ServerRpc]
        public void RpcSetPlayerName(string newName)
        {
            Name = newName;
        }

        [ServerRpc]
        public void RpcSetPlayerSkill(int slot, string skillName)
        {
            if (slot == 1)
            {
                SkillOneName = skillName;
            }
            else
            {
                SkillTwoName = skillName;
            }
        }

        [TargetRpc]
        private void RpcDummySpawned(NetworkConnection networkConnection)
        {
            ViewManager.Instance.Show<PrematchView>();
        }
    }
}
