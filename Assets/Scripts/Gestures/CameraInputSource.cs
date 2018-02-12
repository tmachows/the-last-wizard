using System;
using System.Collections;
using UnityEngine;

namespace TheLastWizard {
    public class CameraInputSource : InputSource {
        [SerializeField] Transform trackedObject;
        [SerializeField] Camera aRCamera;

        Func<Point, Point> trans;

        void Awake() {
            int centerX = Screen.width / 2;
            int centerY = Screen.height / 2;
            trans = point => {
                Point result;
                result.x = (point.x - centerX) * 2 + centerX;
                result.y = (point.y + centerY) * 2 - centerY;
                return result;
            };
        }

        public void MarkerFound() {
            StartRecording();
        }

        public void MarkerLost() {
            StopRecording();
        }

        protected override Point GetCurrentInput() {
            var screenPosition = aRCamera.WorldToScreenPoint(trackedObject.position);
            Point point;
            point.x = screenPosition.x;
            point.y = screenPosition.y * (-1);
            return trans(point);
        }
    }
}