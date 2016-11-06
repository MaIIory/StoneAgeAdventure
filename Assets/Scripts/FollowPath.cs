using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class FollowPath : MonoBehaviour {

	public enum FollowType
	{
		MoveTowards,
		Lerp
	}

	public FollowType Type = FollowType.MoveTowards;
	public PathDefinition Path;
	public float Speed = 5;
	public float MaxDistanceToGoal = 0.1f;

	private IEnumerator<Transform> _currentPoint;

	public void Start()
	{

		if (Path == null) {
			Debug.LogError ("Path cannot be null", gameObject);
			return;
		}
		 
		_currentPoint = Path.GetPathEnumerator (); //Note: it not executes GetPathEnumerator
		_currentPoint.MoveNext (); //Trigger first exeution of iterator block

		if (_currentPoint == null)
			return;

		transform.position = _currentPoint.Current.position;
	}

	public void Update()
	{
		if (_currentPoint == null || _currentPoint.Current == null)
			return;

		if (Type == FollowType.MoveTowards)
			transform.position = Vector3.MoveTowards (transform.position, _currentPoint.Current.position, Time.deltaTime * Speed);
		else if (Type == FollowType.Lerp)
			transform.position = Vector3.Lerp (transform.position, _currentPoint.Current.position, Time.deltaTime * Speed);

		float distance = (transform.position - _currentPoint.Current.position).magnitude;

		if (distance <= MaxDistanceToGoal)
			_currentPoint.MoveNext ();

		/* Code block presented below is optimized version of above - we avoid calculation of square root 
		float distance = (transform.position - _currentPoint.Current.position)sqrMmagnitude;

		if (distance <= (MaxDistanceToGoal * MaxDistanceToGoal)) 
			_currentPoint.MoveNext ();
		 */ 
	}


}
