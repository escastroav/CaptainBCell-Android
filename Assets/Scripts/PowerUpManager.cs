using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum PowerUps { KillerCell = 0, MemoryCell = 1, Mitochondria = 2}

public class PowerUpManager : MonoBehaviour
{
    
    public int[] itemCounts = new int[3];
    public int[] itemCaps = new int[3];
    public int[] itemPrices = new int[3];
    int totalCoins = 0;
    GameNColorManager gameManager;
    MenuManager menuManager;
    [SerializeField]Enemy[] enemiesInGame;
    private void Awake()
    {
        menuManager = FindObjectOfType<MenuManager>();
        gameManager = FindObjectOfType<GameNColorManager>();        
    }

    public void UpdateItemData() 
    {
        totalCoins = GameData.gameDataIns.coins;
        itemCounts = GameData.gameDataIns.itemCounts;
        menuManager.ShowADNPocket(totalCoins.ToString());
        menuManager.PrintItemCount();
    }

    public void StoreItemData(int coins, int[] itemCounts) 
    {
        GameData.gameDataIns.coins = coins;
        GameData.gameDataIns.lifeCap = gameManager.lifeCap;
        GameData.gameDataIns.itemCounts = itemCounts;
        menuManager.ShowADNPocket(totalCoins.ToString());
    }

    public void UseItemOfButton(int itemIndex) 
    {
        if (itemCounts[itemIndex] > 0) 
        {
            itemCounts[itemIndex]--;
            ActivatePowerUp(itemIndex);
            UpdateItemData();

        }
    }

    void ActivatePowerUp(int itemIndex) 
    {
        switch (itemIndex) 
        {
            case (int)PowerUps.KillerCell:
                UseKillerCell();
                break;
            case (int)PowerUps.MemoryCell:
                UseMemoryCell();
                break;
        }
    }
    void UseKillerCell() 
    {
        SeekEnemies();
        KillEnemies();
        
        StoreItemData(totalCoins, itemCounts);
    }
    void UseMemoryCell() 
    {
        gameManager.HealPlayer();
        
        StoreItemData(totalCoins, itemCounts);
    }
    public void UseMemoryCellInDeath() 
    {
        SeekEnemies();
        DespawnEnemies();
        gameManager.HealPlayer();
        itemCounts[(int)PowerUps.MemoryCell]--;
        StoreItemData(totalCoins, itemCounts);
        UpdateItemData();
    }

    void SeekEnemies() 
    {
        enemiesInGame = FindObjectsOfType<Enemy>();
    }

    void KillEnemies() 
    {
        foreach (Enemy enemy in enemiesInGame)
        {
            enemy.DieByKillerCell();
        }
        Array.Clear(enemiesInGame, 0, enemiesInGame.Length);
        enemiesInGame = new Enemy[0];
    }

    void DespawnEnemies() 
    {
        foreach (Enemy enemy in enemiesInGame)
        {
            enemy.Despawn();
        }
        Array.Clear(enemiesInGame, 0, enemiesInGame.Length);
        enemiesInGame = new Enemy[0];
    }

    public void BuyItem(int itemIndex) 
    {        
        int price = itemPrices[itemIndex];
        //UpdateItemData();
        if (totalCoins >= price && itemCounts[itemIndex] < itemCaps[itemIndex]) 
        {
            itemCounts[itemIndex]++;
            totalCoins -= price;
            if (itemIndex == (int)PowerUps.Mitochondria) 
            {
                gameManager.lifeCap += itemCounts[itemIndex];
                gameManager.HealPlayer();
            }
            StoreItemData(totalCoins, itemCounts);
            UpdateItemData();
        }
        
    }


    
}
