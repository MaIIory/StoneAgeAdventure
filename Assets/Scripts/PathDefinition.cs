using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PathDefinition : MonoBehaviour {

	//Container for points that creates the path
	public Transform[] Points;

	//This method is a special iterator block
	public IEnumerator<Transform> GetPathEnumerator(){

		if (Points == null || Points.Length < 2)
			yield break;

		int index = 0;
		int direction = 1;

		while (true) {

			yield return Points[index];

			if(index <= 0)
				direction = 1;
			else if(index >= Points.Length - 1)
				direction = -1;

			index += direction;
		}
	}

	//Method derived from Unity - it draws stuffs in design mode
	void OnDrawGizmos() {

		if (Points == null || Points.Length <= 1)
			return;

		for (int i = 1; i < Points.Length; i++) {
			Gizmos.color = Color.yellow;
			Gizmos.DrawLine(Points[i - 1].position, Points[i].position);
		}
	}
}
