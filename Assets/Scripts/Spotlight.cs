using UnityEngine;

public class Spotlight : MonoBehaviour 
{
	#region public
	#endregion

	#region protected
	#endregion
	
	#region private
    private SpriteRenderer mySpriteRenderer;
    private float Timer = 0f;
	#endregion
	
	#region MonoBehaviour
	void Start() 
	{
        mySpriteRenderer = GetComponent<SpriteRenderer>();
	}

	void Update()
    {
        SpotlightUpdate();
    }

    /// <summary>
    /// Update logic for the spotlights in the background of the gameplay.
    /// They rotate and change their light color.
    /// </summary>
    private void SpotlightUpdate()
    {
        if (Timer <= 0f)
        {
            if (Random.Range(0, 2) == 1)
            {
                mySpriteRenderer.enabled = false;
            }
            else
            {
                mySpriteRenderer.enabled = true;
                //mySpriteRenderer.color = new Color(Random.Range(0.01f, 1f), Random.Range(0.01f, 1f), Random.Range(0.01f, 1f), 0.5f);

                int tRandomNumber = Random.Range(0, 6);

                switch (tRandomNumber)
                {
                    case 0:
                        mySpriteRenderer.color = new Color(Random.Range(0.5f, 1f), 0f, 0f, 0.5f);
                        break;

                    case 1:
                        mySpriteRenderer.color = new Color(0f, Random.Range(0.5f, 1f), 0f, 0.5f);
                        break;

                    case 2:
                        mySpriteRenderer.color = new Color(0f, 0f, Random.Range(0.5f, 1f), 0.5f);
                        break;

                    case 3:
                        mySpriteRenderer.color = new Color(Random.Range(0.5f, 1f), Random.Range(0.5f, 1f), 0f, 0.5f);
                        break;

                    case 4:
                        mySpriteRenderer.color = new Color(0f, Random.Range(0.5f, 1f), Random.Range(0.5f, 1f), 0.5f);
                        break;

                    case 5:
                        mySpriteRenderer.color = new Color(Random.Range(0.5f, 1f), 0f, Random.Range(0.5f, 1f), 0.5f);
                        break;

                    default:
                        mySpriteRenderer.color = new Color(Random.Range(0.5f, 1f), 0f, 0f, 0.5f);
                        break;
                }
            }

            Timer = Random.Range(4f, 8f);
        }
        else
        {
            Timer -= Time.deltaTime;
        }
    }
    #endregion
}
