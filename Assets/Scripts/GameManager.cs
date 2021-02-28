using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    #region public
    [HideInInspector]
    public bool GameIsEnding = false;
    [HideInInspector]
    public Sprite PlayerSprite;
    [Header("Prefabs")]
    public GameObject SimpleEnemyPrefab;
    public GameObject FollowEnemyPrefab;
    public GameObject AdvancedEnemyLeftPrefab;
    public GameObject AdvancedEnemyRightPrefab;
    public GameObject BlockChangerEnemyPrefab;
    public GameObject Coin;

    public static int Level = 1;
    public static bool Pause = false;
    public static int Points = 0;
    public static int Lives = 3;

    [Header("Variables for current level")]
    public float Timer = 0f;
    public float TimeForLevel = 120f;
    public float BlocksToFinish = 1f;
    public float MaxEnemies = 1f;
    public float TimeBetweenSimplesMax = 5f;
    public float TimeBetweenSimplesMin = 2f;
    public bool AdvancedEnemies = false;
    public float TimeBetweenAdvancedMax = 10f;
    public float TimeBetweenAdvancedMin = 4f;
    public bool FollowEnemy = false;
    public float TimeUntilFollowerMax = 9f;
    public float TimeUntilFollowerMin = 7f;
    public bool BlockChangerEnemy = false;
    public float TimeUntilBlockChangerMax = 15f;
    public float TimeUntilBlockChangerMin = 10f;
    public float PointsPerSecond = 1f;
    public float PointsPerEnemy = 10f;
    public float PointsPerSpecial = 50;
    public int Specials = 2;
    public float WaitBetweenPointCalculation = 1f;
    public float WaitBetweenPointToPointCalculation = 0.02f;
    public float WaitBeforeLevelReload = 2f;
    [HideInInspector]
    public Color[] ColorPalette;

    [Header("Modifiers(Addition) for raising level")]
    public float TimeForLevelModifier = 20f;
    public float BlocksToFinishModifier = 0.5f;
    public float MaxEnemiesModifier = 0.25f;
    public float TimeBetweenSimplesMaxModifier = 0.05f;
    public float TimeBetweenSimplesMinModifier = -0.05f;
    public int AdvancedEnemiesAtLevel = 5;
    public float TimeBetweenAdvancedMaxModifier = 0.05f;
    public float TimeBetweenAdvancedMinModifier = -0.05f;
    public int FollowEnemyAtLevel = 3;
    public float TimeUntilFollowerMaxModifier = 0.05f;
    public float TimeUntilFollowerMinModifier = -0.05f;
    public int BlockChangerEnemyAtLevel = 10;
    public float TimeUntilBlockChangerMaxModifier = 0.05f;
    public float TimeUntilBlockChangerMinModifier = -0.05f;
    public float PointsPerSecondModifier = 0.5f;
    public float PointsPerEnemyModifier = 0.5f;
    public float PointsPerSpecialModifier = 0.25f;
    public int BlockLoopTriggerAtLevel = 20;
    #endregion

    #region protected
    #endregion

    #region private
    private List<IsoObject> Characters = new List<IsoObject>();
    private List<CharacterMovement> CharacterMovementScripts = new List<CharacterMovement>();
    private CharacterSpritesHolder CharacterHolder;
    private Button PauseButton, MuteButton;
    private Text MiddleText;
    private Outline MiddleTextOutline;
    private GameObject DarkenerGO;
    private Image Darkener;
    private List<string> Quotes = new List<string>();
    private AudioSource PlayerJump, PlayerDie, EnemyDie, EnemySpawn, Special;
    private float SimpleTimer, AdvancedTimer, FollowTimer;
    private float TimeBetweenSimples, TimeBetweenAdvanced;
    private bool ActiveFollower = false;
    private bool ActiveBlockChanger = false;
    private int FollowerCount = 0;
    private bool BlockChangerIsDying = false;
    private bool SkipPointCalculation = false;
    private int Coins = 0;
    private string TutorialText = "Tutorial\n\nMove your character by tapping into the direction you wish to move.\n\nThe goal is to change all cubes to the desired color by jumping on them. You can see the color in the upper left corner.\n\nTry to avoid all enemies, otherwise you will get killed and lose one life.\n\nIf you jump into a discoball, you will kill all the enemies that are on the dancefloor but be aware, you will get less points!\n\nGood luck and have fun.";
    #endregion

    #region MonoBehaviour
    void Awake()
    {
        //Level = BlockLoopTriggerAtLevel;
        //PlayerPrefs.DeleteAll();
        CalculateVariables();

        CharacterHolder = GameObject.Find("CharacterSpritesHolder").GetComponent<CharacterSpritesHolder>();
        PlayerSprite = CharacterHolder.CharacterSprites[PlayerPrefs.GetInt("CharacterIndex")];

        ColorPalette = new Color[(int)BlocksToFinish + 1];

        for (int i = 0; i < ColorPalette.Length; i++)
        {
            while (ColorPalette[i] == new Color() || ColorPalette[i] == Color.black || (ColorPalette[i].r + ColorPalette[i].g + ColorPalette[i].b) < 1.5f)
            {
                ColorPalette[i] = new Color(Random.Range(0.01f, 1f), Random.Range(0.01f, 1f), Random.Range(0.01f, 1f));
            }

            float tThisColorSum = ColorPalette[i].r + ColorPalette[i].g + ColorPalette[i].b;
            float tOtherColorSum = 0f;
            for (int i2 = 0; i2 < i; i2++)
            {
                tOtherColorSum = ColorPalette[i2].r + ColorPalette[i2].g + ColorPalette[i2].b;

                if (Mathf.Abs(tThisColorSum - tOtherColorSum) < 0.25f)
                {
                    ColorPalette[i] = Color.black;
                    i--;
                    break;
                }
            }
        }

        GameObject tPause = GameObject.Find("Pause");
        GameObject tMute = GameObject.Find("Music");
        PauseButton = tPause.GetComponent<Button>();
        MuteButton = tMute.GetComponent<Button>();

        PlayerJump = transform.Find("PlayerJumpSound").GetComponent<AudioSource>();
        PlayerDie = transform.Find("PlayerDieSound").GetComponent<AudioSource>();
        EnemyDie = transform.Find("EnemyDieSound").GetComponent<AudioSource>();
        EnemySpawn = transform.Find("EnemySpawnSound").GetComponent<AudioSource>();
        Special = transform.Find("SpecialSound").GetComponent<AudioSource>();

        Coins = PlayerPrefs.GetInt("Coins");

        GameObject MiddleTextGO = GameObject.Find("MiddleText");
        MiddleText = MiddleTextGO.GetComponent<Text>();
        MiddleTextOutline = MiddleTextGO.GetComponent<Outline>();
        DarkenerGO = GameObject.Find("Darkener");
        Darkener = DarkenerGO.GetComponent<Image>();
        Darkener.CrossFadeAlpha(0f, 0f, true);
        DarkenerGO.SetActive(false);

        bool tMuteBool = PlayerPrefsX.GetBool("Mute");
        PlayerJump.mute = tMuteBool;
        PlayerDie.mute = tMuteBool;
        EnemyDie.mute = tMuteBool;
        EnemySpawn.mute = tMuteBool;
        Special.mute = tMuteBool;

        Quotes.Add("I think you're shagedelic baby! You're switched on! You're smashing!");
        Quotes.Add("Smashing Baby! When this ship comes a' rockin', don't come a' knockin', baby!");
        Quotes.Add("It's freedom, baby, yeah!");
        Quotes.Add("It seems the tables have turned again, Dr. Evil.");
        Quotes.Add("Its time to swing, baby.");
        Quotes.Add("Ooo, Behave!");
        Quotes.Add("Yeah, baby, yeah!");
        Quotes.Add("What say, you, we go out on the town and swing, baby? Yeah!");
        Quotes.Add("Let's hop on the good foot and do the bad thing.");
        Quotes.Add("Oops. I did it again, baby.");
        Quotes.Add("Yeah Baby.");
        Quotes.Add("Very Shagadelic, baby, yeah.");
    }

    void Start()
    {
        Pause = false;

        TimeBetweenSimples = Random.Range(TimeBetweenSimplesMin, TimeBetweenSimplesMax);
        TimeBetweenAdvanced = Random.Range(TimeBetweenAdvancedMin, TimeBetweenAdvancedMax);
        SimpleTimer = TimeBetweenSimples;
        AdvancedTimer = TimeBetweenAdvanced;
        FollowTimer = Random.Range(TimeUntilFollowerMin, TimeUntilFollowerMax);

        GameObject Player = GameObject.FindWithTag("Player");
        Characters.Add(Player.GetComponent<IsoObject>());
        CharacterMovementScripts.Add(Player.GetComponent<CharacterMovement>());

        StartCoroutine(GameStart());
    }

    void Update()
    {
        if (GameManager.Pause)
        {
            return;
        }

        Timer += Time.deltaTime;
        CharacterHandler();
    }

    /// <summary>
    /// Calls the correct handler of the characters.
    /// </summary>
    private void CharacterHandler()
    {
        if (FollowEnemy)
        {
            if (ActiveFollower && FollowerCount < 1)
            {
                FollowerCount = 1;
            }
            else if (!ActiveFollower && FollowerCount > 0)
            {
                FollowerCount = 0;
            }

            FollowEnemyHandler();
        }

        if (Characters.Count - (1 + FollowerCount) < (int)MaxEnemies)
        {
            if (AdvancedEnemies)
            {
                int tRandom = Random.Range(0, 3);

                if (tRandom == 0)
                {
                    AdvancedEnemyHandler();
                }
                else
                {
                    SimpleEnemyHandler();
                }
            }
            else
            {
                SimpleEnemyHandler();
            }
        }


        for (int i = 1; i < Characters.Count; i++)
        {
            if (CharacterMovementScripts[i].isAdvanced)
            {
                if (CharacterMovementScripts[i].AdvancedLeft)
                {
                    if (Vector3.Distance(Characters[0].Position, Characters[i].Position + new Vector3(-1f, 0f, -0.7f)) <= 0.3f)
                    {
                        StartCoroutine(Lose());
                    }
                }
                else
                {
                    if (Vector3.Distance(Characters[0].Position, Characters[i].Position + new Vector3(0f, -1f, -0.7f)) <= 0.3f)
                    {
                        StartCoroutine(Lose());
                    }
                }
            }
            else if (CharacterMovementScripts[i].isBlockChanger && !BlockChangerIsDying)
            {
                if (Vector3.Distance(Characters[0].Position, Characters[i].Position) <= 0.3f)
                {
                    BlockChangerIsDying = true;
                    //StartCoroutine(BlockChangerDie(CharacterMovementScripts[i]));
                    StartCoroutine(Lose());
                }
            }
            else if (!CharacterMovementScripts[i].isBlockChanger)
            {
                if (Vector3.Distance(Characters[0].Position, Characters[i].Position) <= 0.3f)
                {
                    StartCoroutine(Lose());
                }
            }
        }
    }

    /// <summary>
    /// The update logic for the behaviour of the simple enemies.
    /// </summary>
    void SimpleEnemyHandler()
    {
        if (SimpleTimer <= 0f)
        {
            GameObject tGO;
            if (Random.Range(0, 2) == 1 && !ActiveBlockChanger && BlockChangerEnemy)
            {
                tGO = Instantiate(BlockChangerEnemyPrefab) as GameObject;
                ActiveBlockChanger = true;
            }
            else
            {
                tGO = Instantiate(SimpleEnemyPrefab) as GameObject;
            }
            //GameObject tGO = Instantiate(FollowEnemyPrefab, EnemySpawner.position, Quaternion.identity) as GameObject;
            Characters.Add(tGO.GetComponent<IsoObject>());
            CharacterMovementScripts.Add(tGO.GetComponent<CharacterMovement>());
            EnemySpawn.Play();

            SimpleTimer = Random.Range(TimeBetweenSimplesMin, TimeBetweenSimplesMax);
        }
        else
        {
            SimpleTimer -= Time.deltaTime;
        }
    }

    /// <summary>
    /// The update logic for the behaviour of the following enemies.
    /// </summary>
    void FollowEnemyHandler()
    {
        if (FollowTimer <= 0f && !ActiveFollower && FollowEnemy)
        {
            GameObject tGO = Instantiate(FollowEnemyPrefab) as GameObject;
            //GameObject tGO = Instantiate(FollowEnemyPrefab, EnemySpawner.position, Quaternion.identity) as GameObject;
            Characters.Add(tGO.GetComponent<IsoObject>());
            CharacterMovementScripts.Add(tGO.GetComponent<CharacterMovement>());
            EnemySpawn.Play();

            ActiveFollower = true;
            FollowTimer = Random.Range(TimeUntilFollowerMin, TimeUntilFollowerMax);
        }
        else if (!ActiveFollower && FollowEnemy)
        {
            FollowTimer -= Time.deltaTime;
        }
    }

    /// <summary>
    /// The update logic for the behaviour of the advanced enemies.
    /// </summary>
    void AdvancedEnemyHandler()
    {
        if (AdvancedTimer <= 0f && AdvancedEnemies)
        {
            GameObject tGO;
            IsoObject tIsoObject;
            CharacterMovement tCharacterMovement;
            int tRandom = Random.Range(0, 2);

            if (tRandom == 0)
            {
                tGO = Instantiate(AdvancedEnemyRightPrefab) as GameObject;
                tIsoObject = tGO.GetComponent<IsoObject>();
                tCharacterMovement = tGO.GetComponent<CharacterMovement>();
            }
            else
            {
                tGO = Instantiate(AdvancedEnemyLeftPrefab) as GameObject;
                tIsoObject = tGO.GetComponent<IsoObject>();
                tCharacterMovement = tGO.GetComponent<CharacterMovement>();
            }

            Characters.Add(tIsoObject);
            CharacterMovementScripts.Add(tCharacterMovement);
            EnemySpawn.Play();

            AdvancedTimer = Random.Range(TimeBetweenAdvancedMin, TimeBetweenAdvancedMax);
        }
        else if (AdvancedEnemies)
        {
            AdvancedTimer -= Time.deltaTime;
        }
    }

    /// <summary>
    /// Add an enemy to the list of active enemies.
    /// </summary>
    /// <param name="_Enemy">Instance of the spawned enemy.</param>
    public void AddEnemy(IsoObject _Enemy)
    {
        EnemySpawn.Play();
        Characters.Add(_Enemy);
    }

    /// <summary>
    /// Remove an enemy to the list of active enemies.
    /// </summary>
    /// <param name="_Enemy">Instance of the enemy that will be removed.</param>
    public void RemoveEnemy(IsoObject _Enemy)
    {
        EnemyDie.Play();
        if (_Enemy.transform.tag == "BlockChangerEnemy")
        {
            ActiveBlockChanger = false;
        }

        Characters.Remove(_Enemy);
        Destroy(_Enemy.gameObject);
    }

    /// <summary>
    /// Remove an enemy to the list of active enemies.
    /// </summary>
    /// <param name="_Enemy">Transform of the instance of the enemy that will be removed.</param>
    public void RemoveEnemy(Transform _Enemy)
    {
        EnemyDie.Play();
        if (_Enemy.tag == "BlockChangerEnemy")
        {
            ActiveBlockChanger = false;
        }

        Characters.Remove(_Enemy.GetComponent<IsoObject>());
        Destroy(_Enemy.gameObject);
    }

    /// <summary>
    /// Remove all enemies on the active scenes.
    /// Useful for example to end the level.
    /// </summary>
    public void RemoveAllEnemies()
    {
        Characters = new List<IsoObject>();
        CharacterMovementScripts = new List<CharacterMovement>();

        Characters.Add(GameObject.FindWithTag("Player").GetComponent<IsoObject>());
        CharacterMovementScripts.Add(GameObject.FindWithTag("Player").GetComponent<CharacterMovement>());

        List<GameObject> tAllEnemies = new List<GameObject>();
        tAllEnemies.AddRange(GameObject.FindGameObjectsWithTag("SimpleEnemy"));
        tAllEnemies.AddRange(GameObject.FindGameObjectsWithTag("AdvancedEnemy"));
        tAllEnemies.AddRange(GameObject.FindGameObjectsWithTag("FollowEnemy"));

        for (int i = tAllEnemies.Count - 1; i >= 0; i--)
        {
            EnemyDie.Play();
            Destroy(tAllEnemies[i]);
        }

        SimpleTimer = Random.Range(TimeBetweenSimplesMin, TimeBetweenSimplesMax);
        AdvancedTimer = Random.Range(TimeBetweenAdvancedMin, TimeBetweenAdvancedMax);
        FollowTimer = Random.Range(TimeUntilFollowerMin, TimeUntilFollowerMax);
        ActiveFollower = false;
        ActiveBlockChanger = false;
    }

    /// <summary>
    /// Get a list of all enemies in the active scene.
    /// </summary>
    /// <returns></returns>
    public IsoObject[] GetAllEnemies()
    {
        List<IsoObject> tEnemies = Characters;
        tEnemies.RemoveAt(0);
        return tEnemies.ToArray();
    }

    /// <summary>
    /// Start the lose update logic in a coroutine.
    /// </summary>
    /// <returns>Default coroutine return value.</returns>
    public IEnumerator Lose()
    {
        Pause = true;
        GameIsEnding = true;
        Lives--;

        PlayerDie.Play();

        //if (Lives > 0)
        {
            string tBlame = "You lose";
            MiddleText.text = tBlame;
            MiddleText.CrossFadeAlpha(1f, 0.5f, true);
        }

        while (Characters[0].transform.localScale.x > 0f)
        {
            Characters[0].transform.localScale -= new Vector3(0.05f, 0f, 0f);

            yield return null;
        }
        Characters[0].transform.localScale = Vector3.zero;

        yield return new WaitForSeconds(WaitBeforeLevelReload - 0.5f);
        //if (Lives > 0)
        {
            MiddleText.CrossFadeAlpha(0f, 0.5f, true);
        }
        yield return new WaitForSeconds(0.5f);

        if (Lives > 0)
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
        else
        {
            StartCoroutine(GameOver(CharacterMovementScripts[0].GetBlocks()));
        }
    }

    /// <summary>
    /// Skip the point calculation at the end of the levels.
    /// </summary>
    /// <returns>Default coroutine return value.</returns>
    IEnumerator CheckForSkipInput()
    {
        while (Pause)
        {
            if (Input.GetMouseButtonDown(0))
            {
                SkipPointCalculation = true;
                break;
            }

            yield return null;
        }
    }

    /// <summary>
    /// Start the game over update logic in a coroutine.
    /// This method includes the point calculation in the end of levels.
    /// </summary>
    /// <param name="Blocks">A list of all enemies and powerups.</param>
    /// <returns>Default coroutine return value.</returns>
    public IEnumerator GameOver(IsoObject[] Blocks)
    {
        //Win Situation
        Pause = true;
        GameIsEnding = true;

        MuteButton.interactable = false;
        PauseButton.interactable = false;

        StartCoroutine(CheckForSkipInput());

        List<IsoObject> tSpecials = new List<IsoObject>();
        List<IsoObject> tNoSpecials = new List<IsoObject>();
        List<Transform> tSpecialTransforms = new List<Transform>();
        tSpecials.AddRange(Blocks);

        int tRandomNumber = Random.Range(0, Quotes.Count);

        while (tRandomNumber >= Quotes.Count)
        {
            tRandomNumber = Random.Range(0, Quotes.Count);
        }

        string tCongratulations;

        if (Lives > 0)
        {
            tCongratulations = Quotes[tRandomNumber] + "\nYou have completed level " + Level + " with " + Points + " points.";
        }
        else
        {
            tCongratulations = Quotes[tRandomNumber] + "\nYou have finished the game with " + Points + " points.";
        }

        MiddleText.text = tCongratulations;
        MiddleText.CrossFadeAlpha(1f, 0.5f, true);

        if (Lives > 0)
        {
            for (int i = 0; i < tSpecials.Count; i++)
            {
                if (i < 0 || i >= tSpecials.Count)
                {
                    break;
                }

                if (tSpecials[i] != null)
                {
                    if (tSpecials[i].gameObject.GetComponent<Block>() != null)
                    {
                        tNoSpecials.Add(tSpecials[i]);
                    }
                }
            }

            for (int i = 0; i < tNoSpecials.Count; i++)
            {
                if (i < 0 || i >= tNoSpecials.Count)
                {
                    break;
                }

                tSpecials.Remove(tNoSpecials[i]);
            }

            for (int i = 0; i < tSpecials.Count; i++)
            {
                if (i < 0 || i >= tSpecials.Count)
                {
                    break;
                }

                if (tSpecials[i] != null)
                {
                    tSpecialTransforms.Add(tSpecials[i].GetComponent<Transform>());
                }
            }

            for (int i = (int)Timer; i < (int)TimeForLevel; i++)
            {
                Points += (int)PointsPerSecond;
                Timer++;
                if (Lives > 0)
                {
                    tCongratulations = Quotes[tRandomNumber] + "\nYou have completed level " + Level + " with " + Points + " points.";
                }
                else
                {
                    tCongratulations = Quotes[tRandomNumber] + "\nYou have finished the game with " + Points + " points.";
                }

                MiddleText.text = tCongratulations;

                if (!SkipPointCalculation)
                {
                    yield return new WaitForSeconds(WaitBetweenPointToPointCalculation);
                }
            }
            Timer = TimeForLevel;

            yield return new WaitForSeconds(WaitBetweenPointCalculation);

            while (tSpecialTransforms.Count > 0)
            {
                while (tSpecialTransforms[tSpecialTransforms.Count - 1].localScale.x > 0f)
                {
                    tSpecialTransforms[tSpecialTransforms.Count - 1].localScale -= new Vector3(0.05f, 0f, 0f);

                    if (!SkipPointCalculation)
                    {
                        yield return null;
                    }
                }
                tSpecialTransforms[tSpecialTransforms.Count - 1].localScale = Vector3.zero;

                for (int i = 0; i < (int)PointsPerSpecial; i++)
                {
                    Points++;
                    if (Lives > 0)
                    {
                        tCongratulations = Quotes[tRandomNumber] + "\nYou have completed level " + Level + " with " + Points + " points.";
                    }
                    else
                    {
                        tCongratulations = Quotes[tRandomNumber] + "\nYou have finished the game with " + Points + " points.";
                    }

                    MiddleText.text = tCongratulations;

                    if (!SkipPointCalculation)
                    {
                        yield return new WaitForSeconds(WaitBetweenPointToPointCalculation);
                    }
                }

                tSpecialTransforms.RemoveAt(tSpecialTransforms.Count - 1);
                if (!SkipPointCalculation)
                {
                    yield return null;
                }
            }
        }
        else
        {
            yield return new WaitForSeconds(1f);
        }

        CalculateHighscores();
        
        yield return new WaitForSeconds(WaitBeforeLevelReload - 0.5f);

        MiddleText.CrossFadeAlpha(0f, 0.5f, true);

        if (Lives > 0)
        {
            yield return new WaitForSeconds(0.25f);

            Vector3 tSpawnPos = Characters[0].transform.position + Vector3.up;
            for (int i = 0; i < Level; i++)
            {
                Instantiate(Coin, tSpawnPos, Quaternion.identity);

                Coins++;

                yield return new WaitForSeconds(0.2f);
            }

            PlayerPrefs.SetInt("Coins", Coins);
            PlayerPrefs.Save();

            yield return new WaitForSeconds(1f);
        }
        else
        {
            yield return new WaitForSeconds(0.5f);
        }

        if (Lives > 0)
        {
            NextLevel();
        }
        else
        {
            SceneManager.LoadScene(0);
            SetToDefault();
        }
    }

    /// <summary>
    /// Reset all values of this class to default values.
    /// </summary>
    public void SetToDefault()
    {
        Level = 1;
        Points = 0;
        Lives = 3;
        Pause = false;
    }

    /// <summary>
    /// Play jump sound of the player.
    /// </summary>
    public void PlayerJumpPlay()
    {
        PlayerJump.Play();
    }

    /// <summary>
    /// Play jump sound of an enemy.
    /// </summary>
    public void SpecialPlay()
    {
        Special.Play();
    }

    /// <summary>
    /// Set the mute state of the soundeffects.
    /// </summary>
    /// <param name="_MuteBool">true = mute all soundeffects; false = unmute all soundeffects.</param>
    public void UpdateMute(bool _MuteBool)
    {
        PlayerJump.mute = _MuteBool;
        PlayerDie.mute = _MuteBool;
        EnemyDie.mute = _MuteBool;
        EnemySpawn.mute = _MuteBool;
        Special.mute = _MuteBool;
    }

    /// <summary>
    /// Calculate all variables such as time, max enemies, time between enemy spawns and such things, depending on the level count.
    /// </summary>
    protected void CalculateVariables()
    {
        int tLevel = Level;

        if (Level >= BlockChangerEnemyAtLevel)
        {
            tLevel -= (int)((float)BlockChangerEnemyAtLevel * 0.75f);
        }

        if (Level >= BlockLoopTriggerAtLevel)
        {
            tLevel -= (int)((float)BlockChangerEnemyAtLevel * 0.5f);
        }

        TimeForLevel += TimeForLevelModifier * (float)(tLevel - 1f);
        BlocksToFinish += BlocksToFinishModifier * (float)(tLevel - 1f);
        MaxEnemies += MaxEnemiesModifier * (float)(tLevel - 1f);
        TimeBetweenSimplesMax += TimeBetweenSimplesMaxModifier * (float)(tLevel - 1f);
        TimeBetweenSimplesMin += TimeBetweenSimplesMinModifier * (float)(tLevel - 1f);
        TimeBetweenAdvancedMax += TimeBetweenAdvancedMaxModifier * (float)(tLevel - 1f);
        TimeBetweenAdvancedMin += TimeBetweenAdvancedMinModifier * (float)(tLevel - 1f);
        TimeUntilFollowerMax += TimeUntilFollowerMaxModifier * (float)(tLevel - 1f);
        TimeUntilFollowerMin += TimeUntilFollowerMinModifier * (float)(tLevel - 1f);
        TimeUntilBlockChangerMax += TimeUntilBlockChangerMaxModifier * (float)(tLevel - 1f);
        TimeUntilBlockChangerMin += TimeUntilBlockChangerMinModifier * (float)(tLevel - 1f);

        if (tLevel >= AdvancedEnemiesAtLevel)
        {
            AdvancedEnemies = true;
        }

        if (tLevel >= FollowEnemyAtLevel)
        {
            FollowEnemy = true;
        }

        if (tLevel >= BlockChangerEnemyAtLevel)
        {
            BlockChangerEnemy = true;
        }

        PointsPerSecond += PointsPerSecondModifier * (float)(tLevel - 1f);
        PointsPerEnemy += PointsPerEnemyModifier * (float)(tLevel - 1f);
        PointsPerSpecial += PointsPerSpecialModifier * (float)(tLevel - 1f);
    }

    /// <summary>
    /// Load the next level.
    /// </summary>
    protected void NextLevel()
    {
        Level++;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    /// <summary>
    /// Coroutine to let the character die, if he stands on the same field as an enemy.
    /// </summary>
    /// <param name="_MovementScript"></param>
    /// <returns>Default coroutine return value.</returns>
    IEnumerator BlockChangerDie(CharacterMovement _MovementScript)
    {
        while (_MovementScript.transform.localScale.x > 0f)
        {
            _MovementScript.transform.localScale -= new Vector3(0.05f, 0f, 0f);

            yield return null;
        }
        _MovementScript.transform.localScale = Vector3.zero;

        RemoveEnemy(_MovementScript.transform);
        BlockChangerIsDying = false;
    }

    /// <summary>
    /// Calculate and rearrange the highscore table.
    /// </summary>
    void CalculateHighscores()
    {
        int[] tHighscores = new int[6];

        for (int i = 0; i < tHighscores.Length - 1; i++)
        {
            tHighscores[i] = PlayerPrefs.GetInt(((i + 1) + "."));
            //Debug.Log("Get " + (i + 1) + ": " + tHighscores[i]);
        }

        tHighscores[5] = Points;

        System.Array.Sort<int>(tHighscores);
        System.Array.Reverse(tHighscores);

        for (int i = 0; i < tHighscores.Length - 1; i++)
        {
            PlayerPrefs.SetInt(((i + 1) + "."), tHighscores[i]);
            //Debug.Log("Set " + (i + 1) + ": " + tHighscores[i]);
        }

        PlayerPrefs.Save();
    }

    /// <summary>
    /// Start the gameplay logic.
    /// </summary>
    /// <returns></returns>
    IEnumerator GameStart()
    {
        Pause = true;

        if (Level == 1 && Lives == 3)
        {
            DarkenerGO.SetActive(true);
            Darkener.CrossFadeAlpha(0.5f, 0f, true);
            MiddleText.fontSize = 40;
            MiddleTextOutline.effectDistance = new Vector2(2f, -2f);
            MiddleText.text = TutorialText;
            MiddleText.CrossFadeAlpha(1f, 0.5f, true);

            while (!Input.GetMouseButtonDown(0))
            {
                yield return null;
            }

            MiddleText.CrossFadeAlpha(0f, 0.5f, true);
            Darkener.CrossFadeAlpha(0f, 0.4f, true);
            yield return new WaitForSeconds(0.55f);
            MiddleText.fontSize = 60;
            MiddleTextOutline.effectDistance = new Vector2(1f, -1f);
            DarkenerGO.SetActive(false);
        }

        string tTap = "Level " + Level + "\nTap to start";

        MiddleText.text = tTap;
        MiddleText.CrossFadeAlpha(1f, 0.5f, true);

        while (!Input.GetMouseButtonDown(0))
        {
            yield return null;
        }

        MiddleText.CrossFadeAlpha(0f, 0.5f, true);

        yield return new WaitForSeconds(0.55f);

        Pause = false;
    }
    #endregion
}
