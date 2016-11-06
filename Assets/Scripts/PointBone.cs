using UnityEngine;
using System.Collections;

public class PointBone : MonoBehaviour, IPlayerRespawnListener {

	public GameObject Effect;
	public int PointsToAdd = 10;
    public Animator Animator;
    private bool _isCollected = false;


	public void OnTriggerEnter2D(Collider2D other)
	{
        if (_isCollected)
            return;

		var player = other.GetComponent<Player> ();
		
		if (player != null)
        {
			GameManager.Instance.AddPoints(PointsToAdd);
			//Instantiate(Effect, transform.position, transform.rotation);
            _isCollected = true;
            Animator.SetTrigger("Bone Collected");
		}
	}

	public void OnPlayerRespawnAtThisCheckpoint(Checkpoint checkpoint, Player player)
	{
		gameObject.SetActive (true);
        _isCollected = false;
	}

    public void AnimationHasFinished()
    {
        gameObject.SetActive(false);
    }
}
