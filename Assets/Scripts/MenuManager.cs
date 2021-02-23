using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEditor;
using TMPro;

public enum MenuIndex { main = 0, gameOver = 1, settings = 2, pause = 3, shop = 4, tutorial = 5, credits = 6 }
public class MenuManager : MonoBehaviour
{
    public static bool isPaused = false;

    public Camera cam;
    public TMP_Text scoreText, lifeText;
    public Image[] fireButtons = new Image[4];
    public RectTransform 
    ADNHold, 
    buttonHolderLeft, 
    buttonHolderRight,
    pauseButton;
    public TMP_Text 
    killerCountText, 
    memoryCountText, 
    ADNShopPocket;
    public GameObject highscoreUI;
    TMP_Text highscoreText;
    Canvas canvasMain;
    Camera camInstance;
    GameNColorManager gameManager;
    const int menuCount = 7;
    RectTransform[] menus = new RectTransform[menuCount];
    TMP_Text ADNText;
    Animator ADNHoldAnim;
    Slider settingsSlider;
    Toggle settingsToggle;
    
    PlayerMain player;

    public void InitMenu()
    {
        gameManager = FindObjectOfType<GameNColorManager>();
        player = FindObjectOfType<PlayerMain>();
        SpawnCamera();
        for (int i = 0; i < menuCount; i++)
        {
            menus[i] = canvasMain.transform.GetChild(i).GetComponent<RectTransform>();
        }
        

        ADNText = ADNHold.transform.GetChild(1).GetComponent<TMP_Text>();
        ADNHoldAnim = ADNHold.gameObject.GetComponent<Animator>();

        settingsSlider = menus[(int)MenuIndex.settings].GetChild(0).GetComponent<Slider>();
        settingsToggle = menus[(int)MenuIndex.settings].GetChild(3).GetComponent<Toggle>();
        
        
    }

    void SpawnCamera() 
    {
        camInstance = FindObjectOfType<Camera>();
        
        if (camInstance == null) 
        {
            camInstance = Instantiate(cam, Vector3.forward * -10f, Quaternion.identity);
        }
        canvasMain = FindObjectOfType<Canvas>();
        if (canvasMain == null) 
        {
            Debug.LogError("No canvas in scene!!!");
        }
    }
    public void SetHolders()
    {
        float width = cam.pixelWidth;
        float height = cam.pixelHeight;

        float aspectRatio = ((width - height) * .5f) / width;

        buttonHolderLeft.anchorMin = new Vector2(0f, 0f);
        buttonHolderLeft.anchorMax = new Vector2(aspectRatio, 1f);
        buttonHolderRight.anchorMin = new Vector2(1-aspectRatio, 0f);
        buttonHolderRight.anchorMax = new Vector2(1f, 1f);

        buttonHolderLeft.offsetMax = Vector2.zero; buttonHolderLeft.offsetMin = Vector2.zero;
        buttonHolderRight.offsetMax = Vector2.zero; buttonHolderRight.offsetMin = Vector2.zero;        

        foreach (RectTransform menu in menus) 
        {
            menu.anchorMin = new Vector2(aspectRatio, 0f);
            menu.anchorMax = new Vector2(1f - aspectRatio, 1f);
            menu.offsetMin = Vector2.zero; menu.offsetMax = Vector2.zero;
        }

        cam.rect = new Rect(aspectRatio, 0f, height / width, 1f);
    }

    public void AssignColorToButtons()
    {
        Color col;
        
        for (int i = 0; i < fireButtons.Length; i++)
        {
            col = gameManager.colors[i]; col.a = 0.7f;
            fireButtons[i].color = col;
        }
    }

    public void RemoveColorToButtons()
    {
        Color col = Color.grey;

        for (int i = 0; i < fireButtons.Length; i++)
        {            
            fireButtons[i].color = col;
        }
    }

    public void ShowSingleMenu(int exceptionIndex) 
    {
        foreach (RectTransform menu in menus)
        {
            if(menu != menus[exceptionIndex])
                menu.gameObject.SetActive(false);
        }

        menus[exceptionIndex].gameObject.SetActive(true);
    }
    public void SetMainMenu() 
    {
        gameManager.onSettings = false;
        ShowSingleMenu((int)MenuIndex.main);
        pauseButton.gameObject.SetActive(false);
        //Button startButton = menuHolder.GetChild(0).GetComponent<Button>();
        //menuHolder.gameObject.SetActive(true);
        //gameOverHolder.gameObject.SetActive(false);
        //settingsHolder.gameObject.SetActive(false);
        print("main Menu setted");
        //startButton.onClick.RemoveAllListeners();
        //startButton.onClick.AddListener(GameNColorManager.gameManagerInstance.LoadGameScene);
    }

    public void SetMenuToGame() 
    {
        foreach (RectTransform menu in menus) 
        {
            menu.gameObject.SetActive(false);
        }
        pauseButton.gameObject.SetActive(true);
    }

    public void SetSettingsMenu() 
    {
        ShowSingleMenu((int)MenuIndex.settings);
        gameManager.onSettings = true;
        
    }

    public void StartTutorial() 
    {
        ShowSingleMenu((int)MenuIndex.tutorial);
        gameManager.onTutorial = true;
        AssignColorToButtons();
    }

    public void EndTutorial() 
    {
        gameManager.onTutorial = false;
        RemoveColorToButtons();
        gameManager.ResetRotationOfPlayer();
    }

    public void PauseGame()
    {
        ShowSingleMenu((int)MenuIndex.pause);
        Time.timeScale = 0f;
        Time.fixedDeltaTime = 0.02f * Time.timeScale;
        isPaused = true;
    }

    public void ResumeGame()
    {
        SetMenuToGame();
        Time.timeScale = 1f;
        Time.fixedDeltaTime = 0.02f * Time.timeScale;
        isPaused = false;
    }

    public void ChangeSensitivity() 
    {        
        player.rotationSensitivity = settingsSlider.value;
    }

    public void ToggleJoystick() 
    {        
        player.lockJoystick = settingsToggle.isOn;
    }
    public void SetGameOverMenu() 
    {
        ShowSingleMenu((int)MenuIndex.gameOver);
        pauseButton.gameObject.SetActive(false);
    }

    public void PrintHighscore(int highscore)
    {
        highscoreText = highscoreUI.transform.GetChild(0).GetComponent<TMP_Text>();
        highscoreText.text = highscore.ToString();
        highscoreUI.SetActive(true);
    }

    public void StartGame() 
    {
        gameManager.InitializeGame();
    }

    public void RestartGame(bool tryAgain) 
    {
        highscoreUI.SetActive(false);
        GameData.gameDataIns.gameStarted = tryAgain;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    
    public void CloseGame()
    {
        GameData.gameDataIns.SaveAllData();
        #if UNITY_EDITOR
            EditorApplication.isPlaying = false;
        #else
            Application.Quit();
        #endif
    }

    public void SetScoreText(string score) 
    {
        scoreText.text = score;        
    }

    public void SetLifesText(string lifes) 
    {
        lifeText.text = lifes;
    }

    public void ShowADNs() 
    {
        ADNHoldAnim.SetTrigger("Show");
        ADNText.text = gameManager.coins.ToString();
    }

    public void ShowADNPocket(string count) 
    {
        ADNShopPocket.text = count;
    }

    public void PrintItemCount() 
    {
        killerCountText.text = GameData.gameDataIns.itemCounts[(int)PowerUps.KillerCell].ToString();
        memoryCountText.text = GameData.gameDataIns.itemCounts[(int)PowerUps.MemoryCell].ToString();
        SetLifesText(gameManager.lifes.ToString());
    }
    
}
