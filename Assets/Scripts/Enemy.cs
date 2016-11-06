using UnityEngine;
using System.Collections;

public class Enemy : MonoBehaviour, IPlayerRespawnListener
{

    public bool IsFacingRight = false;
    public int PointsForKill = 10;
    public float Gravity = -25f;

    private Vector2 _startPosition;
    private float _lastPosition;
    private bool _isDead = false;
    private Vector2 _velocity;

    // Use this for initialization
    void Start () {
        _startPosition = transform.position;
        _lastPosition = transform.position.x;
	}
	
	// Update is called once per frame
	void Update ()
    {

        if (!_isDead)
        {
            float deltaX = _lastPosition - transform.position.x;

            if (deltaX > 0 && IsFacingRight)
                Flip();
            else if (deltaX < 0 && !IsFacingRight)
                Flip();

            _lastPosition = transform.position.x;
        }
        else
        {
            _velocity.y += Gravity * Time.deltaTime;
            Move(_velocity * Time.deltaTime);
        }
	}

    private void Flip()
    {
        transform.localScale = new Vector3(-transform.localScale.x, transform.localScale.y, transform.localScale.z);
        //IsFacingRight = !IsFacingRight;
        IsFacingRight = transform.localScale.x < 0;
    }

    private void Move(Vector2 deltaMovement)
    {
        transform.Translate(deltaMovement, Space.World);
    }

    public IEnumerator OnTriggerEnter2D(Collider2D other)
    {
        var cudgel = other.GetComponent<Cudgel>();

        if (cudgel != null)
        {
            
            GameManager.Instance.AddPoints(PointsForKill);
            //Instantiate(Effect, transform.position, transform.rotation);
            GetComponent<FollowPath>().enabled = false;
            GetComponent<Collider2D>().enabled = false;
            _isDead = true;
            //transform.localScale = new Vector2(transform.localScale.x, -transform.localScale.y);

            yield return new WaitForSeconds(3f);

            gameObject.SetActive(false);
            transform.position = _startPosition;
            //IsFacingRight = false;
        }
    }

    public void OnPlayerRespawnAtThisCheckpoint(Checkpoint checkpoint, Player player)
    {
        _velocity.y = 0;
        GetComponent<FollowPath>().enabled = true;
        GetComponent<Collider2D>().enabled = true;
        gameObject.SetActive(true);
        _isDead = false;
    }

}
