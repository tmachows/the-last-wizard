using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOverMessage
{
    public Component Sender;
}

public class GameOverController : MonoBehaviour {

    [SerializeField] public int SceneToLoad;

    private void Receive(GameOverMessage message)
    {
        Debug.Log("Game Over Message to GameOverController");

        var msg = new HowManyPointsMessage
        {
            Sender = this
        };
        MessageDispatcher.Send(msg, gameObject);

        PlayerPrefs.SetInt("Last Score", msg.Points);
        PlayerPrefs.SetInt("Last Killed Enemies", msg.KilledEnemies);

        SceneManager.LoadScene(SceneToLoad);
    }

}
