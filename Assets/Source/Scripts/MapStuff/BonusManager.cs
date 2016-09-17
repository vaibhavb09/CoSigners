using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class BonusManager : MonoBehaviour {
	
	public class Bonus
	{
		public BonusManager.BonusType type;
		public int hexIndex;
		public bool captured;
		public Transform myIndicator;
		public Transform m_PasswordPlatform;
		
		public Bonus(int i_hexIndex,  BonusManager.BonusType i_type, Transform i_indicator, Transform i_passwordPlatform)
		{
			hexIndex = i_hexIndex;
			type = i_type;
			captured = false;
			myIndicator = i_indicator;
			m_PasswordPlatform = i_passwordPlatform;
		}
	}
	
	
	static BonusManager _instance;
	private Dictionary<int, Bonus> _bonuses;
	public enum BonusType {Password, Laser, IRBoost, TranmitterBoost}
	
	Transform passwordPrefab;
	Transform passwordPlatformPrefab;
	Material passwordTheifInactive;
	Material passwordTheifActive;
	Material passwordHackerInactive; 
	Material passwordHackerActive;
	
	public static BonusManager Manager
	{
		get
		{
			if(_instance == null)
			{
				_instance = new BonusManager();			
			}
			return _instance;
		}
	}

	
	public BonusManager () 
    { 
        _instance = this;
		
    }
	
	// Use this for initialization
	void Start () {
		
	}
	
	
	// Update is called once per frame
	void Update () {
	}
	
	
	public void LoadBonuses(GraphData gData)
	{
		_bonuses = new Dictionary<int, Bonus>();
		
		LoadPrefabs();
		
		BonusPasswordData[] bonusPasswordData = gData.BonusPasswords;
		BasicScoreSystem.Manager.PasswordsInTheLevel = bonusPasswordData.Length;
		for ( int i=0 ; i<bonusPasswordData.Length ; i++ )
		{
			int hexIndex = bonusPasswordData[i].HexIndex;
			Transform tempIndicator;
			Transform tempPasswordPlatform = null;
			float _facing = bonusPasswordData[i].Facing;
			if ( GameManager.Manager.PlayerType == 1)
			{
				tempIndicator = (Transform) Instantiate(passwordPrefab, HexGrid.Manager.GetCoordHex(hexIndex, 0.01f), Quaternion.identity);
				tempPasswordPlatform = (Transform) Instantiate(passwordPlatformPrefab, HexGrid.Manager.GetCoordHex(hexIndex, 0.01f), Quaternion.identity);
				tempPasswordPlatform.localEulerAngles = new Vector3( 0, _facing - 90, 0 );
			}
			else
			{
				tempIndicator = (Transform) Instantiate(passwordPrefab, HexGrid.Manager.GetCoordHex(hexIndex, 60.0f), Quaternion.identity);
				//tempIndicator.renderer.enabled = false;
			}
			//Transform tempIndicator = (Transform) Instantiate(passwordPrefab, HexGrid.Manager.GetCoordHex(26, 0.1f), Quaternion.identity);
			Bonus tempBonus = new Bonus( hexIndex, BonusType.Password, tempIndicator, tempPasswordPlatform );
			_bonuses.Add ( hexIndex, tempBonus );
		}
		 
	}
	
	
	private BonusType GetType(string i_type)
	{
		if ( i_type.Equals("Password"))
			return BonusType.Password;
		else
			return BonusType.IRBoost;
	}
	
	
	private void LoadPrefabs()
	{
		passwordPrefab = Resources.Load("Prefabs/Theif/PasswordIndicator", typeof(Transform)) as Transform;
		passwordPlatformPrefab = Resources.Load("Prefabs/Theif/Password_Platform", typeof(Transform)) as Transform;
		
		passwordTheifActive = Resources.Load("Materials/Thief/PasswordActiveMaterial", typeof(Material)) as Material;
		passwordTheifInactive = Resources.Load("Materials/Thief/PasswordInactiveMaterial", typeof(Material)) as Material;
		passwordHackerActive = Resources.Load("Materials/Hacker/PasswordActiveMaterial", typeof(Material)) as Material;
		passwordHackerInactive = Resources.Load("Materials/Hacker/PasswordInactiveMaterial", typeof(Material)) as Material;
	}
	
	
	public void RefreshBonuses()
	{
		
		foreach ( KeyValuePair<int, Bonus> bonus in _bonuses )
		{
			if ( HexGrid.Manager.IsHexCaptured ( bonus.Value.hexIndex ) )
			{
				if ( GameManager.Manager.PlayerType == 2)
				{
					//Debug.Log("type:"+bonus.Value.type);
					
					if ( bonus.Value.captured == false )
					{	
						// [SOUND TAG] [Password_Hex_Surround]
						// if the player type is the hacker
						if( GameManager.Manager.PlayerType == 2)
							soundMan.soundMgr.playOneShotOnSource(null,"Password_Hex_Surround",GameManager.Manager.PlayerType,2);
						
						switch( bonus.Value.type )
						{
							
							case BonusType.Password: 		
							{
								bonus.Value.myIndicator.renderer.material = passwordHackerActive; 
								NetworkManager.Manager.PasswordPlatformStateChange( bonus.Key, true );
								break;
							}
									
							case BonusType.IRBoost: 		
							{
                                bonus.Value.myIndicator.renderer.material = passwordHackerActive;
                           		List<Node> IRs = GraphManager.Manager.GetNodes(GameManager.NodeType.InfraRed);
								
	                            foreach ( Node n in IRs )
								{    
									((IRNode)n).AddIRBoost();
								}
								
						    	break;
							}
								
							case BonusType.Laser: 		
							{
                               bonus.Value.myIndicator.renderer.material = passwordHackerActive; 
                              
									
                                foreach( GameObject laser in  LaserManager.Manager.lasers )
								{
									if( laser.GetComponent<LaserController>().isActive == true )
										laser.GetComponent<LaserController>().DeactivateLaser();
								}
                               	break;
                			}	
							case BonusType.TranmitterBoost: 
							{
			                  	bonus.Value.myIndicator.renderer.material = passwordHackerActive; 
			              
			                  	HexGrid.Manager.AddTransmitterBoost();
			                  	break;
	                    	}
						}
					}
				}
				else
				{
					/*
					switch( bonus.Value.type )
					{
						case BonusType.Password: 		bonus.Value.myIndicator.renderer.material = passwordTheifActive; break;
							
						case BonusType.IRBoost: 		bonus.Value.myIndicator.renderer.material = passwordTheifActive; break;
							
						case BonusType.Laser: 			bonus.Value.myIndicator.renderer.material = passwordTheifActive; break;
							
						case BonusType.TranmitterBoost: bonus.Value.myIndicator.renderer.material = passwordTheifActive; break;
					}
					*/
				}
				
				bonus.Value.captured = true;
			}
			else
			{	
				if ( GameManager.Manager.PlayerType == 2 )
				{
					if ( bonus.Value.captured == true )
					{
						switch( bonus.Value.type )
						{
							case BonusType.Password: 		
							{
								bonus.Value.myIndicator.renderer.material = passwordHackerInactive; 
								NetworkManager.Manager.PasswordPlatformStateChange( bonus.Key, false );
								break;
							}
								
							case BonusType.IRBoost: 		
							{
		                        bonus.Value.myIndicator.renderer.material = passwordHackerInactive; 
		                        List<Node> IRs = GraphManager.Manager.GetNodes(GameManager.NodeType.InfraRed);
									foreach ( Node n in IRs )
									{
										((IRNode)n).RemoveIRBoost();
									}
		                        break;
							}
								
							case BonusType.Laser: 			
							{
	                           	bonus.Value.myIndicator.renderer.material = passwordHackerInactive;
	                          
	                        	foreach( GameObject laser in  LaserManager.Manager.lasers )
								{
									if( laser.GetComponent<LaserController>().isActive == false )
										laser.GetComponent<LaserController>().ActivateLaser();
								}
	                           	break;
                            }
						 	
							case BonusType.TranmitterBoost: 
							{
                   				bonus.Value.myIndicator.renderer.material = passwordHackerInactive; 
                       			HexGrid.Manager.RemoveTransmitterBoost();
                           		break;
                            }
						}
					}
					bonus.Value.captured = false;
				}
				else
				{
					/*
					switch( bonus.Value.type )
					{
						case BonusType.Password: 		bonus.Value.myIndicator.renderer.material = passwordTheifInactive; break;
							
						case BonusType.IRBoost: 		bonus.Value.myIndicator.renderer.material = passwordTheifInactive; break;
							
						case BonusType.Laser: 			bonus.Value.myIndicator.renderer.material = passwordTheifInactive; break;
							
						case BonusType.TranmitterBoost: bonus.Value.myIndicator.renderer.material = passwordTheifInactive; break;
					}
					*/
				}
			}
			//Debug.Log ("Bonus on Hex " + bonus.Value.hexIndex + " is " + ((bonus.Value.captured)?"captured" : "not captured") );
		}
		
	}
	
	
	public bool ContainsBonus(int i_hexIndex)
	{
		if ( _bonuses.ContainsKey(i_hexIndex) )
			return true;
		else
			return false;
	}
	
	public void TriggerPasswordPlatformAnimation( int i_hexID, bool i_open )
	{
		Bonus tempPasswordBonus = null;
		_bonuses.TryGetValue( i_hexID, out tempPasswordBonus );
		
		if( tempPasswordBonus != null )
		{
			if( i_open )
			{
				tempPasswordBonus.m_PasswordPlatform.GetComponent<PasswordPlatformAnimation>().OpenPasswordPlatform();
			}
			else
			{
				tempPasswordBonus.m_PasswordPlatform.GetComponent<PasswordPlatformAnimation>().ClosePasswordPlatform();
			}
		}
	}
	
	public Vector3 GetPasswordPlatformPosition( int i_hexID )
	{
		Bonus tempPasswordBonus = null;
		_bonuses.TryGetValue( i_hexID, out tempPasswordBonus );
		
		if( tempPasswordBonus != null )
			return tempPasswordBonus.m_PasswordPlatform.position;
		
		return Vector3.zero;
	}
}
