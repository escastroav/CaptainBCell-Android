using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireFeedback : MonoBehaviour
{
    Camera cam;
    SpriteRenderer fireFeedbackRender;
    Animator fireFeedbackAnim;
    //AudioSource audioSrc;
    void Start()
    {
        cam = FindObjectOfType<Camera>();
        //audioSrc = GetComponent<AudioSource>();
        SetFireFeedback();
    }

    

    void SetFireFeedback()
    {
        fireFeedbackRender = GetComponent<SpriteRenderer>();
        fireFeedbackAnim = GetComponent<Animator>();

        float worldWidth = cam.ViewportToWorldPoint(Vector2.right).x*2f;
        fireFeedbackRender.transform.localScale = new Vector3(worldWidth, worldWidth, 0f);
    }

    public void FireFlash(Color col)
    {
        fireFeedbackRender.color = col;
        fireFeedbackAnim.SetTrigger("Fire");
    }

    
}
