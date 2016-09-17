/**********************************************************************

Filename    :	SFCamera_Imports.cs
Content     :	
Created     :   
Authors     :   Ankur Mohan

Copyright   :   Copyright 2013 Autodesk, Inc. All Rights reserved.

Use of this software is subject to the terms of the Autodesk license
agreement provided at the time of installation or download, or which
otherwise accompanies this software in either electronic or hard copy form.
 
***********************************************************************/

using UnityEngine;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.Runtime.InteropServices;


// For a class or pointer to class to be passed to unmanaged code, it must have
// StructLayout Attribute.
[StructLayout(LayoutKind.Sequential)]
public partial class SFCamera : MonoBehaviour
{
#if (UNITY_STANDALONE || UNITY_EDITOR || UNITY_METRO) && !UNITY_WP8
	[DllImport("libgfxunity3d")]
	public static extern void SF_SetKey(String key);

	[DllImport("libgfxunity3d")]
	public static extern void SF_SetTextureCount(int textureCount);

	[DllImport("libgfxunity3d")]
	private static extern void SF_Uninit();

#elif UNITY_IPHONE
	[DllImport("__Internal")]
	public static extern void SF_SetKey(String key);

	[DllImport("__Internal")]
	public static extern void SF_SetTextureCount(int textureCount);

	[DllImport("__Internal")]
	private static extern void SF_Uninit();

	[DllImport("__Internal")]
	public static extern void UnityRenderEvent(int id);
#elif UNITY_ANDROID
	[DllImport("gfxunity3d")]
	public static extern void SF_SetKey(String key);

	[DllImport("gfxunity3d")]
	public static extern void SF_SetTextureCount(int textureCount);

	[DllImport("gfxunity3d")]
	private static extern void SF_Uninit();

	[DllImport("gfxunity3d")]
	public static extern void UnityRenderEvent(int id);
#elif UNITY_WP8
	public delegate void SF_SetKey(String key);
	public static SF_SetKey sf_setKey;
	public void SetSF_SetKey(SF_SetKey func){sf_setKey = func;}

	public delegate void SF_SetTextureCount(int textureCount);
	public static SF_SetTextureCount sf_setTextureCount;
	public void SetSF_SetTextureCount(SF_SetTextureCount func){sf_setTextureCount = func;}

	public delegate void SF_Uninit();
	public static SF_Uninit sf_uninit;
	public void SetSF_Uninit(SF_Uninit func){sf_uninit = func;}

#endif
}

