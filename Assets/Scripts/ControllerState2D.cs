using UnityEngine;
using System.Collections;

//keeping track of the states of the 2D controller 
public class ControllerState2D
{
	public bool IsCollidingRight { get; set; }
	public bool IsCollidingLeft { get; set; }
	public bool IsCollidingAbove { get; set; }
	public bool IsCollidingBelow { get; set; }
	public bool IsMovingDownSlope { get; set; }
	public bool IsMovingUpSlope { get; set; }
	public bool IsGrounded { get { return IsCollidingBelow; }}
	public float SlopeAngle { get; set; }

	public bool HasCollision { get { return IsCollidingAbove || IsCollidingBelow || IsCollidingLeft || IsCollidingRight; }}

	public void Reset() 
	{
		IsMovingDownSlope = false;
		IsCollidingAbove = false;
		IsCollidingBelow = false;
		IsCollidingLeft = false;
		IsCollidingRight = false;
		SlopeAngle = 0;
	}

	public override string ToString ()
	{
		return string.Format ("[ControllerState2D: IsCollidingRight={0}, IsCollidingLeft={1}, IsCollidingAbove={2}, IsCollidingBelow={3}, IsMovingDownSlope={4}, IsMovingUpSlope={5}, IsGrounded={6}, SlopeAngle={7}]", IsCollidingRight, IsCollidingLeft, IsCollidingAbove, IsCollidingBelow, IsMovingDownSlope, IsMovingUpSlope, IsGrounded, SlopeAngle);
	}




}
