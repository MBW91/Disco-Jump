using UnityEngine;

public class SpecialBlock : MonoBehaviour
{
    #region public
    #endregion

    #region protected
    #endregion

    #region private
    private Animator myAnimator;
    #endregion

    #region MonoBehaviour
    void Awake()
    {
        myAnimator = GetComponent<Animator>();
    }

    void Update()
    {
        if(GameManager.Pause)
        {
            myAnimator.speed = 0f;
        }
        else
        {
            myAnimator.speed = 1f;
        }
    }
    #endregion
}
