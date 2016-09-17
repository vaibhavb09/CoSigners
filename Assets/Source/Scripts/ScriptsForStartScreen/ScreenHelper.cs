using UnityEngine;
using System;
using System.Collections;

public class ScreenHelper {
	
	static float ratio = 16.0f / 9.0f;
	static bool tooWide = (Screen.width/Screen.height) > ratio;
	static float fontSizeRatio = tooWide? Screen.height / 900 : Screen.width / 1600;
	static public float oneUnitLength = tooWide? Screen.height / 36.0f : Screen.width / 64.0f;
	static public int startX = tooWide? (int)(Screen.width/2 - oneUnitLength * 32.0f): 0;
	static public int startY = tooWide? 0 : (int)(Screen.height/2 - oneUnitLength * 18.0f);
	static public GUIStyle StartScreenStyle = new GUIStyle();
	static private Color Gray =  new Color(0.9f, 0.9f, 0.9f, 1.0f);
	static private Color Blue = new Color(0.0f, 0.5f, 0.5f, 1.0f);
	static private Color Green = new Color( 0.0f, 1.0f, 0.5f, 1.0f );
	static private Color DarkBlue = new Color(0.1f, 0.5f, 0.7f, 1.0f);
	static public Font currentFont = Resources.Load("Fonts/Sansation-Regular",typeof(Font)) as Font;


	static public void SlideInTexture(int i_startGridX, int i_startGridY,
	                                  int i_endGridX, int i_endGridY,
	                                  int i_gridWidth, int i_gridHeight,
	                                  Texture2D i_texture,
	                                  float i_ticker,
	                                  float i_duration = 2.0f,
	                                  float i_delay = 0.0f, float i_tickerStart = 0.0f)
	{
		float actualTick = i_ticker - i_delay - i_tickerStart;
		float realX = i_startGridX;
		float realY = i_startGridY;

		if(actualTick >= 0.0f)
		{
			float ratio  = actualTick/i_duration;
			if(ratio > 1.0f)
			{
				realX = i_endGridX;
				realY = i_endGridY;
			}
			else
			{
				float percentage = Mathf.Sin(ratio * (Mathf.PI/2));
				realX = Mathf.Lerp(i_startGridX, i_endGridX, percentage);
				realY = Mathf.Lerp(i_startGridY, i_endGridY, percentage);
			}
		}
		
		DrawTexture(realX, realY, i_gridWidth, i_gridHeight, i_texture);
	}

	static public void SlideInText(int i_startGridX, int i_startGridY,
	                               int i_endGridX, int i_endGridY,
	                               int i_gridWidth, int i_gridHeight,
	                               string i_string,
	                               float i_ticker,
	                               float i_duration = 2.0f,
	                               float i_delay = 0.0f, float i_tickerStart = 0.0f)
	{
		float actualTick = i_ticker - i_delay - i_tickerStart;
		float realX = i_startGridX;
		float realY = i_startGridY;
		
		if(actualTick >= 0.0f)
		{
			float ratio  = actualTick/i_duration;
			if(ratio > 1.0f)
			{
				realX = i_endGridX;
				realY = i_endGridY;
			}
			else
			{
				float percentage = Mathf.Sin(ratio * (Mathf.PI/2));
				realX = Mathf.Lerp(i_startGridX, i_endGridX, percentage);
				realY = Mathf.Lerp(i_startGridY, i_endGridY, percentage);
			}
		}
		StartScreenStyle.normal.background = null;
		StartScreenStyle.normal.textColor = DarkBlue;
		StartScreenStyle.fontSize = (int)(20 * fontSizeRatio);
		StartScreenStyle.font = currentFont;
		StartScreenStyle.alignment = TextAnchor.LowerLeft;
		StartScreenStyle.wordWrap = false;
		//StartScreenStyle.font.material.color = Gray;
		GUI.Label(new Rect(startX + realX * oneUnitLength, startY + realY * oneUnitLength, i_gridWidth * oneUnitLength, i_gridHeight * oneUnitLength),
		          i_string, StartScreenStyle);
	}

