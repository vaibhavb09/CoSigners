using UnityEngine;
using System.Collections;
using System.Xml;
using System.Xml.Serialization;
using System.IO;

[ExecuteInEditMode]
public class OSAppear : MonoBehaviour 
{
	public bool DrawOverrideSwitches = false; 
	public string SceneFileName;

	OverrideData[] overrideData;
	GlobalData[] globalData;
	
	void Start () 
	{
	}

	void Update () 
	{
		if( DrawOverrideSwitches == true )
		{
			LoadOSData(SceneFileName);
			
			GameObject overrideNode = (GameObject)Resources.Load("Prefabs/Hacker/Graph/Override_Prefab");
			GameObject _overridePlatformPrefab = (GameObject)Resources.Load("Prefabs/Theif/OverridePlatform");
			
			foreach( OverrideData overrideHex in overrideData )
			{
				GameObject tempOverride = (GameObject)Instantiate( overrideNode, GetCoordHex(overrideHex.HexIndex, globalData, 0.01f), Quaternion.identity);
				GameObject tempOverridePlatform = (GameObject)Instantiate( _overridePlatformPrefab, GetCoordHex(overrideHex.HexIndex, globalData, 0.84f), Quaternion.identity);
			}
			
			DrawOverrideSwitches = false;
		}
	}

	void LoadOSData(string i_level)
	{
		XmlSerializer serializer = new XmlSerializer(typeof(GraphData));

		serializer.UnknownNode += new XmlNodeEventHandler(serializer_UnknownNode);

		serializer.UnknownAttribute += new XmlAttributeEventHandler(serializer_UnknownAttribute);
		
		TextAsset txt = (TextAsset) Resources.Load("Levels/" + i_level + "/" + i_level + "_CONFIG");	

		string str = (string) txt.text;

		TextReader tr = new StringReader(str);

		GraphData gData;
		gData = (GraphData) serializer.Deserialize(tr);

		overrideData = gData.Overrides;

		globalData = gData.Globals;
	}

	void serializer_UnknownNode(object sender, XmlNodeEventArgs e)
	{
		//Debug.Log("Unknown Node:" +   e.Name + "\t" + e.Text);
	}
	
	void serializer_UnknownAttribute(object sender, XmlAttributeEventArgs e)
	{
		System.Xml.XmlAttribute attr = e.Attr;
		//Debug.Log("Unknown attribute " + attr.Name + "='" + attr.Value + "'");
	}

	public Vector3 GetCoordHex( int i, GlobalData[] gData, float Yval=0)
	{
		int rowSize = gData[0].LevelWidth;

		int col = (int)(i%rowSize);
		int row = (int)(i/rowSize);
		
		// Calculate x coordinate
		float x = col*3;
		x += (row%2 == 0)? (( col%2 == 0 )? 0 : 1) : ( ( col%2 == 0 )? 1 : 0 );
		x *= -(1 * 0.5f);
		x += 0;
		x -= 1;
		
		// Calculate y coordinate
		float y = (row*1*0.866f);
		y += 0;
		
		return new Vector3 (x,Yval,y);
	}
}
