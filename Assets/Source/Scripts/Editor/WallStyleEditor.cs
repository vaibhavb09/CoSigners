using UnityEngine;
using System.Collections;
using UnityEditor;


[CustomEditor(typeof(WallStyleSelect))]
[CanEditMultipleObjects]
public class WallStyleEditor : Editor {
	
	#region MEMBER VARIABLES ------------------
	// STYLE: Add style member variables.
	private SerializedProperty ApplyProp, TEMPLATE_Prop, BLUE_BAR_Prop, SW_Prop, HB_Prop;
	private bool initialSet, value, TEMPLATE_Value, BLUE_BAR_Value, SW_Value, HB_Value;

	private WallStyle oldActive, newActive;

	#endregion --------------------------------
	
	
	void OnEnable() {
		// STYLE: Setup serialized property
		ApplyProp = serializedObject.FindProperty("ApplyStyles");
		TEMPLATE_Prop = serializedObject.FindProperty("Style_00");
		BLUE_BAR_Prop = serializedObject.FindProperty("Style_BB");
		SW_Prop = serializedObject.FindProperty("Style_SW");
		HB_Prop = serializedObject.FindProperty("Style_HB");
		
		// Initial Variables state
		initialSet = false;
		value = false;

		// STYLE: Set initial values.
		TEMPLATE_Value = false;
		BLUE_BAR_Value = false;
		SW_Value = false;
		HB_Value = false;

	}
	
	
	public override void OnInspectorGUI() 
	{
		// Display Editor Fields
		serializedObject.Update();
		value = EditorGUILayout.Toggle("APPLY STYLE", value, new GUILayoutOption[0] );

		// STYLE: Add Wall Style Toggles
		TEMPLATE_Value = EditorGUILayout.Toggle("Tempalte", TEMPLATE_Value, new GUILayoutOption[0] );
		BLUE_BAR_Value = EditorGUILayout.Toggle("Blue Bar", BLUE_BAR_Value, new GUILayoutOption[0] );
		SW_Value = EditorGUILayout.Toggle("Slotted Wall", SW_Value, new GUILayoutOption[0] );
		HB_Value = EditorGUILayout.Toggle("Happy Bunny", HB_Value, new GUILayoutOption[0] );

		
		// On change
		if (GUI.changed )
		{
			// Set changed property values
			ApplyProp.boolValue = value;

			// STYLE: Set Wall style values
			TEMPLATE_Prop.boolValue = TEMPLATE_Value;
			BLUE_BAR_Prop.boolValue = BLUE_BAR_Value;
			SW_Prop.boolValue = SW_Value;
			SW_Prop.boolValue = HB_Value;

			serializedObject.ApplyModifiedProperties();

			if ( initialSet && !value)
			{
				SetRadioButtons ();
				
			}
			else
			{
				newActive = GetActiveStyle();
				initialSet = true;
			}

			// If Build Scene is selected then run the scene building algorithm.
			if ( value && newActive != WallStyle.NONE)
			{
				// Run the Style Editor
				Debug.Log ("RAN: ");

				Debug.Log( " Active style is: " + newActive );

				//GameObject thisObj = Selection.activeGameObject;
				GameObject[] theseObjs = Selection.gameObjects;

				foreach ( GameObject gObj in theseObjs )
				{
					foreach (Transform child in gObj.transform)
					{
						GameObject childObj = child.gameObject;
						Debug.Log ("    " + childObj.name);
						
						foreach ( Transform style in child )
						{
							GameObject styleObj = style.gameObject;
							WallStyle thisStyle = GetStyle(styleObj.name);
							Debug.Log ("        " + thisStyle);
							if ( newActive == thisStyle )
							{
								styleObj.SetActive( true );
							}
							else
							{
								styleObj.SetActive( false );
							}
							
						}
					}
				}

				value = false;
				ApplyProp.boolValue = value;
			}

			serializedObject.ApplyModifiedProperties();
		}

		oldActive = newActive;
	}


	void SetRadioButtons()
	{
		Debug.Log("SET RADIO BUTTONS");
		if ( GetActiveStyle() == WallStyle.NONE ) // if there is no active style selected.
		{
			SetPropertyForStyle( oldActive, true); // set the old active again.
		}
		else
		{
			SetPropertyForStyle( oldActive, false );
			newActive = GetActiveStyle();
			Debug.Log("CURRENT ACTIVE IS: " + newActive);
		}

	}


	void SetPropertyForStyle( WallStyle style, bool i_value)
	{
		Debug.Log("Settign Property - " + style);

		// STYLE: 
		if ( style == WallStyle.TEMPLATE )
		{
			TEMPLATE_Value = i_value;
			TEMPLATE_Prop.boolValue = TEMPLATE_Value;
		}
		else if ( style == WallStyle.BLUE_BAR )
		{
			BLUE_BAR_Value = i_value;
			BLUE_BAR_Prop.boolValue = BLUE_BAR_Value;
		}
		else if ( style == WallStyle.SLOTTED_WALL )
		{
			SW_Value = i_value;
			SW_Prop.boolValue = SW_Value;
		}
		else if ( style == WallStyle.HAPPY_BUNNY )
		{
			HB_Value = i_value;
			HB_Prop.boolValue = HB_Value;
		}

		serializedObject.ApplyModifiedProperties();

	}


	bool GetValueForStyle(WallStyle style)
	{
		// STYLE:
		if ( style == WallStyle.TEMPLATE )
			return TEMPLATE_Value;
		else if (  style == WallStyle.BLUE_BAR )
			return BLUE_BAR_Value;
		else if (  style == WallStyle.SLOTTED_WALL )
			return SW_Value;
		else if (  style == WallStyle.HAPPY_BUNNY )
			return HB_Value;
		else 
			return false;
	}


	WallStyle GetStyle(string name)
	{
		WallStyle thisStyle;
		string suffix = name.Substring( name.Length-2 );

		// STYLE:
		if ( suffix.Equals ("00") )
			thisStyle = WallStyle.TEMPLATE;
		else if ( suffix.Equals ("BB") )
			thisStyle = WallStyle.BLUE_BAR;
		else if ( suffix.Equals ("SW") )
			thisStyle = WallStyle.SLOTTED_WALL;
		else if ( suffix.Equals ("HB") )
			thisStyle = WallStyle.HAPPY_BUNNY;
		else
			thisStyle = WallStyle.TEMPLATE;
		
		return thisStyle;
	}


	WallStyle GetActiveStyle()
	{
		// STYLE:
		if ( TEMPLATE_Value )
			return WallStyle.TEMPLATE;
		else if ( BLUE_BAR_Value )
			return WallStyle.BLUE_BAR;
		else if ( SW_Value )
			return WallStyle.SLOTTED_WALL;
		else if ( HB_Value )
			return WallStyle.HAPPY_BUNNY;
		else
			return WallStyle.NONE;
	}
}
