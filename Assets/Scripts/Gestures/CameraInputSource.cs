using System;
using System.Collections;

namespace TheLastWizard {
    public class CameraInputSource : InputSource {
        public void MarkerFound() {
            StartRecording();
        }

        public void MarkerLost() {
            StopRecording();
        }

        protected override Point GetCurrentInput() {
            Point point;//TODO
            point.x = 0;
            point.y = 0;
            return point;
        }
    }
}