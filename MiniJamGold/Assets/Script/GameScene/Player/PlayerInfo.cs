using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInfo : MonoBehaviour
{
    private static PlayerInfo sInstance;

    public static PlayerInfo Instance { get { return sInstance; } }

    private void Awake()
    {
        if (sInstance != null && sInstance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            sInstance = this;
        }
    }

    [SerializeField] private int mGoldAmount = 0;

    public int GetGoldAmount()
    {
        return mGoldAmount;
    }

    public void AddGold(int argAmount)
    {
        mGoldAmount += argAmount;
    }
}
