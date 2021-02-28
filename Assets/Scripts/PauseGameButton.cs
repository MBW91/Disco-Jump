using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PauseGameButton : MonoBehaviour
{
    #region public
    public Sprite Pause, Play;
    #endregion

    #region protected
    #endregion

    #region private
    private Image myButtonImage;
    private GameManager Manager;
    private GameObject PauseMenuButtons;
    #endregion

    #region MonoBehaviour
    void Awake()
    {
        myButtonImage = GetComponent<Image>();
        Manager = GameObject.Find("GameManager").GetComponent<GameManager>();
        PauseMenuButtons = GameObject.Find("PauseMenuButtons");
        PauseMenuButtons.SetActive(false);
    }

    void Update()
    {
        if (!Manager.GameIsEnding)
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                TogglePause();
            }
        }
    }

    /// <summary>
    /// Toggles the pause state.
    /// </summary>
    public void TogglePause()
    {
        GameManager.Pause = !GameManager.Pause;

        if(GameManager.Pause)
        {
            myButtonImage.sprite = Play;
            PauseMenuButtons.SetActive(true);
        }
        else
        {
            myButtonImage.sprite = Pause;
            PauseMenuButtons.SetActive(false);
        }
    }

    /// <summary>
    /// Loads the main menu.
    /// </summary>
    public void QuitToMainMenu()
    {
        SceneManager.LoadScene(0);
        Manager.SetToDefault();
    }
    #endregion
}
