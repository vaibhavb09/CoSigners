using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.IO;

public class PasswordGenerator
{
	
	private static PasswordGenerator _instance;
	private List<string> passwords;
	private string currentPassword;
	public	List<string> AcceptablePasswords;
	
	public PasswordGenerator()
	{
		currentPassword = "legendary";
		passwords = null;
		GeneratePasswordList();
		AcceptablePasswords = new List<string>();
	}
	
	public static PasswordGenerator Manager
	{
		get
		{
			if(_instance == null)
			{
				_instance = new PasswordGenerator();			
			}
			return _instance;	
		}
	}
	
	public void GeneratePasswordList()
	{
		if( passwords == null )
		{
			passwords = new List<string>();
			TextAsset passwordDictionary = (TextAsset) Resources.Load( "TextAssets/PasswordDictionary",typeof(TextAsset) );
			for( int i = 0; i < passwordDictionary.text.Split("\n"[0]).Length; i++ )
				passwords.Add( passwordDictionary.text.Split("\n"[0])[i] );
		}
	}
	
	public string GetPassword()
	{
		if( currentPassword.Equals("legendary" ) ) //Dev Password Default: legendary
		{
			currentPassword = passwords[ UnityEngine.Random.Range( 0, passwords.Count ) ];
		}
		return currentPassword;
	}
	
	public string GeneratePassword( string i_pswd )
	{
		bool breakNow = false;
		if( i_pswd.Equals( "legendary" ) ) //Dev Password Default: legendary
		{
			currentPassword = passwords[ UnityEngine.Random.Range( 0, passwords.Count ) ];
			foreach( string s in passwords )
			{
				if( s.Equals( currentPassword ) )
				{
					passwords.Remove( s );
					breakNow = true;
				}
				if( breakNow )
					break;
			}
			AcceptablePasswords.Add( currentPassword );
			NetworkManager.Manager.AddPassword( currentPassword );
			return currentPassword;
		}	
		return i_pswd;
	}
	
//	private void PrintPasswords()
//	{
//		foreach( string p in passwords )
//		{
//			Debug.Log("P = " + p);
//		}
//	}
	
	public void SetPassword( string i_password )
	{
		currentPassword = i_password;
	}
	
	// Update is called once per frame
	void Update () {
		//Debug.Log( GetPasswordForCurrentLevel() );
	}
}

