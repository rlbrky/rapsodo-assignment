using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    
    [Header("UI Elements")]
    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private TextMeshProUGUI gameOverText;
    [SerializeField] private Slider staminaSlider;
    
    [Header("Debug")]
    [SerializeField] private float points;

    private void Awake()
    {
        if(instance == null)
            instance = this;
        else
            Destroy(gameObject);
    }

    public void SetStaminaInformation(float maxStamina)
    {
        staminaSlider.maxValue = maxStamina;
        staminaSlider.value = maxStamina;
    }
    
    //Method is called from the NPC to update UI slider as well.
    public void UpdateStaminaInformation(float val)
    {
        staminaSlider.value = val;
    }
    
    //Updates the UI and keeps track of the current score.
    public void UpdateScore(float pointToAdd)
    {
        points += pointToAdd;
        scoreText.text = "Score: " + points.ToString();
    }

    //Closes the app after 5 seconds of showing end game screen.
    public void EndGame()
    {
        gameOverText.gameObject.SetActive(true);
        Time.timeScale = 0;
        StartCoroutine(CloseGame());
    }

    IEnumerator CloseGame()
    {
        yield return new WaitForSecondsRealtime(5f);
        Application.Quit();
    }
}
