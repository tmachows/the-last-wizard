using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace TheLastWizard {
    public class GestureInputReader : MonoBehaviour {
        [SerializeField] ScreenInputSource screenInputSource;
        [SerializeField] CameraInputSource cameraInputSource;

        private DollarWrapper _Dollar;
        private SpellCaster _SpellCaster;

        void Start() {
            Initialize();
            if (CameraAvailable())
                InitCameraInputReading();
            InitScreenInputReading();
        }

        public void InterpretInput(LinkedList<Point> points) {
            var result = _Dollar.Recognize(points);
            _SpellCaster.CastSpell(result);
        }

        void Initialize() {
            _Dollar = new DollarWrapper();
            _SpellCaster = GetComponent<SpellCaster>();
        }

        bool CameraAvailable() {
            return WebCamTexture.devices.Length != 0;
        }

        void InitScreenInputReading() {
            screenInputSource.Initialize();
        }

        void InitCameraInputReading() {
            cameraInputSource.Initialize();
        }
    }
}
