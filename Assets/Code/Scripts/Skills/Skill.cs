using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace StormDreams
{
    public class Skill : ScriptableObject
    {
        [field: Header("Core")]
        [field: SerializeField]
        public Sprite Icon { get; private set; }
        [field: SerializeField]
        public string Name { get; private set; }
        [field: SerializeField]
        public float ActiveTime { get; private set; }
        [field: SerializeField]
        public float CooldownTime { get; private set; }
        [field: SerializeField]
        public bool NeedDirection { get; private set; }

        public virtual void Activate(GameObject parent, Vector3 direction, bool active, float delta) { }

        public virtual void OnActive(GameObject parent, Vector3 direction, bool active, float delta) { }

        public virtual void BeginCooldown(GameObject parent, Vector3 direction, bool active, float delta) { }
    }
}
