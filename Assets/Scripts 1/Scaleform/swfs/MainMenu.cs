/**********************************************************************

Filename    :	UI_Scene_Demo1.cs
Content     :  
Created     :   
Authors     :   Ankur Mohan

Copyright   :   Copyright 2012 Autodesk, Inc. All Rights reserved.

Use of this software is subject to the terms of the Autodesk license
agreement provided at the time of installation or download, or which
otherwise accompanies this software in either electronic or hard copy form.
 
***********************************************************************/

using System;
using System.Collections;
using UnityEngine;
using Scaleform;
using Scaleform.GFx;

public class MainMenu : Movie
{
    protected Value	theMovie = null;
	private MyCamera parent = null;
    
    public MainMenu(MyCamera parent, SFManager sfmgr, SFMovieCreationParams cp) :
        base(sfmgr, cp)
    {
		this.parent = parent;
        SFMgr = sfmgr;
        this.SetFocus(true);
    }
	
	
    public void OnRegisterSWFCallback(Value movieRef)
    {
        theMovie = movieRef;
		GameObject monitor_screen = GameObject.Find("monitor_screen");
		monitor_screen.GetComponent<MyRTT>().enabled = true;
		/*
		int a = theMovie.Invoke("test2Ints", 1, 2, "param", true, theMovie);
		
		theMovie.Invoke("test2Strings", "a", "b");
		theMovie.Invoke("test2Booleans", true, false);
		theMovie.Invoke("test2Anything", theMovie, theMovie);
		theMovie.Invoke("testIntString", 1, "b");
		*/
	}

	public void SetState(int newState)
	{
		parent.currentState = newState;	
		if(parent.anchors[newState]!=null)
			parent.currentTarget = parent.anchors[newState];
	}
	
	
	public void SetSFxVolume(double val)
	{
	}
	
	public void SetMusicVolume(double val)
	{
	}
	
	public void SetSensitivity(double val)
	{
		parent.sensitivity = (float)val;
	}
	
	public void SetFieldOfView(double val)
	{
		parent.camera.fieldOfView = (float)val;
	}
	
	public void SetDynamicLighting(bool isOn)
	{
		parent.dynamicLight.SetActive(isOn);
		Debug.Log ("set dynamic lighting " + isOn);
	}
	
	public void SetFreeLook(bool isOn)
	{
		parent.freeLook = isOn;
	}
	
	public void SetInvertY(bool isOn)
	{
		if(isOn)
		{
			parent.sensitivityY = -Math.Abs(parent.sensitivityY);	
		}
		else
		{
			parent.sensitivityY = Math.Abs(parent.sensitivityY);	
		}
	}
	
	public void SetStageMouse(bool isDown)
	{
		parent.stageMouseDown = isDown;	
	}
	
	public void GotoURL(string theUrl)
	{
		Application.OpenURL(theUrl);
	}
	
	public void OpenGate()
	{
		RenderTextureDemo.instance.OpenGate();
	}
	
	public void CloseGate()
	{
		RenderTextureDemo.instance.CloseGate();
	}
	
	public void LoadXML(string url)
	{
		parent.StartCoroutine(LoadRSS(url));
	}
	
	IEnumerator LoadRSS(string url) {
        WWW www = new WWW(url);
        yield return www;
		
		
		theMovie.Invoke("ParseXML", www.text);
		
    }
	
	public void SetCrosshairPos(float x, float y)
	{
		Value targetMovie = theMovie.GetMember("CrosshairMC");
		if (targetMovie != null)
		{
			SFDisplayInfo dInfo = targetMovie.GetDisplayInfo();
			
			if (dInfo == null) return;
			dInfo.X += x;				
			dInfo.Y += y;
			targetMovie.SetDisplayInfo(dInfo);
		}
		
		Value textMC = theMovie.GetMember("InfoText");
		if (textMC != null)
		{
			textMC.SetText("x: " + x + "\ny: " + y);
		}
	}
	
}
	
	