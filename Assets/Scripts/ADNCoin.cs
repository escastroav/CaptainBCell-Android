using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ADNCoin : MonoBehaviour
{
    public float despawnTime = 10f;
    public GameObject particles;
    GameNColorManager gameManager;
    Animator anim;
    Collider2D coll;
    AudioSource audioSrc;
    private void Start()
    {
        gameManager = FindObjectOfType<GameNColorManager>();
        anim = GetComponent<Animator>();
        coll = GetComponent<Collider2D>();
        audioSrc = GetComponent<AudioSource>();
        StartCoroutine("CoinLife");
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        Bullet bulletCol = collision.GetComponent<Bullet>();

        if (bulletCol != null)
        {
            StopCoroutine("CoinLife");            
            gameManager.AddCoin();
            StartCoroutine("Gathered");            
        }
    }

    IEnumerator CoinLife() 
    {
        yield return new WaitForSeconds(despawnTime);
        coll.enabled = false;
        anim.SetTrigger("Despawn");
        yield return new WaitForSeconds(1f);
        Destroy(gameObject);
    }

    IEnumerator Gathered() 
    {
        GameObject prtInst;
        prtInst = Instantiate(particles, transform.position, Quaternion.Euler(90f,0f,0f));
        audioSrc.Play();
        anim.SetTrigger("Gather");
        yield return new WaitForSeconds(.5f);
        Destroy(prtInst);
        Destroy(gameObject);
    }
    
}