	static public void SlideInText(int i_startGridX, int i_startGridY,
	                                  int i_endGridX, int i_endGridY,
	                                  int i_gridWidth, int i_gridHeight,
	                                  string i_string,
	                                  Color i_color,
	                                  float i_ticker,
	                                  float i_duration = 2.0f,
	                               	  float i_delay = 0.0f, float i_tickerStart = 0.0f, TextAnchor i_anchor = TextAnchor.LowerLeft, int i_fontSize = 20)
	{
		float actualTick = i_ticker - i_delay - i_tickerStart;
		float realX = i_startGridX;
		float realY = i_startGridY;
		
		if(actualTick >= 0.0f)
		{
			float ratio  = actualTick/i_duration;
			if(ratio > 1.0f)
			{
				realX = i_endGridX;
				realY = i_endGridY;
			}
			else
			{
				float percentage = Mathf.Sin(ratio * (Mathf.PI/2));
				realX = Mathf.Lerp(i_startGridX, i_endGridX, percentage);
				realY = Mathf.Lerp(i_startGridY, i_endGridY, percentage);
			}
		}
		StartScreenStyle.normal.background = null;
		StartScreenStyle.normal.textColor = i_color;
		StartScreenStyle.fontSize = (int)(i_fontSize * fontSizeRatio);
		StartScreenStyle.font = currentFont;
		StartScreenStyle.alignment = i_anchor;
		StartScreenStyle.wordWrap = false;
		//StartScreenStyle.font.material.color = Gray;
		GUI.Label(new Rect(startX + realX * oneUnitLength, startY + realY * oneUnitLength, i_gridWidth * oneUnitLength, i_gridHeight * oneUnitLength),
		          i_string, StartScreenStyle);
	}

	static public void SlideOutTexture(int i_startGridX, int i_startGridY,
	                                  int i_endGridX, int i_endGridY,
	                                  int i_gridWidth, int i_gridHeight,
	                                  Texture2D i_texture,
	                                  float i_ticker,
	                                  float i_duration = 2.0f,
	                                  float i_delay = 0.0f, float i_tickerStart = 0.0f)
	{

		float actualTick = i_ticker - i_delay - i_tickerStart;
		float realX = i_startGridX;
		float realY = i_startGridY;
		if(actualTick >= 0.0f)
		{
			float ratio  = actualTick/i_duration;
			if(ratio > 1.0f)
			{
				realX = i_endGridX;
				realY = i_endGridY;
			}
			else
			{
				float percentage = Mathf.Sin((1 - ratio) * (Mathf.PI/2));
				realX = Mathf.Lerp(i_startGridX, i_endGridX, percentage);
				realY = Mathf.Lerp(i_startGridY, i_endGridY, percentage);
			}
		}
		
		DrawTexture(realX, realY, i_gridWidth, i_gridHeight, i_texture);
	}

	static public bool SlideInButton(int i_startGridX, int i_startGridY,
	                                 int i_endGridX, int i_endGridY,
	                                 int i_gridWidth, int i_gridHeight,
	                                 Texture2D i_textureNormal,
	                                 float i_ticker,
	                                 float i_duration = 2.0f,
	                                 float i_delay = 0.0f, float i_tickerStart = 0.0f)
	{
		
		float actualTick = i_ticker - i_delay - i_tickerStart;
		float realX = i_startGridX;
		float realY = i_startGridY;
		if(actualTick >= 0.0f)
		{
			float ratio  = actualTick/i_duration;
			if(ratio > 1.0f)
			{
				realX = i_endGridX;
				realY = i_endGridY;
			}
			else
			{
				float percentage = Mathf.Sin(ratio * (Mathf.PI/2));
				realX = Mathf.Lerp(i_startGridX, i_endGridX, percentage);
				realY = Mathf.Lerp(i_startGridY, i_endGridY, percentage);
			}
		}
		
		return DrawButton(realX, realY, i_gridWidth, i_gridHeight, i_textureNormal);
	}

