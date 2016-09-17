/**********************************************************************

Filename    :   SFCamera.cs
Content     :   Creates ScaleformManager, Hooks into Unity event handling system
Created     :
Authors     :   Ankur Mohan

Copyright   :   Copyright 2013 Autodesk, Inc. All Rights reserved.

Use of this software is subject to the terms of the Autodesk license
agreement provided at the time of installation or download, or which
otherwise accompanies this software in either electronic or hard copy form.

***********************************************************************/
using System;
using UnityEngine;
using System.Collections;
using System.Runtime.InteropServices;
using System.Diagnostics;
using System.IO;
using System.Threading;

using Scaleform;

/// <summary>
/// The SFCamera class performs Scaleform initialization and hooks into the Unity event system and passes mouse/key/touch events to Scaleform runtime. 
/// </summary>
public partial class SFCamera : MonoBehaviour
{
    public enum RenderTime
    {
        EndOfFrame = 0,
        PreCamera,
        PostCamera
    };

    /// <summary>
    /// When to trigger Scaleform rendering during the frame. 
    /// Scaleform rendering can be performed at the end of the frame (most common use case), or before/after a certain camera. This can be used to render Scaleform before/after a particular object in the	scene. 
    /// </summary>
    public RenderTime WhenToRender = RenderTime.EndOfFrame;

    /// <summary>
    /// True if the SFCamera has been initialized; false otherwise.
    /// </summary>
    public bool Initialized = false;

    /// <summary>
    /// Reference to the SFManager that manages all SFMovies.
    /// </summary>
    protected static SFManager SFMgr;

    /// <summary>
    /// Reference to SFGamepad. This class defines mapping between Unity Gamepad buttons (for example "Fire") and Keystrokes.
    /// </summary>
    protected SFGamepad GamePad;

    /// <summary>
    /// Indicates whether a gamepad is connected or not.
    /// </summary>
    protected bool      GamepadConnected;

    /// <summary>
    /// Scaleform Initialization Parameters.
    /// </summary>   
    /* Scaleform Initialization Parameters.
     * AS VM: AS2/AS3/Both.
     * Initialize Video/Sound subsystems- note that video support will have to be purchased separately.
     * Initialize IME: IME (Input Method Editor) support is currently not available with the Unity Integration.
     * Progressive Loading: Enables loading SWF files on a background thread, so that large files can be loaded without interrupting your application.
     * FontCache Configuration parameters: Dimensions and number of texture used to implement dynamic font cache. Refer to the font config doc for more info about this topic.
     * FontPack Params: Refer to font config doc for more info.
     * Sound Volume
     * ImageFormats: Allows you to specify which image loaders are instantiated. See reference doc for the ImageFormat structure.
     */
	public SFInitParams InitParams;

	/// <summary>
    /// The mouse position during the last onGUI() call. Used for MouseMove tracking.
	/// </summary>
	private Vector2 LastMousePos;

	public void Awake()
    {
    }

    /// <summary>
    /// During the Start function, the Scaleform Manager and internal Scaleform runtime is initialized, Gamepad is created and initialized.
    /// </summary>
    /// <returns></returns>
	public IEnumerator Start()
	{
		DontDestroyOnLoad(this.gameObject);
		SFMgr = new SFManager(InitParams);
		if (SFMgr.IsSFInitialized())
		{
			GamePad = new SFGamepad(SFMgr);
			GamePad.Init();
			//SFMgr.InstallDelegates();
			InitParams.Print();
			SFMgr.SetNewViewport(0, 0, Screen.width, Screen.height);
#if UNITY_STANDALONE || UNITY_EDITOR || UNITY_METRO
			GL.IssuePluginEvent(0);
#elif UNITY_IPHONE  || UNITY_ANDROID
            UnityRenderEvent(0);
#endif
			GL.InvalidateState();

			// Figure out if gamepad is connected.
			GamepadConnected = false;
#if !UNITY_WP8
			if (Input.GetJoystickNames().Length != 0)
			{
				GamepadConnected = true;
			}
#endif
			if (WhenToRender == RenderTime.EndOfFrame)
			{
				yield return StartCoroutine("CallPluginAtEndOfFrame");
			}
		}
	}


