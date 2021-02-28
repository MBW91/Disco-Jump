using UnityEngine;
using UnityEngine.UI;

public class UI : MonoBehaviour 
{
	#region public
    public float AlphaFade = 0.35f;
	#endregion

	#region protected
	#endregion
	
	#region private
    private GameManager GameManagerScript;
    private Text Points, TimeText, Level;
    private Image DesiredBlock, Life_1, Life_2, Life_3;
	#endregion
	
	#region MonoBehaviour
	void Start() 
	{
        GameManagerScript = GameObject.Find("GameManager").GetComponent<GameManager>();
        Points = transform.Find("Points").GetComponent<Text>();
        TimeText = transform.Find("Time").GetComponent<Text>();
        Level = transform.Find("Level").GetComponent<Text>();
        Life_1 = transform.Find("Life_1").GetComponent<Image>();
        Life_2 = transform.Find("Life_2").GetComponent<Image>();
        Life_3 = transform.Find("Life_3").GetComponent<Image>();
        Life_1.sprite = Life_2.sprite = Life_3.sprite = GameManagerScript.PlayerSprite;
        DesiredBlock = transform.Find("DesiredBlock").GetComponent<Image>();
        DesiredBlock.color = GameManagerScript.ColorPalette[(int)GameManagerScript.BlocksToFinish];
        int tLevel = GameManager.Level;
        Level.text = "Level: " + tLevel.ToString();

        float tAlphaFadeBackup = AlphaFade;
        AlphaFade = 0f;
        AdjustAlpha();
        AlphaFade = tAlphaFadeBackup;
	}

	void Update() 
	{
        TimeText.text = ((int)GameManagerScript.Timer).ToString();
        Points.text = GameManager.Points.ToString();
	}

    void FixedUpdate()
    {
        AdjustAlpha();
    }

    /// <summary>
    /// Set the alpha of the life icons, depending on the lasted lives.
    /// </summary>
    void AdjustAlpha()
    {
        switch (GameManager.Lives)
        {
            case 0:
                if (Life_1.color.a > 0f)
                {
                    Life_1.CrossFadeAlpha(0f, AlphaFade, true);
                }
                if (Life_2.color.a > 0f)
                {
                    Life_2.CrossFadeAlpha(0f, AlphaFade, true);
                }
                if (Life_3.color.a > 0f)
                {
                    Life_3.CrossFadeAlpha(0f, AlphaFade, true);
                }
                break;

            case 1:
                if (Life_1.color.a <= 0f)
                {
                    Life_1.CrossFadeAlpha(1f, AlphaFade, true);
                }
                if (Life_2.color.a > 0f)
                {
                    Life_2.CrossFadeAlpha(0f, AlphaFade, true);
                }
                if (Life_3.color.a > 0f)
                {
                    Life_3.CrossFadeAlpha(0f, AlphaFade, true);
                }
                break;

            case 2:
                if (Life_1.color.a <= 0f)
                {
                    Life_1.CrossFadeAlpha(1f, AlphaFade, true);
                }
                if (Life_2.color.a <= 0f)
                {
                    Life_2.CrossFadeAlpha(1f, AlphaFade, true);
                }
                if (Life_3.color.a > 0f)
                {
                    Life_3.CrossFadeAlpha(0f, AlphaFade, true);
                }
                break;

            case 3:
                if (Life_1.color.a <= 0f)
                {
                    Life_1.CrossFadeAlpha(1f, AlphaFade, true);
                }
                if (Life_2.color.a <= 0f)
                {
                    Life_2.CrossFadeAlpha(1f, AlphaFade, true);
                }
                if (Life_3.color.a <= 0f)
                {
                    Life_3.CrossFadeAlpha(1f, AlphaFade, true);
                }
                break;

            default:
                if (Life_1.color.a > 0f)
                {
                    Life_1.CrossFadeAlpha(0f, AlphaFade, true);
                }
                if (Life_2.color.a > 0f)
                {
                    Life_2.CrossFadeAlpha(0f, AlphaFade, true);
                }
                if (Life_3.color.a > 0f)
                {
                    Life_3.CrossFadeAlpha(0f, AlphaFade, true);
                }
                break;
        }
    }

    /// <summary>
    /// Toggle the pause state.
    /// </summary>
    public void TogglePause()
    {
        if(Time.timeScale < 1f)
        {
            Time.timeScale = 1f;
        }
        else
        {
            Time.timeScale = 0f;
        }
    }
	#endregion
}