	static public bool SlideOutButton(int i_startGridX, int i_startGridY,
	                                  int i_endGridX, int i_endGridY,
	                                  int i_gridWidth, int i_gridHeight,
	                                  Texture2D i_textureNormal,
	                                  float i_ticker,
	                                  float i_duration = 2.0f,
	                                  float i_delay = 0.0f, float i_tickerStart = 0.0f)
	{
		
		float actualTick = i_ticker - i_delay - i_tickerStart;
		float realX = i_startGridX;
		float realY = i_startGridY;
		if(actualTick >= 0.0f)
		{
			float ratio  = actualTick/i_duration;
			if(ratio > 1.0f)
			{
				realX = i_endGridX;
				realY = i_endGridY;
			}
			else
			{
				float percentage = Mathf.Sin((1 - ratio) * (Mathf.PI/2));
				realX = Mathf.Lerp(i_startGridX, i_endGridX, percentage);
				realY = Mathf.Lerp(i_startGridY, i_endGridY, percentage);
			}
		}
		
		return DrawButton(realX, realY, i_gridWidth, i_gridHeight, i_textureNormal);
	}

	
	static public bool SlideInButton(int i_startGridX, int i_startGridY,
	                                   int i_endGridX, int i_endGridY,
	                                   int i_gridWidth, int i_gridHeight,
	                                   Texture2D i_textureActive,
	                                   Texture2D i_textureNormal,
	                                   float i_ticker,
	                                   float i_duration = 2.0f,
	                                   float i_delay = 0.0f, float i_tickerStart = 0.0f)
	{
		
		float actualTick = i_ticker - i_delay - i_tickerStart;
		float realX = i_startGridX;
		float realY = i_startGridY;
		if(actualTick >= 0.0f)
		{
			float ratio  = actualTick/i_duration;
			if(ratio > 1.0f)
			{
				realX = i_endGridX;
				realY = i_endGridY;
			}
			else
			{
				float percentage = Mathf.Sin(ratio * (Mathf.PI/2));
				realX = Mathf.Lerp(i_startGridX, i_endGridX, percentage);
				realY = Mathf.Lerp(i_startGridY, i_endGridY, percentage);
			}
		}

		return DrawButton(realX, realY, i_gridWidth, i_gridHeight, i_textureActive, i_textureNormal);
	}

	static public bool SlideOutButton(int i_startGridX, int i_startGridY,
	                                  int i_endGridX, int i_endGridY,
	                                  int i_gridWidth, int i_gridHeight,
	                                  Texture2D i_textureActive,
	                                  Texture2D i_textureNormal,
	                                  float i_ticker,
	                                  float i_duration = 2.0f,
	                                  float i_delay = 0.0f, float i_tickerStart = 0.0f)
	{
		
		float actualTick = i_ticker - i_delay - i_tickerStart;
		float realX = i_startGridX;
		float realY = i_startGridY;
		if(actualTick >= 0.0f)
		{
			float ratio  = actualTick/i_duration;
			if(ratio > 1.0f)
			{
				realX = i_endGridX;
				realY = i_endGridY;
			}
			else
			{
				float percentage = Mathf.Sin((1 - ratio) * (Mathf.PI/2));
				realX = Mathf.Lerp(i_startGridX, i_endGridX, percentage);
				realY = Mathf.Lerp(i_startGridY, i_endGridY, percentage);
			}
		}
		
		return DrawButton(realX, realY, i_gridWidth, i_gridHeight, i_textureActive, i_textureNormal);
	}
 
