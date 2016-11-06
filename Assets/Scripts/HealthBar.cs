using UnityEngine;
using System.Collections;

//THIS SCRIPT IS ONLY FOR FUTURE PURPOSE
public class HealthBar : MonoBehaviour {

    Player Player;
    public Transform ForegroundSprite;
    public SpriteRenderer ForegroundRenderer;
    public Color MaxHealthColor = new Color(255 / 255f, 63 / 255f, 63 / 255f);
    public Color MinHealthColor = new Color(64/255f,137/255f,255/255f);

    public void Update()
    {
        var healthPercent = Player.Health / (float)Player.MaxHealth;
        ForegroundSprite.localScale = new Vector3(healthPercent,1,1);
        ForegroundRenderer.color = Color.Lerp(MinHealthColor, MaxHealthColor, healthPercent);



    }
}
