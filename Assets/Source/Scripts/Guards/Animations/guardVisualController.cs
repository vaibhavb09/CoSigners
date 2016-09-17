using UnityEngine;
using System.Collections;

public class guardVisualController 
{

	private static Material sPatrollingMaterial_Blue;
	private static Material sSeenMaterial_Orange;
	private static Material sAlertMaterial_Red;

	public static void initializeGuardVisuals()
	{
		sPatrollingMaterial_Blue = UnityEngine.Resources.Load ("Materials/Thief/GlowMesh_Blue", typeof(Material)) as Material;
		sSeenMaterial_Orange = UnityEngine.Resources.Load ("Materials/Thief/GlowMesh_Orange", typeof(Material)) as Material;
		sAlertMaterial_Red = UnityEngine.Resources.Load ("Materials/Thief/GlowingTexture", typeof(Material)) as Material;
	}


	MeshRenderer mGuardVisorRenderer;
	//ArrayList<MeshRenderer> mAllGuardVisibleAreas;

	public void switchToPatrolling()
	{

	}

	public void switchToSearching()
	{
		
	}

	public void switchToAlert()
	{
		
	}


}

