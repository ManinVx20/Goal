using System;
using System.Collections;
using System.Collections.Generic;
using FishNet.Object;
using UnityEngine;
using UnityEngine.InputSystem;

namespace StormDreams
{
    public class DummyInput : NetworkBehaviour
    {
        public Vector2 MovementInput { get; private set; }
        public bool JumpInput { get; private set; }
        public bool ActiveSkillOneInput { get; private set; }
        public bool ActiveSkillTwoInput { get; private set; }

        private GameControls gameControls;

        private void Awake()
        {
            gameControls = new GameControls();
        }

        private void OnEnable()
        {
            gameControls.Enable();

            gameControls.Player.Move.started += OnMove;
            gameControls.Player.Move.performed += OnMove;
            gameControls.Player.Move.canceled += OnMove;

            gameControls.Player.Jump.started += OnJump;
            gameControls.Player.Jump.performed += OnJump;
            gameControls.Player.Jump.canceled += OnJump;

            gameControls.Player.ActiveSkillOne.started += OnActiveSkillOne;
            gameControls.Player.ActiveSkillOne.performed += OnActiveSkillOne;
            gameControls.Player.ActiveSkillOne.canceled += OnActiveSkillOne;

            gameControls.Player.ActiveSkillTwo.started += OnActiveSkillTwo;
            gameControls.Player.ActiveSkillTwo.performed += OnActiveSkillTwo;
            gameControls.Player.ActiveSkillTwo.canceled += OnActiveSkillTwo;
        }

        private void OnDisable()
        {
            gameControls.Player.Move.started -= OnMove;
            gameControls.Player.Move.performed -= OnMove;
            gameControls.Player.Move.canceled -= OnMove;

            gameControls.Player.Jump.started -= OnJump;
            gameControls.Player.Jump.performed -= OnJump;
            gameControls.Player.Jump.canceled -= OnJump;

            gameControls.Player.ActiveSkillOne.started -= OnActiveSkillOne;
            gameControls.Player.ActiveSkillOne.performed -= OnActiveSkillOne;
            gameControls.Player.ActiveSkillOne.canceled -= OnActiveSkillOne;

            gameControls.Player.ActiveSkillTwo.started -= OnActiveSkillTwo;
            gameControls.Player.ActiveSkillTwo.performed -= OnActiveSkillTwo;
            gameControls.Player.ActiveSkillTwo.canceled -= OnActiveSkillTwo;

            gameControls.Disable();
        }

        private void OnMove(InputAction.CallbackContext context)
        {
            if (!IsOwner)
            {
                return;
            }

            if (context.started || context.performed)
            {
                MovementInput = context.ReadValue<Vector2>();
            }
            else if (context.canceled)
            {
                MovementInput = Vector2.zero;
            }
        }

        private void OnJump(InputAction.CallbackContext context)
        {
            if (!IsOwner)
            {
                return;
            }
            
            if (context.started || context.performed)
            {
                JumpInput = true;
            }
            else if (context.canceled)
            {
                JumpInput = false;
            }
        }

        private void OnActiveSkillOne(InputAction.CallbackContext context)
        {
            if (!IsOwner)
            {
                return;
            }
            
            if (context.started || context.performed)
            {
                ActiveSkillOneInput = true;
            }
            else if (context.canceled)
            {
                ActiveSkillOneInput = false;
            }
        }

        private void OnActiveSkillTwo(InputAction.CallbackContext context)
        {
            if (!IsOwner)
            {
                return;
            }
            
            if (context.started || context.performed)
            {
                ActiveSkillTwoInput = true;
            }
            else if (context.canceled)
            {
                ActiveSkillTwoInput = false;
            }
        }
    }
}
