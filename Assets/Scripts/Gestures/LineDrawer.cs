using System.Collections.Generic;
using UnityEngine;

namespace TheLastWizard {
    public class LineDrawer : MonoBehaviour {
        [SerializeField] Camera drawingCamera;
        [SerializeField] LineRenderer line;
        [SerializeField] float threshold = 0.001f;

        Vector3 lastPosition = Vector3.one * float.MaxValue;
        List<Vector3> linePoints = new List<Vector3>();
        int lineCount = 0;

        void Awake() {
            if (drawingCamera == null)
                drawingCamera = Camera.main;
        }

        public void DrawLine(Point currentInput) {
            Vector3 inputPos = new Vector3(currentInput.x, currentInput.y * (-1), drawingCamera.nearClipPlane);
            Vector3 inputWorld = drawingCamera.ScreenToWorldPoint(inputPos);
            inputWorld.z = drawingCamera.nearClipPlane + inputWorld.y * 0.01f;

            float dist = Vector3.Distance(lastPosition, inputWorld);
            if (dist <= threshold)
                return;

            lastPosition = inputWorld;
            if (linePoints == null)
                linePoints = new List<Vector3>();
            inputWorld = inputWorld + Vector3.forward * 7f + Vector3.down * 0.1f;
            linePoints.Add(inputWorld);

            UpdateLine();
        }

        public void ClearLine() {
            linePoints.Clear();
            UpdateLine();
        }

        void UpdateLine() {
            line.startWidth = 0.01f;
            line.endWidth = 0.01f;

            line.positionCount = linePoints.Count;

            for (int i = lineCount; i < linePoints.Count; i++)
                line.SetPosition(i, linePoints[i]);
            lineCount = linePoints.Count;
        }
    }
}