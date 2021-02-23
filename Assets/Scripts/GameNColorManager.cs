using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum colorOfBullet { Red, Blue, Green, Yellow };

public class GameNColorManager : MonoBehaviour
{
    
    
    [Header("Main GamePlay Parameters")]
    public Color[] colors = { Color.red, Color.blue, Color.green, Color.yellow };
    public bool gameStarted;
    public int spawnPointsCount = 10;
    public int points = 0, lifes = 3, level = 1, coins = 0;
    public int lifeCap = 3;
    public float spawnCycleTime = 3f;
    public bool hasWon = false, hasLost = false;
    [Space]
    [Header("GameObjects in Scene")]
    public GameObject player;
    public GameObject enemy;    
    [Space]
    [Header("Enemy Behaviour Parameters")]
    public float minRadialSpeed; 
    public float maxRadialSpeed; 
    public float minOrbitalSpeed;
    public float maxOrbitalSpeed;
    [Space]
    [Header("Difficulty Parameters")]
    [SerializeField]int scoreToLevel = 5;
    [SerializeField]int difficultyFactor = 1;
    [SerializeField]float minDistance = 1f;

    PlayerMain playerInstance;
    Shooter[] playerShooters;
    Vector3[] spawnPoints;
    [HideInInspector]public bool onSettings;
    [HideInInspector]public bool onTutorial;
    [HideInInspector]public MenuManager menuManager;
    [HideInInspector] public PowerUpManager powerUpManager;
    
    private void Awake()
    {
        menuManager = FindObjectOfType<MenuManager>();
        powerUpManager = FindObjectOfType<PowerUpManager>();
        if (menuManager == null)
        {
            Debug.LogError("Menu manager not present on scene!!");
        }
        
        menuManager.InitMenu();
        menuManager.SetHolders();
        menuManager.SetScoreText(points.ToString());
        menuManager.SetLifesText(lifes.ToString());
        lifeCap = GameData.gameDataIns.lifeCap;
        HealPlayer();
        powerUpManager.UpdateItemData();
        if (!GameData.gameDataIns.gameStarted)
        {
            menuManager.SetMainMenu();
            
        }
        else
        {
            InitializeGame();
            
        }
    }

    private void Start()
    {
        lifeCap = GameData.gameDataIns.lifeCap;
        HealPlayer();
    }


    void SpawnPlayer() 
    {
        
        playerInstance = FindObjectOfType<PlayerMain>();
        if (playerInstance == null)
        {
            playerInstance = Instantiate(player, Vector3.zero, Quaternion.identity).GetComponent<PlayerMain>();
            
        }
        
        playerShooters = playerInstance.GetComponentsInChildren<Shooter>();
        HealPlayer();
        ResetRotationOfPlayer();
    }

    public void ResetRotationOfPlayer() 
    {
        if (playerInstance == null) 
        {
            playerInstance = FindObjectOfType<PlayerMain>();
        }
        playerInstance.ResetRotation();
    }
    public void InitializeGame() 
    {
        GameData.gameDataIns.gameStarted = true;
        lifeCap = GameData.gameDataIns.lifeCap;
        HealPlayer();
        menuManager.SetMenuToGame();
        menuManager.AssignColorToButtons();        
        SpawnPlayer();             
        SetSpawnPoints();
        powerUpManager.UpdateItemData();

        minDistance = Random.Range(.75f, 1f);

        //menuManager.SetScoreText(points.ToString());
        //menuManager.SetLifesText(lifes.ToString());

        StartCoroutine(SpawnEnemies());        
    }


   

    
    void SetSpawnPoints()
    {
        spawnPoints = new Vector3[spawnPointsCount];
        float angle = 0f, deltaAngle = 2 * Mathf.PI / spawnPointsCount, radialPos;
        float radialPosX = menuManager.cam.ViewportToWorldPoint(Vector3.up).y;
        
        for (int i = 0; i < spawnPointsCount; i++)
        {
            angle = i * deltaAngle;
            radialPos = (Mathf.Abs(Mathf.Tan(angle)) < 1f) ? 
            radialPosX / Mathf.Abs(Mathf.Cos(angle)) : radialPosX / Mathf.Abs(Mathf.Sin(angle));
            spawnPoints[i].x = radialPos * Mathf.Cos(angle);
            spawnPoints[i].y = radialPos * Mathf.Sin(angle);
            spawnPoints[i].z = 0f;
        }
    }
    IEnumerator SpawnEnemies()
    {
        colorOfBullet colorSpawn;
        int i = 0;
        
        Enemy enemySpawned;
        while (!hasWon && !hasLost) 
        {            
            i = Random.Range(0, spawnPointsCount);
            colorSpawn = (colorOfBullet)Random.Range(0, 4);            
            enemySpawned = Instantiate(enemy, spawnPoints[i]*minDistance, transform.rotation).GetComponent<Enemy>();
            enemySpawned.colorType = colorSpawn;
            enemySpawned.initLinearSpeed = Random.Range(minOrbitalSpeed, maxOrbitalSpeed * difficultyFactor * .5f);
            enemySpawned.radiusSpeed = Random.Range(minRadialSpeed, maxRadialSpeed * difficultyFactor);
            yield return new WaitForSeconds(spawnCycleTime);
        }
        
        
    }
    public void AssignColorToObject(SpriteRenderer sprite, colorOfBullet colorType) 
    {
        sprite.color = colors[(int)colorType];
    }