	static public void DrawTexture(int i_startGridX, int i_startGridY, int i_gridWidth, int i_gridHeight, Texture2D i_texture)
	{
		if(i_texture != null)
		{
			GUI.DrawTexture(new Rect(startX + i_startGridX * oneUnitLength, startY + i_startGridY * oneUnitLength, i_gridWidth * oneUnitLength, i_gridHeight * oneUnitLength), i_texture);
		}
		else
		{
			//Debug.Log("Some texture you are trying to draw is null. Stop drawing it!");
		}
	}
	
	static public void DrawTexture(float i_startGridX, float i_startGridY, float i_gridWidth, float i_gridHeight, Texture2D i_texture)
	{
		GUI.DrawTexture(new Rect(startX + i_startGridX * oneUnitLength, startY + i_startGridY * oneUnitLength, i_gridWidth * oneUnitLength, i_gridHeight * oneUnitLength), i_texture);
	}
	
	static public void DrawPowerMeterTexture(float i_initialOffset, float i_startGridX, float i_startGridY, float i_gridWidth, float i_gridHeight, Texture2D i_texture)
	{
		GUI.DrawTexture(new Rect(Screen.width - i_initialOffset * oneUnitLength + i_startGridX * oneUnitLength, i_startGridY * oneUnitLength, i_gridWidth * oneUnitLength, i_gridHeight * oneUnitLength), i_texture);
	}

	static public bool DrawButton(float i_startGridX, float i_startGridY, float i_gridWidth, float i_gridHeight, Texture2D i_textureActive, Texture2D i_textureInActive)
	{
		StartScreenStyle.normal.background = i_textureInActive;
		StartScreenStyle.active.background = i_textureActive;
		StartScreenStyle.hover.background = i_textureActive;
		//GUI.skin.button.onActive.background = i_textureActive;
		//GUI.skin.button.onHover.background = i_textureActive;
		
		// [UI_Beep]
		if(GUI.Button(new Rect(startX + i_startGridX * oneUnitLength, startY + i_startGridY * oneUnitLength, i_gridWidth * oneUnitLength, i_gridHeight * oneUnitLength), GUIContent.none, StartScreenStyle))
		{
			soundMan.soundMgr.playOneShotOnSource(null,"UI_Beep",GameManager.Manager.PlayerType,GameManager.Manager.PlayerType);
			return true;
		}
		else
			return false;
		
		//return GUI.Button(new Rect(startX + i_startGridX * oneUnitLength, startY + i_startGridY * oneUnitLength, i_gridWidth * oneUnitLength, i_gridHeight * oneUnitLength), GUIContent.none, StartScreenStyle);
	}
	
	static public bool DrawButton(int i_startGridX, int i_startGridY, int i_gridWidth, int i_gridHeight, Texture2D i_textureActive, Texture2D i_textureInActive)
	{
		StartScreenStyle.normal.background = i_textureInActive;
		StartScreenStyle.active.background = i_textureActive;
		StartScreenStyle.hover.background = i_textureActive;
		//GUI.skin.button.onActive.background = i_textureActive;
		//GUI.skin.button.onHover.background = i_textureActive;
		
		// [UI_Beep]
		if(GUI.Button(new Rect(startX + i_startGridX * oneUnitLength, startY + i_startGridY * oneUnitLength, i_gridWidth * oneUnitLength, i_gridHeight * oneUnitLength), GUIContent.none, StartScreenStyle))
		{
			soundMan.soundMgr.playOneShotOnSource(null,"UI_Beep",GameManager.Manager.PlayerType,GameManager.Manager.PlayerType);
			return true;
		}
		else
			return false;
		
		//return GUI.Button(new Rect(startX + i_startGridX * oneUnitLength, startY + i_startGridY * oneUnitLength, i_gridWidth * oneUnitLength, i_gridHeight * oneUnitLength), GUIContent.none, StartScreenStyle);
	}
	
	static public float GetUnitLength()
	{
		return oneUnitLength;	
	}
	
