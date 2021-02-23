using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameData : MonoBehaviour
{
    public static GameData gameDataIns;

    public bool gameStarted = false;
    public int score = 0, highscore = 0, coins = 0, lifeCap = 3;
    public int[] itemCounts = new int[3];
    private void Awake()
    {
        Singleton();
        LoadAllData();
    }

    void Singleton() 
    {
        if (gameDataIns == null)
        {
            gameDataIns = this;
            DontDestroyOnLoad(gameObject);
        }
        else 
        {
            Destroy(gameObject);
        }
    }

    public void SaveAllData() 
    {
        PlayerPrefs.SetInt("Highscore", highscore);
        PlayerPrefs.SetInt("Coins", coins);
        //PlayerPrefs.SetInt("Life cap", lifeCap);

        PlayerPrefs.SetInt("Killer Cells", itemCounts[(int)PowerUps.KillerCell]);
        PlayerPrefs.SetInt("Memory Cells", itemCounts[(int)PowerUps.MemoryCell]);
        PlayerPrefs.SetInt("Mitochondrias", itemCounts[(int)PowerUps.Mitochondria]);
    }

    void LoadAllData() 
    {
        highscore = PlayerPrefs.GetInt("Highscore");
        coins = PlayerPrefs.GetInt("Coins");
        //lifeCap = PlayerPrefs.GetInt("Life Cap");

        itemCounts[(int)PowerUps.KillerCell] = PlayerPrefs.GetInt("Killer Cells");
        itemCounts[(int)PowerUps.MemoryCell] = PlayerPrefs.GetInt("Memory Cells");
        itemCounts[(int)PowerUps.Mitochondria] = PlayerPrefs.GetInt("Mitochondrias");

        lifeCap += itemCounts[(int)PowerUps.Mitochondria];
    }
    
}
