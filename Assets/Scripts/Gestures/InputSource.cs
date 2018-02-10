using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TheLastWizard {
    public abstract class InputSource : MonoBehaviour {
        [SerializeField] protected GestureInputReader gestureInputReader;
        [SerializeField] protected LineDrawer lineDrawer;

        LinkedList<Point> points;
        bool isRecording;

        public virtual void Initialize() {
            gameObject.SetActive(true);
        }

        public void StartRecording() {
            Debug.LogFormat("{0}: Starting recording", name);
            StartCoroutine(InputCoroutine());
        }

        public void StopRecording() {
            Debug.LogFormat("{0}: Stopping recording with {1} points", name, points.Count);
            isRecording = false;
            gestureInputReader.InterpretInput(points);
            lineDrawer.ClearLine();
        }

        IEnumerator InputCoroutine() {
            isRecording = true;
            points = new LinkedList<Point>();
            while (isRecording) {
                points.AddLast(GetCurrentInput());
                lineDrawer.DrawLine();
                yield return null;
            }
        }

        protected abstract Point GetCurrentInput();
    }
}