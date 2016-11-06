using UnityEngine;
using System.Collections;

public class Cudgel : MonoBehaviour {

    //How long smash takes
    public float SmashTime = 0.20f;
    

    private float _smashTimer = 0f;

	// Use this for initialization
	void Awake () {
        gameObject.SetActive(false);
	}

    public void Update()
    {
        if(gameObject.activeInHierarchy)
        {
            _smashTimer += Time.deltaTime;
            if (SmashTime <= _smashTimer)
                gameObject.SetActive(false);
        }
    }

    public void Smash()
    {
        gameObject.SetActive(true);
        _smashTimer = 0f;
    }

}
