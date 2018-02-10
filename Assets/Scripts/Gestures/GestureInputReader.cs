using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace TheLastWizard
{
    public class GestureInputReader : MonoBehaviour
    {
        private LinkedList<Point> _Points;
        private bool _IsRecording;
        private DollarWrapper _Dollar;
        private SpellCaster _SpellCaster;

        #region shape drawing fields

        private List<Vector3> _LinePoints = new List<Vector3>();
        private LineRenderer _Line;
        private Camera _Camera;
        private Vector3 _LastPos = Vector3.one * float.MaxValue;
        private float _Threshold = 0.001f;
        private int _LineCount = 0;

        #endregion

        #region device camera fields

        private bool _CamAvailable;
        private WebCamTexture _FrontCam;
        private Texture _DefaultBackground;

        public RawImage _Background;
        public AspectRatioFitter _Fit;

        #endregion

        void Start()
        {
            InitCameraInputReading();
            if (!_CamAvailable)
            {
                InitScreenInputReading();
            }
        }

        private void InitScreenInputReading()
        {
            _IsRecording = false;
            _Dollar = new DollarWrapper();
            _SpellCaster = GetComponent<SpellCaster>();
            _Camera = Camera.main;
            _Line = gameObject.GetComponent<LineRenderer>();
        }

        private void InitCameraInputReading()
        {
            _DefaultBackground = _Background.texture;
            WebCamDevice[] devices = WebCamTexture.devices;

            if (devices.Length == 0)
            {
                Debug.Log("No camera detected");
                _CamAvailable = false;
                return;
            }

            foreach (var webCamDevice in devices)
            {
                if (webCamDevice.isFrontFacing)
                {
                    _FrontCam = new WebCamTexture(webCamDevice.name, Screen.width, Screen.height);
                }
            }

            if (_FrontCam == null)
            {
                Debug.Log("Unable to find front camera");
                return;
            }

            _FrontCam.Play();
            _Background.texture = _FrontCam;

            _CamAvailable = true;
        }

        void Update()
        {
            if (_CamAvailable)
            {
                ReadCameraInput();
            }
            ReadScreenInput();
        }

        private void ReadCameraInput()
        {
            float ratio = (float) _FrontCam.width / (float) _FrontCam.height;
//            _Fit.aspectRatio = ratio;

            float scaleY = _FrontCam.videoVerticallyMirrored ? -1f : 1f;
            _Background.rectTransform.localScale = new Vector3(1f, scaleY, 1f);

            int orient = - _FrontCam.videoRotationAngle;
            _Background.rectTransform.localEulerAngles = new Vector3(0, 0, orient);

            Debug.Log(_FrontCam.GetPixels().Length);
        }

        private void ReadScreenInput()
        {
            if (_IsRecording)
            {
                if (Input.GetButtonUp("Fire1"))
                {
                    _IsRecording = false;
                    Result res = _Dollar.Recognize(_Points);
                    _SpellCaster.CastSpell(res);
                    ClearLine();
                }
                else
                {
                    Point p;
                    p.x = Input.mousePosition.x;
                    p.y = Input.mousePosition.y * (-1);
                    _Points.AddLast(p);
                    DrawLine();
                }
            }
            else if (Input.GetButtonDown("Fire1"))
            {
                Point p;
                p.x = Input.mousePosition.x;
                p.y = Input.mousePosition.y * (-1);
                if (!_IsRecording)
                {
                    _IsRecording = true;
                    _Points = new LinkedList<Point>();
                }
                _Points.AddLast(p);
            }
        }

        #region shape drawing functions

        void DrawLine()
        {
            Vector3 mousePos = Input.mousePosition;
            mousePos.z = _Camera.nearClipPlane;
            Vector3 mouseWorld = _Camera.ScreenToWorldPoint(mousePos);
            mouseWorld.z = _Camera.nearClipPlane + mouseWorld.y * 0.01f;

            float dist = Vector3.Distance(_LastPos, mouseWorld);
            if (dist <= _Threshold)
                return;

            _LastPos = mouseWorld;
            if (_LinePoints == null)
                _LinePoints = new List<Vector3>();
            mouseWorld = mouseWorld + Vector3.forward * 7f + Vector3.down * 0.1f;
            _LinePoints.Add(mouseWorld);

            UpdateLine();
        }


        void UpdateLine()
        {
            _Line.startWidth = 0.01f;
            _Line.endWidth = 0.01f;

            _Line.positionCount = _LinePoints.Count;

            for (int i = _LineCount; i < _LinePoints.Count; i++)
            {
                _Line.SetPosition(i, _LinePoints[i]);
            }
            _LineCount = _LinePoints.Count;
        }

        void ClearLine()
        {
            _LinePoints.Clear();
            UpdateLine();
        }

        #endregion
    }
}
