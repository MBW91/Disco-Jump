using UnityEngine;
using System.Collections;

public class CharacterMovement : MonoBehaviour 
{
	#region public
    public float JumpTime = 0.5f;
    public float JumpHeight = 1.25f;
    public float DieHeight = -10f;
    public float JumpDelay = 0f;
    public bool isAdvanced = false;
    public bool AdvancedLeft = false;
    public bool isBlockChanger = false;
	#endregion

	#region protected
	#endregion
	
	#region private
    private GameManager GameManagerScript;
    private IsoObject myTransform;
    private Transform mySpriteTransform;
    private Transform PlayerTransform;
    private IsoObject[] Blocks;
    private IsoObject NearestBlock;
    private Block NearestBlockScript;
    private Block[] BlockScripts;
    private RectTransform PauseButtonRectTransform, MuteButtonRectTransform;
    private bool isJumping = false;
    private bool isFalling = false;
    private int FinishedBlockCounter = 0;
    private bool isPlayer = false;
    private bool isSimpleEnemy = true;
    private bool isFollowing = false;
    private int EnemyCounter = 0;
	#endregion
	
	#region MonoBehaviour
	void Start() 
	{
        if(this.tag == "Player")
        {
            isPlayer = true;
        }
        else if(this.tag == "FollowEnemy")
        {
            isSimpleEnemy = false;
        }

        GameManagerScript = GameObject.Find("GameManager").GetComponent<GameManager>();
        myTransform = GetComponent<IsoObject>();
        mySpriteTransform = this.transform.Find("Sprite").GetComponent<Transform>();
        GameObject[] tBlocks = GameObject.FindGameObjectsWithTag("Environment");
        Blocks = new IsoObject[tBlocks.Length];
        BlockScripts = new Block[tBlocks.Length];

        if(isPlayer)
        {
            SpriteRenderer tPlayerSpriteRenderer = this.transform.Find("Sprite").GetComponent<SpriteRenderer>();
            tPlayerSpriteRenderer.sprite = GameManagerScript.PlayerSprite;
        }

        for(int i = 0; i < tBlocks.Length; i++)
        {
            Blocks[i] = tBlocks[i].GetComponent<IsoObject>();
            BlockScripts[i] = tBlocks[i].GetComponent<Block>();
        }

        NearestBlock = GetNearestBlock();
        if(!isPlayer)
        {
            isFalling = true;
            PlayerTransform = GameObject.FindWithTag("Player").GetComponent<Transform>();
        }

        GameObject tPause = GameObject.Find("Pause");
        GameObject tMute = GameObject.Find("Music");
        PauseButtonRectTransform = tPause.GetComponent<RectTransform>();
        MuteButtonRectTransform = tMute.GetComponent<RectTransform>();

        //if (isAdvanced)
        //{
        //    this.tag = "AdvancedEnemy";
        //}

        //if(this.tag == "AdvancedEnemy")
        //{
        //    isAdvanced = true;
        //}
    }

    void Update()
    {
        if (GameManager.Pause)
        {
            return;
        }

        RequestHandling();

        if (isPlayer)
        {
            PlayerUpdate();
        }
        else
        {
            EnemyUpdate();
        }

        FallingUpdate();
    }

    /// <summary>
    /// Update the falling logic.
    /// </summary>
    private void FallingUpdate()
    {
        if (isFalling)
        {
            if (Vector3.Distance(NearestBlock.Position, myTransform.Position) <= 0.5f)
            {
                isFalling = false;
                NearestBlock = GetNearestBlock();
            }
            else
            {
                if (!isAdvanced)
                {
                    myTransform.Position -= Vector3.forward * 0.4f;
                }
                else
                {
                    if (AdvancedLeft)
                    {
                        myTransform.Position += Vector3.right * 0.4f;
                    }
                    else
                    {
                        myTransform.Position += Vector3.up * 0.4f;
                    }
                }
            }
        }
    }

