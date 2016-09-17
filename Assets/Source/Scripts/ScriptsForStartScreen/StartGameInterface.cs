using UnityEngine;
using System.Collections;

public class StartGameInterface : MonoBehaviour {
	
	public  bool AutoLogin = true;
	
	private bool _showStartGameInterface = false;
	
	private string _playerName = "";
	
	private GameObject _playerUtil;
	private GameObject _camera;
	private StartGameFlow _flow;
	
	private Texture2D _startScreenBackground;
	private Texture2D _playButtonActive;
	private Texture2D _playButtonNormal;
	private Texture2D _changeNameButton;
	private Texture2D _exitButtonNormal;
	private Texture2D _exitButtonActive;
	
	private Rect _createAccountWindow;
	
	private bool _showRewardMenu = false;
	private RewardMenu _rewardMenu;

	public GUISkin CustomSkin;
	
	public void Show()
	{		
		_showStartGameInterface = true;
	}
	
	public void ShutDownLoginInterface()
	{

	}
	
	public void ShutDownStartGameInterface()
	{
		//Debug.Log("#Max:ShutDown Start");
		_showStartGameInterface = false;
	}

	public void CheckLoginForIGN()
	{
		_playerName = _playerUtil.GetComponent<AccountSystem>().GetName();
	}
	
	public void ShowLoginForIGN()
	{
		ScreenHelper.DrawGrayText(26, 31, 7, 1, "Code Name:", 30);
		ScreenHelper.DrawBlueText(34, 31, 10, 1, _playerName, 30);
	}
	
	public void UpdateName()
	{
		_playerName = _playerUtil.GetComponent<AccountSystem>().GetName(); 
	}

	public bool IsStillActive()
	{
		return _showStartGameInterface;
	}

	private void OnGUI()
	{
		if(_showStartGameInterface)
		{
			ScreenHelper.DrawTexture(0, 0, 64, 36, _startScreenBackground);
			ScreenHelper.DrawTexture(23, 24, 4, 2, _flow.AccentLeft);
			ScreenHelper.DrawTexture(37, 24, 4, 2, _flow.AccentRight);
			if(ScreenHelper.DrawButton(26, 24, 12, 2, _playButtonActive, _playButtonNormal))
			{
				PlayerProfile.LoadPlayerProfile();
				_flow.ShowGameLobby();
			}
			
			if(ScreenHelper.DrawButton(26, 27, 12, 2, _exitButtonActive, _exitButtonNormal))
			{
				_showRewardMenu = true;
				_showStartGameInterface = false;
			}
			ShowLoginForIGN();
		}
		
		if(_showRewardMenu)
		{
			_rewardMenu.LoadRewardMenuContent();
			_rewardMenu.Display();
		}
	}
	
	// Use this for initialization
	void Start () {
		_playerUtil = GameObject.Find("PlayerUtil");
		_camera = GameObject.Find("TopDownCamera");
		_flow = gameObject.GetComponent<StartGameFlow>();
		_rewardMenu = new RewardMenu(false);
		_startScreenBackground = Resources.Load("Textures/NewGameLobby/TitleScreenFinal", typeof(Texture2D)) as Texture2D;
		_playButtonActive = Resources.Load("Textures/StartScreenUI/btn_Play_active", typeof(Texture2D)) as Texture2D;
	 	_playButtonNormal = Resources.Load("Textures/StartScreenUI/btn_Play_normal", typeof(Texture2D)) as Texture2D;
		_changeNameButton = Resources.Load("Textures/StartScreenUI/btn_ChangeName_normal", typeof(Texture2D)) as Texture2D;
		_exitButtonActive = Resources.Load("Textures/ESCMenu/PauseMenu_ExitButton_Active", typeof(Texture2D)) as Texture2D;
		_exitButtonNormal = Resources.Load("Textures/ESCMenu/PauseMenu_ExitButton_Available", typeof(Texture2D)) as Texture2D;
		
		//ign only
		CheckLoginForIGN();
	}
}
