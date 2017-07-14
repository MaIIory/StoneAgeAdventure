using UnityEngine;
using System.Collections;
using CnControls;
using System.Collections.Generic;

public class Player : MonoBehaviour {

	private bool _isFacingRight;
	private CharacterController2D _controller;
	private float _normalizedHorizontalSpeed; //indicates if playes moving left or right
    private Animator _animator;

	public float MaxSpeed = 8;
	public float SpeedAccelerationOnGround = 10f;
	public float SpeedAccelerationInAir = 5f;
	public bool IsDead { get; private set; }

    public int MaxHealth = 100;
    public int Health { get; private set; }
    public GameObject OuchEffect;

    public Color BasePlayerColor;
    public Color BoosterPlayerColor;
    //public int StepColorFactor = 1;
    //private int _colorCounter = 0;
    private SpriteRenderer bodyRenderer;
    public Cudgel _cudgel;

	public void Awake()
	{
		_controller = GetComponent<CharacterController2D> ();
        _animator = GetComponent<Animator>();

		_isFacingRight = transform.localScale.x > 0;
		IsDead = false;
        Health = MaxHealth;

        var body = transform.Find("Body");

        if (body != null)
        {
            //BasePlayerColor = body.GetComponent<SpriteRenderer>().color;
            bodyRenderer = body.GetComponent<SpriteRenderer>();
            //body.GetComponent<SpriteRenderer>().color = Color.Lerp()
        }

        var colliderList = GetComponents< BoxCollider2D>();            
	}

	public void Update()
	{
		if(!IsDead)
			HandleInput ();
		float movementFactor = _controller.State.IsGrounded ? SpeedAccelerationOnGround : SpeedAccelerationInAir;
		_controller.SetHorizontalForce (Mathf.Lerp (_controller.Velocity.x, _normalizedHorizontalSpeed * MaxSpeed, Time.deltaTime * movementFactor));

        _animator.SetBool("IsGrounded", _controller.State.IsGrounded);
        _animator.SetFloat("Speed", Mathf.Abs(_controller.Velocity.x) / MaxSpeed);

        /*
        bodyRenderer.color = Color.Lerp(BasePlayerColor, BoosterPlayerColor, _colorCounter / 255f);
        _colorCounter += StepColorFactor;
        if (_colorCounter >= 255)
        {
            StepColorFactor = -StepColorFactor;
            _colorCounter = 255;
        }
        else if(_colorCounter < 0)
        {
            _colorCounter = 0;
            StepColorFactor = -StepColorFactor;
        }
        */
    }

	public void Kill() {
		_controller.HandleCollision = false;
		GetComponent<Collider2D> ().enabled = false;
        _cudgel.gameObject.SetActive(false);
		IsDead = true;
        Health = 0;
		_normalizedHorizontalSpeed = 0;
		_controller.SetForce (new Vector2 (0, 10));
	}

	public void RespawnAt(Transform spawnPoint){

		if (!_isFacingRight)
			Flip ();

		_controller.HandleCollision = true;
		GetComponent<Collider2D> ().enabled = true;
        _cudgel.gameObject.SetActive(false);
        IsDead = false;
        Health = MaxHealth;

		transform.position = spawnPoint.position;

	}

    public void TakeDamage(int damage)
    {
        Instantiate(OuchEffect, transform.position, transform.rotation);
        Health -= damage;

        if(Health <= 0)
        {
            LevelManager.Instance.KillPlayer();
            Health = 0;
        }

    }

	private void HandleInput()
	{
        /*
		if (Input.GetKey (KeyCode.D)) {
			_normalizedHorizontalSpeed = 1;
			if (!_isFacingRight)
				Flip ();
		} else if (Input.GetKey (KeyCode.A)) {
			_normalizedHorizontalSpeed = -1;
			if (_isFacingRight)
				Flip ();
		} else {
			_normalizedHorizontalSpeed = 0;
		}
        */

        
        if (CnInputManager.GetAxis("Horizontal") > 0)
        {
            _normalizedHorizontalSpeed = CnInputManager.GetAxis("Horizontal");
            if (!_isFacingRight)
                Flip();
        }
        else if (CnInputManager.GetAxis("Horizontal") < 0)
        {
            _normalizedHorizontalSpeed = CnInputManager.GetAxis("Horizontal");
            if (_isFacingRight)
                Flip();
        }
        else
        {
            _normalizedHorizontalSpeed = 0;
        }


        //if (_controller.CanJump && Input.GetKeyDown(KeyCode.Space)) {
        //	_controller.Jump ();
        //}

        
        if (_controller.CanJump && CnInputManager.GetButtonDown("Jump")){
            _controller.Jump();
        }

        //if (_controller.CanJump && CnInputManager.GetAxis("Vertical") > 0.7f)
        //{
        //   _controller.Jump();
        //}

        if (CnInputManager.GetButtonDown("lucki"))
        {
            Smash();
        }


    }

    private void Smash()
    {
        _cudgel.Smash();
        _animator.SetTrigger("Hit");
    }

    public void CudgelAnimationHasFinished()
    {
        Debug.Log("Cudgel animation has finished");
        _cudgel.gameObject.SetActive(false);
    }

    private void Flip()
	{
		transform.localScale = new Vector3 (-transform.localScale.x, transform.localScale.y, transform.localScale.z);
		_isFacingRight = transform.localScale.x > 0;
	}

}
