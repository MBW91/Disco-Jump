using UnityEngine;
using UnityEngine.SceneManagement;

public class Music : MonoBehaviour
{
    #region public
    [HideInInspector]
    public AudioSource myMusic;
    public AudioClip MainMenuMusic;
    public AudioClip[] IngameMusic;
    #endregion

    #region protected
    #endregion

    #region private
    private static Music instance = null;
    private int IngameMusicIndex = 0;
    #endregion

    #region MonoBehaviour
    void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(this.gameObject);
            return;
        }
        else
        {
            instance = this;
        }

        myMusic = GetComponent<AudioSource>();
        myMusic.mute = PlayerPrefsX.GetBool("Mute");

        DontDestroyOnLoad(this.gameObject);

        int tOldIndex = IngameMusicIndex;
        IngameMusicIndex = Random.Range(0, IngameMusic.Length);

        if(IngameMusic.Length > 1)
        {
            while(IngameMusicIndex == tOldIndex)
            {
                IngameMusicIndex = Random.Range(0, IngameMusic.Length);
            }
        }
        else
        {
            myMusic.loop = true;
        }
    }

    void Update()
    {
        MusicChanger();
    }

    /// <summary>
    /// Changes the music depending on the active scene.
    /// </summary>
    private void MusicChanger()
    {
        if (SceneManager.GetActiveScene().buildIndex == 0)
        {
            if (myMusic.clip != MainMenuMusic)
            {
                myMusic.Stop();
                myMusic.clip = MainMenuMusic;
                myMusic.Play();
            }

            if (!myMusic.isPlaying)
            {
                myMusic.Play();
            }
        }
        else
        {
            if (!myMusic.isPlaying)
            {
                int tOldIndex = IngameMusicIndex;
                IngameMusicIndex = Random.Range(0, IngameMusic.Length);

                if (IngameMusic.Length > 1)
                {
                    while (IngameMusicIndex == tOldIndex)
                    {
                        IngameMusicIndex = Random.Range(0, IngameMusic.Length);
                    }
                }
            }

            if (myMusic.clip != IngameMusic[IngameMusicIndex])
            {
                myMusic.Stop();
                myMusic.clip = IngameMusic[IngameMusicIndex];
                myMusic.Play();
            }
        }
    }

    /// <summary>
    /// Get the the active music instance.
    /// </summary>
    public static Music Instance
    {
        get { return instance; }
    }
    #endregion
}
