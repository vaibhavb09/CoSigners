using UnityEngine;
using System.Collections;

public class LevelTransitionControl : MonoBehaviour {


	float _timer1 = 0.5f;
	float _timer2 = 0.7f;
	bool _doingStuff1 = false;
	bool _doingStuff2 = false;
	LevelDescription _targetLevel;

	public static bool scaleformCameraCreated = false;

	void Awake()
	{
		if( !scaleformCameraCreated )
		{
			GameObject sfCamera = (GameObject) Resources.Load("Prefabs/ScaleformCamera");
			GameObject.Instantiate( sfCamera );
			scaleformCameraCreated = true;
		}
	}

	// Use this for initialization
	void Start () 
	{
		_targetLevel = LevelTransition.LoadLevelConfig(true, GameManager.Manager.InStartMenu);
	}
	
	// Update is called once per frame
	void Update ()
	{
		if(_doingStuff1 == false)
		{
			if(_timer1 > 0)
			{
				_timer1 -= Time.deltaTime;
			}
			else
			{
				_doingStuff1 = true;
				NetworkManager.Manager.ImReady(GameManager.Manager.PlayerType, true);

			}
		}

		if(_doingStuff2 == false)
		{
			if(_timer2 > 0)
			{
				_timer2 -= Time.deltaTime;
			}
			else
			{
				_doingStuff2 = true;
				GameManager.Manager.CurrentLevelTexture = _targetLevel.LevelDetail;
				NetworkManager.Manager.SetupLevelInfoInternally(_targetLevel.LevelName, 
				                                                _targetLevel.TransmitterNumber.ToString(), 
				                                                _targetLevel.EstimatedTime, 
				                                                _targetLevel.Difficulty, 
				                                                _targetLevel.Description,
				                                                _targetLevel.LevelDetail,
				                                                0);
				//GameManager.Manager.AO = Application.LoadLevelAsync(_targetLevel.SceneFile);
				//GameManager.Manager.AO.allowSceneActivation = false;

			}
		}
	}
}
