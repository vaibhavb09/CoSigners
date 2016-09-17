/**************************************************************************

Filename    :   SFManager.h
Content     :   Implementation of SFManager
Created     :   Sep 01, 2011
Authors     :	Ankur
Copyright   :   Copyright 2011 Autodesk, Inc. All Rights reserved.

Use of this software is subject to the terms of the Autodesk license
agreement provided at the time of installation or download, or which
otherwise accompanies this software in either electronic or hard copy form.

**************************************************************************/

#ifndef INC_SFMANAGER_H
#define INC_SFMANAGER_H

#include "SFInitParams.h"

//Next round of pacackages will re-enable sound --JP
#if defined (SF_OS_WINMETRO)
#undef GFX_ENABLE_SOUND
#endif

#if defined (SF_OS_WIN32) && !defined (SF_OS_WPHONE8) 
#define SF_UNITY_EXPORT  __declspec(dllexport) 
#define SF_CALLING_CONV __stdcall 
#elif (defined SF_OS_IPHONE) || (defined SF_OS_ANDROID) || (defined SF_OS_MAC) || (defined SF_OS_WPHONE8)
#define SF_UNITY_EXPORT
#define SF_CALLING_CONV
#endif

enum SFUnityErrorCodes
{
	Failure				= 0,
	Success,
	IncompatibleRenderer,
	NullManager
};

#define SF_SENTINAL 127
// Graphics device identifiers in Unity
enum GfxDeviceRenderer
{
	kGfxRendererOpenGL = 0,          // OpenGL
	kGfxRendererD3D9,                // Direct3D 9
	kGfxRendererD3D11,               // Direct3D 11
	kGfxRendererGCM,                 // Sony PlayStation 3 GCM
	kGfxRendererNull,                // "null" device (used in batch mode)
	kGfxRendererHollywood,           // Nintendo Wii
	kGfxRendererXenon,               // Xbox 360
	kGfxRendererOpenGLES,            // OpenGL ES 1.1
	kGfxRendererOpenGLES20Mobile,    // OpenGL ES 2.0 mobile variant
	kGfxRendererMolehill,            // Flash 11 Stage3D
	kGfxRendererOpenGLES20Desktop,   // OpenGL ES 2.0 desktop variant (i.e. NaCl)
	kGfxRendererCount
};

// Event types for UnitySetGraphicsDevice
enum GfxDeviceEventType {
	kGfxDeviceEventInitialize = 0,
	kGfxDeviceEventShutdown,
	kGfxDeviceEventBeforeReset,
	kGfxDeviceEventAfterReset,
};

struct SFCxForm;

namespace Scaleform{
    struct SFMovieCreationParams
    {
	    // Scale mode
	    enum ScaleModeType
	    {
		    SM_NoScale,
		    SM_ShowAll,
		    SM_ExactFit,
		    SM_NoBorder
	    };
	    char*   MovieName;
	    int     OX;
	    int     OY;
	    int     Width;
	    int     Height;
	    void*   pData;
	    int		Length;
	    ScaleModeType TheScaleModeType;
	    bool    InitFirstFrame;
	    bool    AutoManageViewport;
	    bool	OverrideBackgroundColor;
	    bool	RenderToTexture;
	    UInt32  TexWidth;
	    UInt32  TexHeight;
	    unsigned char BGRed;
	    unsigned char BGGreen;
	    unsigned char BGBlue;
	    unsigned char BGAlpha;
	    UInt32  TextureId;
	    int		Depth;
	    bool    IsMemoryFile;
	    bool    Pad0, Pad1, Pad2;
	    UInt32	Sentinal;
    };

    class SFViewPort
    {
    public:
	    int OX;
	    int OY;
	    int Width;
	    int Height;
    };

    class SFManager
    {
    public:
	    // ExternalInterface Callback
	    typedef void ( SF_STDCALL *ONEXTERNALINTERFACE )( int, const char*, const SFValueManaged*, int, int );
	    static ONEXTERNALINTERFACE OnExternalInterface;

