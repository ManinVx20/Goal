using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace StormDreams
{
    public class ViewManager : MonoBehaviour
    {
        public static ViewManager Instance;

        [SerializeField]
        private View[] _views;
        [SerializeField]
        private View _defaultView;
        [SerializeField]
        private bool _localInitialize = false;

        private View currentView;

        private void Awake()
        {
            Instance = this;

            if (_localInitialize)
            {
                Initialize();
            }
        }

        public void Initialize()
        {
            foreach (View view in _views)
            {
                view.Initialize();
                view.Hide();
            }

            if (_defaultView != null)
            {
                currentView = _defaultView;
                currentView.Show();
            }
        }

        public void Show<T>(bool hideCurrentView = true) where T : View
        {
            foreach (View view in _views)
            {
                if (view is not T)
                {
                    continue;
                }

                if (currentView != null && hideCurrentView)
                {
                    currentView.Hide();
                }

                currentView = view;
                currentView.Show();

                return;
            }
        }

        public T Get<T>() where T : View
        {
            foreach (View view in _views)
            {
                if (view is T)
                {
                    return view as T;
                }
            }

            return null;
        }
    }
}
