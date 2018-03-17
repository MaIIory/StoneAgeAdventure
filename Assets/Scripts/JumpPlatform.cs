using UnityEngine;
using System.Collections;

public class JumpPlatform : MonoBehaviour {

	public float jumpMagnitude = 1;
    public AudioClip JumpSound;
    private AudioSource _audioSource;

    void Awake()
    {
        _audioSource = GetComponent<AudioSource>();
    }

    public void ControllerEnter2D(CharacterController2D characterController2D) {

        _audioSource.PlayOneShot(JumpSound, 0.5f);
        characterController2D.SetVerticalForce (jumpMagnitude);

	}
}
