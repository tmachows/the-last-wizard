using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TheLastWizard {
    public class ScreenInputSource : InputSource {
        LinkedList<Point> points;
        bool isRecording;

        void Update() {
            if (Input.GetButtonDown("Fire1"))
                StartCoroutine(ScreenInputCoroutine());
        }

        public override void Initialize() {
            base.Initialize();
            isRecording = false;
        }

        IEnumerator ScreenInputCoroutine() {
            isRecording = true;
            points = new LinkedList<Point>();
            while (isRecording) {
                if (Input.GetButtonUp("Fire1")) {
                    isRecording = false;
                    gestureInputReader.InterpretInput(points);
                    lineDrawer.ClearLine();
                } else {
                    Point p;
                    p.x = Input.mousePosition.x;
                    p.y = Input.mousePosition.y * (-1);
                    points.AddLast(p);
                    lineDrawer.DrawLine();
                }
                yield return null;
            }
        }
    }
}