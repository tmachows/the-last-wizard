using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GestureInputReader : MonoBehaviour {
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


    void Start () {
        _IsRecording = false;
        _Dollar = new DollarWrapper();
        _SpellCaster = GetComponent<SpellCaster>();
        _Camera = Camera.main;
        _Line = gameObject.GetComponent<LineRenderer>();
    }
	
	void Update () {
        if(_IsRecording)
        {
            if (Input.GetButtonUp("Fire1"))
            {
                _IsRecording = false;
                Result res = _Dollar.Recognize(_Points);
                _SpellCaster.CastSpell(res);
                ClearLine();
            } else
            {
                Point p;
                p.x = Input.mousePosition.x;
                p.y = Input.mousePosition.y * (-1);
                _Points.AddLast(p);
                DrawLine();
            }
        } else if (Input.GetButtonDown("Fire1"))
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