    /// <summary>
    /// Update the player logic, like jumping to blocks.
    /// </summary>
    void PlayerUpdate()
    {
        if (Input.GetMouseButtonDown(0) && !isJumping && !isFalling)
        {
            Vector3 tPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            tPos.z = 0f;

            Vector2 tUIPos = Input.mousePosition;
            if (!RectTransformUtility.RectangleContainsScreenPoint(PauseButtonRectTransform, tUIPos, null) && !RectTransformUtility.RectangleContainsScreenPoint(MuteButtonRectTransform, tUIPos, null))
            {
                if (tPos.x >= this.transform.position.x)
                {
                    mySpriteTransform.localScale = new Vector3(1, 1f, 1f);

                    if (tPos.y >= transform.position.y)
                    {
                        tPos = new Vector3(myTransform.Position.x + 1f, myTransform.Position.y, myTransform.Position.z + 0.7f);
                        StartCoroutine(JumpTo(tPos));
                    }
                    else
                    {
                        tPos = new Vector3(myTransform.Position.x, myTransform.Position.y - 1f, myTransform.Position.z - 0.7f);
                        StartCoroutine(JumpTo(tPos));
                    }
                }
                else
                {
                    mySpriteTransform.localScale = new Vector3(-1f, 1f, 1f);

                    if (tPos.y >= transform.position.y)
                    {
                        tPos = new Vector3(myTransform.Position.x, myTransform.Position.y + 1f, myTransform.Position.z + 0.7f);
                        StartCoroutine(JumpTo(tPos));
                    }
                    else
                    {
                        tPos = new Vector3(myTransform.Position.x - 1f, myTransform.Position.y, myTransform.Position.z - 0.7f);
                        StartCoroutine(JumpTo(tPos));
                    }
                }
            }
        }
    }

    /// <summary>
    /// Update the enemy logic, like jumping to blocks or follow the player.
    /// </summary>
    void EnemyUpdate()
    {
        Vector3 tPos = new Vector3();

        if(!isFalling && !isJumping && isSimpleEnemy)
        {
            if (isAdvanced)
            {
                if (AdvancedLeft)
                {
                    if (Random.Range(0, 2) == 1)
                    {
                        tPos = new Vector3(myTransform.Position.x + 1f, myTransform.Position.y, myTransform.Position.z + 0.7f);
                        StartCoroutine(JumpTo(tPos));
                    }
                    else
                    {
                        tPos = new Vector3(myTransform.Position.x + 1f, myTransform.Position.y - 1f, myTransform.Position.z);
                        StartCoroutine(JumpTo(tPos));
                    }
                }
                else
                {
                    if (Random.Range(0, 2) == 1)
                    {
                        tPos = new Vector3(myTransform.Position.x, myTransform.Position.y + 1f, myTransform.Position.z + 0.7f);
                        StartCoroutine(JumpTo(tPos));
                    }
                    else
                    {
                        tPos = new Vector3(myTransform.Position.x - 1f, myTransform.Position.y + 1f, myTransform.Position.z);
                        StartCoroutine(JumpTo(tPos));
                    }
                }
            }
            else
            {
                if (Random.Range(0, 2) == 1)
                {
                    tPos = new Vector3(myTransform.Position.x, myTransform.Position.y - 1f, myTransform.Position.z - 0.7f);
                    StartCoroutine(JumpTo(tPos));
                }
                else
                {
                    tPos = new Vector3(myTransform.Position.x - 1f, myTransform.Position.y, myTransform.Position.z - 0.7f);
                    StartCoroutine(JumpTo(tPos));
                }
            }
        }
        else if (!isFalling && !isJumping && !isSimpleEnemy)
        {
            if (isFollowing)
            {
                tPos = PlayerTransform.position;
                tPos.z = 0f;

                if (tPos.x >= this.transform.position.x)
                {
                    mySpriteTransform.localScale = new Vector3(1, 1f, 1f);

                    if (tPos.y >= transform.position.y)
                    {
                        tPos = new Vector3(myTransform.Position.x + 1f, myTransform.Position.y, myTransform.Position.z + 0.7f);
                        StartCoroutine(JumpTo(tPos));
                    }
                    else
                    {
                        tPos = new Vector3(myTransform.Position.x, myTransform.Position.y - 1f, myTransform.Position.z - 0.7f);
                        StartCoroutine(JumpTo(tPos));
                    }
                }
                else
                {
                    mySpriteTransform.localScale = new Vector3(-1f, 1f, 1f);

                    if (tPos.y >= transform.position.y)
                    {
                        tPos = new Vector3(myTransform.Position.x, myTransform.Position.y + 1f, myTransform.Position.z + 0.7f);
                        StartCoroutine(JumpTo(tPos));
                    }
                    else
                    {
                        tPos = new Vector3(myTransform.Position.x - 1f, myTransform.Position.y, myTransform.Position.z - 0.7f);
                        StartCoroutine(JumpTo(tPos));
                    }
                }
            }
            else
            {
                if (Random.Range(0, 2) == 1)
                {
                    mySpriteTransform.localScale = new Vector3(1, 1f, 1f);
                    tPos = new Vector3(myTransform.Position.x, myTransform.Position.y - 1f, myTransform.Position.z - 0.7f);
                    StartCoroutine(JumpTo(tPos));
                }
                else
                {
                    mySpriteTransform.localScale = new Vector3(-1f, 1f, 1f);
                    tPos = new Vector3(myTransform.Position.x - 1f, myTransform.Position.y, myTransform.Position.z - 0.7f);
                    StartCoroutine(JumpTo(tPos));
                }
            }
        }
    }

