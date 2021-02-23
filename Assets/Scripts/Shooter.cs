using UnityEngine;
using System.Collections;

public class Shooter : MonoBehaviour
{

    public colorOfBullet colorType;
    public GameObject bullet;
    public GameObject particles;
    //public SpriteRenderer sprite;

    float shootOffset = 0.5f;

    GameNColorManager gameManager;
    GameObject bulletInstance;
    SpriteRenderer sprite;
    AudioSource audioSource;
    Animator anim;

    private void Start()
    {
        gameManager = FindObjectOfType<GameNColorManager>();               
        //bullet = bulletObject.GetComponent<Bullet>();
        sprite = GetComponent<SpriteRenderer>();
        audioSource = GetComponent<AudioSource>();
        anim = GetComponent<Animator>();
        gameManager.AssignColorToObject(sprite, colorType);
        
    }

    private void FixedUpdate()
    {
        sprite.enabled = GameData.gameDataIns.gameStarted || gameManager.onSettings || gameManager.onTutorial;
    }


    public void ShootBullet() 
    {                
        if ((!gameManager.hasLost && GameData.gameDataIns.gameStarted) || gameManager.onTutorial) 
        {
            Vector2 launchPoint = transform.position + transform.right.normalized * shootOffset;

            bulletInstance = Instantiate(bullet, launchPoint, transform.rotation);            
            bulletInstance.GetComponent<Bullet>().colorType = colorType;
            
            audioSource.Play();
            anim.SetTrigger("Shoot");
        }      


    }
    


}
