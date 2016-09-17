using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System;

public struct InfoNodeStruct
{
	private string m_type;
	private int m_hexIndex;
	private int m_ID;

	public InfoNodeStruct( int i_hexIndex, int i_ID, string i_type )
	{
		m_hexIndex = i_hexIndex;
		m_ID = i_ID;
		m_type = i_type;
	}

	public int HexIndex
	{
		get
		{
			return m_hexIndex;
		}
		set
		{
			m_hexIndex = value;
		}
	}
	public int ID
	{
		get
		{
			return m_ID;
		}
		set
		{
			m_ID = value;
		}
	}
	public string Type
	{
		get
		{
			return m_type;
		}
		set
		{
			m_type = value;
		}
	}
}

public class InfoNodeManager : MonoBehaviour 
{
	private static InfoNodeManager 			m_instance;
	private float 							_infoNodeAngle = 0.0f;
	private List<InfoNodeStruct> 			_infoNodes;
	public Dictionary<string, string>		_MovieList;

	private Dictionary<int, GameObject> 	_infoNodePlatforms;
	private Dictionary<int, GameObject> 	_infoNodeTextures;
	private GameObject 						_infoNodePrefab;
	private GameObject 						_infoNodePlatformPrefab;
	private Material   						_infoNodeActiveMat;
	private Material   						_infoNodeInActiveMat;
	private int 							_infoNodeIndex;
	private bool							_onGUI;
	private AudioSource             		_infoNodeSource;
	public	int								_currentInfoNodeID = -1;

//	public InfoNodeManager()
//	{
//		m_instance = this;
//	}
//	
	public static InfoNodeManager Manager
	{
		get
		{
			if( m_instance == null )
			{
				GameObject infonodeGameObject = GameObject.Find("InfoNodeManager");
				if( infonodeGameObject != null)
					m_instance = infonodeGameObject.GetComponent<InfoNodeManager>();
				else
				{
					infonodeGameObject = (GameObject)Resources.Load("Prefabs/Manager/InfoNodeManager");
					m_instance = infonodeGameObject.GetComponent<InfoNodeManager>();
				}
			}
			return m_instance;
		}
	}

	public void InfoHexCaptured( int i_ID, bool i_open )
	{
		GameObject currentInfoNodePlatform = null;
		_infoNodePlatforms.TryGetValue( i_ID, out currentInfoNodePlatform );

		GameObject currentInfoNodeTexture = null;
		_infoNodeTextures.TryGetValue( i_ID, out currentInfoNodeTexture );

		if( i_open )
		{
			currentInfoNodePlatform.GetComponent<InfoNodePlatform>().UnlockInfoNodePlatform(i_open);
			currentInfoNodeTexture.renderer.material = _infoNodeActiveMat;
		}
		else
		{
			currentInfoNodePlatform.GetComponent<InfoNodePlatform>().LockInfoNodePlatform(i_open);
			currentInfoNodeTexture.renderer.material = _infoNodeInActiveMat;
		}
	}

	public void LoadInfoNodeDataFromConfig(GraphData i_gData)
	{
		_infoNodes = new List<InfoNodeStruct>();
		_infoNodePlatforms = new Dictionary<int, GameObject>();
		_infoNodeTextures = new Dictionary<int, GameObject>();
		LoadResources();
		_currentInfoNodeID = -1;
		//InfoNodes.Clear();

		InfoData[] infoNodeData = i_gData.Infos;
		if( infoNodeData.Length != 0 )
		{
			foreach( InfoData infonodes in infoNodeData )
			{
				_infoNodes.Add( new InfoNodeStruct( infonodes.InfoHexIndex, infonodes.InfoID, infonodes.InfoType ) );

				if( GameManager.Manager.PlayerType == 1 )
				{
					GameObject tempInfoPlatform = (GameObject)Instantiate( _infoNodePlatformPrefab, HexGrid.Manager.GetCoordHex(infonodes.InfoHexIndex, 0.01f), Quaternion.identity);
					tempInfoPlatform.GetComponent<InfoNodePlatform>().InfoID = infonodes.InfoID;

					_infoNodePlatforms.Add( infonodes.InfoID, tempInfoPlatform);

					_infoNodeTextures.Add( infonodes.InfoID, (GameObject) Instantiate( _infoNodePrefab, HexGrid.Manager.GetCoordHex( infonodes.InfoHexIndex, 0.01f), Quaternion.identity ) );
				}
				if( GameManager.Manager.PlayerType == 2 )
				{
					_infoNodeTextures.Add( infonodes.InfoID, (GameObject) Instantiate( _infoNodePrefab, HexGrid.Manager.GetCoordHex( infonodes.InfoHexIndex, 60.0f), Quaternion.identity ) );
				}
			}

			InfoNodeManager.Manager.LoadSwfInformation();
		}
		else
		{
			//Debug.Log( "InfoNodes count is: " +_infoNodes.Count );
		}
	}

	public void Refresh()
	{
		if( _infoNodes.Count != 0 )
		{
			foreach( InfoNodeStruct infoNode in _infoNodes )
			{
				if( HexGrid.Manager.IsHexCaptured ( infoNode.HexIndex ) )
				{
					if( GameManager.Manager.PlayerType == 2 )
					{
						GameObject currentInfoNodeTexture = null;
						_infoNodeTextures.TryGetValue( infoNode.ID, out currentInfoNodeTexture );
						currentInfoNodeTexture.renderer.material = _infoNodeActiveMat;
						NetworkManager.Manager.InfoNodeHexCaptured( infoNode.ID, true );
					}
				}
				else
				{
					if( GameManager.Manager.PlayerType == 2 )
					{
						GameObject currentInfoNodeTexture = null;
						_infoNodeTextures.TryGetValue( infoNode.ID, out currentInfoNodeTexture );
						currentInfoNodeTexture.renderer.material = _infoNodeInActiveMat;
						NetworkManager.Manager.InfoNodeHexCaptured( infoNode.ID, false );
					}
				}
			}
		}
	}
	
