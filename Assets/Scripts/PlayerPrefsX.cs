using UnityEngine;

public class PlayerPrefsX : MonoBehaviour
{
    /// <summary>
    /// Set a boolean in the playerprefs.
    /// </summary>
    /// <param name="name">Name of the playerpref value.</param>
    /// <param name="booleanValue">The new value.</param>
    public static void SetBool(string name, bool booleanValue)
    {
        PlayerPrefs.SetInt(name, booleanValue ? 1 : 0);
    }

    /// <summary>
    /// Get a boolean of the playerprefs.
    /// </summary>
    /// <param name="name">Name of the playerpref value.</param>
    /// <returns>The boolean of the requested playerpref value.</returns>
    public static bool GetBool(string name)
    {
        return PlayerPrefs.GetInt(name) == 1 ? true : false;
    }

    /// <summary>
    /// Return the boolean of the playerpref if it exists, otherwise give me back the passed boolean.
    /// </summary>
    /// <param name="name">Name of the playerpref value.</param>
    /// <param name="defaultValue">The value you get back, if teh requested playerpref value doesn't exists.</param>
    /// <returns></returns>
    public static bool GetBool(string name, bool defaultValue)
    {
        if (PlayerPrefs.HasKey(name))
        {
            return GetBool(name);
        }

        return defaultValue;
    }
}