	static public int GetFontSizeBasedOnScreenSize(int i_fontSize)
	{
		return (int)(i_fontSize * fontSizeRatio);
	}

	static public bool DrawButton(int i_startGridX, int i_startGridY, int i_gridWidth, int i_gridHeight, Texture2D i_textureActive, Action<int, int, int, int> i_action)
	{
		StartScreenStyle.normal.background = i_textureActive;
		StartScreenStyle.active.background = i_textureActive;
		StartScreenStyle.hover.background = i_textureActive;
		//GUI.skin.button.onActive.background = i_textureActive;
		//GUI.skin.button.onHover.background = i_textureActive;
		Rect thisRect = new Rect(startX + i_startGridX * oneUnitLength, startY + i_startGridY * oneUnitLength, i_gridWidth * oneUnitLength, i_gridHeight * oneUnitLength);


		// [UI_Beep]
		if(GUI.Button(thisRect, GUIContent.none, StartScreenStyle))
		{
			if(i_action!= null && thisRect.Contains(Event.current.mousePosition))
			{
				i_action(i_startGridX, i_startGridY, i_gridWidth, i_gridHeight);
			}
			soundMan.soundMgr.playOneShotOnSource(null,"UI_Beep",GameManager.Manager.PlayerType,GameManager.Manager.PlayerType);
			return true;
		}
		else
		{
			if(i_action!= null && thisRect.Contains(Event.current.mousePosition))
			{
				i_action(i_startGridX, i_startGridY, i_gridWidth, i_gridHeight);
			}
			return false;
		}


	}
	
	static public bool DrawButton(int i_startGridX, int i_startGridY, int i_gridWidth, int i_gridHeight, Texture2D i_textureActive)
	{
		StartScreenStyle.normal.background = i_textureActive;
		StartScreenStyle.active.background = i_textureActive;
		StartScreenStyle.hover.background = i_textureActive;
		//GUI.skin.button.onActive.background = i_textureActive;
		//GUI.skin.button.onHover.background = i_textureActive;
		
		// [UI_Beep]
		if(GUI.Button(new Rect(startX + i_startGridX * oneUnitLength, startY + i_startGridY * oneUnitLength, i_gridWidth * oneUnitLength, i_gridHeight * oneUnitLength), GUIContent.none, StartScreenStyle))
		{
			soundMan.soundMgr.playOneShotOnSource(null,"UI_Beep",GameManager.Manager.PlayerType,GameManager.Manager.PlayerType);
			return true;
		}
		else
			return false;
	}

	static public bool DrawButton(float i_startGridX, float i_startGridY, float i_gridWidth, float i_gridHeight, Texture2D i_textureActive)
	{
		StartScreenStyle.normal.background = i_textureActive;
		StartScreenStyle.active.background = i_textureActive;
		StartScreenStyle.hover.background = i_textureActive;
		//GUI.skin.button.onActive.background = i_textureActive;
		//GUI.skin.button.onHover.background = i_textureActive;
		
		// [UI_Beep]
		if(GUI.Button(new Rect(startX + i_startGridX * oneUnitLength, startY + i_startGridY * oneUnitLength, i_gridWidth * oneUnitLength, i_gridHeight * oneUnitLength), GUIContent.none, StartScreenStyle))
		{
			soundMan.soundMgr.playOneShotOnSource(null,"UI_Beep",GameManager.Manager.PlayerType,GameManager.Manager.PlayerType);
			return true;
		}
		else
			return false;
	}

	static public void DrawText(float i_startGridX, float i_startGridY, float i_gridWidth, float i_gridHeight, string i_string, int i_fontSize, Color i_color)
	{
		StartScreenStyle.normal.background = null;
		StartScreenStyle.normal.textColor = i_color;
		StartScreenStyle.fontSize = (int)(i_fontSize * fontSizeRatio);
		StartScreenStyle.font = currentFont;
		StartScreenStyle.alignment = TextAnchor.UpperCenter;
		//StartScreenStyle.font.material.color = Gray;
		GUI.Label(new Rect(startX + i_startGridX * oneUnitLength, startY + i_startGridY * oneUnitLength, i_gridWidth * oneUnitLength, i_gridHeight * oneUnitLength),
		          i_string, StartScreenStyle);		
	}	