	    typedef void ( SF_STDCALL *PFNDELEGATEDEF )();
	    static PFNDELEGATEDEF pfnDelegate;
	    // Allocates space for Values in unmanaged memory by C# and returns a pointer. C# handles creation
	    // and destruction of this memory
	    typedef void* ( SF_STDCALL  *ALLOCATEVALUES )( int numVal);
	    static ALLOCATEVALUES AllocateValues ;

	    // and destruction of this memory
	    typedef void* ( SF_STDCALL  *ALLOCATEDISPLAYINFO )(int*);
	    static ALLOCATEDISPLAYINFO AllocateDisplayInfo ;

	    // Send Scaleform error messages to Unity for logging
	    typedef void( SF_STDCALL  *LOGSFMESSAGE )(char* message);
	    static LOGSFMESSAGE LogSFMessage ;
	    void*				pDevice;
	    GfxDeviceRenderer	DeviceType;
	    enum LifeCycleEvents
	    {
		    Movie_Created = 0,
		    Movie_Destroyed,
	    };
	    virtual ~SFManager() = 0;
    #ifdef SF_OS_ANDROID
	    JavaVM*         GetJVM();
    #endif
	    virtual int		Init(SFInitParams* pinitParams) = 0;
	    virtual void	InitializeGFxSystem() = 0;
	    virtual void	Uninit() = 0;
	    virtual int		InitHAL() = 0;
	    void			SetDevice(void* pdevice, GfxDeviceRenderer deviceType) { pDevice = pdevice; DeviceType = deviceType; }
	    void*			GetDevice() { return pDevice; }
	    GfxDeviceRenderer GetDeviceType() { return DeviceType; }
	    virtual int		InitGraphics(void* pdevice, int deviceType, ThreadId tid) = 0;
	    virtual void	ResetRenderHAL() = 0;
	    virtual void	ShutdownRenderHAL() = 0;
	    virtual void	SetRenderThreadId(ThreadId tid) = 0;
	    virtual ThreadId GetCurrentThreadId() = 0;
	    virtual void	HandleDeviceReset(GfxDeviceEventType eventType) = 0;
	    virtual void 	SetSharedData(UInt32* poffset, void* pcommand, UInt32 id) = 0;
	    virtual void 	ClearCommandBuffer(int numCommands) = 0;
	    virtual void 	Advance(SInt64 movieId, float deltaTime,  bool advanceWhilePaused) = 0;
	    virtual void	SetNewViewport(int ox, int oy, int width, int height) = 0;
	
