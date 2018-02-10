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

        public void DrawLine() {
            Vector3 mousePos = Input.mousePosition;
            mousePos.z = drawingCamera.nearClipPlane;
            Vector3 mouseWorld = drawingCamera.ScreenToWorldPoint(mousePos);
            mouseWorld.z = drawingCamera.nearClipPlane + mouseWorld.y * 0.01f;

            float dist = Vector3.Distance(lastPosition, mouseWorld);
            if (dist <= threshold)
                return;

            lastPosition = mouseWorld;
            if (linePoints == null)
                linePoints = new List<Vector3>();
            mouseWorld = mouseWorld + Vector3.forward * 7f + Vector3.down * 0.1f;
            linePoints.Add(mouseWorld);

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