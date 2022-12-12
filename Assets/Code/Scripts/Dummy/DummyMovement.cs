using System;
using System.Collections;
using FishNet.Connection;
using FishNet.Managing.Timing;
using FishNet.Object;
using FishNet.Object.Prediction;
using UnityEngine;

namespace StormDreams
{
    public class DummyMovement : NetworkBehaviour
    {
        public struct MoveData
        {
            public float Horizontal;
            public float Vertical;
            public bool Jump;
            public bool ActiveSkillOne;
            public bool ActiveSkillTwo;
        }

        public struct ReconcileData
        {
            public Vector3 Position;
            public Quaternion Rotation;
            public Vector3 Velocity;
            public bool IsGrounded;
        }

        [Header("Movement")]
        public bool CanMove = true;
        public float BaseSpeedMultiplier = 1.0f;
        [SerializeField]
        private float _baseSpeed = 5.0f;
        [SerializeField]
        private float _rotationSpeed = 720.0f;

        [Header("Ground Check")]
        [SerializeField]
        private bool _isGrounded;
        [SerializeField]
        private Transform _groundCheckTransform;
        [SerializeField]
        private float _groundCheckRadius = 0.1f;
        [SerializeField]
        private LayerMask _groundLayers;

        [Header("Jump")]
        [SerializeField]
        private float _gravityValue = -50.0f;
        [SerializeField]
        private float _jumpHeight = 1.0f;

        [Header("Push")]
        public float BasePushForceMultiplier = 1.0f;
        [SerializeField]
        private float _basePushForce = 1.0f;

        [Header("Skills")]
        [SerializeField]
        private SkillHolder _skillOneHolder;
        [SerializeField]
        private SkillHolder _skillTwoHolder;


        public CharacterController Controller { get; private set; }

        private Dummy _dummy;
        private DummyInput _input;
        private MeshTrail _trail;

        private MoveData _clientMoveData;
        private Vector3 _movementDirection;
        private Vector3 _velocity;
        private bool _jumpQueued;
        private bool _activeSkillOneQueued;
        private bool _activeSkillTwoQueued;

        private bool _canControl = true;

        private void Awake()
        {
            Controller = GetComponent<CharacterController>();

            _dummy = GetComponent<Dummy>();
            _input = GetComponent<DummyInput>();
            _trail = GetComponent<MeshTrail>();
        }

        private void Start()
        {
            if (IsServer || IsClient)
            {
                TimeManager.OnTick += TimeManager_OnTick;
            }

            _skillOneHolder.Skill = ResourceManager.Instance.Skills[_dummy.ControllingPlayer.SkillOneName];
            _skillTwoHolder.Skill = ResourceManager.Instance.Skills[_dummy.ControllingPlayer.SkillTwoName];
        }

        private void OnDestroy()
        {
            if (TimeManager != null)
            {
                TimeManager.OnTick -= TimeManager_OnTick;
            }
        }

        private void Update()
        {
            if (!IsOwner)
            {
                return;
            }

            _jumpQueued |= _input.JumpInput;
            _activeSkillOneQueued |= _input.ActiveSkillOneInput;
            _activeSkillTwoQueued |= _input.ActiveSkillTwoInput;
        }

        private void TimeManager_OnTick()
        {
            if (IsOwner)
            {
                Reconcile(default, false);

                GatherInputs(out MoveData moveData);

                Move(moveData, false);
            }

            if (IsServer)
            {
                Move(default, true);

                ReconcileData rd = new ReconcileData()
                {
                    Position = transform.position,
                    Rotation = transform.rotation,
                    Velocity = _velocity,
                    IsGrounded = _isGrounded
                };

                Reconcile(rd, true);
            }
        }

        private void GatherInputs(out MoveData md)
        {
            md = default;
            md.Horizontal = _input.MovementInput.x;
            md.Vertical = _input.MovementInput.y;
            md.Jump = _jumpQueued;
            md.ActiveSkillOne = _activeSkillOneQueued;
            md.ActiveSkillTwo = _activeSkillTwoQueued;

            _jumpQueued = false;
            _activeSkillOneQueued = false;
            _activeSkillTwoQueued = false;
        }

        [Replicate]
        private void Move(MoveData md, bool asServer, bool replaying = false)
        {
            float delta = (float)TimeManager.TickDelta;

            if (!_canControl)
            {
                return;
            }

            if (NetworkManager.ServerManager.OneServerStarted())
            {
                _skillOneHolder?.UpdateSkillState(gameObject, _movementDirection, md.ActiveSkillOne, delta);
                _skillTwoHolder?.UpdateSkillState(gameObject, _movementDirection, md.ActiveSkillTwo, delta);
            }

            if (!CanMove)
            {
                return;
            }

            GroundCheck();

            if (_isGrounded && _velocity.y < 0.0f)
            {
                _velocity.y = -2.0f;
            }

            if (md.Jump && _isGrounded)
            {
                _velocity.y = Mathf.Sqrt(_jumpHeight * -3.0f * _gravityValue * BaseSpeedMultiplier);
            }

            _velocity.y += _gravityValue * BaseSpeedMultiplier * delta;

            _movementDirection = new Vector3(md.Horizontal, 0.0f, md.Vertical).normalized;

            _velocity.x = _movementDirection.x * _baseSpeed * BaseSpeedMultiplier;
            _velocity.z = _movementDirection.z * _baseSpeed * BaseSpeedMultiplier;

            if (_movementDirection != Vector3.zero)
            {
                Quaternion targetRotation = Quaternion.LookRotation(_movementDirection, Vector3.up);
                transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, _rotationSpeed * delta);
            }

            Controller.Move(_velocity * delta);
        }

        [Reconcile]
        private void Reconcile(ReconcileData recData, bool asServer)
        {
            transform.position = recData.Position;
            transform.rotation = recData.Rotation;
            _velocity = recData.Velocity;
            _isGrounded = recData.IsGrounded;
        }

        private void GroundCheck()
        {
            _isGrounded = Physics.CheckSphere(_groundCheckTransform.position, _groundCheckRadius, _groundLayers, QueryTriggerInteraction.Ignore);
        }

        private void OnControllerColliderHit(ControllerColliderHit hit)
        {
            if (TimeManager.IsReplaying())
            {
                return;
            }

            Rigidbody rb = hit.collider.attachedRigidbody;

            if (rb != null)
            {
                Vector3 forceDirection = hit.gameObject.transform.position - transform.position;
                // forceDirection.y = 0.0f;
                forceDirection.Normalize();

                rb.AddForceAtPosition(forceDirection * _basePushForce * BasePushForceMultiplier, hit.gameObject.transform.position, ForceMode.Impulse);
            }
        }

        [TargetRpc(RunLocally = true)]
        public void RpcSetDummyControl(NetworkConnection conn, bool canControl)
        {
            _canControl = canControl;
        }

        [ObserversRpc(RunLocally = true)]
        public void RpcSetDummyPositionAndRotation(Vector3 position, Quaternion rotation)
        {
            Controller.enabled = false;

            transform.SetPositionAndRotation(position, rotation);

            Controller.enabled = true;

            ClearReplicateCache();
        }

        [ObserversRpc(RunLocally = true)]
        public void RpcSpawnTrail(float activeTime)
        {
            _trail.SpawnTrail(activeTime);
        }
    }
}
