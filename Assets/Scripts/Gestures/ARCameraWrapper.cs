using UnityEngine;

namespace TheLastWizard {
    public class ARCameraWrapper : MonoBehaviour {
        [SerializeField] CameraInputSource inputSource;

        void OnMarkerFound(ARMarker marker) {
            inputSource.MarkerFound();
        }

        void OnMarkerTracked(ARMarker marker) { }

        void OnMarkerLost(ARMarker marker) {
            inputSource.MarkerLost();
        }
    }
}