	void LoadResources()
	{
		_infoNodeIndex = -1;
		_infoNodePrefab = (GameObject)Resources.Load("Prefabs/Hacker/Graph/InfoTerminal_Prefab");
		_infoNodePlatformPrefab = (GameObject)Resources.Load("Prefabs/Theif/IT_Stand_Parent");
		if(_infoNodePlatformPrefab == null)
		{
			//Debug.Log("platform load fail");
		}
		_infoNodeActiveMat = Resources.Load("Materials/Hacker/InfoTerminalActive", typeof(Material)) as Material;
		_infoNodeInActiveMat = Resources.Load("Materials/Hacker/InfoTerminal_Inactive", typeof(Material)) as Material;
		_infoNodeSource	= GameObject.Find("InfoNodeSource").GetComponent<AudioSource>();
	}

	public void PlayInformation( int i_ID )
	{
		//Debug.Log("ID: " + i_ID);
		InfoNodeStruct current = new InfoNodeStruct();

		foreach( InfoNodeStruct info in _infoNodes )
		{
			if( i_ID == info.ID )
			{
				current = info;
				break;
			}
		}



		if( current.Type == "video" )
		{
			// pause game functionality exists in PlayInfoNodeAnimation function

			// Call function for playing Video - Scaleform 
			// This call to play movie just plays the video frame animation. Actual video is played from PlayInfoNodeAnimation() in SwfLayout.cs. 
			//Audio is also played from there.

			if( GameManager.Manager._currentInfoData.m_currentMovie == "" )
			{
				//Video
				NetworkManager.Manager.PlayMovieRPC(InfoNodeManager.Manager._MovieList["InfoNodeVideo_" +InfoNodeManager.Manager._currentInfoNodeID], GameManager.Manager.PlayerType);
				//Audio
				AudioSource _infoNodeSource = GameObject.Find("InfoNodeSource").GetComponent<AudioSource>();
				soundMan.soundMgr.playOneShotOnSource(_infoNodeSource, "InfoNodeAudio_"+InfoNodeManager.Manager._currentInfoNodeID.ToString(),-1);
				NetworkManager.Manager.SetCurrentInfoDataParamsRPC(InfoNodeManager.Manager._MovieList["InfoNodeVideo_" +InfoNodeManager.Manager._currentInfoNodeID.ToString()], GameManager.Manager.PlayerType);

				if(GameManager.Manager.PlayerType == 1) // only Thief can do this
				{
					NetworkManager.Manager.PauseGame(true);
				}
				
				if(GameManager.Manager.PlayerType == 1)
				{
					ThiefManager.Manager.DisableThiefActions();
				}
				else
				{
					HackerManager.Manager.DisableHackerActions();
				}
			}

			// Play Audio
			//Debug.Log("Audio Source ID Info Node: Play one shot" + _infoNodeSource.GetInstanceID() );
			//soundMan.soundMgr.playOneShotOnSource(_infoNodeSource, "audio_"+i_ID.ToString(),-1);
		}
		else
		{
			//Debug.Log( "Type is: " + current.Type );

			// Play Audio
			soundMan.soundMgr.playOneShotOnSource(_infoNodeSource, "audio_"+i_ID.ToString(),-1, -1, 1.0f);
		}

		// Final Info Node
		if( i_ID == 99 )
		{
			NetworkManager.Manager.OnFinalVideoEnd();
		}

		//Play info based on ID of current.
		_onGUI = true;
	}

	void OnGUI()
	{
		if( _onGUI )
		{
			bool clicked = GUI.Button( new Rect( 100.0f, 100.0f, 10.0f, 10.0f ), "Quit" );
			if( clicked )
			{
				UnpauseGame();
			}
		}
	}

	public void PauseGame()
	{
		ThiefManager.Manager.PauseGame();
	}
	
	public void UnpauseGame()
	{
	    ThiefManager.Manager.UnPauseGame();
	}

	public void CloseInfoVideoAudio()
	{
		//isplayin
		NetworkManager.Manager.PauseGame(false);
		NetworkManager.Manager.SilenceInfoNodeSource();
		NetworkManager.Manager.EnableActions();
	}

	public void SilenceInfoNodeSource()
	{
		if( _infoNodePlatforms.ContainsKey(_currentInfoNodeID) )
		_infoNodePlatforms[_currentInfoNodeID].GetComponent<InfoNodePlatform>().m_state = InfoPlatformStates.READY_TO_PLAY_MOVIE;		

		soundMan.soundMgr.silenceSource(_infoNodeSource);
	}

	public void LoadSwfInformation()
	{
		_MovieList = new Dictionary<string, string>();
		TextAsset file = (TextAsset)Resources.Load("TextAssets/SwfConfig");

		string line = "Default";
		StringReader reader = new StringReader(file.text);

		while( true )
		{
			line = reader.ReadLine();

			if( line != null )
			{
				string[] names = line.Split(("#").ToCharArray());

				if(names.Length == 2 )
				{
					_MovieList.Add(names[0], names[1]);
				}
			}
			else
			{
				break;
			}
		}
	}

	// Use this for initialization
	void Start () 
	{

	}
	
	// Update is called once per frame
	void Update () 
	{
	}
}
