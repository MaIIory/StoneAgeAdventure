using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class ScoreHud : MonoBehaviour {

    private GameManager _gameManager;

	// Use this for initialization
	void Start () {
        _gameManager = GameManager.Instance;
	}
	
	// Update is called once per frame
	void Update () {

        var obj = GetComponent<Text>();

        if(obj == null)
        {
            Debug.Log("null");
        }
        else
        {
            string output = string.Concat("Score: ", _gameManager.Points.ToString());
            obj.text = output;
        }
	}
}
