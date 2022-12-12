using System.Collections;
using System.Collections.Generic;
using FishNet;
using FishNet.Object;
using FishNet.Object.Prediction;
using UnityEngine;

namespace StormDreams
{
    public class BallMovement : NetworkBehaviour
    {
        public struct ReconcileData
        {
            public Vector3 Position;
            public Quaternion Rotation;
            public Vector3 Velocity;
            public Vector3 AngularVelocity;
        }

        public struct MoveData { }

        private Rigidbody _rigidbody;

        private void Awake()
        {
            _rigidbody = GetComponent<Rigidbody>();
        }

        public override void OnStartNetwork()
        {
            base.OnStartNetwork();

            if (IsServer)
            {
                TimeManager.OnTick += TimeManager_OnTick;
                TimeManager.OnPostTick += TimeManager_OnPostTick;
            }
        }

        public override void OnStopNetwork()
        {
            base.OnStopNetwork();

            if (TimeManager != null)
            {
                TimeManager.OnTick -= TimeManager_OnTick;
                TimeManager.OnPostTick -= TimeManager_OnPostTick;
            }
        }

        private void TimeManager_OnTick()
        {
            if (IsOwner)
            {
                Reconciliation(default, false);
                Move(new MoveData(), false);
            }

            if (IsServer)
            {
                Move(default, true);
            }
        }

        private void TimeManager_OnPostTick()
        {
            if (IsServer)
            {
                ReconcileData reconcileData = new ReconcileData
                {
                    Position = transform.position,
                    Rotation = transform.rotation,
                    Velocity = _rigidbody.velocity,
                    AngularVelocity = _rigidbody.angularVelocity
                };
                Reconciliation(reconcileData, true);
            }
        }

        [Replicate]
        private void Move(MoveData moveData, bool asServer, bool replaying = false)
        {

        }

        [Reconcile]
        private void Reconciliation(ReconcileData reconcileData, bool asServer)
        {
            transform.position = reconcileData.Position;
            transform.rotation = reconcileData.Rotation;
            _rigidbody.velocity = reconcileData.Velocity;
            _rigidbody.angularVelocity = reconcileData.AngularVelocity;
        }
    }
}
