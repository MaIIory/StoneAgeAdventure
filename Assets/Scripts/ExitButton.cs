using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class ExitButton : MonoBehaviour {

    public void onClick()
    {
        //If we are running in a standalone build of the game
		Application.Quit();

        //If we are running in the editor
//#if UNITY_EDITOR
        //Stop playing the scene
  //      UnityEditor.EditorApplication.isPlaying = false;
//#endif
    }
}
