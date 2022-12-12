using System.Collections;
using System.Collections.Generic;
using FishNet.Object;
using UnityEngine;

namespace StormDreams
{
    public class MeshTrail : MonoBehaviour
    {
        [Header("Mesh Related")]
        [SerializeField]
        private Mesh _mesh;
        [SerializeField]
        private float _meshRefreshRate = 0.1f;
        [SerializeField]
        private Transform _spawnTransform;

        [Header("Shader Related")]
        [SerializeField]
        private Material _material;
        [SerializeField]
        private string _shaderVarRef = "_Alpha";
        [SerializeField]
        private float _shaderVarRate = 0.1f;
        [SerializeField]
        private float _shaderVarRefreshRate = 0.05f;

        private Transform _dynamicTransform;

        public void SpawnTrail(float activeTime)
        {
            StartCoroutine(ActivateTrailCoroutine(activeTime));
        }

        private IEnumerator ActivateTrailCoroutine(float activeTime)
        {
            while (activeTime > 0.0f)
            {
                activeTime -= _meshRefreshRate;

                if (_dynamicTransform == null)
                {
                    _dynamicTransform = GameObject.Find("_Dynamic").transform;
                }

                GameObject go = new();
                go.name = "MeshTrail";
                go.transform.SetParent(_dynamicTransform);
                go.transform.SetPositionAndRotation(_spawnTransform.position, _spawnTransform.rotation);

                MeshFilter mf = go.AddComponent<MeshFilter>();
                mf.mesh = _mesh;

                MeshRenderer mr = go.AddComponent<MeshRenderer>();
                mr.material = _material;
                mr.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;

                StartCoroutine(AnimateMaterialFloat(go, mr.material, 0.0f, _shaderVarRate, _shaderVarRefreshRate));

                yield return new WaitForSeconds(_meshRefreshRate);
            }
        }

        private IEnumerator AnimateMaterialFloat(GameObject go, Material mat, float goal, float rate, float refreshRate)
        {
            float animateValue = mat.GetFloat(_shaderVarRef);

            while (animateValue > goal)
            {
                animateValue -= rate;
                mat.SetFloat(_shaderVarRef, animateValue);

                yield return new WaitForSeconds(refreshRate);
            }

            Destroy(go);
        }
    }
}
