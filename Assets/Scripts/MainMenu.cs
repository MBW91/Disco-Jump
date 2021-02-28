using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    #region public
    #endregion

    #region protected
    #endregion

    #region private
    private CharacterSpritesHolder CharacterHolder;
    private GameObject HighscoresScreen;
    private bool ShowHighscores = false;
    private int[] HighscoreNumbers = new int[5];
    private Text HighscoresText;
    private Text CoinsText;
    private int CharacterIndex = 0;
    private Image CharacterSelectionImage;
    private GameObject UnlockButtonGO;
    private Button UnlockButton;
    private Text NeededCoins;
    private int Coins = 0;
    private bool[] CharacterUnlock;
    #endregion

    #region MonoBehaviour
    void Awake()
    {
        CharacterHolder = GameObject.Find("CharacterSpritesHolder").GetComponent<CharacterSpritesHolder>();
        HighscoresScreen = transform.Find("HighscoresScreen").gameObject;
        HighscoresText = HighscoresScreen.transform.Find("RankValues").GetComponent<Text>();
        CharacterSelectionImage = GameObject.Find("Character").GetComponent<Image>();
        CoinsText = GameObject.Find("Coins").GetComponent<Text>();
        UnlockButtonGO = GameObject.Find("Unlock");
        UnlockButton = UnlockButtonGO.GetComponent<Button>();
        UnlockButtonGO.SetActive(false);
        CharacterUnlock = new bool[CharacterHolder.CharacterSprites.Length];
        NeededCoins = UnlockButtonGO.transform.Find("NeededCoins").GetComponent<Text>();

        if (PlayerPrefs.HasKey("CharacterIndex"))
        {
            CharacterIndex = PlayerPrefs.GetInt("CharacterIndex");
            CharacterSelectionImage.sprite = CharacterHolder.CharacterSprites[CharacterIndex];
        }
        else
        {
            CharacterIndex = 0;
            PlayerPrefs.SetInt("CharacterIndex", 0);
            PlayerPrefs.Save();
            CharacterSelectionImage.sprite = CharacterHolder.CharacterSprites[CharacterIndex];
        }

        NeededCoins.text = CharacterHolder.CoinCost[CharacterIndex].ToString();

        if (PlayerPrefs.HasKey("Coins"))
        {
            Coins = PlayerPrefs.GetInt("Coins");
            CoinsText.text = Coins.ToString();
        }
        else
        {
            Coins = 0;
            PlayerPrefs.SetInt("Coins", 0);
            PlayerPrefs.Save();
            CoinsText.text = Coins.ToString();
        }

        string tChar = "";
        for (int i = 0; i < CharacterUnlock.Length; i++)
        {
            tChar = "Character" + i;

            if(!PlayerPrefs.HasKey(tChar))
            {
                if(i != 0)
                {
                    PlayerPrefsX.SetBool(tChar, false);
                }
                else
                {
                    PlayerPrefsX.SetBool(tChar, true);
                }
                
                PlayerPrefs.Save();
            }
            CharacterUnlock[i] = PlayerPrefsX.GetBool(tChar);
        }

        if (CharacterUnlock[CharacterIndex])
        {
            UnlockButtonGO.SetActive(false);
        }
        else
        {
            UnlockButtonGO.SetActive(true);
        }

        if (PlayerPrefs.HasKey("1."))
        {
            HighscoreNumbers[0] = PlayerPrefs.GetInt("1.");
        }
        else
        {
            HighscoreNumbers[0] = 0;
            PlayerPrefs.SetInt("1.", 0);
        }

        if (PlayerPrefs.HasKey("2."))
        {
            HighscoreNumbers[1] = PlayerPrefs.GetInt("2.");
        }
        else
        {
            HighscoreNumbers[1] = 0;
            PlayerPrefs.SetInt("2.", 0);
        }

        if (PlayerPrefs.HasKey("3."))
        {
            HighscoreNumbers[2] = PlayerPrefs.GetInt("3.");
        }
        else
        {
            HighscoreNumbers[2] = 0;
            PlayerPrefs.SetInt("3.", 0);
        }

        if (PlayerPrefs.HasKey("4."))
        {
            HighscoreNumbers[3] = PlayerPrefs.GetInt("4.");
        }
        else
        {
            HighscoreNumbers[3] = 0;
            PlayerPrefs.SetInt("4.", 0);
        }

        if (PlayerPrefs.HasKey("5."))
        {
            HighscoreNumbers[4] = PlayerPrefs.GetInt("5.");
        }
        else
        {
            HighscoreNumbers[4] = 0;
            PlayerPrefs.SetInt("5.", 0);
        }

        string tRankValues = "";

        for (int i = 0; i < HighscoreNumbers.Length; i++)
        {
            tRankValues += HighscoreNumbers[i];

            if (i < HighscoreNumbers.Length - 1)
            {
                tRankValues += "\n";
            }
        }

        HighscoresText.text = tRankValues;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            HighscoresScreen.SetActive(false);
        }

        CheckIfCharacterIsUnlockable();

        Cheat();
    }

    /// <summary>
    /// A cheat to get coins, to test the character unlocking.
    /// </summary>
    private void Cheat()
    {
        #if UNITY_EDITOR
            if (Input.GetKeyDown(KeyCode.KeypadMinus))
            {
                Coins -= 1000;
                CoinsText.text = Coins.ToString();
                PlayerPrefs.SetInt("Coins", Coins);
                PlayerPrefs.Save();
            }
            else if (Input.GetKeyDown(KeyCode.KeypadPlus))
            {
                Coins += 1000;
                CoinsText.text = Coins.ToString();
                PlayerPrefs.SetInt("Coins", Coins);
                PlayerPrefs.Save();
            }
            else if (Input.GetKeyDown(KeyCode.Keypad0))
            {
                PlayerPrefs.DeleteAll();
            }
        #endif
    }

    /// <summary>
    /// Update logic to check if the selected character is unlockable or not.
    /// </summary>
    private void CheckIfCharacterIsUnlockable()
    {
        if (CharacterHolder.CoinCost[CharacterIndex] <= Coins)
        {
            UnlockButton.interactable = true;
        }
        else
        {
            UnlockButton.interactable = false;
        }

        if (CharacterUnlock[CharacterIndex])
        {
            UnlockButtonGO.SetActive(false);
        }
        else
        {
            UnlockButtonGO.SetActive(true);
        }
    }

    /// <summary>
    /// Start the game with the first level.
    /// </summary>
    public void Play()
    {
        SceneManager.LoadScene(1);
    }

    /// <summary>
    /// Exit the game.
    /// </summary>
    public void Quit()
    {
        Application.Quit();
    }

    /// <summary>
    /// Show the highscores window.
    /// </summary>
    public void Highscores()
    {
        ShowHighscores = !ShowHighscores;

        if (ShowHighscores)
        {
            HighscoresScreen.SetActive(true);
        }
        else
        {
            HighscoresScreen.SetActive(false);
        }
    }

    /// <summary>
    /// Get the next character into the selection.
    /// </summary>
    public void NextCharacter()
    {
        CharacterSelectionImage.color = Color.black;

        CharacterIndex++;

        if (CharacterIndex >= CharacterHolder.CharacterSprites.Length)
        {
            CharacterIndex = 0;
        }

        NeededCoins.text = CharacterHolder.CoinCost[CharacterIndex].ToString();
        CharacterSelectionImage.sprite = CharacterHolder.CharacterSprites[CharacterIndex];

        if(CharacterUnlock[CharacterIndex])
        {
            CharacterSelectionImage.color = Color.white;
            PlayerPrefs.SetInt("CharacterIndex", CharacterIndex);
            PlayerPrefs.Save();
        }
    }

    /// <summary>
    /// Get the previous character into the selection.
    /// </summary>
    public void PreviousCharacter()
    {
        CharacterSelectionImage.color = Color.black;

        CharacterIndex--;

        if (CharacterIndex < 0)
        {
            CharacterIndex = CharacterHolder.CharacterSprites.Length - 1;
        }

        NeededCoins.text = CharacterHolder.CoinCost[CharacterIndex].ToString();
        CharacterSelectionImage.sprite = CharacterHolder.CharacterSprites[CharacterIndex];

        if (CharacterUnlock[CharacterIndex])
        {
            CharacterSelectionImage.color = Color.white;
            PlayerPrefs.SetInt("CharacterIndex", CharacterIndex);
            PlayerPrefs.Save();
        }
    }

    /// <summary>
    /// Unlock the selected character.
    /// </summary>
    public void Unlock()
    {
        Coins -= CharacterHolder.CoinCost[CharacterIndex];
        PlayerPrefs.SetInt("Coins", Coins);
        PlayerPrefs.Save();
        CoinsText.text = Coins.ToString();

        CharacterUnlock[CharacterIndex] = true;
        CharacterSelectionImage.color = Color.white;
        PlayerPrefsX.SetBool("Character" + CharacterIndex, true);
        PlayerPrefs.SetInt("CharacterIndex", CharacterIndex);
        PlayerPrefs.Save();
    }

    /// <summary>
    /// Open my website.
    /// </summary>
    public void XerrokHyperlink()
    {
        Application.OpenURL("www.xerrok.de");
    }
    #endregion
}
