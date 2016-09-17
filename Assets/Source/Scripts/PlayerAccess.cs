using UnityEngine;
using System.Collections;

public class PlayerAccess
{
	/// <summary>
	/// The Singleton obkect self.
	/// </summary>
	static PlayerAccess mSelf;

	public GameObject Player;

	public static PlayerAccess Self
	{
		get
		{
			if (mSelf == null)
			{
				mSelf = new PlayerAccess();
			}
			if(mSelf.Player == null)
			{
				mSelf.Player = GameObject.Find("Playertheif(Clone)");
			}

			return mSelf;
		}
	}

	PlayerAccess()
	{
		Player = GameObject.Find("Playertheif(Clone)");
	}
}

