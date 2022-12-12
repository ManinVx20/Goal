using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace StormDreams
{
    public abstract class View : MonoBehaviour
    {
        private bool _isInitialized;
        public bool IsInitialized { get => _isInitialized; }

        public virtual void Initialize()
        {
            _isInitialized = true;
        }

        public virtual void Show()
        {
            gameObject.SetActive(true);
        }

        public virtual void Hide()
        {
            gameObject.SetActive(false);
        }
    }
}
