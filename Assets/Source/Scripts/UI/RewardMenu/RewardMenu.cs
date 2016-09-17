using UnityEngine;
using System.Collections;

public class RewardMenu {
	
	private bool _gameFinished;
	private Texture2D CongratsTexture;
	private Texture2D InfoTexture;
	private Texture2D CancelButtonActive;
	private Texture2D CancelButtonNormal;
	private Texture2D ExitGameButtonActive;
	private Texture2D ExitGameButtonNormal;
	
	public RewardMenu (bool i_isFinished) 
    { 
		_gameFinished = i_isFinished;
		//LoadRewardMenuContent();
    }
	
	public void LoadRewardMenuContent()
	{
		CongratsTexture 	= Resources.Load("Textures/ExitScreen/Congratulations", typeof(Texture2D)) as Texture2D;
		InfoTexture 		= Resources.Load("Textures/ExitScreen/InfoTexture", typeof(Texture2D)) as Texture2D;
		CancelButtonActive 	= Resources.Load("Textures/ExitScreen/btn_mainMenu_active", typeof(Texture2D)) as Texture2D;
		CancelButtonNormal 	= Resources.Load("Textures/ExitScreen/btn_mainMenu_normal", typeof(Texture2D)) as Texture2D;
		ExitGameButtonActive = Resources.Load("Textures/ESCMenu/PauseMenu_ExitButton_Active", typeof(Texture2D)) as Texture2D;
		ExitGameButtonNormal = Resources.Load("Textures/ESCMenu/PauseMenu_ExitButton_Available", typeof(Texture2D)) as Texture2D;
	}
	
	public void Display()
	{
		//LevelTransition.Show();
		ScreenHelper.DrawTexture(0, 0, 64, 36, InfoTexture);
		
		if(_gameFinished)
		{
			ScreenHelper.DrawTexture(9, 2, 45, 8, CongratsTexture);
		}
		
		
		if(ScreenHelper.DrawButton(18, 30, 12, 2, CancelButtonActive, CancelButtonNormal))
		{
			Application.LoadLevel(0);
		}
		if(ScreenHelper.DrawButton(32, 30, 12, 2, ExitGameButtonActive, ExitGameButtonNormal))
		{
			Application.Quit();
		}		
	}

}
