using UnityEngine;

public class Block : MonoBehaviour 
{
	#region public
    [HideInInspector]
    public int ColorIndex = 0;
    public float FadeColorSpeed = 0.1f;
    public bool isMainMenu = false;
	#endregion

	#region protected
	#endregion
	
	#region private
    private GameManager GameManagerScript;
    private SpriteRenderer mySpriteRenderer;
    private Color[] ColorPalette;
    private float Timer = 0f;
	#endregion
	
	#region MonoBehaviour
	void Start() 
	{
        mySpriteRenderer = GetComponent<SpriteRenderer>();

        if (!isMainMenu)
        {
            GameManagerScript = GameObject.Find("GameManager").GetComponent<GameManager>();
            ColorPalette = GameManagerScript.ColorPalette;
        }
        else
        {
            ColorPalette = new Color[6];

            for (int i = 0; i < ColorPalette.Length; i++)
            {
                while (ColorPalette[i] == new Color() || (ColorPalette[i].r + ColorPalette[i].g + ColorPalette[i].b) < 1.5f)
                {
                    ColorPalette[i] = new Color(Random.Range(0.01f, 1f), Random.Range(0.01f, 1f), Random.Range(0.01f, 1f));
                }
            }
        }
	}

	void Update()
    {
        MainMenuBackgroundBlocks();
        BlockColorFader();
    }

    /// <summary>
    /// Update logic for fading the color of the blocks.
    /// </summary>
    private void BlockColorFader()
    {
        if (mySpriteRenderer.color != ColorPalette[ColorIndex])
        {
            float tDifference = 0f;

            if (mySpriteRenderer.color.r < ColorPalette[ColorIndex].r)
            {
                tDifference = ColorPalette[ColorIndex].r - mySpriteRenderer.color.r;

                if (tDifference > FadeColorSpeed)
                {
                    mySpriteRenderer.color = new Color(mySpriteRenderer.color.r + FadeColorSpeed, mySpriteRenderer.color.g, mySpriteRenderer.color.b);
                }
                else
                {
                    mySpriteRenderer.color = new Color(mySpriteRenderer.color.r + tDifference, mySpriteRenderer.color.g, mySpriteRenderer.color.b);
                }
            }
            else if (mySpriteRenderer.color.r > ColorPalette[ColorIndex].r)
            {
                tDifference = mySpriteRenderer.color.r - ColorPalette[ColorIndex].r;

                if (tDifference > FadeColorSpeed)
                {
                    mySpriteRenderer.color = new Color(mySpriteRenderer.color.r - FadeColorSpeed, mySpriteRenderer.color.g, mySpriteRenderer.color.b);
                }
                else
                {
                    mySpriteRenderer.color = new Color(mySpriteRenderer.color.r - tDifference, mySpriteRenderer.color.g, mySpriteRenderer.color.b);
                }
            }

            if (mySpriteRenderer.color.g < ColorPalette[ColorIndex].g)
            {
                tDifference = ColorPalette[ColorIndex].g - mySpriteRenderer.color.g;

                if (tDifference > FadeColorSpeed)
                {
                    mySpriteRenderer.color = new Color(mySpriteRenderer.color.r, mySpriteRenderer.color.g + FadeColorSpeed, mySpriteRenderer.color.b);
                }
                else
                {
                    mySpriteRenderer.color = new Color(mySpriteRenderer.color.r, mySpriteRenderer.color.g + tDifference, mySpriteRenderer.color.b);
                }
            }
            else if (mySpriteRenderer.color.g > ColorPalette[ColorIndex].g)
            {
                tDifference = mySpriteRenderer.color.g - ColorPalette[ColorIndex].g;

                if (tDifference > FadeColorSpeed)
                {
                    mySpriteRenderer.color = new Color(mySpriteRenderer.color.r, mySpriteRenderer.color.g - FadeColorSpeed, mySpriteRenderer.color.b);
                }
                else
                {
                    mySpriteRenderer.color = new Color(mySpriteRenderer.color.r, mySpriteRenderer.color.g - tDifference, mySpriteRenderer.color.b);
                }
            }

            if (mySpriteRenderer.color.b < ColorPalette[ColorIndex].b)
            {
                tDifference = ColorPalette[ColorIndex].b - mySpriteRenderer.color.b;

                if (tDifference > FadeColorSpeed)
                {
                    mySpriteRenderer.color = new Color(mySpriteRenderer.color.r, mySpriteRenderer.color.g, mySpriteRenderer.color.b + FadeColorSpeed);
                }
                else
                {
                    mySpriteRenderer.color = new Color(mySpriteRenderer.color.r, mySpriteRenderer.color.g, mySpriteRenderer.color.b + tDifference);
                }
            }
            else if (mySpriteRenderer.color.b > ColorPalette[ColorIndex].b)
            {
                tDifference = mySpriteRenderer.color.b - ColorPalette[ColorIndex].b;

                if (tDifference > FadeColorSpeed)
                {
                    mySpriteRenderer.color = new Color(mySpriteRenderer.color.r, mySpriteRenderer.color.g, mySpriteRenderer.color.b - FadeColorSpeed);
                }
                else
                {
                    mySpriteRenderer.color = new Color(mySpriteRenderer.color.r, mySpriteRenderer.color.g, mySpriteRenderer.color.b - tDifference);
                }
            }
        }
    }

    /// <summary>
    /// Update logic for the blocks in the background of the main menu.
    /// The blocks will change the color after the timer runs out.
    /// </summary>
    private void MainMenuBackgroundBlocks()
    {
        if (isMainMenu)
        {
            if (Timer <= 0f)
            {
                ColorIndex = Random.Range(0, ColorPalette.Length);
                Timer = Random.Range(1.00f, 2.50f);
            }
            else
            {
                Timer -= Time.deltaTime;
            }
        }
    }

    /// <summary>
    /// Raise the ColorIndex, so the update logic will change the color of this block to the next in the list of colors.
    /// </summary>
    public void ColorTrigger()
    {
        ColorIndex++;
    }
	#endregion
}