	static public void DrawText(int i_startGridX, int i_startGridY, int i_gridWidth, int i_gridHeight, string i_string, int i_fontSize, Color i_color)
	{
		StartScreenStyle.normal.background = null;
		StartScreenStyle.normal.textColor = i_color;
		StartScreenStyle.fontSize = (int)(i_fontSize * fontSizeRatio);
		StartScreenStyle.font = currentFont;
		StartScreenStyle.alignment = TextAnchor.UpperCenter;
		//StartScreenStyle.font.material.color = Gray;
		GUI.Label(new Rect(startX + i_startGridX * oneUnitLength, startY + i_startGridY * oneUnitLength, i_gridWidth * oneUnitLength, i_gridHeight * oneUnitLength),
			i_string, StartScreenStyle);		
	}
	
	static public void DrawText(int i_startGridX, int i_startGridY, int i_gridWidth, int i_gridHeight, string i_string, int i_fontSize, Color i_color, TextAnchor i_alignment, bool i_wordWarp)
	{
		StartScreenStyle.normal.background = null;
		StartScreenStyle.normal.textColor = i_color;
		StartScreenStyle.fontSize = (int)(i_fontSize * fontSizeRatio);
		StartScreenStyle.font = currentFont;
		StartScreenStyle.alignment = i_alignment;
		StartScreenStyle.wordWrap = i_wordWarp;
		//StartScreenStyle.font.material.color = Gray;
		GUI.Label(new Rect(startX + i_startGridX * oneUnitLength, startY + i_startGridY * oneUnitLength, i_gridWidth * oneUnitLength, i_gridHeight * oneUnitLength),
			i_string, StartScreenStyle);		
	}
	
	static public void DrawGreenTitleText(int i_startGridX, int i_startGridY, int i_gridWidth, int i_gridHeight, string i_string)
	{
		StartScreenStyle.normal.background = null;
		StartScreenStyle.normal.textColor = Blue;
		StartScreenStyle.fontSize = (int)(30 * fontSizeRatio);
		StartScreenStyle.font = currentFont;
		StartScreenStyle.alignment = TextAnchor.MiddleLeft;
		StartScreenStyle.wordWrap = false;
		//StartScreenStyle.font.material.color = Gray;
		GUI.Label(new Rect(startX + i_startGridX * oneUnitLength, startY + i_startGridY * oneUnitLength, i_gridWidth * oneUnitLength, i_gridHeight * oneUnitLength),
			i_string, StartScreenStyle);
	}
	
	static public void DrawGameNameText(int i_startGridX, int i_startGridY, int i_gridWidth, int i_gridHeight, string i_string)
	{
		StartScreenStyle.normal.background = null;
		StartScreenStyle.normal.textColor = DarkBlue;
		StartScreenStyle.fontSize = (int)(20 * fontSizeRatio);
		StartScreenStyle.font = currentFont;
		StartScreenStyle.alignment = TextAnchor.LowerLeft;
		StartScreenStyle.wordWrap = false;
		//StartScreenStyle.font.material.color = Gray;
		GUI.Label(new Rect(startX + i_startGridX * oneUnitLength, startY + i_startGridY * oneUnitLength, i_gridWidth * oneUnitLength, i_gridHeight * oneUnitLength),
			i_string, StartScreenStyle);
	}
	