    /// <summary>
    /// Issues a call to perform Scaleform rendering. Rendering is multi-threaded on Windows and single threaded on iOS/Android.
    /// </summary>
    private void PumpPluginRender()
    {
#if UNITY_STANDALONE || UNITY_EDITOR || UNITY_METRO
        GL.IssuePluginEvent(1);
#elif UNITY_IPHONE  || UNITY_ANDROID
        UnityRenderEvent(1);
#endif
    }

    /// <summary>
    /// Used with EndOfFrame render layers, pumps Scaleform once per frame at the end of the frame.
    /// </summary>
    /// <returns></returns>
    protected IEnumerator CallPluginAtEndOfFrame()
    {
        while (true)
        {
            yield return new WaitForEndOfFrame();
            PumpPluginRender();
        }
    }

    /// <summary>
    /// Used with PreCamera render layers, pumps Scaleform once prior to the rendering the camera to which the Scaleform instance is attached. Can be used to render Scaleform before a particular camera renders.
    /// </summary>
    public void OnPreRender()
    {
        if (WhenToRender != RenderTime.PreCamera)
        {
            return;
        }
        PumpPluginRender();
    }

    /// <summary>
    /// Used with PostCamera render layers, pumps Scaleform once after rendering the camera to which the Scaleform instance is attached. Can be used to render Scaleform after a particular camera renders.
    /// </summary>
    public void OnPostRender()
    {
        if (WhenToRender != RenderTime.PostCamera)
        {
            return;
        }
        PumpPluginRender();
	}

    /// <summary>
    /// Uninitializes Scaleform runtime
    /// </summary>	
    void OnDestroy()
    {
	UnityEngine.Debug.Log("In OnDestroy");
		SFMgr.DoCleanup();
        // This is used to initiate RenderHALShutdown, which must take place on the render thread.
#if UNITY_STANDALONE || UNITY_EDITOR || UNITY_METRO || UNITY_WP8
        GL.IssuePluginEvent(2);
#endif
#if UNITY_WP8
		sf_uninit();
#else
        SF_Uninit();
#endif
    }

    void OnApplicationQuit()
    {
    }

    /// <summary>
    /// Returns an instance of the Scaleform Manager.
    /// </summary>
    /// <returns>The Scaleform Manager</returns>
    public SFManager GetSFManager()
    {
        return SFMgr;
    }

    /// <summary>
    /// User Input Events such as Mouse/Key/Touch are obtained from Unity and passed to Scaleform in this function.
    /// </summary>
    public virtual void OnGUI()
    {
        if (SFMgr == null) return;


        // Process touch events:
        if (Input.touchCount > 0)
        {
            for (int i = 0; i < Input.touchCount; i++)
            {
                Touch touch = Input.GetTouch(i);
                SFMgr.HandleTouchEvent(touch);
            }
        }

        Vector2 mousePos;

        // Get Time since last frame: keep in mind the following from Unity Doc
        // Note that you should not rely on Time.deltaTime from inside OnGUI since
        // OnGUI can be called multiple times per frame and deltaTime would hold the
        // same value each call, until next frame where it would be updated again.
        // float deltaTime = Time.deltaTime;
        // Check if the mouse moved:

        mousePos = Event.current.mousePosition;
        if ((mousePos[0] != LastMousePos[0]) || (mousePos[1] != LastMousePos[1]))
        {
            SFMgr.HandleMouseMoveEvent(mousePos[0], mousePos[1]);
            LastMousePos[0] = mousePos[0]; LastMousePos[1] = mousePos[1];
        }

        switch (Event.current.type)
        {
            case EventType.MouseMove:
                break;
            case EventType.MouseDown:
                SFMgr.HandleMouseEvent(Event.current);
                break;
            case EventType.MouseUp:
                SFMgr.HandleMouseEvent(Event.current);
                break;
            case EventType.KeyDown:

                if (Event.current.isKey && Event.current.keyCode != KeyCode.None)
                {
                    SFMgr.HandleKeyDownEvent(Event.current);
                }

                if (Event.current.isKey && Event.current.character != 0)
                {
                    SFMgr.HandleCharEvent(Event.current);
                }
                break;
            case EventType.KeyUp:
                if (Event.current.isKey && Event.current.keyCode != KeyCode.None)
                {
                    SFMgr.HandleKeyUpEvent(Event.current);
                }
                break;

            case EventType.Repaint:
                break;
        }
    }

