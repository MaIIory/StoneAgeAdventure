using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;

public class LevelManager : MonoBehaviour
{

    static public LevelManager Instance { get; private set; }

    public Player Player { get; private set; }

    public CameraController Camera { get; private set; }

    //checkpoint handling
    private List<Checkpoint> _checkpoint;
    private int _currentCheckpointIndex;

    //Points counting
    private DateTime _started;
    private int _savedPoints;
    public TimeSpan RunningTime { get { return DateTime.UtcNow - _started; } }
    public int BonusCutoffSeconds;
    public int BonusSecondMultiplier;
    public int CurrentTimeBonus
    {
        get
        {
            var secondsDifference = (int)BonusCutoffSeconds - RunningTime.TotalSeconds;
            return (int)Math.Max(0, secondsDifference) * BonusSecondMultiplier;
        }
    }

    public Checkpoint DebugSpawn;

    void Awake()
    {
        Instance = this;
    }

    // Use this for initialization
    void Start()
    {
        _checkpoint = FindObjectsOfType<Checkpoint>().OrderBy(t => t.transform.position.x).ToList();
        _currentCheckpointIndex = _checkpoint.Count > 0 ? 0 : -1;

        _started = DateTime.UtcNow;

        Player = FindObjectOfType<Player>();
        Camera = FindObjectOfType<CameraController>();

        var listeners = FindObjectsOfType<MonoBehaviour>().OfType<IPlayerRespawnListener>();
        foreach (var listener in listeners)
        {

            for (var i = _checkpoint.Count - 1; i >= 0; i--)
            {
                var distance = ((MonoBehaviour)listener).transform.position.x - _checkpoint[i].transform.position.x;
                if (distance < 0)
                    continue;
                _checkpoint[i].AssignObjectToCheckpoint(listener);
                break;
            }
        }

#if UNITY_EDITOR
        if (DebugSpawn != null)
            DebugSpawn.SpawnPlayer(Player);
        else if (_currentCheckpointIndex != -1)
            _checkpoint[_currentCheckpointIndex].SpawnPlayer(Player);
#else
		if(_currentCheckpointIndex != -1)
			_checkpoint[_currentCheckpointIndex].SpawnPlayer(Player);
#endif



    }

    // Update is called once per frame
    void Update()
    {

        var isAtLastCheckpoint = _currentCheckpointIndex + 1 >= _checkpoint.Count;
        if (isAtLastCheckpoint)
            return;

        var distanceToNextCheckpoint = _checkpoint[_currentCheckpointIndex + 1].transform.position.x - Player.transform.position.x;
        if (distanceToNextCheckpoint >= 0)
            return;

        _checkpoint[_currentCheckpointIndex].PlayerLeftCheckpoint();
        _currentCheckpointIndex++;
        _checkpoint[_currentCheckpointIndex].PlayerHitCheckpoint();

        _started = DateTime.UtcNow;
        GameManager.Instance.AddPoints(CurrentTimeBonus);
        _savedPoints = GameManager.Instance.Points;

    }

    public void KillPlayer()
    {

        StartCoroutine(KillPlayerCo());
    }

    private IEnumerator KillPlayerCo()
    {

        Player.Kill();
        Camera.isFollowing = false;


        yield return new WaitForSeconds(2f);

        Camera.isFollowing = true;
        if (_currentCheckpointIndex != -1)
            _checkpoint[_currentCheckpointIndex].SpawnPlayer(Player);

        _started = DateTime.UtcNow;
        GameManager.Instance.ResetPoints(_savedPoints);
    }


}
