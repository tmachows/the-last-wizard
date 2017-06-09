using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class KilledEnemyMessage
{
    public int Points;
}

public class TimeBonusMessage
{
    public int Points;
}

public class HowManyPointsMessage
{
    public Component Sender;
    public int Points;
    public int KilledEnemies;
}

public class ScoreMessage
{

}

public class PlayerScore : MonoBehaviour
{
    private int _Score = 0;
    private int _KilledEnemies = 0;
    [SerializeField]
    private Canvas _Canvas;
    private HudView _HUD;

    private void Awake()
    {
        PlayerPrefs.SetInt("Last Score", 0);
        PlayerPrefs.SetInt("Last Killed Enemies", 0);
        _HUD = _Canvas.GetComponent<HudView>();
    }

    private void Receive(KilledEnemyMessage message)
    {
        _Score += message.Points;
        _HUD.Score = _Score;
        _KilledEnemies++;
        _HUD.KilledEnemies = _KilledEnemies;
    }

    private void Receive(TimeBonusMessage message)
    {
        _Score += message.Points;
        _HUD.Score = _Score;
    }

    private void Receive(HowManyPointsMessage message)
    {
        Debug.Log("Player score received howmanypointsmessage");

        message.Points = _Score;
        message.KilledEnemies = _KilledEnemies;
    }

}
