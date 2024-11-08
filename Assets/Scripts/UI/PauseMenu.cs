using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PauseMenu : MonoBehaviour
{
    [Header("UI Elements")]
    [SerializeField] private GameObject quitButton;
    [SerializeField] private GameObject pauseText;
    
    private bool isPaused = false;
    
    // Start is called before the first frame update
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (!isPaused)
            {
                isPaused = true;
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
                quitButton.SetActive(true);
                pauseText.SetActive(true);
                Time.timeScale = 0;
            }
            else
            {
                Time.timeScale = 1;
                isPaused = false;   
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
                quitButton.SetActive(false);
                pauseText.SetActive(false);
            }
        }
    }
    
    public void OnQuitButton()
    {
        Application.Quit();
    }
}
