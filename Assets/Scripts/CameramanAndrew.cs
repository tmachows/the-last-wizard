using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameramanAndrew : MonoBehaviour {
    private LinkedList<Point> _Points;
    private bool _IsRecording;
    private DollarWrapper _Dollar;

    [SerializeField]
    private float _GestureResolution;

    private float _GestureResolutionCounter;

	// Use this for initialization
	void Start () {
        _IsRecording = false;
        _Dollar = new DollarWrapper();
	}
	
	// Update is called once per frame
	void Update () {
        if(_IsRecording)
        {
            if (Input.GetButtonUp("Fire1"))
            {
                Debug.Log("Stopping recording for " + _Points.Count);
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
            } else //if(_GestureResolutionCounter <= 0f)
            {
                _GestureResolutionCounter = _GestureResolution;
                Point p;
                p.x = Input.mousePosition.x;
                p.y = Input.mousePosition.y;
                _Points.AddLast(p);

            } /*else
            {
                _GestureResolutionCounter -= Time.deltaTime;
            }*/
        } else if (Input.GetButtonDown("Fire1"))
        {
            Point p;
            p.x = Input.mousePosition.x;
            p.y = Input.mousePosition.y;
            if (!_IsRecording)
            {
                _GestureResolutionCounter = _GestureResolution;
                Debug.Log("starting recording");
                _IsRecording = true;
                _Points = new LinkedList<Point>();
            }
            _Points.AddLast(p);
        } 
    }
}
