using UnityEngine;
using System.Collections;

public class GiveDamageToPlayer : MonoBehaviour {

    public int DamageToGive = 10;

    private Vector2 _lastPosition;
    private Vector2 _velocity;

    public void LateUpdate()
    {
        _velocity = (_lastPosition - (Vector2)transform.position) / Time.deltaTime;
        _lastPosition = transform.position;

    }

    public void OnTriggerEnter2D(Collider2D other)
    {

        var player = other.GetComponent<Player>();
        if (player == null)
            return;

        player.TakeDamage(DamageToGive);

        if (player.Health <= 0)
            return;

        //push player back
        var controler = player.GetComponent<CharacterController2D>();
        var totalVelocity = controler.Velocity + _velocity;

        controler.SetForce(new Vector2(
            -1 * Mathf.Sign(totalVelocity.x) * Mathf.Clamp(Mathf.Abs(totalVelocity.x) * 2, 5, 20),
            -1 * Mathf.Sign(totalVelocity.y) * Mathf.Clamp(Mathf.Abs(totalVelocity.y) * 2, 5, 10)));
    }
}