    /// <summary>
    /// In update, we process ExternalInterface callbacks and call Advance on Scaleform movies. Note that ExternalInterface callbacks are processed asynchronously. When a callback arrives, it is put on a queue and processed during Update.
    /// </summary>
    public void Update()
    {
        if (SFMgr != null)
        {
            // Update gamepad only if it's connected, otherwise we might get duplicate key presses for WASD, etc.
            if (GamepadConnected)
            {
                GamePad.Update();
            }
            SFMgr.ProcessCommands();
            SFMgr.Update();
            SFMgr.Advance (Time.deltaTime);
        }
    }

    /// <summary>
    /// Helper function to get the viewport from Unity.
    /// </summary>
    /// <param name="ox">The x coordinate.</param>
    /// <param name="oy">The y coordinate</param>
    /// <param name="width">The width of the viewport.</param>
    /// <param name="height">The height of the viewport.</param>
    public static void GetViewport(ref int ox, ref int oy, ref int width, ref int height)
    {
        width = Screen.width;
        height = Screen.height;
        ox = 0;
        oy = 0;
#if UNITY_EDITOR
        oy = 24;
#endif
    }

    /// <summary>
    /// Helper function to initialize creation parameters for RenderTexture movies. 
    /// </summary>
    /// <param name="swfName">String corresponding to the flash movie that will be rendered to a texture.</param>
    /// <param name="depth">Used to sort multiple, overlapping movies in rendering order, so that movies with a lower depth are rendered first.</param>
    /// <param name="RTToX">The offset for the render texture movie viewport. </param>
    /// <param name="RTToY">The offset for the render texture movie viewport.</param>
    /// <param name="texture">The rendertexture.</param>
    /// <param name="clearColor">Specifies the color used to clear the render texture before rendering the movie. </param>
    /// <returns></returns>
	public static SFMovieCreationParams CreateRTTMovieCreationParams(string swfName, int depth, int RTToX, int RTToY, RenderTexture texture, Color32 clearColor)
    {
        // Used for Android only
        Int32 length = 0;
		IntPtr pDataUnManaged = IntPtr.Zero;
        String SwfPath = SFManager.GetScaleformContentPath() + swfName;

		return new SFMovieCreationParams(SwfPath, depth, RTToX, RTToY, texture.width, texture.height, pDataUnManaged, length, false, texture, clearColor, true, ScaleModeType.SM_ShowAll, true);
    }

    /// <summary>
    /// Helper function to initialize creation parameters for RenderTexture movies.
    /// </summary>
    /// <param name="swfName">String corresponding to the flash movie that will be rendered to a texture.</param>
    /// <param name="depth">Used to sort multiple, overlapping movies in rendering order, so that movies with a lower depth are rendered first.</param>
    /// <returns></returns>

    public static SFMovieCreationParams CreateMovieCreationParams(string swfName, int depth = 1)
    {
        return CreateMovieCreationParams(swfName, depth, new Color32(0, 0, 0, 0), false);
    }

    public static SFMovieCreationParams CreateMovieCreationParams(string swfName, int depth, byte bgAlpha)
    {
        return CreateMovieCreationParams(swfName, depth, new Color32(0, 0, 0, bgAlpha), false);
    }

