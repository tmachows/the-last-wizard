using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GestureInputReader : MonoBehaviour {
    private LinkedList<Point> _Points;
    private bool _IsRecording;
    private DollarWrapper _Dollar;
    private SpellCaster _SpellCaster;

	void Start () {
        _IsRecording = false;
        _Dollar = new DollarWrapper();
        _SpellCaster = GetComponent<SpellCaster>();
	}
	
	void Update () {
        if(_IsRecording)
        {
            if (Input.GetButtonUp("Fire1"))
            {
                _IsRecording = false;
                Result res = _Dollar.Recognize(_Points);
                _SpellCaster.CastSpell(res);
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
