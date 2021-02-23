using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class PlayerMain : MonoBehaviour
{
    public AnimationCurve angleRotationCurve = new AnimationCurve();
    public AnimationCurve finalAngleCurve = new AnimationCurve();
    [Range(1.0f,3.0f)]public float rotationSensitivity = 1.2f;
    public FixedJoystick joystick;
    Vector2 dir;
    public bool lockJoystick = false;
    float rotationSmooth = 15f;
    bool isActive, isVisible;
    float boundaryWidth;
    Touch rotationTouch;
    [SerializeField]Vector2 rotTouchInit, rotTouchFinal, shootTouchWorld;
    [SerializeField] float initAngle, finalAngle, smoothFactor;
    //float shootOffset = 0.5f;
    Quaternion currentRot, nextRot;    
    int tchCount;
    Touch[] tchs;

    GameNColorManager gameManager;
    
    [HideInInspector]public Animator anim;
    [HideInInspector] public SpriteRenderer sprite;
    Camera cam;
    
    private void Start()
    {
        gameManager = FindObjectOfType<GameNColorManager>();
        cam = FindObjectOfType<Camera>();
        anim = GetComponent<Animator>();
        sprite = GetComponent<SpriteRenderer>();
        boundaryWidth = cam.ViewportToWorldPoint(Vector3.up).y;
    }


    public void ResetRotation() 
    {
        transform.rotation = Quaternion.Euler(0f, 0f, 0f);
    }
    // Update is called once per frame
    void FixedUpdate()
    {
        tchCount = Input.touchCount;
        tchs = Input.touches;
        Vector2 tchPos;

        isActive = tchCount > 0 && tchCount <= 5 && !gameManager.hasLost;
        isVisible = GameData.gameDataIns.gameStarted || gameManager.onSettings || gameManager.onTutorial;

        sprite.enabled = isVisible;
        
        if (isActive && isVisible)
        {

            if (!lockJoystick) 
            {
                for (int i = 0; i < tchCount; i++)
                {
                    tchPos = cam.ScreenToWorldPoint(tchs[i].position);
                    if (Mathf.Abs(tchPos.x) <= boundaryWidth)
                    {
                        rotationTouch = Input.GetTouch(i);
                        AimPlayer2();
                    }/*
                else if (tchPos.x > boundaryWidth) 
                {
                    swipeRotTouch = Input.GetTouch(i);
                    SwipeAimPlayer();
                }*/
                }
                joystick.enabled = false;
            }
            else
            {
                joystick.enabled = true;
                JoystickAim();
            }
        }
        Debug.DrawLine(Vector3.zero, rotTouchInit, Color.red, Time.fixedDeltaTime);
        Debug.DrawLine(Vector3.zero, rotTouchFinal, Color.green, Time.fixedDeltaTime);
    }
    void JoystickAim() 
    {
        Quaternion currentRot, nextRot;
        smoothFactor = Time.fixedDeltaTime * rotationSmooth * rotationSensitivity;
        float Xjoystick = joystick.Horizontal;
        float Yjoystick = joystick.Vertical;
        float dirAngle = 0f, deltaAngle = 0f;
     
        currentRot = Quaternion.Euler(0f,0f,0f);
        dir = Vector2.right * Xjoystick + Vector2.up * Yjoystick;
        dirAngle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        
        deltaAngle = Mathf.DeltaAngle(0f, dirAngle);
        nextRot = Quaternion.AngleAxis(deltaAngle, Vector3.forward);
        transform.rotation = Quaternion.Slerp(transform.rotation, currentRot * nextRot, smoothFactor);
    }
    /*
    void AimPlayer() 
    {
        float deltaAngle;
        float rotationAngle;
        float roundedAngle;
        Quaternion oldRotation = transform.rotation;
        if (rotationTouch.phase == TouchPhase.Began)
        {
            rotTouchInit = cam.ScreenToWorldPoint(rotationTouch.position);
            rotTouchFinal = rotTouchInit;
            currentRot = transform.rotation;
            //if (Mathf.Abs(rotTouchInit.x) <= boundaryWidth)
            initAngle = Mathf.Rad2Deg * (Mathf.Atan2(rotTouchInit.y, rotTouchInit.x));            
            initAngle += initAngle > 180f ? 360f : 0f;
            finalAngle = initAngle;
            transform.rotation = oldRotation;
        }
        if (rotationTouch.phase == TouchPhase.Moved) 
        {            
            rotTouchFinal = cam.ScreenToWorldPoint(rotationTouch.position);
           
                finalAngle = Mathf.Rad2Deg * Mathf.Atan2(rotTouchFinal.y, rotTouchFinal.x);
                finalAngle += finalAngle > 180f ? 360f : 0f;
            deltaAngle = finalAngle - initAngle;
            roundedAngle = (rotTouchFinal - rotTouchInit).magnitude / rotTouchFinal.magnitude;
            rotationAngle = Mathf.Abs(deltaAngle) < 60f ? deltaAngle : roundedAngle * Mathf.Sign(deltaAngle);  
                nextRot = Quaternion.AngleAxis(rotationAngle * rotationSensitivity * rotationSmooth * Time.fixedDeltaTime, Vector3.forward);
            //rotTouchInit = rotTouchFinal;
            //initAngle = finalAngle;
            //transform.rotation *= nextRot;
            transform.rotation = Quaternion.Slerp(transform.rotation, transform.rotation * nextRot, 10f * Time.fixedDeltaTime);
                angleRotationCurve.AddKey(Time.realtimeSinceStartup, rotationAngle);
                finalAngleCurve.AddKey(Time.realtimeSinceStartup, transform.eulerAngles.z);
                rotTouchInit = rotTouchFinal;
                initAngle = finalAngle;
            
            
        }/*
        if (rotationTouch.phase == TouchPhase.Ended) 
        {
            oldRotation = transform.rotation;            
        }
    }
    */
    void AimPlayer2() 
    {
        float deltaAngle;
        smoothFactor = Time.fixedDeltaTime * rotationSmooth * rotationSensitivity;
        if (rotationTouch.phase == TouchPhase.Began) 
        {
            rotTouchInit = cam.ScreenToWorldPoint(rotationTouch.position);            
            initAngle = Mathf.Atan2(rotTouchInit.y, rotTouchInit.x)*Mathf.Rad2Deg;
            currentRot = transform.rotation;
            rotTouchFinal = rotTouchInit;
        }
        if (rotationTouch.phase == TouchPhase.Moved)
        {
            rotTouchFinal = cam.ScreenToWorldPoint(rotationTouch.position);            
            finalAngle = Mathf.Atan2(rotTouchFinal.y, rotTouchFinal.x) * Mathf.Rad2Deg;
            deltaAngle = Mathf.DeltaAngle(initAngle, finalAngle);
            nextRot = Quaternion.AngleAxis(deltaAngle, Vector3.forward);
            transform.rotation = Quaternion.Slerp(transform.rotation, currentRot*nextRot, smoothFactor);
            //rotTouchInit = rotTouchFinal;
        }
        if (rotationTouch.phase == TouchPhase.Ended) 
        {
            rotTouchInit = rotTouchFinal;
            initAngle = finalAngle;
        }
    }
}
