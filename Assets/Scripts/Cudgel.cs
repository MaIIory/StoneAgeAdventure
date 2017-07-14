using UnityEngine;
using System.Collections;

public class Cudgel : MonoBehaviour {

	void Awake () {
        gameObject.SetActive(false);
	}

    public void Smash()
    {
        gameObject.SetActive(true);
    }
}
