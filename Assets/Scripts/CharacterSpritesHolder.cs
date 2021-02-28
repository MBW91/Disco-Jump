using UnityEngine;

public class CharacterSpritesHolder : MonoBehaviour
{
    #region public
    public Sprite[] CharacterSprites;
    public int[] CoinCost;
    #endregion

    #region protected
    #endregion

    #region private
    private static CharacterSpritesHolder instance = null;
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

        DontDestroyOnLoad(this.gameObject);
    }

    /// <summary>
    /// Get the character sprites holder, which holds all character sprites.
    /// </summary>
    public static CharacterSpritesHolder Instance
    {
        get { return instance; }
    }
    #endregion
}
