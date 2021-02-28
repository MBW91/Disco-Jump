using UnityEngine;

public class CoinReward : MonoBehaviour
{
    #region public
    public float Speed = 0.1f;
    public float Distance = 2f;
    #endregion

    #region private
    private Transform myTransform;
    private AudioSource GetCoin;
    private float TraveledDistance = 0f;
    #endregion

    #region MonoBehaviour
    void Awake()
    {
        myTransform = this.transform;
        myTransform.localScale = Vector3.zero;
        GetCoin = myTransform.GetComponent<AudioSource>();
        GetCoin.mute = PlayerPrefsX.GetBool("Mute");
	}
	
	void Update()
    {
	    if(TraveledDistance >= Distance)
        {
            Destroy(this.gameObject);
        }

        if(myTransform.localScale.x < 2f)
        {
            myTransform.localScale = myTransform.localScale + new Vector3(Speed, Speed, Speed);
        }
        else
        {
            myTransform.localScale = new Vector3(2f, 2f, 2f);
        }

        myTransform.position = new Vector3(myTransform.position.x, myTransform.position.y + Speed, myTransform.position.z);
        TraveledDistance += Speed;
	}
    #endregion
}
