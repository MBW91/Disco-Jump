using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MuteMusicButton : MonoBehaviour
{
    #region public
    public Sprite MusicOn, MusicOff;
    #endregion

    #region protected
    #endregion

    #region private
    private Image myButtonImage;
    private Music MusicManager;
    private GameManager Manager;
    #endregion

    #region MonoBehaviour
	void Awake()
    {
        myButtonImage = GetComponent<Image>();
        MusicManager = GameObject.Find("MusicManager").GetComponent<Music>();

        if(SceneManager.GetActiveScene().buildIndex == 1)
        {
            Manager = GameObject.Find("GameManager").GetComponent<GameManager>();
        }
	}

    void Start()
    {
        if (MusicManager.myMusic.mute)
        {
            myButtonImage.sprite = MusicOff;
        }
        else
        {
            myButtonImage.sprite = MusicOn;
        }
    }

    /// <summary>
    /// Toggles between muted and unmuted state of the music.
    /// </summary>
	public void ToggleMuteMusic()
    {
        MusicManager.myMusic.mute = !MusicManager.myMusic.mute;
        PlayerPrefsX.SetBool("Mute", MusicManager.myMusic.mute);
        PlayerPrefs.Save();

        if (MusicManager.myMusic.mute)
        {
            myButtonImage.sprite = MusicOff;
        }
        else
        {
            myButtonImage.sprite = MusicOn;
        }

        if(Manager != null)
        {
            Manager.UpdateMute(MusicManager.myMusic.mute);
        }
    }

    /// <summary>
    /// Sets the mute state of the music.
    /// </summary>
    /// <param name="_newState"></param>
    public void SetMuteMusic(bool _newState)
    {
        MusicManager.myMusic.mute = _newState;
        PlayerPrefsX.SetBool("Mute", _newState);
        PlayerPrefs.Save();

        if (MusicManager.myMusic.mute)
        {
            myButtonImage.sprite = MusicOff;
        }
        else
        {
            myButtonImage.sprite = MusicOn;
        }

        if (Manager != null)
        {
            Manager.UpdateMute(_newState);
        }
    }
    #endregion
}
