using System.Collections;
using System.Collections.Generic;
using FishNet.Connection;
using FishNet.Object;
using FishNet.Object.Synchronizing;
using UnityEngine;

namespace StormDreams
{
    public class Dummy : NetworkBehaviour
    {
        [SyncVar]
        public Player ControllingPlayer;

        private DummyMovement _dummyMovement;

        private Vector3 _startPosition;
        private Quaternion _startRotation;

        private void Awake()
        {
            _dummyMovement = GetComponent<DummyMovement>();
        }

        public void SetDummyStartPositionAndRotation(Vector3 position, Quaternion rotation)
        {
            _startPosition = position;
            _startRotation = rotation;

            ResetDummyStartPositionAndRotation();
        }

        public void ResetDummyStartPositionAndRotation()
        {
            _dummyMovement.RpcSetDummyPositionAndRotation(_startPosition, _startRotation);
        }

        public void SetDummyControl(bool canControl)
        {
            _dummyMovement.RpcSetDummyControl(Owner, canControl);
        }
    }
}
