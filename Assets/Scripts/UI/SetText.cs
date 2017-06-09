using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SetText : MonoBehaviour
{
    [SerializeField] private Text _TextFiled;
    [SerializeField] private string _PrefsName;
    [SerializeField] private string _Text;

    void Start()
    {
        int points = PlayerPrefs.GetInt(_PrefsName);
        _TextFiled.text = _Text + points;
    }
}
