using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace StormDreams
{
    [CreateAssetMenu(menuName = "Skill/Movement/Flash")]
    public class FlashSkill : Skill
    {
        [Header("Specific")]
        [SerializeField]
        private float _flashDistance = 2.5f;

        private float _wallOffset = 0.05f;
        private Vector2 _fieldUpLeftCorner = new Vector2(-10.0f, 5.0f);
        private Vector2 _fieldDownRightCorner = new Vector2(10.0f, -5.0f);

        public override void Activate(GameObject parent, Vector3 direction, bool skillInput, float delta)
        {
            DummyMovement dummyMovement = parent.GetComponent<DummyMovement>();

            Vector3 newPosition = parent.transform.position + direction * _flashDistance;

            if (newPosition.x < _fieldUpLeftCorner.x + dummyMovement.Controller.radius / 2.0f + _wallOffset)
            {
                newPosition.x = _fieldUpLeftCorner.x + dummyMovement.Controller.radius / 2.0f + _wallOffset;
            }
            else if (newPosition.x > _fieldDownRightCorner.x - dummyMovement.Controller.radius / 2.0f - _wallOffset)
            {
                newPosition.x = _fieldDownRightCorner.x - dummyMovement.Controller.radius / 2.0f - _wallOffset;
            }

            if (newPosition.z > _fieldUpLeftCorner.y - dummyMovement.Controller.radius / 2.0f - _wallOffset)
            {
                newPosition.z = _fieldUpLeftCorner.y - dummyMovement.Controller.radius / 2.0f - _wallOffset;
            }
            else if (newPosition.z < _fieldDownRightCorner.y + dummyMovement.Controller.radius / 2.0f + _wallOffset)
            {
                newPosition.z = _fieldDownRightCorner.y + dummyMovement.Controller.radius / 2.0f + _wallOffset;
            }

            dummyMovement.RpcSetDummyPositionAndRotation(newPosition, parent.transform.rotation);
        }
    }
}