    /// <summary>
    /// Check if the block where the player tapped on is reachable or not and react to it.
    /// </summary>
    void RequestHandling()
    {
        if (myTransform.Position.x <= DieHeight || myTransform.Position.y <= DieHeight || myTransform.Position.z <= DieHeight)
        {
            if (isPlayer)
            {
                //Debug.Log("Falled to death.");
                StartCoroutine(GameManagerScript.Lose());
            }
            else
            {
                GameManagerScript.RemoveEnemy(myTransform);
            }
        }

        if (!isJumping && !isFalling && isPlayer)
        {
            if (Vector3.Distance(NearestBlock.Position, myTransform.Position) >= 0.5f)
            {
                isFalling = true;
            }
        }

        //Debug.Log(FinishedBlockCounter);
        if(FinishedBlockCounter >= Blocks.Length - 2 && isPlayer)
        {
            StartCoroutine(GameManagerScript.GameOver(Blocks));
        }
    }

    /// <summary>
    /// Return the nearest block to this character.
    /// </summary>
    /// <returns>The nearest block to this character.</returns>
    IsoObject GetNearestBlock()
    {
        IsoObject tNearestBlock = null;
        float tNearestDistance = Mathf.Infinity;

        for(int i = 0; i < Blocks.Length; i++)
        {
            float tDistance = Vector3.Distance(Blocks[i].Position, myTransform.Position);

            if(tDistance < tNearestDistance)
            {
                tNearestBlock = Blocks[i];
                tNearestDistance = tDistance;
                NearestBlockScript = BlockScripts[i];
            }
        }

        return tNearestBlock;
    }

