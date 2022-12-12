using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace StormDreams
{
    public class SafeAreaPanel : MonoBehaviour
    {
        [SerializeField]
        private Canvas _canvas;

        private RectTransform _panelSafeArea;

        private Rect _currentSafeArea = new();
        private ScreenOrientation _currentOrientation = ScreenOrientation.AutoRotation;

        private void Awake()
        {
            _panelSafeArea = GetComponent<RectTransform>();
        }

        private void Start()
        {
            _currentOrientation = Screen.orientation;
            _currentSafeArea = Screen.safeArea;

            ApplySafeArea();
        }

        private void Update()
        {
            if (_currentOrientation != Screen.orientation || _currentSafeArea != Screen.safeArea)
            {
                ApplySafeArea();
            }
        }

        private void ApplySafeArea()
        {
            if (_panelSafeArea == null)
            {
                return;
            }

            Rect safeArea = Screen.safeArea;

            Vector2 anchorMin = safeArea.position;
            Vector2 anchorMax = safeArea.position + safeArea.size;

            anchorMin.x /= _canvas.pixelRect.width;
            anchorMin.y /= _canvas.pixelRect.height;

            anchorMax.x /= _canvas.pixelRect.width;
            anchorMax.y /= _canvas.pixelRect.height;

            _panelSafeArea.anchorMin = anchorMin;
            _panelSafeArea.anchorMax = anchorMax;

            _currentOrientation = Screen.orientation;
            _currentSafeArea = Screen.safeArea;
        }
    }
}
