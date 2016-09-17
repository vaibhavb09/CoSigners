using UnityEngine;
using System.Collections;

public struct LevelDescription {
	
	public Texture2D LevelThumbnail;
	public Texture2D LevelDetail;
	public string Description;
	public string EstimatedTime;
	public string Difficulty;
	public string SceneFile;
	public string LevelName;
	public int TransmitterNumber;
	public int Chapter;
	public int Index;
	
}

public struct LevelDescriptionForEditor {
	
	public string LevelThumbnail;
	public string LevelDetail;
	public string Description;
	public string EstimatedTime;
	public string Difficulty;
	public string SceneFile;
	public string LevelName;
	public int TransmitterNumber;
	public int Chapter;
	public int Index;
	
}
