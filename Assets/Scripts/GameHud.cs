using UnityEngine;
using System.Collections;

public class GameHud : MonoBehaviour {
	
	public GUISkin Skin;
	

	public void OnGUI(){

		GUI.skin = Skin;


		GUILayout.BeginArea (new Rect(0, 0, Screen.width, Screen.height));
		{
			GUILayout.BeginVertical(Skin.GetStyle("GameHud"));
			{
				GUILayout.Label(string.Format("Points: {0}",GameManager.Instance.Points), Skin.GetStyle("PointsText"));

				GUILayout.Label(string.Format("{0:00}:{1:00} with {2} bonus",
				                              LevelManager.Instance.RunningTime.Minutes + (LevelManager.Instance.RunningTime.Hours * 60),
				                              LevelManager.Instance.RunningTime.Seconds,
				                              LevelManager.Instance.CurrentTimeBonus),Skin.GetStyle("TimeText"));
			}
			GUILayout.EndVertical();
		}
		GUILayout.EndArea();

	}
}