    public void AssignColorToObject(SpriteRenderer sprite, colorOfBullet colorType, TrailRenderer trail) 
    {
        Gradient trailGradient = new Gradient();
        GradientColorKey[] trailColors = new GradientColorKey[2];
        GradientAlphaKey[] trailAlphas = new GradientAlphaKey[2];

        trailAlphas[0].alpha = .4f; 
        trailAlphas[1].alpha = 0f;
        
        trailColors[0].color = colors[(int)colorType];
        trailColors[1].color = colors[(int)colorType];

        trailGradient.SetKeys(trailColors, trailAlphas);
        trail.colorGradient = trailGradient;
        sprite.color = colors[(int)colorType];
    }

    

    public void AddPoint() 
    {        
        points++;
        if (points == scoreToLevel * level)
        { 
            level++;
            difficultyFactor = level < 10 ? Random.Range(1, level) : Random.Range(5,10);
            minDistance = Random.Range(1f - 1f / (level + 1), 1f);
            if (level % 3 == 0 && spawnCycleTime >= 1f) 
            {
                spawnCycleTime -= .25f;
                //scoreToLevel *= 2;
            }
        }
        menuManager.SetScoreText(points.ToString());        
    }

    public void AddCoin() 
    {
        coins++;
        menuManager.ShowADNs();
    }

    public void HealPlayer() 
    {
        lifes = lifeCap;
        menuManager.SetLifesText(lifes.ToString());
        
    }

    public void RemoveLife() 
    {        
        lifes--;
        playerInstance.anim.SetInteger("Life", lifes);
        menuManager.SetLifesText(lifes.ToString());
        if (lifes == 0)
        {
            if (GameData.gameDataIns.itemCounts[(int)PowerUps.MemoryCell] == 0)
                GameOver();
            else 
            {                
                powerUpManager.UseMemoryCellInDeath();
                playerInstance.anim.SetInteger("Life", lifeCap);
            }
        }
    }

    public void GameOver() 
    {
        lifes = 0;
        playerInstance.anim.SetInteger("Life", lifes);
        CheckIfHighScore();
        GameData.gameDataIns.score = points;
        GameData.gameDataIns.coins += coins;        
        menuManager.SetLifesText("0");
        playerInstance.GetComponent<AudioSource>().Play();
        WhitenShooters();        
        hasLost = true;
        Debug.Log("Player Lost");
        //Destroy(player);
        StartCoroutine("DelayMenuGameOver");
        
    }

    void WhitenShooters() 
    {
        for (int i = 0; i < playerShooters.Length; i++)
        {
            playerShooters[i].GetComponent<SpriteRenderer>().color = Color.gray;
        }
    }

    void CheckIfHighScore() 
    {
        if (points > GameData.gameDataIns.highscore) 
        {
            GameData.gameDataIns.highscore = points;
            menuManager.PrintHighscore(points);
        }
    }

    IEnumerator DelayMenuGameOver() 
    {
        yield return new WaitForSeconds(2f);
        //SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        menuManager.SetGameOverMenu();
    }
}