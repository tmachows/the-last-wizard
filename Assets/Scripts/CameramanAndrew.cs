using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameramanAndrew : MonoBehaviour {
    private LinkedList<Point> _Points;
    private bool _IsRecording;
    private DollarWrapper _Dollar;

	void Start () {
        _IsRecording = false;
        _Dollar = new DollarWrapper();
	}
	
	void Update () {
        if(_IsRecording)
        {
            if (Input.GetButtonUp("Fire1"))
            {
                _IsRecording = false;
                Result res = _Dollar.Recognize(_Points);
                if (res.matched)
                {
                    Debug.Log("result: " + res.score);
                    Debug.Log("pattern: " + res.name);
                }
                else
                {
                    Debug.Log("Match failed");
                }
            } else
            {

                Point p;
                p.x = Input.mousePosition.x;
                p.y = Input.mousePosition.y * (-1);
                _Points.AddLast(p);

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
}
