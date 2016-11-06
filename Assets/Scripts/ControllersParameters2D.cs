using UnityEngine;
using System.Collections;
using System;

//class is marked as Serializable to allow changes in inspector view
[Serializable]
public class ControllersParameters2D 
{
	public enum JumpBehavior 
	{
		CanJumpOnGround,
		CanJumpAnywhere,
		CantJump
	}

	public Vector2 MaxVelocity = new Vector2(float.MaxValue, float.MaxValue);

	//it will be represent as slider in the inspector
	[Range(0,90)]
	public float SlopeLimit = 30;
		
	public float Gravity = -25f;

	public JumpBehavior JumpRestrictions;

	public float JumpFrequency = .25f;

	public float JumpMagnitude = 12;
	
}