    /// <summary>
    /// Helper function to initialize creation parameters for overlay movies. 
    /// </summary>
    /// <param name="swfName">String corresponding to the flash movie that will be rendered to a texture.</param>
    /// <param name="depth">Used to sort multiple, overlapping movies in rendering order, so that movies with a lower depth are rendered first. See the layering demo for an example.</param>
    /// <param name="bgColor">Only the alpha part of the background color is used to set the transparency for the swf background. The color part is ignored for now.</param>
    /// <param name="overrideBackgroundColor">Ignored for now.</param>
    /// <returns></returns>
    public static SFMovieCreationParams CreateMovieCreationParams(string swfName, int depth, Color32 bgColor, bool overrideBackgroundColor)
    {

        Int32 length = 0;

        int ox = 0;
        int oy = 0;
        int width = 0;
        int height = 0;

        IntPtr pDataUnManaged = IntPtr.Zero;
        String swfPath = SFManager.GetScaleformContentPath() + swfName;
        GetViewport(ref ox, ref oy, ref width, ref height);
		return new SFMovieCreationParams(swfPath, depth, ox, oy, width, height, pDataUnManaged, length, false, bgColor, overrideBackgroundColor, ScaleModeType.SM_ShowAll, true);
    }

    /// <summary>
    /// Helper function to initialize creation parameters for overlay movies.
    /// </summary>
    /// <param name="swfName">String corresponding to the flash movie that will be rendered to a texture.</param>
    /// <param name="depth">Used to sort multiple, overlapping movies in rendering order, so that movies with a lower depth are rendered first.</param>
    /// <param name="swfBytes">Used to support loading a swfFile from memory. swfBytes is the memory buffer corresponding to the swfFile. 
    /// <code> using (FileStream fileStream = new FileStream("Assets/StreamingAssets/Demo1.swf", FileMode.Open, FileAccess.Read))
    ///	   {
    ///		   using (BinaryReader reader = new BinaryReader(fileStream))
    ///		   {
    ///			   swfBytes = reader.ReadBytes((int)fileStream.Length);
    ///		   }
    ///	   }</code></param>
    /// <param name="bgColor">Only the alpha part of the background color is used to set the transparency for the swf background. The color part is ignored for now.</param>
    /// <param name="overrideBackgroundColor">Ignored for now.</param>
    /// <example>Here is the sample code to create swfBytes:
    /// <code>	   {
    ///		   using (BinaryReader reader = new BinaryReader(fileStream))
    ///		   {
    ///			   swfBytes = reader.ReadBytes((int)fileStream.Length);
    ///		   }
    ///	   }
    ///	   </code>
    ///	   </example>
    /// <returns></returns>
	public SFMovieCreationParams CreateMovieCreationParams(string swfName, int depth, Byte[] swfBytes, Color32 bgColor, bool overrideBackgroundColor)
    {
        int ox = 0;
        int oy = 0;
        int width = 0;
        int height = 0;

        GetViewport(ref ox, ref oy, ref width, ref height);

        Int32 length = 0;
		IntPtr pDataUnManaged = IntPtr.Zero;

		if (swfBytes != null)
			 length = swfBytes.Length;

		if (length > 0)
		{
			pDataUnManaged = new IntPtr();
			pDataUnManaged = Marshal.AllocCoTaskMem((int)length);
			Marshal.Copy(swfBytes, 0, pDataUnManaged, (int)length);
		}

		String swfPath = SFManager.GetScaleformContentPath() + swfName;

		return new SFMovieCreationParams(swfPath, depth, ox, oy, width, height, pDataUnManaged, length, false, bgColor, overrideBackgroundColor, ScaleModeType.SM_ShowAll, true);
    }

