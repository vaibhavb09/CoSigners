using UnityEngine;
using System.Collections;

public enum WallStyle{
	NONE,
	TEMPLATE,	// This is the default template style for all walls, this is only used for debug
	BLUE_BAR, 	// This is the initial IGF wall style.
	SLOTTED_WALL, // this is a brushed steel wall with horizontal dark grey slots.
	HAPPY_BUNNY // Style with brushed steel and on horizontal bar with occational vertical bar.
}

[ExecuteInEditMode]
public class WallStyleSelect : MonoBehaviour {

	// Add Style Variabls
	public bool ApplyStyles;
	public bool Style_00;
	public bool Style_BB;
	public bool Style_SW;
	public bool Style_HB;

	// Use this for initialization
	void Start () {

	}
	
	// Update is called once per frame
	void Update () {
	}
	
}