	    // Substituting a texture related:
	    //
	    virtual void	ReplaceTexture(SInt64 movieId, const char* textureName, UInt32 textureId, int RTWidth, int RTHeight) = 0;
	    virtual void 	ProcessMarkedForDeleteMovies() = 0;
	    virtual bool 	DoHitTest(SInt64 movieId, float x, float y, int hitTestType) = 0;
	    virtual void 	Display() = 0;
	    virtual int  	CreateMovie(SFMovieCreationParams& params) = 0;
	    virtual void*  	CreateMovieDef(SFMovieCreationParams& params) = 0;
	    virtual bool	DestroyMovieDef(void* pMovieDef) = 0;
	    virtual int  	CreateMovieInstance(void* movieDef, SFMovieCreationParams& params) = 0;
	    virtual bool	LoadFontConfig(const char* fontConfigPath) = 0;
	    virtual void	ApplyLanguage(const char* name) = 0;
	    virtual void 	DestroyMovie(SInt64 movieId) = 0;
	    virtual void	SetDepth(SInt64 movieId, int depth) = 0;
		virtual void	SetPaused(SInt64 movieId, bool pauseState) = 0;
		virtual bool	IsPaused(SInt64 movieId) = 0;
	    virtual void 	SetBackgroundAlpha(SInt64 movieId, float alpha) = 0;
	    virtual bool 	Invoke2(SFValueManaged* ptarget, char* method, int numVal, SFValueManaged* pargs, SFValueManaged* pret ) = 0;
	    virtual bool 	Invoke3(SInt64 movieId, char* method, int numVal, SFValueManaged* pargs, SFValueManaged* pret ) = 0;
	    virtual bool    HandleMouseEvent(SInt64 movieId, float x, float y, int icase, int buttonType) = 0;
	    virtual bool    HandleKeyEvent(SInt64 movieId, int cd, int modifier, int down, int keyboardIndex) = 0;
	    virtual bool    HandleCharEvent(SInt64 movieId, UInt32 wchar) = 0;
	    //virtual bool    HandleIMEEvent(unsigned message, UPInt wParam, UPInt lParam, UPInt hWND, bool preprocess) = 0;
	    virtual bool    HandleTouchEvent(SInt64 movieId, int id, float x, float y, int icase) = 0;
	    virtual void	HandleLifecycleEvent(SInt64 movieId, LifeCycleEvents ev) = 0;
	    virtual bool    SetFocus(SInt64 movieId, bool focus) = 0;
	    virtual float	GetFrameRate(SInt64 movieId) = 0;
	    virtual bool 	Equals(SFValueManaged* val1, SFValueManaged* val2) = 0;
	    virtual void*	AllocateString(char* sval, SInt64 movieId) = 0;
	    virtual void*	AllocateBoolean(bool bval, SInt64 movieId) = 0;
	    virtual void*	AllocateDouble(double nval, SInt64 movieId) = 0;
	    virtual void	DecrementValRefCount(void* mval) = 0;
	    virtual void*	CreateNewValue(void* msrc, SInt64 movieId) = 0;
	    virtual bool	CreateObject(SInt64 movieId, SFValueManaged* pvalManaged, const char* className, int numValues = 0,
						    SFValueManaged* mvalArray = NULL) = 0;
	    virtual bool	CreateFunction(SFValueManaged*, char*, void*, SFValueManaged*) = 0;
	    virtual bool	CreateArray(SInt64 movieId, SFValueManaged* pvalManaged) = 0;
	    virtual bool	GetBool(SFValueManaged* pmanagedVal) = 0;
	    virtual double	GetNumber(SFValueManaged* pmanagedVal) = 0;
	    virtual unsigned int GetUInt(SFValueManaged* pmanagedVal) = 0;
	    virtual int		GetInt(SFValueManaged* pmanagedVal) = 0;
	    virtual const char* GetString(SFValueManaged* pmanagedVal) = 0;
	    virtual void	SetBool(SFValueManaged* pvalManaged, bool bval) = 0;
	    virtual void	SetNumber(SFValueManaged* pmanagedVal, double number) = 0;
	    virtual void	SetUInt(SFValueManaged* pmanagedVal, unsigned int num) = 0;
	    virtual void	SetInt(SFValueManaged* pmanagedVal, int num) = 0;
	    virtual void	SetString(SFValueManaged* pmanagedVal, const char*) = 0;
	    virtual bool 	GetObject(SFValueManaged* pmanagedVal, void* pval) = 0;
		virtual int		GetAVMVersion(SInt64 movieId) = 0;
	    virtual bool 	SetDisplayInfo(SFValueManaged* pmanagedVal, SFDisplayInfo* dinfo, int szdInfoManaged) = 0;
	    virtual bool 	GetDisplayInfo(SFValueManaged* pmanagedVal, SFDisplayInfo* pdinfoManaged, int szdInfoManaged) = 0;
	    virtual bool 	GetDisplayMatrix(SFValueManaged* pmanagedVal, SFDisplayMatrix* pdmatrix, int szdMatManaged) = 0;
	    virtual bool 	SetDisplayMatrix(SFValueManaged* pmanagedVal, SFDisplayMatrix* pdmatrix, int szdMatManaged) = 0;
	    virtual bool 	Invoke4(SInt64 movieId, char* funcName, int numValues, SFValueManaged* mvalArray) = 0;
	    virtual bool	SetMember(SFValueManaged* pmtarget, const char* member, SFValueManaged* pmval) = 0;
	    virtual bool 	GetMember(SFValueManaged* pmtarget, const char* member, SFValueManaged* pmdst) = 0;
	    virtual int		GetArraySize(SFValueManaged* pmtarget) = 0;
	    virtual bool 	SetArraySize(SFValueManaged* pmtarget, unsigned int sz) = 0;
	    virtual bool 	GetElement(SFValueManaged* pmtarget, unsigned int idx, SFValueManaged* pmdst) = 0;
	    virtual bool 	SetElement(SFValueManaged* pmtarget, unsigned int idx, SFValueManaged* pmval) = 0;
		virtual bool 	PopBack(SFValueManaged* pmtarget, SFValueManaged* pmdst) = 0;
		virtual bool 	PushBack(SFValueManaged* pmtarget, SFValueManaged* pmval) = 0;
	    virtual bool 	RemoveElement(SFValueManaged* pmtarget, unsigned int idx) = 0;
	    virtual bool 	ClearElements(SFValueManaged* pmtarget) = 0;
	    virtual bool 	GetColorTransform(SFValueManaged* pmtarget, SFCxForm* sfcxform) = 0;
	    virtual bool 	SetColorTransform(SFValueManaged* pmtarget, SFCxForm* sfcxform) = 0;
	    virtual bool 	SetText(SFValueManaged* pmtarget, const char* ptext) = 0;
	    virtual bool 	GetText(SFValueManaged* pmtarget, SFValueManaged* pmdst) = 0;
	    virtual bool 	CreateEmptyMovieClip(SFValueManaged* pmtarget, SFValueManaged* pmdst, const char* instanceName, SInt32 depth = -1) = 0;
	    virtual bool	AttachMovie(SFValueManaged* pmtarget, SFValueManaged* pmdst, const char* symbolName,  const char* instanceName, SInt32 depth = -1) = 0;
	    virtual bool	GotoAndPlayFrame(SFValueManaged* pmtarget, const char* frame) = 0;
	    virtual bool	GotoAndStopFrame(SFValueManaged* pmtarget, const char* frame) = 0;
	    virtual bool	GotoAndPlay(SFValueManaged* pmtarget, unsigned int frame) = 0;
	    virtual bool	GotoAndStop(SFValueManaged* pmtarget, unsigned int frame) = 0;
	    virtual bool	GetVariable(SInt64 MovieID, SFValueManaged* v, const char* varPath) = 0;
	    virtual void	SetVariable(SInt64 MovieID, const char* varPath, SFValueManaged* v, int setVarType) = 0;
	    virtual bool	GetViewport(SInt64 MoviewId, SFViewPort* vp) = 0;
	    virtual bool	SetViewport(SInt64 MoviewId, const SFViewPort* vp) = 0;
	    virtual int     GetTextureCount() = 0;
	    virtual void    SetTextureCount(int textureCount) = 0;
	    virtual void	ApplyRenderTexture(SInt64 movieId, SInt64 renderTexture) = 0;

