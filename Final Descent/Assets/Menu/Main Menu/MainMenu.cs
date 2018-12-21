using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour {
    public GameObject allWeapons;

    [Space]
    [Header("Volume")]
    public TMP_Text volumeValue;
    public Slider volumeSlider;

    [Space]
    [Header("Mouse sensibility")]
    public TMP_Text sensValue;
    public Slider sensSlider;

    public void Awake()
    {
        PlayerStatsInfo.FindAllWeapons(allWeapons.GetComponent<AllWeapons>().allWeapons);
    }

    public void PlayGame()
    {
        PlayerStatsInfo.ResetInfo();
        SceneManager.LoadScene(1);
    }

    public void LoadGame()
    {
        PlayerStatsInfo.LoadProgress();
        SceneManager.LoadScene(PlayerStatsInfo.stage);
    }

    public void NextScene()
    {
        SceneManager.LoadScene(1);
    }

    public void Multiplayer()
    {
        SceneManager.LoadScene(4);
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


    public void QuitGame()
    {
        Debug.Log("Quit!");
        Application.Quit();
    }
}
