using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace StormDreams
{
    public class RotationLoading : MonoBehaviour
    {
        [SerializeField]
        private float _timeStep = 0.05f;
        [SerializeField]
        private float _oneStepAngle = -36.0f;

        private float _startTime;

        private void Start()
        {
            _startTime = Time.time;
        }

        private void Update()
        {
            if (Time.time - _startTime >= _timeStep)
            {
                Vector3 newAngles = transform.localEulerAngles;
                newAngles.z += _oneStepAngle;
                transform.localEulerAngles = newAngles;
                _startTime = Time.time;
            }
        }
    }
}
