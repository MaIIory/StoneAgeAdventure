using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class CharacterController2D : MonoBehaviour
{

	private const float SkinWidth = .005f;
	private const int TotalHorizontalRays = 5;
	private const int TotalVerticalRays = 4;

	//max slope angle
	private static readonly float SlopeLimitTangant = Mathf.Tan (75f * Mathf.Deg2Rad);
	public LayerMask PlatformMask;
	public ControllersParameters2D DefaultParameters;

	public ControllerState2D State { get; private set; }

	public Vector2 Velocity { get { return _velocity; } }

	public bool CanJump {
		get {
			if (Parameters.JumpRestrictions == ControllersParameters2D.JumpBehavior.CanJumpAnywhere)
				return _jumpIn <= 0;

			if (Parameters.JumpRestrictions == ControllersParameters2D.JumpBehavior.CanJumpOnGround)
				return State.IsGrounded;

			return false;
		}
	}

	public bool HandleCollision { get; set; }

	public ControllersParameters2D Parameters { get { return _overrideParameters ?? DefaultParameters; } }

	public GameObject StandingOn { get; private set; }

	public Vector3 PlatformVelocity { get; private set; }

	private Vector2 _velocity;
	private Transform _transform;
	private BoxCollider2D _boxCollider;
	private float _verticalDistanceBetweenRays;
	private float _horizontalDistanceBetweenRays;
	private ControllersParameters2D _overrideParameters;
	private Vector3 _raycastTopLeft;
	private Vector3 _raycastBottomRight;
	private Vector3 _raycastBottomLeft;
	private float _jumpIn;
	private Vector3 _activeLocalPlatformPoint;
	private Vector3 _activeGlobalPlatformPoint;
	private GameObject _lastStandingOn;
	private Vector3 _lastPlatformPosition;

	public void Awake ()
	{
		State = new ControllerState2D ();
		HandleCollision = true;
		_transform = transform;
		_boxCollider = GetComponent<BoxCollider2D> ();

		float colliderWidth = _boxCollider.size.x * Mathf.Abs (transform.localScale.x) - (2 * SkinWidth);
		_horizontalDistanceBetweenRays = colliderWidth / (TotalVerticalRays - 1);

		float colliderHeight = _boxCollider.size.y * Mathf.Abs (transform.localScale.y) - (2 * SkinWidth);
		_verticalDistanceBetweenRays = colliderHeight / (TotalHorizontalRays - 1);
	}

	public void AddForce (Vector2 force)
	{
		_velocity += force;
	}

	public void SetForce (Vector2 force)
	{
		_velocity = force;
	}

	public void SetHorizontalForce (float x)
	{
		_velocity.x = x;
	}

	public void SetVerticalForce (float y)
	{
		_velocity.y = y;
	}

	public void Jump ()
	{
		//TODO moving platforms
		AddForce (new Vector2 (0, Parameters.JumpMagnitude));
		//timer for jump frequency
		_jumpIn = Parameters.JumpFrequency;

	}

	public void LateUpdate ()
	{
		_jumpIn -= Time.deltaTime;
		_velocity.y += Parameters.Gravity * Time.deltaTime;
		Move (Velocity * Time.deltaTime);
	}

	private void Move (Vector2 deltaMovement)
	{
		bool wasGrounded = State.IsCollidingBelow;
		State.Reset ();

		if (HandleCollision) {
			HandlePlatforms ();
			CalculateRayOrigins ();

			//if (deltaMovement.y < 0 && wasGrounded)
			//	HandleVerticalSlope (ref deltaMovement);

			if (Mathf.Abs (deltaMovement.x) > 0.001f)
				MoveHorizontally (ref deltaMovement);

			MoveVertically (ref deltaMovement);

			CorrectHorizontalPlacement(ref deltaMovement, true);
			CorrectHorizontalPlacement(ref deltaMovement, false);

		}

		_transform.Translate (deltaMovement, Space.World);

		if (Time.deltaTime > 0)
			_velocity = deltaMovement / Time.deltaTime;

		_velocity.x = Mathf.Min (_velocity.x, Parameters.MaxVelocity.x);
		_velocity.y = Mathf.Min (_velocity.y, Parameters.MaxVelocity.y);

		//if (State.IsMovingUpSlope)
		//	_velocity.y = 0;

		if (StandingOn != null) {
			//_activeGlobalPlatformPoint = transform.position;
			//_activeLocalPlatformPoint = StandingOn.transform.InverseTransformPoint (transform.position);
			_lastPlatformPosition = StandingOn.transform.position;

			if (_lastStandingOn != StandingOn) {

				if (_lastStandingOn != null)
					_lastStandingOn.SendMessage ("ControllerExit2D", this, SendMessageOptions.DontRequireReceiver);

				StandingOn.SendMessage ("ControllerEnter2D", this, SendMessageOptions.DontRequireReceiver);
				_lastStandingOn = StandingOn;

			} else if (StandingOn != null)
				StandingOn.SendMessage ("ControllerStay2D", this, SendMessageOptions.DontRequireReceiver);

		} else if (_lastStandingOn != null) {
			_lastStandingOn.SendMessage ("ControllerExit2D", this, SendMessageOptions.DontRequireReceiver);
			_lastStandingOn = null;
		}
	}

	private void HandlePlatforms ()
	{
		if (StandingOn != null) {

			Vector3 moveDistance = StandingOn.transform.position - _lastPlatformPosition;


			//Vector3 newGlobalPlatformPoint = StandingOn.transform.TransformPoint (_activeLocalPlatformPoint);
			//Vector3 moveDistance = newGlobalPlatformPoint - _activeGlobalPlatformPoint;

			if (moveDistance != Vector3.zero) {
				transform.Translate (moveDistance, Space.World);
			}

			//PlatformVelocity = (newGlobalPlatformPoint - _activeGlobalPlatformPoint) / Time.deltaTime;

		} else {
			//PlatformVelocity = Vector3.zero;
		}

		StandingOn = null;
	}

	//This method handles player movement triggered by moving platforms
	private void CorrectHorizontalPlacement (ref Vector2 deltaMovement, bool isRight)
	{

		var halfWidth = (_boxCollider.size.x * Mathf.Abs (transform.localScale.x)) / 2f;
		var rayOrigin = isRight ? _raycastBottomRight : _raycastBottomLeft;
	
		if (isRight)
			rayOrigin.x -= (halfWidth - SkinWidth);
		else
			rayOrigin.x += (halfWidth - SkinWidth);

		var rayDirection = isRight ? Vector2.right : Vector2.left;

		var offset = 0f;



		for (var i = 1; i < TotalHorizontalRays - 1; i++) {
			var rayVector = new Vector2 (deltaMovement.x + rayOrigin.x, deltaMovement.y + rayOrigin.y + (i * _verticalDistanceBetweenRays));
			Debug.DrawRay (rayVector, rayDirection * halfWidth, isRight ? Color.red : Color.cyan);

			RaycastHit2D rayCastHit = Physics2D.Raycast (rayVector, rayDirection, halfWidth, PlatformMask);

			if (!rayCastHit)
				continue;

			offset = isRight ? (rayCastHit.point.x - _transform.position.x) - halfWidth : halfWidth - (_transform.position.x - rayCastHit.point.x);
		}

		deltaMovement.x += offset;
	}

	private void CalculateRayOrigins ()
	{

		/*  *1======*
		    |       |
		    |       |
		    *3=====*2 */

		Vector2 halfSize = new Vector2 (_boxCollider.size.x * Mathf.Abs (transform.localScale.x), _boxCollider.size.y * Mathf.Abs (transform.localScale.y)) / 2;
		Vector2 center = new Vector2 (_boxCollider.offset.x * transform.localScale.x, _boxCollider.offset.y * transform.localScale.y);

		_raycastTopLeft = transform.position + new Vector3 (center.x - halfSize.x + SkinWidth, center.y + halfSize.y - SkinWidth);
		_raycastBottomRight = transform.position + new Vector3 (center.x + halfSize.x - SkinWidth, center.y - halfSize.y + SkinWidth);
		_raycastBottomLeft = transform.position + new Vector3 (center.x - halfSize.x + SkinWidth, center.y - halfSize.y + SkinWidth);

	}

	private void MoveHorizontally (ref Vector2 deltaMovement)
	{
		//Prepare some setting related to ray casting
		bool isGoingRight = deltaMovement.x > 0;
		float rayDistance = Mathf.Abs (deltaMovement.x) + SkinWidth;
		Vector2 rayDirection = isGoingRight ? Vector2.right : -Vector2.right;
		Vector3 rayOrigin = isGoingRight ? _raycastBottomRight : _raycastBottomLeft;


		//start casting rays horizontally
		for (int i = 0; i < TotalHorizontalRays; i++) {

			Vector2 rayVector = new Vector2 (rayOrigin.x, rayOrigin.y + (i * _verticalDistanceBetweenRays));
			Debug.DrawRay (rayVector, rayDirection * rayDistance, Color.red);

			//shoot ray!
			RaycastHit2D rayCastHit = Physics2D.Raycast (rayVector, rayDirection, rayDistance, PlatformMask);
			//if there was no hit just continue
			if (!rayCastHit)
				continue;

			if (i == 0 && HandleHorizontalSlope (ref deltaMovement, Vector2.Angle (rayCastHit.normal, Vector2.up), isGoingRight)) {
				break;
			}

			//find delta between ray origin and hit
			deltaMovement.x = rayCastHit.point.x - rayVector.x;
			rayDistance = Mathf.Abs (deltaMovement.x);

			if (isGoingRight) {
				deltaMovement.x -= SkinWidth;
				State.IsCollidingRight = true;
			} else {
				deltaMovement.x += SkinWidth;
				State.IsCollidingLeft = true;
			}

			if (rayDistance < SkinWidth + .0001f)
				break;
		}
	}

	private void MoveVertically (ref Vector2 deltaMovement)
	{
		//Prepare some setting related to ray casting
		bool isGoingUp = deltaMovement.y > 0;
		float rayDistance = Mathf.Abs (deltaMovement.y) + SkinWidth;
		Vector2 rayDirection = isGoingUp ? Vector2.up : -Vector2.up;
		Vector3 rayOrigin = isGoingUp ? _raycastTopLeft : _raycastBottomLeft;

		rayOrigin.x += deltaMovement.x;

		float standingOnDistance = float.MaxValue;

		for (int i = 0; i < TotalVerticalRays; i++) {

			Vector2 rayVector = new Vector2 (rayOrigin.x + (i * _horizontalDistanceBetweenRays), rayOrigin.y);
			Debug.DrawRay (rayVector, rayDirection * rayDistance, Color.green);

			//shoot ray!
			RaycastHit2D rayCastHit = Physics2D.Raycast (rayVector, rayDirection, rayDistance, PlatformMask);
			//if there was no hit just continue
			if (!rayCastHit)
				continue;

			if (!isGoingUp) {
				//LUCN I have changed rayCastHit.point.x to rayCastHit.point.y in the next line
				float verticalDistanceToHit = transform.position.y - rayCastHit.point.y;
				if (verticalDistanceToHit < standingOnDistance) {
					standingOnDistance = verticalDistanceToHit;
					StandingOn = rayCastHit.collider.gameObject;
					_lastPlatformPosition = StandingOn.transform.position; 

				}
			}


			//TODO correct position of the player

			deltaMovement.y = rayCastHit.point.y - rayVector.y;
			rayDistance = Mathf.Abs (deltaMovement.y);


			if (isGoingUp) {
				deltaMovement.y -= SkinWidth;
				State.IsCollidingAbove = true;
			} else {

				deltaMovement.y += SkinWidth;
				State.IsCollidingBelow = true;
			}

			if (!isGoingUp && deltaMovement.y > .0001f)
				State.IsMovingUpSlope = true;


			//if (rayDistance < SkinWidth + .0001f)
			//	break;

		}
	

	}
	

	private void HandleVerticalSlope (ref Vector2 deltaMovement)
	{
		//Prepare and draw vector to detect slope
		float center = (_raycastBottomLeft.x + _raycastBottomRight.x) / 2;
		Vector2 direction = Vector2.down;
		float slopeDistance = SlopeLimitTangant * (_raycastBottomRight.x - center);
		Vector2 slopeRayVector = new Vector2 (center, _raycastBottomLeft.y);

		Debug.DrawRay (slopeRayVector, direction * slopeDistance, Color.yellow);

		//cast a ray
		RaycastHit2D raycastHit = Physics2D.Raycast (slopeRayVector, direction, slopeDistance, PlatformMask);
		if (!raycastHit)
			return;
		bool isMovingDownSlope = Mathf.Sign (raycastHit.normal.x) == Mathf.Sign (deltaMovement.x);
		if (!isMovingDownSlope)
			return;

		var angle = Vector2.Angle (raycastHit.normal, Vector2.up);
		if (Mathf.Abs (angle) < .0001f)
			return;

		State.IsMovingDownSlope = true;
		State.SlopeAngle = angle;
		deltaMovement.y = raycastHit.point.y - slopeRayVector.y;



	}

	private bool HandleHorizontalSlope (ref Vector2 deltaMovement, float angle, bool isGoingRight)
	{
		if (Mathf.RoundToInt (angle) == 90)
			return false;

		if (angle > Parameters.SlopeLimit) {
			deltaMovement.x = 0;
			return true;
		}

		if (deltaMovement.y > .07f)
			return true;

		//deltaMovement.x += isGoingRight ? -SkinWidth : SkinWidth;
		deltaMovement.y = Mathf.Abs (Mathf.Tan (angle * Mathf.Deg2Rad) * deltaMovement.x);
		State.IsMovingUpSlope = true;
		State.IsCollidingBelow = true;
		return true;
	}

	public void OnTriggerEnter2D (Collider2D other)
	{

	}

	public void OnTriggerExit2D (Collider2D other)
	{

	}
}
