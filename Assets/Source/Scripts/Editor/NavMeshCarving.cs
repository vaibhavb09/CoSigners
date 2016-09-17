using UnityEngine;
using System.Collections;
using System.Xml;
using System.Xml.Serialization;
using System.IO;

public class NavMeshCarving : MonoBehaviour 
{
	static GlobalData[] globalData;
	static InfoData[] infoData;
	static BonusPasswordData[] bonusPasswordData;
	static JammerData[] jammerData;

	void Start () 
	{
	
	}

	static void serializer_UnknownNode(object sender, XmlNodeEventArgs e)
	{
		Debug.Log("Unknown Node:" +   e.Name + "\t" + e.Text);
	}
	
	static void serializer_UnknownAttribute(object sender, XmlAttributeEventArgs e)
	{
		System.Xml.XmlAttribute attr = e.Attr;
		Debug.Log("Unknown attribute " + attr.Name + "='" + attr.Value + "'");
	}

	static void LoadData(string i_level)
	{
		XmlSerializer serializer = new XmlSerializer(typeof(GraphData));
		
		serializer.UnknownNode += new XmlNodeEventHandler(serializer_UnknownNode);
		
		serializer.UnknownAttribute += new XmlAttributeEventHandler(serializer_UnknownAttribute);
		
		TextAsset txt = (TextAsset) Resources.Load("Levels/" + i_level + "/" + i_level + "_CONFIG");	
		
		string str = (string) txt.text;
		
		TextReader tr = new StringReader(str);
		
		GraphData gData;
		gData = (GraphData) serializer.Deserialize(tr);

		globalData = gData.Globals;

		infoData = gData.Infos;

		bonusPasswordData = gData.BonusPasswords;

		jammerData = gData.Jammers;
	}

	public static void InstantiateNavMeshCarvings(string sceneFileName)
	{
		GameObject navCraveObj = (GameObject)Resources.Load("Prefabs/Theif/NavMeshCarve");

		GameObject newEmptyObject = new GameObject("LEVEL_CARVINGS");

		LoadData(sceneFileName);

		foreach( InfoData info in infoData )
		{
			GameObject tempCarveObj = (GameObject)Instantiate( navCraveObj, GetCoordHex(info.InfoHexIndex, globalData, 0.01f), Quaternion.identity);
			tempCarveObj.transform.parent = newEmptyObject.transform;
		}

		foreach( BonusPasswordData password in bonusPasswordData )
		{
			GameObject tempCarveObj = (GameObject)Instantiate( navCraveObj, GetCoordHex(password.HexIndex, globalData, 0.01f), Quaternion.identity);
			tempCarveObj.transform.parent = newEmptyObject.transform;
		}

		foreach( JammerData jammer in jammerData )
		{
			GameObject tempCarveObj = (GameObject)Instantiate( navCraveObj, GetCoordHex(jammer.HexIndex, globalData, 0.01f), Quaternion.identity);
			tempCarveObj.transform.parent = newEmptyObject.transform;
		}
	}

	static Vector3 GetCoordHex( int i, GlobalData[] gData, float Yval=0)
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
