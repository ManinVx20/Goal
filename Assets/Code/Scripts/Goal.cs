using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace StormDreams
{
    public class Goal : MonoBehaviour
    {
        [SerializeField]
        private int _player;

        private void OnTriggerEnter(Collider other)
        {
            Rigidbody rb = other.attachedRigidbody;

            if (rb != null && other.CompareTag("Ball"))
            {
                MatchManager.Instance.HandleGoal(_player);
            }
        }
    }
}
