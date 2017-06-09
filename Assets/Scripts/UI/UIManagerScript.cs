using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class UIManagerScript : MonoBehaviour
{

    public int SceneToLoad;
    public bool Sound;
    public bool Vibrations;

    public void StartGame()
    {
        SceneManager.LoadScene(SceneToLoad);
    }

    public void toogleSound(bool value)
    {
        Sound = value;
    }

    public void toogleVibrations(bool value)
    {
        Vibrations = value;
    }

    public void QuitApplication()
    {
        //If we are running in a standalone build of the game
        //#if UNITY_STANDALONE
        //Quit the application
        Application.Quit();
        //#endif

        //If we are running in the editor
#if UNITY_EDITOR
        //Stop playing the scene
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}
