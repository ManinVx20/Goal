using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace StormDreams
{
    [CreateAssetMenu(menuName = "Skill/Movement/Dash")]
    public class DashSkill : Skill
    {
        [Header("Specific")]
        [SerializeField]
        private float _dashSpeed = 15.0f;

        public override void Activate(GameObject parent, Vector3 direction, bool active, float delta)
        {
            DummyMovement dummyMovement = parent.GetComponent<DummyMovement>();

            dummyMovement.CanMove = false;
        }

        public override void OnActive(GameObject parent, Vector3 direction, bool active, float delta)
        {
            CharacterController controller = parent.GetComponent<CharacterController>();

            controller.Move(direction * _dashSpeed * delta);
        }

        public override void BeginCooldown(GameObject parent, Vector3 direction, bool active, float delta)
        {
            DummyMovement dummyMovement = parent.GetComponent<DummyMovement>();

            dummyMovement.CanMove = true;
        }
    }
}
