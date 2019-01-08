using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuController : MonoBehaviour {
    public bool GameIsPaused, GameOver;
    public GameObject pauseMenuUI;
    public GameObject confirmQuitCanvasGroup;
    public GameObject gameoverMenu;
    public GameObject optionMenu;

    [Space]
    [Header("StartScene")]
    public bool doesItNeedIntro = true;
    public TMP_Text Stage;
    public Canvas otherCanvas;
    float count = 0;


    [Space]
    [Header("Volume")]
    public TMP_Text volumeValue;
    public Slider volumeSlider;

    [Space]
    [Header("Mouse sensibility")]
    public TMP_Text sensValue;
    public Slider sensSlider;

    [Space]
    [Header("Crosshairs")]
    public Sprite[] crosshairSprites = new Sprite[12];
    public Sprite currentCrosshair;
    public GameObject optionsCrosshair;
    public GameObject inGameCrosshair;

    public bool isOnline = false;

    private void Start()
    {
        sensSlider.value = PlayerStatsInfo.sens;
    }

    // Update is called once per frame
    void Update()
    {
        if (doesItNeedIntro)
        {
            count += 1 * Time.deltaTime;
            if (count >= 4)
            {
                otherCanvas.gameObject.SetActive(false);
            }
            Stage.text = "STAGE " + (PlayerStatsInfo.stage - 1).ToString();
        }
        if (!GameOver)
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                if (GameIsPaused)
                {
                    Cursor.lockState = CursorLockMode.Locked;
                    Resume();
                    Cursor.visible = false;
                }
                else
                {
                    Cursor.visible = true;
                    Cursor.lockState = CursorLockMode.None;
                    Pause();
                }
            }
        }
        else if (GameOver)
        {
            Cursor.lockState = CursorLockMode.None;
            Defeat();
        }

        SetCurrentCrosshair(optionsCrosshair, inGameCrosshair);
    }

    private void SetCurrentCrosshair(params GameObject[] gO)
    {
        currentCrosshair = crosshairSprites[PlayerStatsInfo.crosshairNumber];
        PlayerStatsInfo.currentCrosshair = currentCrosshair;
        gO[0].GetComponent<Image>().sprite = currentCrosshair;
        gO[1].GetComponent<Image>().sprite = currentCrosshair;
    }

    //CLOSING THE GAME FUNCTION
    public void QuitGame()
    {
        Application.Quit();
    }

    //PAUSE MENU
    #region Pause Menu
    public void Resume()
    {
        pauseMenuUI.SetActive(false);
        confirmQuitCanvasGroup.SetActive(false);
        optionMenu.SetActive(false);
        if (!isOnline)
            Time.timeScale = 1f;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        GameIsPaused = false;
    }

    public void Pause()
    {
        pauseMenuUI.SetActive(true);
        if (!isOnline)
            Time.timeScale = 0f;
        GameIsPaused = true;
    }

    public void LoadMenu()
    {
        SceneManager.LoadScene(0); 
    }

    public void Options()
    {
        pauseMenuUI.GetComponent<CanvasGroup>().alpha = 0.5f;
        pauseMenuUI.GetComponent<CanvasGroup>().interactable = false;
        pauseMenuUI.GetComponent<CanvasGroup>().blocksRaycasts = false;

        optionMenu.SetActive(true);
        optionMenu.GetComponent<CanvasGroup>().alpha = 1f;
        optionMenu.GetComponent<CanvasGroup>().interactable = true;
        optionMenu.GetComponent<CanvasGroup>().blocksRaycasts = true;
    }

    public void OptionsBack()
    {
        pauseMenuUI.GetComponent<CanvasGroup>().alpha = 1f;
        pauseMenuUI.GetComponent<CanvasGroup>().interactable = true;
        pauseMenuUI.GetComponent<CanvasGroup>().blocksRaycasts = true;

        optionMenu.SetActive(false);
        optionMenu.GetComponent<CanvasGroup>().interactable = false;
        optionMenu.GetComponent<CanvasGroup>().blocksRaycasts = false;
        optionMenu.GetComponent<CanvasGroup>().alpha = 0f;
    }

    public void VolumeValue()
    {
        volumeValue.text = ((int)(volumeSlider.value * 100)).ToString(); 
    }

    public void SensValue()
    {
        sensValue.text = ((int)(sensSlider.value)).ToString();
        PlayerStatsInfo.sens = sensSlider.value;
    }


    public void RightArrow()
    {
        if (PlayerStatsInfo.crosshairNumber < crosshairSprites.Length - 1)
        { 
            PlayerStatsInfo.crosshairNumber++;
        }
        else
        {
            PlayerStatsInfo.crosshairNumber = 0;
        }
    }

    public void LeftArrow()
    {
        if (PlayerStatsInfo.crosshairNumber > 0)
        {
            PlayerStatsInfo.crosshairNumber--;
        }
        else
        {
            PlayerStatsInfo.crosshairNumber = crosshairSprites.Length - 1;
        }
    }
    #endregion


    //QUIT MENU
    #region Quit Menu

    public void ConfirmQuit()
    {
        pauseMenuUI.GetComponent<CanvasGroup>().alpha = 0.5f;
        pauseMenuUI.GetComponent<CanvasGroup>().interactable = false;
        pauseMenuUI.GetComponent<CanvasGroup>().blocksRaycasts = false;

        confirmQuitCanvasGroup.SetActive(true);
        confirmQuitCanvasGroup.GetComponent<CanvasGroup>().alpha = 1f;
        confirmQuitCanvasGroup.GetComponent<CanvasGroup>().interactable = true;
        confirmQuitCanvasGroup.GetComponent<CanvasGroup>().blocksRaycasts = true;
    }

    public void ConfirmNo()
    {
        pauseMenuUI.GetComponent<CanvasGroup>().alpha = 1f;
        pauseMenuUI.GetComponent<CanvasGroup>().interactable = true;
        pauseMenuUI.GetComponent<CanvasGroup>().blocksRaycasts = true;

        confirmQuitCanvasGroup.SetActive(false);
        confirmQuitCanvasGroup.GetComponent<CanvasGroup>().alpha = 0f;
        confirmQuitCanvasGroup.GetComponent<CanvasGroup>().interactable = false;
        confirmQuitCanvasGroup.GetComponent<CanvasGroup>().blocksRaycasts = false;
    }

    #endregion

    //GAME OVER MENU
    #region Game Over Menu

    private void Defeat()
    {
		transform.Find("CrossHair").gameObject.SetActive(false);
		transform.Find("CurrentWeapon").gameObject.SetActive(false);
		transform.Find("EnemyInfo").gameObject.SetActive(false);
		gameoverMenu.SetActive(true);
        Time.timeScale = 0f;
        GameIsPaused = true;
    }

    public void RestartGame()
    {
        //INSERT CODE
    }

    #endregion

}
