using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace StormDreams
{
    [CreateAssetMenu(menuName = "Skill/Movement/Ghost")]
    public class GhostSkill : Skill
    {
        [Header("Specific")]
        [SerializeField]
        private float _ghostSpeedMultiplier = 1.5f;
        
        public override void Activate(GameObject parent, Vector3 direction, bool active, float delta)
        {
            DummyMovement dummyMovement = parent.GetComponent<DummyMovement>();

            dummyMovement.BaseSpeedMultiplier = _ghostSpeedMultiplier;

            dummyMovement.RpcSpawnTrail(ActiveTime);
        }

        public override void BeginCooldown(GameObject parent, Vector3 direction, bool active, float delta)
        {
            DummyMovement dummyMovement = parent.GetComponent<DummyMovement>();

            dummyMovement.BaseSpeedMultiplier = 1.0f;
        }
    }
}
