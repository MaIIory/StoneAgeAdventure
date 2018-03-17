using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EndLevel : MonoBehaviour
{

    public int LevelToBeLoad;
    public AudioClip EndLevelSound;
    private AudioSource _audioSource;

    void Awake()
    {
        _audioSource = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void OnTriggerEnter2D(Collider2D other)
    {

        var player = other.GetComponent<Player>();

        if (player != null)
        {
            _audioSource.PlayOneShot(EndLevelSound, 0.5f);
            Invoke("SwitchToNextLevel", 3);
            player.Win();
            
        }
    }

    void SwitchToNextLevel()
    {
        SceneManager.LoadScene(LevelToBeLoad);
        GameManager.Instance.ResetPoints(0);
    }
}
