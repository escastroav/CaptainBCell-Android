using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    SpriteRenderer sprite;
    TrailRenderer trail;
    Gradient trailGradient;    
    GradientColorKey[] trailColors;
    GradientAlphaKey[] trailAlphas;
    Transform renderTransform;
    public colorOfBullet colorType;
    public float bulletSpeed = 10f, range = 20f;
    GameNColorManager gameManager;
    private void Start()
    {
        gameManager = FindObjectOfType<GameNColorManager>();
        sprite = GetComponentInChildren<SpriteRenderer>();
        trail = GetComponentInChildren<TrailRenderer>();
        renderTransform = transform.GetChild(0).transform;
        gameManager.AssignColorToObject(sprite, colorType, trail);
        //AssignColorAndGradient();
        
    }

    void AssignColorAndGradient() 
    {
        trailGradient = new Gradient();
        trailColors = new GradientColorKey[2];
        trailAlphas = new GradientAlphaKey[2];

        trailAlphas[0].alpha = .2f; trailAlphas[1].alpha = 0f;
        trailColors[1].color = Color.white;
        switch (colorType)
        {
            case colorOfBullet.Blue:
                sprite.color = Color.blue;
                trailColors[0].color = Color.blue;
                trailColors[1].color = Color.blue;
                break;
            case colorOfBullet.Red:
                sprite.color = Color.red;
                trailColors[0].color = Color.red;
                trailColors[1].color = Color.red;
                break;
            case colorOfBullet.Green:
                sprite.color = Color.green;
                trailColors[0].color = Color.green;
                trailColors[1].color = Color.green;
                break;
            case colorOfBullet.Yellow:
                sprite.color = Color.yellow;
                trailColors[0].color = Color.yellow;
                trailColors[1].color = Color.yellow;
                break;
        }
        trailGradient.SetKeys(trailColors, trailAlphas);
        trail.colorGradient = trailGradient;
    }
    // Update is called once per frame
    void FixedUpdate()
    {
        renderTransform.Rotate(0f, 0f, Time.fixedDeltaTime * 250f);
        transform.Translate(Vector2.right * bulletSpeed * Time.fixedDeltaTime);
        if (transform.position.magnitude > range) 
        {
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Enemy enemyColl = collision.GetComponent<Enemy>();
        if (enemyColl != null) 
        {
            Destroy(gameObject);
        }
    }
}
