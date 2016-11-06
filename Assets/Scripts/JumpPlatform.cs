using UnityEngine;
using System.Collections;

public class JumpPlatform : MonoBehaviour {

	public float jumpMagnitude = 1;

	public void ControllerEnter2D(CharacterController2D characterController2D) {

		characterController2D.SetVerticalForce (jumpMagnitude);

	}
}
