using System.Collections.Generic;
using UnityEngine;

namespace TheLastWizard {
    public class ScreenInputSource : InputSource {
        LinkedList<Point> points;
        bool isRecording;

        void Update() {
            const string FIRE_1 = "Fire1";

            if (Input.GetButtonDown(FIRE_1))
                StartRecording();
            else if (Input.GetButtonUp(FIRE_1))
                StopRecording();
        }

        protected override Point GetCurrentInput() {
            Point p;
            p.x = Input.mousePosition.x;
            p.y = Input.mousePosition.y * (-1);
            return p;
        }
    }
}