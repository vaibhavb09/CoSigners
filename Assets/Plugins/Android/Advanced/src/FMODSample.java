package com.Autodesk.HelloWorldDemo;

import com.unity3d.player.UnityPlayerActivity;

import android.content.res.Configuration;
import android.os.Bundle;
import android.util.Log;
import android.view.KeyEvent;

import org.fmod.FMODAudioDevice;

public class FMODSample extends UnityPlayerActivity {


	protected void onCreate (Bundle savedInstanceState)
	{
		
		try {
				System.loadLibrary("fmodex");
	        }
		catch( UnsatisfiedLinkError e ) {
				System.err.println("Native code library failed to load.\n" + e);
			}
			
		super.onCreate(savedInstanceState);
	}
	
	protected void onDestroy ()
	{
		super.onDestroy();
	}
	
	protected void onPause()
	{
		super.onPause();
	}	
	
	protected void onResume()
	{
		super.onResume();
	}
	
	public void onConfigurationChanged(Configuration newConfig)
	{
		super.onConfigurationChanged(newConfig);
	}

	public void onWindowFocusChanged(boolean hasFocus)
	{
		super.onWindowFocusChanged(hasFocus);
	}

	// Pass any keys not handled by (unfocused) views straight to UnityPlayer
	public boolean onKeyDown(int keyCode, KeyEvent event)
	{
		boolean bOnKeyDown = super.onKeyDown(keyCode, event);
		
		return bOnKeyDown;
	}

	public boolean onKeyUp(int keyCode, KeyEvent event)
	{
		boolean bOnKeyUp = super.onKeyUp(keyCode, event);
		
		return bOnKeyUp;
	}

}