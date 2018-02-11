using UnityEngine;

namespace TheLastWizard {
    public class ARCameraWrapper : MonoBehaviour {
        [SerializeField] CameraInputSource inputSource;

        bool initialized;

        const string AR_CAMERA_NAME = "Video background";
        const string VIDEO_SOURCE_NAME = "Video source 0";

        void OnMarkerFound(ARMarker marker) {
            Initialize();
            inputSource.MarkerFound();
        }

        void OnMarkerTracked(ARMarker marker) { }

        void OnMarkerLost(ARMarker marker) {
            inputSource.MarkerLost();
        }

        void Initialize() {
            if (initialized)
                return;
            initialized = true;

            FixARCamera();
            FixVideoSource();
        }

        void FixARCamera() {
            FindComponent<Camera>(AR_CAMERA_NAME).clearFlags = CameraClearFlags.Depth;
        }

        void FixVideoSource() {
            FindComponent<Renderer>(VIDEO_SOURCE_NAME).enabled = false;
        }

        T FindComponent<T>(string objectName) {
            return GameObject.Find(objectName).GetComponent<T>();
        }
    }
}