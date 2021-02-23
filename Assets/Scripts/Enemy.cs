using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public colorOfBullet colorType;
    public GameObject deathParticles;
    public GameObject AdnDrop;
    public float circularFrequency = 15f;
    public float initLinearSpeed = 0f;
    public float radiusSpeed = 1.5f;
    float limitForceFieldX = 0f, limitForceFieldY = 0f;
    float forceMagnitude = 0f;
    bool isDead = false;
    int dropValue = 0;
    [SerializeField]float outScreenTimer = 0f, limitOutScreenTime = 1.5f;
    Vector3 initPos;
    GameNColorManager gameManager;
    PowerUpManager powerUpManager;
    Camera cam;
    SpriteRenderer sprite, faceSprite;
    PolygonCollider2D coll;    
    Rigidbody2D rb;
    Animator anim;
    AudioSource audioSource;
    // Start is called before the first frame update
    void Start()
    {
        gameManager = FindObjectOfType<GameNColorManager>();
        powerUpManager = FindObjectOfType<PowerUpManager>();
        cam = FindObjectOfType<Camera>();
        sprite = GetComponent<SpriteRenderer>();
        faceSprite = transform.GetChild(0).GetComponent<SpriteRenderer>();
        coll = GetComponent<PolygonCollider2D>();
        anim = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();
        rb = GetComponent<Rigidbody2D>();
        rb.AddForce(Vector3.Cross(transform.position.normalized, Vector3.forward) * initLinearSpeed);
        gameManager.AssignColorToObject(sprite, colorType);
    }

    private void OnBecameInvisible()
    {
        coll.enabled = false;
        //Debug.Log("Enemy" + gameObject.ToString() + "not visible!");
    }

    private void OnBecameVisible()
    {
        coll.enabled = true;
        outScreenTimer = 0f;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (!coll.enabled) 
        {
            outScreenTimer += Time.fixedDeltaTime;
            if (outScreenTimer > limitOutScreenTime) 
            {
                Despawn();
            }
        }
        if (!gameManager.hasWon && !gameManager.hasLost && !isDead)
        {
            limitForceFieldX = cam.ViewportToWorldPoint(Vector3.right).x * 0f;
            limitForceFieldY = cam.ViewportToWorldPoint(Vector3.up).y * 0f;
            transform.rotation = Quaternion.LookRotation(Vector3.forward, -transform.position.normalized);
            if (Mathf.Abs(transform.position.x) > limitForceFieldX || Mathf.Abs(transform.position.y) > limitForceFieldY) 
            {
                Debug.DrawRay(transform.position, -transform.position.normalized, Color.blue);
                forceMagnitude = Time.fixedDeltaTime * radiusSpeed / transform.position.sqrMagnitude;
                rb.AddForce(-1 * transform.position * forceMagnitude);
            }
                
        }
        else 
        {
            rb.Sleep();
        }
            
    }

    public void DieByKillerCell() 
    {
        StartCoroutine(Die(Color.clear));
    }

    IEnumerator Die(Color col) 
    {
        GameObject particleInstance;
        ParticleSystem.MainModule partSys;
        FireFeedback fireFeedback = FindObjectOfType<FireFeedback>();

        isDead = true;
        coll.enabled = false;
        dropValue = Random.Range(1, 10);
        anim.SetTrigger("Death");
        yield return new WaitForSeconds(.375f);

        audioSource.Play();
        sprite.enabled = false; faceSprite.enabled = false;
        fireFeedback.FireFlash(col);
        particleInstance = Instantiate(deathParticles, transform.position, Quaternion.identity, transform);

        if (dropValue <= gameManager.level) 
        {
            Instantiate(AdnDrop, transform.position, Quaternion.identity);
        }

        partSys = particleInstance.GetComponent<ParticleSystem>().main;
        partSys.startColor = gameManager.colors[(int)colorType];
        yield return new WaitForSeconds(partSys.duration);

        Destroy(particleInstance);
        Destroy(gameObject);
        yield return null;
    }

    public void Despawn() 
    {
        Destroy(gameObject);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        //Debug.Log("collision detected");
        Bullet bulletColl = collision.GetComponent<Bullet>();
        PlayerMain playerColl = collision.GetComponent<PlayerMain>();
        if (bulletColl != null)
        {
            //Debug.Log("Enemy hit bullet!");
            if (bulletColl.colorType == colorType)
            {
                gameManager.AddPoint();
                StartCoroutine(Die(Color.grey));
            }
            else 
            {
                gameManager.RemoveLife();
                StartCoroutine(Die(Color.red));
            }
            
        }
        else if (playerColl != null) 
        {
            if (GameData.gameDataIns.itemCounts[(int)PowerUps.MemoryCell] == 0)
                gameManager.GameOver();
            else
            {
                powerUpManager.UseMemoryCellInDeath();
                playerColl.GetComponent<Animator>().SetInteger("Life", GameData.gameDataIns.lifeCap);
            }
            //Debug.Log("Enemy hit player!");
            StartCoroutine(Die(Color.clear));
        }
        //Destroy(gameObject);
    }
}
