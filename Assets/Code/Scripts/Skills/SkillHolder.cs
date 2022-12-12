using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace StormDreams
{
    public class SkillHolder : MonoBehaviour
    {
        public enum SkillState
        {
            Ready,
            Active,
            Cooldown
        }

        [SerializeField]
        public Skill Skill;

        private SkillState _skillState = SkillState.Ready;
        private float _activeTime;
        private float _cooldownTime;

        public void UpdateSkillState(GameObject parent, Vector3 movementDirection, bool active, float delta)
        {
            if (Skill.NeedDirection && movementDirection == Vector3.zero)
            {
                return;
            }

            switch (_skillState)
            {
                case SkillState.Ready:
                    if (active)
                    {
                        Skill.Activate(parent, movementDirection, active, delta);
                        _skillState = SkillState.Active;
                        _activeTime = Skill.ActiveTime;
                    }
                    break;
                case SkillState.Active:
                    if (_activeTime > 0.0f)
                    {
                        _activeTime -= delta;
                        Skill.OnActive(parent, movementDirection, active, delta);
                    }
                    else
                    {
                        Skill.BeginCooldown(parent, movementDirection, active, delta);
                        _skillState = SkillState.Cooldown;
                        _cooldownTime = Skill.CooldownTime;
                    }
                    break;
                case SkillState.Cooldown:
                    if (_cooldownTime > 0.0f)
                    {
                        _cooldownTime -= delta;
                    }
                    else
                    {
                        _skillState = SkillState.Ready;
                    }
                    break;
            }
        }
    }
}