	static public void DrawGameListText(int i_startGridX, int i_startGridY, int i_gridWidth, int i_gridHeight, string i_string)
	{
		StartScreenStyle.normal.background = null;
		StartScreenStyle.normal.textColor = Color.green;
		StartScreenStyle.fontSize = (int)(20 * fontSizeRatio);
		StartScreenStyle.font = currentFont;
		StartScreenStyle.alignment = TextAnchor.LowerLeft;
		StartScreenStyle.wordWrap = false;
		//StartScreenStyle.font.material.color = Gray;
		GUI.Label(new Rect(startX + i_startGridX * oneUnitLength, startY + i_startGridY * oneUnitLength, i_gridWidth * oneUnitLength, i_gridHeight * oneUnitLength),
			i_string, StartScreenStyle);
	}	
	
	static public void DrawGrayText(int i_startGridX, int i_startGridY, int i_gridWidth, int i_gridHeight, string i_string, int i_fontSize)
	{
		StartScreenStyle.normal.background = null;
		StartScreenStyle.normal.textColor = Gray;
		StartScreenStyle.fontSize = (int)(i_fontSize * fontSizeRatio);
		StartScreenStyle.font = currentFont;
		StartScreenStyle.alignment = TextAnchor.UpperLeft;
		StartScreenStyle.wordWrap = true;
		//StartScreenStyle.font.material.color = Gray;
		GUI.Label(new Rect(startX + i_startGridX * oneUnitLength, startY + i_startGridY * oneUnitLength, i_gridWidth * oneUnitLength, i_gridHeight * oneUnitLength),
			i_string, StartScreenStyle);
	}
	
	static public void DrawBlueText(int i_startGridX, int i_startGridY, int i_gridWidth, int i_gridHeight, string i_string, int i_fontSize)
	{
		StartScreenStyle.normal.background = null;
		StartScreenStyle.fontSize = (int)(i_fontSize * fontSizeRatio);
		StartScreenStyle.font = currentFont;
		StartScreenStyle.normal.textColor = Blue;
		//StartScreenStyle.font.material.color = Blue;
		GUI.Label(new Rect(startX + i_startGridX * oneUnitLength, startY + i_startGridY * oneUnitLength, i_gridWidth * oneUnitLength, i_gridHeight * oneUnitLength),
			i_string, StartScreenStyle);
	}

	static public string DrawTextFieldForChat( int i_startGridX, float i_startGridY, int i_gridWidth, float i_gridHeight, string i_string, int i_fontSize, GUISkin i_skin)
	{
		GUI.skin = i_skin;
		return GUI.TextField( new Rect( startX + i_startGridX * oneUnitLength, startY + i_startGridY * oneUnitLength, i_gridWidth * oneUnitLength, i_gridHeight * oneUnitLength ),
			i_string );
	}

	static public void DrawTextBoxForChat( int i_startGridX, int i_startGridY, int i_gridWidth, float i_gridHeight, string i_string, int i_fontSize, GUISkin i_skin)
	{
		GUI.skin = i_skin;
		GUI.Box( new Rect( startX + i_startGridX * oneUnitLength, startY + i_startGridY * oneUnitLength, i_gridWidth * oneUnitLength, i_gridHeight * oneUnitLength ),
		                     i_string );
	}

	static public string DrawTextField( int i_startGridX, int i_startGridY, int i_gridWidth, int i_gridHeight, string i_string, int i_fontSize )
	{
		StartScreenStyle.normal.background = null;
		StartScreenStyle.active.background = null;
		StartScreenStyle.hover.background = null;
		StartScreenStyle.fontSize = (int)(i_fontSize * fontSizeRatio);
		StartScreenStyle.font = currentFont;
		StartScreenStyle.normal.textColor = Green;
		StartScreenStyle.alignment = TextAnchor.MiddleCenter;
		return GUI.TextField( new Rect( startX + i_startGridX * oneUnitLength, startY + i_startGridY * oneUnitLength, i_gridWidth * oneUnitLength, i_gridHeight * oneUnitLength ),
		                     i_string, StartScreenStyle );
	}
}