    /// <summary>
    /// Let the character jump to the specified position.
    /// </summary>
    /// <param name="NewPos"> Position where to jump.</param>
    /// <returns>Default coroutine return value.</returns>
    IEnumerator JumpTo(Vector3 NewPos)
    {
        if (GameManager.Pause)
        {
            yield return null;
        }

        isJumping = true;
        Vector3 tStart = myTransform.Position;
        float tTimer = 0f;
        //Debug.Log("Form: " + myTransform.Position + "   To: " + NewPos);

        if(isPlayer)
        {
            GameManagerScript.PlayerJumpPlay();
        }

        yield return new WaitForSeconds(JumpDelay);

        while (tTimer <= 1f)
        {
            while (GameManager.Pause)
            {
                yield return null;
            }

            float tHeight = Mathf.Sin(Mathf.PI * tTimer);

            if (!isAdvanced)
            {
                myTransform.Position = Vector3.Lerp(tStart, NewPos, tTimer) + new Vector3(0f, 0f, tHeight);
            }
            else
            {
                if (AdvancedLeft)
                {
                    myTransform.Position = Vector3.Lerp(tStart, NewPos, tTimer) + new Vector3(-tHeight, 0f, 0f);
                }
                else
                {
                    myTransform.Position = Vector3.Lerp(tStart, NewPos, tTimer) + new Vector3(0f, -tHeight, 0f);
                }
            }

            if (tTimer < 0.5f)
            {
                mySpriteTransform.localScale = mySpriteTransform.localScale + new Vector3(0f, 0.01f, 0f);
            }
            else
            {
                mySpriteTransform.localScale = mySpriteTransform.localScale - new Vector3(0f, 0.01f, 0f);
            }

            tTimer += Time.deltaTime / JumpTime;
            yield return null;
        }

        mySpriteTransform.localScale = new Vector3(mySpriteTransform.localScale.x, 1f, mySpriteTransform.localScale.z);
        //myTransform.Position = new Vector3(Mathf.Round(myTransform.Position.x * 10f) / 10f, Mathf.Round(myTransform.Position.y * 10f) / 10f, Mathf.Round(myTransform.Position.z * 10f) / 10f);
        //myTransform.Position += new Vector3(0f, 0f, 0.001f);

        //Debug.Log("Landed at: " + myTransform.Position);

        NearestBlock = GetNearestBlock();

        if (isPlayer || isBlockChanger)
        {
            if (NearestBlockScript == null)
            {
                StartCoroutine(SpecialTrigger());
            }
            else
            {
                if (Vector3.Distance(NearestBlock.Position, myTransform.Position) <= 0.5f)
                {
                    if (!isBlockChanger)
                    {
                        if (GameManager.Level < GameManagerScript.BlockLoopTriggerAtLevel)
                        {
                            if (NearestBlockScript.ColorIndex < (int)GameManagerScript.BlocksToFinish)
                            {
                                NearestBlockScript.ColorIndex++;

                                if (NearestBlockScript.ColorIndex == (int)GameManagerScript.BlocksToFinish)
                                {
                                    FinishedBlockCounter++;
                                }
                            }
                        }
                        else
                        {
                            NearestBlockScript.ColorIndex++;

                            if (NearestBlockScript.ColorIndex == GameManagerScript.BlocksToFinish)
                            {
                                FinishedBlockCounter++;
                            }
                            else if (NearestBlockScript.ColorIndex > GameManagerScript.BlocksToFinish)
                            {
                                FinishedBlockCounter--;
                                NearestBlockScript.ColorIndex = 0;
                            }
                        }
                    }
                    else
                    {
                        int tRandomNumber = Random.Range(0, (int)GameManagerScript.BlocksToFinish + 1);

                        if(tRandomNumber > (int)GameManagerScript.BlocksToFinish)
                        {
                            tRandomNumber = (int)GameManagerScript.BlocksToFinish;
                        }
                        else if(tRandomNumber < 0)
                        {
                            tRandomNumber = 0;
                        }

                        NearestBlockScript.ColorIndex = tRandomNumber;
                    }
                }
            }
        }
        
        if(!isFollowing)
        {
            EnemyCounter++;

            if (EnemyCounter >= 6 && !isAdvanced)
            {
                if (isSimpleEnemy)
                {
                    isFalling = true;
                }
                else
                {
                    isFollowing = true;
                    JumpDelay = 0.75f;
                    JumpTime -= 0.25f;
                }
            }
            else if(EnemyCounter >= 5 && isAdvanced)
            {
                isFalling = true;
            }
        }

        if(Vector3.Distance(NearestBlock.Position, myTransform.Position) <= 0.5f)
        {
            myTransform.Position = NearestBlock.Position + new Vector3(0f, 0f, 0.01f);
        }

        isJumping = false;
    }