    protected static void GetFileInformation(String name, ref long length, ref IntPtr pDataUnManaged)
    {
#if UNITY_ANDROID

		long start;
		Int32 fd;
        IntPtr cls_Activity = (IntPtr)AndroidJNI.FindClass("com/unity3d/player/UnityPlayer");
        IntPtr fid_Activity = AndroidJNI.GetStaticFieldID(cls_Activity, "currentActivity", "Landroid/app/Activity;");
        IntPtr obj_Activity = AndroidJNI.GetStaticObjectField(cls_Activity, fid_Activity);

        IntPtr obj_cls = AndroidJNI.GetObjectClass(obj_Activity);
        IntPtr asset_func = AndroidJNI.GetMethodID(obj_cls, "getAssets", "()Landroid/content/res/AssetManager;");

        IntPtr assetManager = AndroidJNI.CallObjectMethod(obj_Activity, asset_func, new jvalue[2]);

        IntPtr assetManagerClass = AndroidJNI.GetObjectClass(assetManager);
        IntPtr openFd = AndroidJNI.GetMethodID(assetManagerClass, "openFd", "(Ljava/lang/String;)Landroid/content/res/AssetFileDescriptor;");
        jvalue[] param_array2 = new jvalue[2];
        jvalue param = new jvalue();
        param.l = AndroidJNI.NewStringUTF(name);
        param_array2[0] = param;
        IntPtr jfd;
        try
        {
            jfd = AndroidJNI.CallObjectMethod(assetManager, openFd, param_array2);
            IntPtr assetFdClass = AndroidJNI.GetObjectClass(jfd);
            IntPtr getParcelFd = AndroidJNI.GetMethodID(assetFdClass, "getParcelFileDescriptor", "()Landroid/os/ParcelFileDescriptor;");
            IntPtr getStartOffset = AndroidJNI.GetMethodID(assetFdClass, "getStartOffset", "()J");
            IntPtr getLength = AndroidJNI.GetMethodID(assetFdClass, "getLength", "()J");
            start = AndroidJNI.CallLongMethod(jfd, getStartOffset, new jvalue[2]);
            length = AndroidJNI.CallLongMethod(jfd, getLength, new jvalue[2]);

            IntPtr fileInputStreamId = AndroidJNI.GetMethodID(assetFdClass, "createInputStream", "()Ljava/io/FileInputStream;");
            IntPtr fileInputStream = AndroidJNI.CallObjectMethod(jfd, fileInputStreamId, new jvalue[2]);
            IntPtr fileInputStreamClass = AndroidJNI.GetObjectClass(fileInputStream);
            // Method signatures:newbytear B: byte, Z: boolean
            IntPtr read = AndroidJNI.GetMethodID(fileInputStreamClass, "read", "([BII)I");
            jvalue[] param_array = new jvalue[3];
            jvalue param1 = new jvalue();
            IntPtr pData = AndroidJNI.NewByteArray((int)(length));
            param1.l = pData;
            jvalue param2 = new jvalue();
            param2.i = 0;
            jvalue param3 = new jvalue();
            param3.i = (int)(length);
            param_array[0] = param1;
            param_array[1] = param2;
            param_array[2] = param3;
            int numBytesRead = AndroidJNI.CallIntMethod(fileInputStream, read, param_array);
            UnityEngine.Debug.Log("Bytes Read = " + numBytesRead);

            Byte[] pDataManaged = AndroidJNI.FromByteArray(pData);
            pDataUnManaged = Marshal.AllocCoTaskMem((int)length);
            Marshal.Copy(pDataManaged, 0, pDataUnManaged, (int)length);

            jfd = AndroidJNI.CallObjectMethod(jfd, getParcelFd, new jvalue[2]);

            IntPtr parcelFdClass = AndroidJNI.GetObjectClass(jfd);
            jvalue[] param_array3 = new jvalue[2];
            IntPtr getFd = AndroidJNI.GetMethodID(parcelFdClass, "getFileDescriptor", "()Ljava/io/FileDescriptor;");
            jfd = AndroidJNI.CallObjectMethod(jfd, getFd, param_array3);
            IntPtr fdClass = AndroidJNI.GetObjectClass(jfd);

            IntPtr descriptor = AndroidJNI.GetFieldID(fdClass, "descriptor", "I");
            fd = AndroidJNI.GetIntField(jfd, descriptor);

        }
        catch (IOException ex)
        {
            UnityEngine.Debug.Log("IO Exception: Failed to load swf file");
        }
#endif
        return;
    }
}