	    virtual bool	WasDeviceReset() = 0;
	    virtual void	ClearDeviceReset() = 0;
        virtual void*   GetMovieDef(SInt64 MovieID) = 0;

	    // For printing messages on Unity console
	    virtual void	WriteMessageToUnityConsole(char* sval) = 0;
	    virtual void    WriteMessageToUnityConsole(const char* sval) = 0;
	    // Get/Set Version
	    virtual void	SetVersion(const char* version) = 0;
	    virtual const char*	GetVersion() = 0;

		// MovieDef Related
		virtual int		GetSWFVersion(MovieDef*) = 0;
		virtual float	GetMovieHeight(MovieDef*) = 0;
		virtual float	GetMovieWidth(MovieDef*) = 0;
		virtual int		GetFrameCount(MovieDef*) = 0;
		virtual float	GetFrameRate(MovieDef*) = 0;
		virtual const char*	GetFileURL(MovieDef*, int*) = 0;
    };

    SFManager* CreateManager();

    class ManagerFactory
    {
    private:
	    ManagerFactory();
	    ManagerFactory(const ManagerFactory &) { }
	    ManagerFactory &operator=(const ManagerFactory &) { return *this; }

    public:
	    ~ManagerFactory();

	    static ManagerFactory *Get()
	    {
		    static ManagerFactory instance;
		    return &instance;
	    }
	    SFManager* CreateManager();
	    static void DestroyManager();
	    static void InitGFxSystem();
	    static void DestroyGFxSystem();
    };
};
    #endif