    /// <summary>
    /// Trigger the special fields (powerups), to destroy all enemies at once.
    /// </summary>
    /// <returns>Default coroutine return value.</returns>
    IEnumerator SpecialTrigger()
    {
        if (NearestBlock != null)
        {
            GameManager.Pause = true;
            Transform tBlockSpriteTransform = NearestBlock.gameObject.GetComponent<Transform>();
            IsoObject[] tEnemies = GameManagerScript.GetAllEnemies();
            Transform[] tEnemyTransforms = new Transform[tEnemies.Length];

            GameManagerScript.SpecialPlay();

            for (int i = 0; i < tEnemyTransforms.Length; i++)
            {
                tEnemyTransforms[i] = tEnemies[i].gameObject.GetComponent<Transform>();
            }

            mySpriteTransform.localScale = new Vector3(1f, 1f, 1f);
            while (mySpriteTransform.localScale.x > 0f)
            {
                mySpriteTransform.localScale -= new Vector3(0.05f, 0f, 0f);

                yield return null;
            }
            mySpriteTransform.localScale = new Vector3(0f, 1f, 1f);
            myTransform.Position = new Vector3(3f, 3f, 4.201f);

            if (tEnemyTransforms.Length > 0)
            {
                while (tEnemyTransforms[0].localScale.x > 0f)
                {
                    for (int i = 0; i < tEnemyTransforms.Length; i++)
                    {
                        tEnemyTransforms[i].localScale -= new Vector3(0.05f, 0f, 0f);
                    }

                    yield return null;
                }

                for (int i = 0; i < tEnemyTransforms.Length; i++)
                {
                    tEnemyTransforms[i].localScale = Vector3.zero;
                }
            }

            GameManagerScript.RemoveAllEnemies();

            while (tBlockSpriteTransform.localScale.x > 0f)
            {
                tBlockSpriteTransform.localScale -= new Vector3(0.05f, 0f, 0f);

                yield return null;
            }
            tBlockSpriteTransform.localScale = new Vector3(0f, 1f, 1f);
            NearestBlock.Position = new Vector3(4f, 4f, 5f);

            while (tBlockSpriteTransform.localScale.x < 1f)
            {
                tBlockSpriteTransform.localScale += new Vector3(0.05f, 0f, 0f);

                yield return null;
            }
            tBlockSpriteTransform.localScale = new Vector3(1f, 1f, 1f);

            while (mySpriteTransform.localScale.x < 1f)
            {
                mySpriteTransform.localScale += new Vector3(0.05f, 0f, 0f);

                yield return null;
            }
            mySpriteTransform.localScale = new Vector3(1f, 1f, 1f);

            while (tBlockSpriteTransform.localScale.x > 0f)
            {
                tBlockSpriteTransform.localScale -= new Vector3(0.05f, 0f, 0f);

                yield return null;
            }
            tBlockSpriteTransform.localScale = new Vector3(0f, 1f, 1f);
            NearestBlock.Position = new Vector3(3f, 3f, 5.5f);

            Destroy(NearestBlock.gameObject);
            GameManagerScript.Specials--;
            NearestBlock = GetNearestBlock();

            GameManager.Pause = false;
        }
    }

    /// <summary>
    /// Get a list of all blocks in the active scene.
    /// </summary>
    /// <returns></returns>
    public IsoObject[] GetBlocks()
    {
        return Blocks;
    }
	#endregion
}
