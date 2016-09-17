using UnityEngine;
using System.Collections;
using System.IO;
using System;

public class LevelTransition {

	public static LevelDescription LoadLevelConfig(bool i_transition = false, bool i_startScreen = false)
	{
		//Debug.Log("Level Config Info: In transition: " + i_transition + " In StartScreen: " + i_startScreen);
		TextAsset levelText = (TextAsset) Resources.Load("Levels/LevelConfig");
		bool findIt = false;
		LevelDescription targetLevel = new LevelDescription();

		using (TextReader levelReader = new StringReader((string)levelText.text))
		{
			int index = 0;
			while(levelReader.Peek() >= 0)
			{
				// ---- Level Data - Max - 10/18/13
				// - 0.Chapter Number
				// - 1.Scene File Name
				// - 2.Level Name
				// - 3.Path for thumbnail image
				// - 4.Path for detail image
				// - 5.Level Description
				// - 6.Estimated Time
				// - 7.Start Transmitters
				// - 8.Difficulty
				
				string[] levelData = levelReader.ReadLine().Split("#".ToCharArray());
				LevelDescription thisLevel = new LevelDescription();
				thisLevel.Chapter = Convert.ToInt32(levelData[0]);
				thisLevel.SceneFile = levelData[1];
				thisLevel.LevelName = levelData[2];
				thisLevel.LevelThumbnail = Resources.Load(levelData[3], typeof(Texture2D)) as Texture2D;
				thisLevel.LevelDetail = Resources.Load(levelData[4], typeof(Texture2D)) as Texture2D;
				thisLevel.Description = levelData[5];
				thisLevel.EstimatedTime = levelData[6];
				thisLevel.TransmitterNumber = Convert.ToInt32(levelData[7]);
				thisLevel.Difficulty = levelData[8];
				thisLevel.Index = index;


				if(findIt == true)
				{
					if(!i_startScreen)
					{
						targetLevel = thisLevel;
						GameManager.Manager.NextLevelName = targetLevel.SceneFile;
						//Debug.Log("Level Config Info: Loaded Level Name(Transition): " + targetLevel.SceneFile);
					}
					break;
				}

				index++;



				if(thisLevel.SceneFile.CompareTo(GameManager.Manager.CurrentLevelName) == 0)
				{
					findIt = true;
					if(i_startScreen)
					{
						//Debug.Log("Level Config Info: Loaded Level Name(StartScreen): " + GameManager.Manager.CurrentLevelName);
						targetLevel = thisLevel;
						GameManager.Manager.NextLevelName = targetLevel.SceneFile;
						break;
					}

					//Debug.Log("#Max:Level name read : " + GameManager.Manager.CurrentLevelName);
				}

			}
			
			levelReader.Close();			
		}

		if(targetLevel.SceneFile != "" && i_transition == false)
		{
			GameManager.Manager.NextLevelName = targetLevel.SceneFile;
			GameManager.Manager.CurrentLevelTexture = targetLevel.LevelDetail;
			NetworkManager.Manager.SetupLevelInfoInternally(targetLevel.LevelName, 
			                                                targetLevel.TransmitterNumber.ToString(), 
			                                                targetLevel.EstimatedTime, 
			                                                targetLevel.Difficulty, 
			                                                targetLevel.Description,
			                                                targetLevel.LevelDetail,
			                                                GameManager.Manager.PlayerType);
		}

		/**** Texture ****/

		return targetLevel;

	}

	public static void Show()
	{
		NetworkManager.Manager.PlayMovie("LevelLoadMenu5_New.swf");
	}

}
