using System;
using System.Collections;
using UnityEngine;

namespace TheLastWizard {
    public class CameraInputSource : InputSource {
        [SerializeField] Transform trackedObject;
        [SerializeField] Transform aRCamera;

        public void MarkerFound() {
            StartRecording();
        }

        public void MarkerLost() {
            StopRecording();
        }

        protected override Point GetCurrentInput() {
            var positionDiff = trackedObject.position - aRCamera.position;
            Debug.Log(positionDiff);

            Point point;//TODO
            point.x = 0;
            point.y = 0;
            return point;
        }
    }
}