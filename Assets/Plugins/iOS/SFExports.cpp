/**************************************************************************

Filename    :   SFExports.cpp
Content     :   Exported Function Declarations
Created     :   Sep 01, 2011
Authors     :	Ankur
Copyright   :   Copyright 2011 Autodesk, Inc. All Rights reserved.

Use of this software is subject to the terms of the Autodesk license
agreement provided at the time of installation or download, or which
otherwise accompanies this software in either electronic or hard copy form.

**************************************************************************/

#include "Kernel/SF_Types.h"
#include "Kernel/SF_Atomic.h"
#include "Kernel/SF_String.h"


// Debugging
#if (defined SF_OS_ANDROID)
#if defined SF_BUILD_DEBUG
#include "android/log.h"
#endif
#include <jni.h>
#endif
typedef void* ThreadId;
using namespace Scaleform;

// Unity Integration Includes
//#include "SFValueManaged.h"
struct SFDisplayInfo;
struct SFDisplayMatrix;
struct SFCxForm;
struct SFValueManaged;
struct MovieDef;

#include "SFManager.h"
#include "SFUnityDelegates.h"

// Synchronization Primitive- Can't be member of Manager since the garbage collector thread might
// kick in after the manager has been destroyed. Other alternatives to making it global?
Lock	SFUnityLock;

extern "C"
{
	SFManager* pManager = 0;
};

SFUnityErrorCodes		ErrorCode = Success;

// Initialize Static Variables
SFManager::ALLOCATEDISPLAYINFO	SFManager::AllocateDisplayInfo = 0;
SFManager::ALLOCATEVALUES		SFManager::AllocateValues = 0;
SFManager::LOGSFMESSAGE			SFManager::LogSFMessage = 0;
SFManager::ONEXTERNALINTERFACE	SFManager::OnExternalInterface = 0;
SFManager::PFNDELEGATEDEF		SFManager::pfnDelegate = 0;

void GFx_SetEvalKey(const char* key);

extern "C"
{

	bool CheckForNullManager(SFManager* pManager)
	{
		if (pManager == NULL || ErrorCode == IncompatibleRenderer)
		{
			if (SFManager::LogSFMessage)
			{
				SFManager::LogSFMessage((char*)("SF:Warning: Trying to access Null SFManager or using incompatible renderers!"));
			}
			return false;
		}
		return true;
	}

	bool IsMovieIdValid(SInt64 movieId)
	{
		return (movieId != 0);
	}

	bool CheckForSentinal(UInt32 sentinal)
	{
		return sentinal == SF_SENTINAL;
	}

#ifdef SF_OS_ANDROID
    static JavaVM   *pJVM = 0;                                            
        
    jint JNICALL JNI_OnLoad(JavaVM *jvm, void *reserved) 
    {                                                                      
        pJVM = jvm;                                                         
        return JNI_VERSION_1_6;                                                                                                   
    }    
#endif
	SFManager::~SFManager()
	{
	}
#ifdef SF_OS_ANDROID
    JavaVM* SFManager::GetJVM()
    {
        return pJVM;
    }

#endif
#if defined (SF_OS_WPHONE8)
namespace GFxUnity3D
{
    public ref class SFExports sealed
    {
    public:
#endif
		SF_UNITY_EXPORT void SF_CALLING_CONV SF_Destroy()
		{
			ManagerFactory::DestroyGFxSystem();
		}

		// Called from Unity's Render Thread. 
		SF_UNITY_EXPORT void UnityRenderEvent(int eventID)
		{
			// Issued during Start of the game object to which the SFCamera is attached. We use GL.IssuePluginEvent(0) to
			// put a command on Unity's shared queue. The command is actually processed by the Render Thread. 
		
			if (eventID == 0)
			{
				// Needs locking here as Unity main thread can kick in and start calling Advance before Graphics/HAL initialization is fully finished.
				Lock::Locker locker(&SFUnityLock);
#if defined(SF_OS_WIN32) || defined(SF_OS_MAC)
				// Check for renderer type consistency
				void*				pdevice		= pManager->GetDevice();
				GfxDeviceRenderer	deviceType	= pManager->GetDeviceType();
#ifdef FXPLAYER_RENDER_OPENGL
				if (deviceType == kGfxRendererD3D9 || deviceType == kGfxRendererD3D11 )
				{
					pManager->WriteMessageToUnityConsole("Renderer Incompatibility: You are using an OpenGL version of the Scaleform plug-in with a D3D Unity Renderer. Trying reopening Unity editor with -force-opengl");
					ErrorCode = IncompatibleRenderer;
					return;
				}
#else
				if (deviceType != kGfxRendererD3D9 && deviceType != kGfxRendererD3D11 )
				{
					pManager->WriteMessageToUnityConsole("Renderer Incompatibility: You are using an D3D version of the Scaleform plug-in with a OpenGL Unity Renderer. Trying reopening Unity editor without -force-opengl");
					ErrorCode = IncompatibleRenderer;
					return;
				}
#endif
				ThreadId tid = pManager->GetCurrentThreadId();
				pManager->SetRenderThreadId(tid);
				pManager->InitGraphics(pdevice, deviceType, tid);
#endif

#if defined(SF_OS_IPHONE) || defined(SF_OS_ANDROID)
				ThreadId tid = pManager->GetCurrentThreadId();
				// Initialize HAL and create renderer
				pManager->SetRenderThreadId(tid);
				pManager->InitGraphics(0, kGfxRendererOpenGL, tid);
#endif
				pManager->InitHAL();
			}
		

			// Issued from within a coroutine that's kicked off during ScaleformCamera::Start. The eventID is passed as a parameter
			// and thus can be arbitrary. 
			if (eventID == 1)
			{
				//	Lock::Locker locker(&SFUnityLock);
				if(!CheckForNullManager(pManager)) return;
				pManager->Display();
			}

			if (eventID == 2)
			{
				if(!CheckForNullManager(pManager)) return;
				pManager->ShutdownRenderHAL();
			}
		}
	
		SF_UNITY_EXPORT void SF_CALLING_CONV SF_SetTextureCount(int textureCount)
		{
			if(pManager)
			{
				pManager->SetTextureCount(textureCount);
			}
		}

		// must be called before the loader is created, otherwise Scaleform initialization will fail.
		SF_UNITY_EXPORT void SF_CALLING_CONV SF_SetKey(char* key)
		{
			GFx_SetEvalKey(key);
		}
		// Called from the constructor of SFManager
		SF_UNITY_EXPORT int SF_CALLING_CONV SF_Init(SFInitParams* pinitParams, int managedSize, const char* version)
		{
			// On iOS, UnitySetGraphicsDevice is not called and therefore Initialization of GFx::System and
			// creation of SFManager must be done in Init.
#if  (defined SF_OS_IPHONE)	|| (defined SF_OS_ANDROID)
			// RenderThread object is created in SFManager constructor
			ManagerFactory* pinstance = ManagerFactory::Get();
			pinstance->InitGFxSystem();
			pManager = pinstance->CreateManager();
#endif
			if(!CheckForNullManager(pManager))
				return Failure;
			if (!CheckForSentinal(pinitParams->Sentinal))
				return Failure;

			pManager->SetVersion(version);
			int sz = sizeof(*pinitParams);
			SF_ASSERT(sz == managedSize);
			pManager->Init(pinitParams);

#if (defined SF_OS_WIN32) && (defined SF_ENABLE_IME)
			// Added code for subclassing Unity's WndProc and inserting our own to tap into IME messages. The code doesn't quite work right now
			// but should be fixable with some more investigation
		//	HWND unityWindow = GetForegroundWindow();
		//	pManager->AddIMEHook(unityWindow);
#endif
			return Success;
		}

		// Called when the game object to which SFCamera is attached is destroyed. We destroy all Movies in this step, but 
		// not the SFManager or the Scaleform runtime. 
		// This function is called from the main thread.
		SF_UNITY_EXPORT void SF_CALLING_CONV SF_Uninit()
		{
			if(!CheckForNullManager(pManager)) return;
			pManager->Uninit();
#if (defined SF_OS_WIN32) && (defined SF_ENABLE_IME)
			// Unsubclass
			// pManager->RemoveIMEHook();
#endif
		}

		// Will be called by Unity on the RENDER thread. This is the only opportunity to initialize Scaleform 
		// and obtain the D3D device pointer. 
		SF_UNITY_EXPORT void UnitySetGraphicsDevice (void* pdevice, int deviceType, int eventType)
		{
			ManagerFactory* pinstance;
			switch (eventType)
			{
			case kGfxDeviceEventInitialize:
				// Initialize GFx
				// RenderThread object is created in SFManager constructor
				pinstance = ManagerFactory::Get();
				pinstance->InitGFxSystem();
				pManager = pinstance->CreateManager();
				if (pManager)
				{
					pManager->SetDevice(pdevice, (GfxDeviceRenderer)deviceType);
				}
			
				break;
				// In Unity 3.50b5, Shutdown, BeforeReset and AfterReset events are not sent. Need to work with
				// Unity to resolve this issue.
			case kGfxDeviceEventShutdown:
				if (pManager)
				{
					pManager->ProcessMarkedForDeleteMovies();
					ManagerFactory::DestroyManager();
					pManager = 0;
				}
				break;
			case kGfxDeviceEventBeforeReset:
				if (pManager)
				{
					pManager->HandleDeviceReset(kGfxDeviceEventBeforeReset);
				}
				break;
			case kGfxDeviceEventAfterReset:
				if (pManager)
				{
					pManager->HandleDeviceReset(kGfxDeviceEventAfterReset);
				}
				break;
			}

		}

		SF_UNITY_EXPORT void SF_CALLING_CONV SF_DestroyManager()
		{
			
			Lock::Locker locker(&SFUnityLock);
			// Null out the delegates
			SFManager::AllocateDisplayInfo	= 0;
			SFManager::AllocateValues		= 0;
			SFManager::LogSFMessage			= 0;
			if(!CheckForNullManager(pManager)) return;
			pManager->Display();
			pManager->ProcessMarkedForDeleteMovies();
			delete (pManager);
			pManager = 0;
		}

		SF_UNITY_EXPORT bool SF_CALLING_CONV SF_ReplaceTexture(SInt64 movieId, const char* textureName, int textureId, int RTWidth, int RTHeight)
		{
			
			Lock::Locker locker(&SFUnityLock);
		
			if(!CheckForNullManager(pManager)) return false;
			// In Unity3.5, replacetexture is supported only for opengl
			const char* UnityVersion = pManager->GetVersion();
			if (!strncmp(UnityVersion, "4", 1))
			{
				pManager->ReplaceTexture(movieId, textureName, textureId, RTWidth, RTHeight);
			}
			else
			{
#ifdef FXPLAYER_RENDER_OPENGL
				pManager->ReplaceTexture(movieId, textureName, textureId, RTWidth, RTHeight);
#else	
				pManager->WriteMessageToUnityConsole("On Unity3.5, due to Unity limitation, ReplaceTexture functionality is only supported in OpenGL mode");
#endif		
			}

			return true;
		}

		SF_UNITY_EXPORT void SF_CALLING_CONV SF_SetSharedData(UInt32* poffset, void* pcommand, UInt32 id)
		{
			if(!CheckForNullManager(pManager)) return;
			pManager->SetSharedData(poffset, pcommand, id);
		}

		SF_UNITY_EXPORT void SF_CALLING_CONV SF_ClearCommandBuffer(int numCommands)
		{
			if(!CheckForNullManager(pManager)) return;
			for (int i = 0; i < numCommands; i++)
			{
				pManager->ClearCommandBuffer(i);

			}
		}

		// Called from the main thread.
		SF_UNITY_EXPORT int SF_CALLING_CONV SF_Advance(SInt64 movieId, float deltaTime,  bool advanceWhilePaused)
		{
			
			Lock::Locker locker(&SFUnityLock);
			if(CheckForNullManager(pManager))
			{
				pManager->Advance(movieId, deltaTime, advanceWhilePaused);
			}
			return ErrorCode;
		}

		SF_UNITY_EXPORT void SF_CALLING_CONV SF_ProcessMarkedForDeleteMovies()
		{
			
			Lock::Locker locker(&SFUnityLock);
			if(!CheckForNullManager(pManager)) return;
			pManager->ProcessMarkedForDeleteMovies();
		}

		SF_UNITY_EXPORT bool SF_CALLING_CONV SF_DoHitTest(SInt64 movieId, float x, float y, int hitTestType)
		{
			
			Lock::Locker locker(&SFUnityLock);
			if(!CheckForNullManager(pManager)) return false;
			return pManager->DoHitTest(movieId, x, y, hitTestType);
		}

		// Movies must be displayed in the Render Thread while running in multi threaded mode. This means that 
		// Display can not be called from script, since script execution takes place on the main thread. Therefore 
		// Movies are displayed in the UnityRenderEvent call.
		SF_UNITY_EXPORT void SF_CALLING_CONV SF_Display()
		{
            Lock::Locker locker(&SFUnityLock);
			if(!CheckForNullManager(pManager)) return;
			pManager->Display();
		}

		SF_UNITY_EXPORT void SF_CALLING_CONV SF_PumpDisplay()
		{
			if(!CheckForNullManager(pManager)) return;
		}

		SF_UNITY_EXPORT void SF_CALLING_CONV SF_SetNewViewport(int ox, int oy, int width, int height)
		{
			
			Lock::Locker locker(&SFUnityLock);
			if(!CheckForNullManager(pManager)) return;
			pManager->SetNewViewport(ox, oy, width, height);
		}
		SF_UNITY_EXPORT bool SF_CALLING_CONV SF_GetVariable(SInt64 MovieID, SFValueManaged* v, const char* varPath)
		{
			
			Lock::Locker locker(&SFUnityLock);
			if(!CheckForNullManager(pManager)) return false;
			return pManager->GetVariable(MovieID, v, varPath);
		}
		SF_UNITY_EXPORT void SF_CALLING_CONV SF_SetVariable(SInt64 MovieID, const char* varPath, SFValueManaged* v, int setVarType)
		{
			
			Lock::Locker locker(&SFUnityLock);
			if(!CheckForNullManager(pManager)) return;
			pManager->SetVariable( MovieID, varPath, v, setVarType);
		}
		SF_UNITY_EXPORT bool SF_CALLING_CONV SF_GetViewport(SInt64 MoviewId, SFViewPort* vp)
		{
			
			Lock::Locker locker(&SFUnityLock);
			if(!CheckForNullManager(pManager)) return false;
			return pManager->GetViewport(MoviewId, vp);
		}
		SF_UNITY_EXPORT bool SF_CALLING_CONV SF_SetViewport(SInt64 MoviewId, const SFViewPort* vp)
		{
			
			Lock::Locker locker(&SFUnityLock);
			if(!CheckForNullManager(pManager)) return false;
			return pManager->SetViewport(MoviewId, vp);
		}
		SF_UNITY_EXPORT void* SF_CALLING_CONV SF_GetMovieDef(SInt64 MoviewId)
		{			
			Lock::Locker locker(&SFUnityLock);
			if(!CheckForNullManager(pManager)) return NULL;
            return pManager->GetMovieDef(MoviewId);
		}

		//Will be obsoleted instead of MovieDef::CreateInstance
		SF_UNITY_EXPORT int SF_CALLING_CONV SF_CreateMovie(SFMovieCreationParams* params)
		{
			
			Lock::Locker locker(&SFUnityLock);
			if(!CheckForNullManager(pManager)) return Failure;
			// Check if the sentinal came in as expected
			if (!CheckForSentinal(params->Sentinal)) return Failure;
			return pManager->CreateMovie(*params);
		}

		SF_UNITY_EXPORT void* SF_CALLING_CONV SF_CreateMovieDef(SFMovieCreationParams* params)
		{			
			Lock::Locker locker(&SFUnityLock);
			if(!CheckForNullManager(pManager)) return NULL;
			// Check if the sentinal came in as expected
			if (!CheckForSentinal(params->Sentinal)) return 0;
			return pManager->CreateMovieDef(*params);
		}

		SF_UNITY_EXPORT bool SF_CALLING_CONV SF_DestroyMovieDef(void* pMovieDef)
		{	
			Lock::Locker locker(&SFUnityLock);
			if(!CheckForNullManager(pManager)) return NULL;
			// Check if the sentinal came in as expected
			//if (!CheckForSentinal(params->Sentinal)) return 0;
			return pManager->DestroyMovieDef(pMovieDef);
		}
		SF_UNITY_EXPORT int SF_CALLING_CONV SF_CreateMovieInstance(void* pMovieDef, SFMovieCreationParams* params)
		{
			
			Lock::Locker locker(&SFUnityLock);
			if(!CheckForNullManager(pManager)) return Failure;
			// Check if the sentinal came in as expected
			if (!CheckForSentinal(params->Sentinal)) return Failure;
			return pManager->CreateMovieInstance(pMovieDef,*params);
		}

		SF_UNITY_EXPORT void SF_CALLING_CONV SF_DestroyMovie(SInt64 movieId)
		{
			
			Lock::Locker locker(&SFUnityLock);
			if(!CheckForNullManager(pManager)) return;
			pManager->DestroyMovie(movieId);
		}

		SF_UNITY_EXPORT void SF_CALLING_CONV SF_SetDepth(SInt64 movieId, int depth)
		{
			
			Lock::Locker locker(&SFUnityLock);
			if(!CheckForNullManager(pManager)) return;
			pManager->SetDepth(movieId, depth);
		}

		SF_UNITY_EXPORT void SF_CALLING_CONV SF_SetPaused(SInt64 movieId, bool pauseState)
		{
			
			Lock::Locker locker(&SFUnityLock);
			if(!CheckForNullManager(pManager)) return;
				pManager->SetPaused(movieId, pauseState);
		}

		SF_UNITY_EXPORT bool SF_CALLING_CONV SF_IsPaused(SInt64 movieId)
		{
			
			Lock::Locker locker(&SFUnityLock);
			if(!CheckForNullManager(pManager)) return false;
				return pManager->IsPaused(movieId);
		}

		SF_UNITY_EXPORT bool SF_CALLING_CONV SF_LoadFontConfig(const char* fontConfigPath)
		{
			
			Lock::Locker locker(&SFUnityLock);
			if(!CheckForNullManager(pManager)) return false;
			return pManager->LoadFontConfig(fontConfigPath);
		}

		SF_UNITY_EXPORT void SF_CALLING_CONV SF_ApplyLanguage(const char* name)
		{
			
			Lock::Locker locker(&SFUnityLock);
			if(!CheckForNullManager(pManager)) return;
			return pManager->ApplyLanguage(name);
		}

		SF_UNITY_EXPORT void SF_CALLING_CONV SF_SetBackgroundAlpha(SInt64 movieId, float alpha)
		{
			
			Lock::Locker locker(&SFUnityLock);
			if(!CheckForNullManager(pManager)) return;
			pManager->SetBackgroundAlpha(movieId, alpha);
		}

		SF_UNITY_EXPORT void SF_CALLING_CONV SF_NotifyNativeManager(SInt64 movieId, SFManager::LifeCycleEvents ev)
		{
			
			Lock::Locker locker(&SFUnityLock);
			if(!CheckForNullManager(pManager)) return;
			pManager->HandleLifecycleEvent(movieId, ev);
		}

		SF_UNITY_EXPORT bool SF_CALLING_CONV SF_Invoke2(SFValueManaged* ptarget, char* method, int numVal, SFValueManaged* pargs, SFValueManaged* pret )
		{
			
			Lock::Locker locker(&SFUnityLock);
			if(!CheckForNullManager(pManager)) return false;

			if (!ptarget) return false;
			return pManager->Invoke2(ptarget, method, numVal, pargs, pret );
		}

		SF_UNITY_EXPORT bool SF_CALLING_CONV SF_Invoke3(SInt64 movieId, char* method, int numVal, SFValueManaged* pargs, SFValueManaged* pret )
		{
			
			Lock::Locker locker(&SFUnityLock);
			if(!CheckForNullManager(pManager)) return false;
			return pManager->Invoke3(movieId, method, numVal, pargs, pret);
		}

		SF_UNITY_EXPORT void SF_CALLING_CONV SF_SetExternalInterfaceDelegate(SFManager::ONEXTERNALINTERFACE ei)
		{
			if(!CheckForNullManager(pManager)) return;
			SFManager::OnExternalInterface = ei;
		}

		SF_UNITY_EXPORT void SF_CALLING_CONV SF_SetAllocateValues(SFManager::ALLOCATEVALUES av)
		{
			SFManager::AllocateValues = av;
		}
		SF_UNITY_EXPORT void SF_CALLING_CONV SF_SetLogDelegate(SFManager::LOGSFMESSAGE logDel)
		{
			SFManager::LogSFMessage = logDel;
		}

		SF_UNITY_EXPORT void SF_CALLING_CONV SF_SetDisplayInfoDelegate(SFManager::ALLOCATEDISPLAYINFO dInfoDel)
		{
			SFManager::AllocateDisplayInfo = dInfoDel;
		}
		// ************Event Handling***************
		SF_UNITY_EXPORT bool SF_CALLING_CONV SF_HandleMouseEvent(SInt64 movieId, float x, float y, int icase, int buttonType)
		{
			
			Lock::Locker locker(&SFUnityLock);
			if(movieId == 0 || !CheckForNullManager(pManager)) return false;
			return pManager->HandleMouseEvent(movieId, x,y, icase, buttonType);
		}

		SF_UNITY_EXPORT bool SF_CALLING_CONV SF_HandleKeyEvent(SInt64 movieId, int cd, int modifier, int down, int keyboardIndex)
		{
			
			Lock::Locker locker(&SFUnityLock);
			if(movieId == 0 || !CheckForNullManager(pManager)) return false;
			return pManager->HandleKeyEvent(movieId, cd, modifier, down, keyboardIndex);
		}

		SF_UNITY_EXPORT bool SF_CALLING_CONV SF_HandleCharEvent(SInt64 movieId, UInt32 wchar)
		{
			
			Lock::Locker locker(&SFUnityLock);
			if(movieId == 0 || !CheckForNullManager(pManager)) return false;
			return pManager->HandleCharEvent(movieId, wchar);
		}

		SF_UNITY_EXPORT bool SF_CALLING_CONV SF_HandleTouchEvent(SInt64 movieId, int id, float x, float y, int icase)
		{
			
			Lock::Locker locker(&SFUnityLock);
			if(movieId == 0 || !CheckForNullManager(pManager)) return false;
			return pManager->HandleTouchEvent(movieId, id, x, y, icase);
		}

		SF_UNITY_EXPORT bool SF_CALLING_CONV SF_WasDeviceReset()
		{
			
			Lock::Locker locker(&SFUnityLock);
			if (!CheckForNullManager(pManager)) return false;
			return pManager->WasDeviceReset();
		}

		SF_UNITY_EXPORT void SF_CALLING_CONV SF_ClearDeviceReset()
		{
			
			Lock::Locker locker(&SFUnityLock);
			if (!CheckForNullManager(pManager)) return;
			pManager->ClearDeviceReset();
		}

		SF_UNITY_EXPORT void SF_CALLING_CONV SF_ApplyRenderTexture(SInt64 movieId, SInt64 renderTexture)
		{
			
			Lock::Locker locker(&SFUnityLock);
			if (!CheckForNullManager(pManager)) return;
			pManager->ApplyRenderTexture(movieId, renderTexture);
		}

		SF_UNITY_EXPORT void SF_CALLING_CONV SF_SetFocus(SInt64 movieId, bool focus)
		{
			
			Lock::Locker locker(&SFUnityLock);
			if(movieId == 0 || !CheckForNullManager(pManager)) return;
			pManager->SetFocus(movieId, focus);
		}

		SF_UNITY_EXPORT float SF_CALLING_CONV SF_GetFrameRate(SInt64 movieId)
		{
			
			Lock::Locker locker(&SFUnityLock);
			if(movieId == 0 || !CheckForNullManager(pManager)) return 0.0;
			return pManager->GetFrameRate(movieId);
		}


		SF_UNITY_EXPORT bool SF_CALLING_CONV SF_Equals(SFValueManaged* val1, SFValueManaged* val2)
		{
			
			Lock::Locker locker(&SFUnityLock);
			if (pManager)
			{
				return pManager->Equals(val1, val2);
			}
			return false;
		}

		SF_UNITY_EXPORT void* SF_CALLING_CONV SF_AllocateString(char* sval, SInt64 movieId)
		{
			
			Lock::Locker locker(&SFUnityLock);
			if (pManager && IsMovieIdValid(movieId))
			{
				return pManager->AllocateString(sval, movieId);
			}
			return NULL;
		}

		SF_UNITY_EXPORT void* SF_CALLING_CONV SF_AllocateBoolean(bool bval, SInt64 movieId)
		{
			
			Lock::Locker locker(&SFUnityLock);
			if (pManager && IsMovieIdValid(movieId))
			{
				return pManager->AllocateBoolean(bval, movieId);
			}
			return NULL;
		}

		SF_UNITY_EXPORT void* SF_CALLING_CONV SF_AllocateDouble(double nval, SInt64 movieId)
		{
			
			Lock::Locker locker(&SFUnityLock);
			if (pManager && IsMovieIdValid(movieId))
			{
				return pManager->AllocateDouble(nval, movieId);
			}
			return NULL;
		}


		// This is called when the Values held in C# are garbage collected. This is a good 
		// occasion for us to release the underlying SF::Value as well. However, the timing of the 
		// garbage collection process is indeterministic, and it's possible that the SFManager is 
		// destroyed before all the Values held by C# are released. To take care of this case, we 
		// put all values we create on a list, and then remove from list whenever those values are 
		// garbage collected. If there are any values left on this list when SFManager is destroyed,
		// we manually release them. This makes sure that all Values created by us are properly destroyed.
		SF_UNITY_EXPORT void SF_CALLING_CONV SF_DecrementValRefCount(void* mval)
		{
			if (mval == 0) return;
			
			Lock::Locker locker(&SFUnityLock);
			if (pManager)
			{
				pManager->DecrementValRefCount(mval);
			}
			// Make sure not delete mvalArray! That's allocated in managed memory space and will be released there. 
		}

		SF_UNITY_EXPORT void* SF_CALLING_CONV SF_CreateNewValue(void* msrc, SInt64 movieId)
		{
			if (msrc == 0) return NULL;
			
			Lock::Locker locker(&SFUnityLock);
			if (pManager && IsMovieIdValid(movieId))
			{
				return pManager->CreateNewValue(msrc, movieId);
			}
			return NULL;
		}

		SF_UNITY_EXPORT bool SF_CALLING_CONV SF_GetBool(SFValueManaged* pmanagedVal)
		{
			// Do error checking
			// Returning false doesn't help since we are actually returning a bool, but at least prevents crashes
			if (pmanagedVal == NULL) return false;
			
			Lock::Locker locker(&SFUnityLock);
			if (pManager)
			{
				return pManager->GetBool(pmanagedVal);
			}
			return false;
		}

		SF_UNITY_EXPORT double SF_CALLING_CONV SF_GetNumber(SFValueManaged* pmanagedVal)
		{
			// Do error checking
			if (!pmanagedVal) return 0;
			
			Lock::Locker locker(&SFUnityLock);
			if (pManager)
			{
				return pManager->GetNumber(pmanagedVal);
			}
			return 0;
		}

		SF_UNITY_EXPORT unsigned int SF_CALLING_CONV SF_GetUInt(SFValueManaged* pmanagedVal)
		{
			if (!pmanagedVal) return 0;
			
			Lock::Locker locker(&SFUnityLock);
			if (pManager)
			{
				return pManager->GetUInt(pmanagedVal);
			}
			return 0;
		}

		SF_UNITY_EXPORT int SF_CALLING_CONV SF_GetInt(SFValueManaged* pmanagedVal)
		{
			if (!pmanagedVal) return 0;
			
			Lock::Locker locker(&SFUnityLock);
			if (pManager)
			{
				return pManager->GetInt(pmanagedVal);
			}
			return 0;
		}

		SF_UNITY_EXPORT const char* SF_CALLING_CONV SF_GetString(SFValueManaged* pmanagedVal)
		{
			if (!pmanagedVal) return NULL;
			
			Lock::Locker locker(&SFUnityLock);
			if (pManager)
			{
				return pManager->GetString(pmanagedVal);
			}
			return NULL;
		}

		SF_UNITY_EXPORT void SF_CALLING_CONV SF_SetBool(SFValueManaged* pmanagedVal, bool bval)
		{
			// Do error checking
			// Returning false doesn't help since we are actually returing a bool, but at least prevents crashes
			if (pmanagedVal == NULL) return;
			
			Lock::Locker locker(&SFUnityLock);
			if (pManager)
			{
				pManager->SetBool(pmanagedVal, bval);
			}
		}

		SF_UNITY_EXPORT void SF_CALLING_CONV SF_SetNumber(SFValueManaged* pmanagedVal, double number)
		{
			// Do error checking
			if (!pmanagedVal) return;
			
			Lock::Locker locker(&SFUnityLock);
			if (pManager)
			{
				pManager->SetNumber(pmanagedVal, number);
			}
		}

		SF_UNITY_EXPORT void SF_CALLING_CONV SF_SetUInt(SFValueManaged* pmanagedVal, unsigned int uival)
		{
			if (!pmanagedVal) return ;
			
			Lock::Locker locker(&SFUnityLock);
			if (pManager)
			{
				pManager->SetUInt(pmanagedVal, uival);
			}
		}

		SF_UNITY_EXPORT void SF_CALLING_CONV SF_SetInt(SFValueManaged* pmanagedVal, int ival)
		{
			if (!pmanagedVal) return;
			
			Lock::Locker locker(&SFUnityLock);
			if (pManager)
			{
				pManager->SetInt(pmanagedVal, ival);
			}
		}

		SF_UNITY_EXPORT void SF_CALLING_CONV SF_SetString(SFValueManaged* pmanagedVal, const char* str)
		{
			if (!pmanagedVal) return;
			
			Lock::Locker locker(&SFUnityLock);
			if (pManager)
			{
				pManager->SetString(pmanagedVal, str);
			}
		}

		SF_UNITY_EXPORT bool SF_CALLING_CONV SF_GetObject(SFValueManaged* pmanagedVal, void* pval)
		{
			if (!pmanagedVal || !pval) return false;
			
			Lock::Locker locker(&SFUnityLock);
			if (pManager)
			{
				return pManager->GetObject(pmanagedVal, pval);
			}
			return false;
		}

		SF_UNITY_EXPORT int SF_GetAVMVersion(SInt64 movieId)
		{
			Lock::Locker locker(&SFUnityLock);
			if (pManager)
			{
				return pManager->GetAVMVersion(movieId);
			}
			return Failure;
		}

		SF_UNITY_EXPORT bool SF_CALLING_CONV SF_SetDisplayInfo(SFValueManaged* pmanagedVal, SFDisplayInfo* dinfo, int szdInfoManaged)
		{
			if (!pmanagedVal || !dinfo) return false;
			
			Lock::Locker locker(&SFUnityLock);
			if (pManager)
			{
				return pManager->SetDisplayInfo(pmanagedVal, dinfo, szdInfoManaged);
			}
			return false;
		}

		SF_UNITY_EXPORT bool SF_CALLING_CONV  SF_GetDisplayInfo(SFValueManaged* pmanagedVal, SFDisplayInfo* pdinfoManaged, int szdInfoManaged)
		{

			if (!pmanagedVal || !pdinfoManaged) return false;
			// Prevent concurrent access by the garbage collector thread
			
			Lock::Locker locker(&SFUnityLock);
			if (pManager)
			{
				return pManager->GetDisplayInfo(pmanagedVal, pdinfoManaged, szdInfoManaged);
			}
			return false;
		}

		SF_UNITY_EXPORT bool SF_CALLING_CONV SF_GetDisplayMatrix(SFValueManaged* pmanagedVal, SFDisplayMatrix* pdmatrix, int szdMatManaged)
		{

			if (!pmanagedVal || !pdmatrix) return false;
			
			Lock::Locker locker(&SFUnityLock);
			if (pManager)
			{
				return pManager->GetDisplayMatrix(pmanagedVal, pdmatrix, szdMatManaged);
			}
			return false;
		}

		SF_UNITY_EXPORT bool SF_CALLING_CONV SF_SetDisplayMatrix(SFValueManaged* pmanagedVal, SFDisplayMatrix* pdmatrix, int szdMatManaged)
		{
			if (!pmanagedVal || !pdmatrix) return false;
			
			Lock::Locker locker(&SFUnityLock);
			if (pManager)
			{
				return pManager->SetDisplayMatrix(pmanagedVal, pdmatrix, szdMatManaged);
			}
			return false;
		}

		SF_UNITY_EXPORT bool SF_CALLING_CONV SF_Invoke4(SInt64 movieId, char* funcName, int numValues, SFValueManaged* mvalArray)
		{
			
			Lock::Locker locker(&SFUnityLock);
			if (pManager)
			{
				return pManager->Invoke4(movieId, funcName, numValues, mvalArray);
			}
			return false;
		}

		SF_UNITY_EXPORT bool SF_CALLING_CONV SF_CreateObject(SInt64 movieId, SFValueManaged* pvalManaged, const char* className, int numValues,
			SFValueManaged* mvalArray)
		{
			
			Lock::Locker locker(&SFUnityLock);
			if (pManager)
			{
				return pManager->CreateObject(movieId, pvalManaged, className, numValues, mvalArray);
			}
			return false;
		}

		SF_UNITY_EXPORT bool SF_CALLING_CONV SF_CreateArray(SInt64 movieId, SFValueManaged* pvalManaged)
		{
			
			Lock::Locker locker(&SFUnityLock);
			if (pManager)
			{
				return pManager->CreateArray(movieId, pvalManaged);
			}
			return false;
		}

		SF_UNITY_EXPORT void SF_CALLING_CONV SF_SetMember(SFValueManaged* pmtarget, char* member, SFValueManaged* pmval)
		{
			if (!member || !pmtarget || !pmval) return;
			Lock::Locker locker(&SFUnityLock);
			// Check if the target is an object or not.
			if (pManager)
			{
				pManager->SetMember(pmtarget, member, pmval);
			}
		}

		SF_UNITY_EXPORT bool SF_CALLING_CONV SF_GetMember(SFValueManaged* pmtarget, char* member, SFValueManaged* pmdst)
		{
			if (!member || !pmdst || !pmtarget) return false;
			Lock::Locker locker(&SFUnityLock);
			if (pManager)
			{
				return pManager->GetMember(pmtarget, member, pmdst);
			}
			return false;
		}

		///////////////// Array Manipulation //////////////////////
		// Return 0 for error
		SF_UNITY_EXPORT int SF_CALLING_CONV SF_GetArraySize(SFValueManaged* pmtarget)
		{
			if (!pmtarget) return Failure;
			
			Lock::Locker locker(&SFUnityLock);
			if (pManager)
			{
				return pManager->GetArraySize(pmtarget);
			}
			return Failure;
		}

		SF_UNITY_EXPORT bool SF_CALLING_CONV SF_SetArraySize(SFValueManaged* pmtarget, unsigned int sz)
		{
			if (!pmtarget) return false;
			
			Lock::Locker locker(&SFUnityLock);
			if (pManager)
			{
				return pManager->SetArraySize(pmtarget, sz);
			}
			return false;
		}

		SF_UNITY_EXPORT bool SF_CALLING_CONV SF_GetElement(SFValueManaged* pmtarget, unsigned int idx, SFValueManaged* pmdst)
		{
			if (!pmtarget || !pmdst) return false;
			
			Lock::Locker locker(&SFUnityLock);
			if (pManager)
			{
				return pManager->GetElement(pmtarget, idx, pmdst);
			}
			return false;

		}

		SF_UNITY_EXPORT bool SF_CALLING_CONV SF_SetElement(SFValueManaged* pmtarget, unsigned int idx, SFValueManaged* pmval)
		{
			if (!pmtarget || !pmval) return false;
			
			Lock::Locker locker(&SFUnityLock);

			if (pManager)
			{
				return pManager->SetElement(pmtarget, idx, pmval);
			}
			return false;
		}

		SF_UNITY_EXPORT bool SF_CALLING_CONV SF_PopBack(SFValueManaged* pmtarget, SFValueManaged* pmdst)
		{
			if (!pmtarget || !pmdst) return false;

			Lock::Locker locker(&SFUnityLock);
			if (pManager)
			{
				return pManager->PopBack(pmtarget, pmdst);
			}
			return false;

		}

		SF_UNITY_EXPORT bool SF_CALLING_CONV SF_PushBack(SFValueManaged* pmtarget, SFValueManaged* pmval)
		{
			if (!pmtarget || !pmval) return false;

			Lock::Locker locker(&SFUnityLock);

			if (pManager)
			{
				return pManager->PushBack(pmtarget, pmval);
			}
			return false;
		}

		SF_UNITY_EXPORT bool SF_CALLING_CONV SF_RemoveElement(SFValueManaged* pmtarget, unsigned int idx)
		{
			if (!pmtarget) return false;
			
			Lock::Locker locker(&SFUnityLock);

			if (pManager)
			{
				return pManager->RemoveElement(pmtarget, idx);
			}
			return false;
		}


		SF_UNITY_EXPORT bool SF_CALLING_CONV SF_ClearElements(SFValueManaged* pmtarget)
		{
			if (!pmtarget) return false;
			
			Lock::Locker locker(&SFUnityLock);

			if (pManager)
			{
				return pManager->ClearElements(pmtarget);
			}
			return false;
		}

		/////////////////// Color Transform //////////////////

		SF_UNITY_EXPORT bool SF_CALLING_CONV SF_GetColorTransform(SFValueManaged* pmtarget, SFCxForm* sfcxform)
		{
			if (!pmtarget) return false;
			
			Lock::Locker locker(&SFUnityLock);

			if (pManager)
			{
				return pManager->GetColorTransform(pmtarget, sfcxform);
			}
			return false;
		}

		SF_UNITY_EXPORT bool SF_CALLING_CONV SF_SetColorTransform(SFValueManaged* pmtarget, SFCxForm* sfcxform)
		{
			if (!pmtarget) return false;
			
			Lock::Locker locker(&SFUnityLock);

			// GFx::Value* pvtarget = (GFx::Value*)pmtarget->DataPtr;

			if (pManager)
			{
				return pManager->SetColorTransform(pmtarget, sfcxform);
			}
			return false;
		}

		//////////////////////// TextField ////////////////////////

		SF_UNITY_EXPORT bool SF_CALLING_CONV SF_SetText(SFValueManaged* pmtarget, const char* ptext)
		{
			if (!pmtarget || !ptext) return false;
			
			Lock::Locker locker(&SFUnityLock);

			if (pManager)
			{
				return pManager->SetText(pmtarget, ptext);
			}
			return false;
		}

		SF_UNITY_EXPORT bool SF_CALLING_CONV SF_GetText(SFValueManaged* pmtarget, SFValueManaged* pmdst)
		{
			if (!pmtarget || !pmdst) return false;
			
			Lock::Locker locker(&SFUnityLock);

			if (pManager)
			{
				return pManager->GetText(pmtarget, pmdst);
			}
			return false;
		}
		/////////////////// AS MovieClip Support ///////////////////////

		SF_UNITY_EXPORT bool SF_CALLING_CONV SF_CreateEmptyMovieClip(SFValueManaged* pmtarget, SFValueManaged* pmdst, const char* instanceName, SInt32 depth)
		{
			if (!pmtarget || !pmdst) return false;
			
			Lock::Locker locker(&SFUnityLock);

			if (pManager)
			{
				return pManager->CreateEmptyMovieClip(pmtarget, pmdst, instanceName, depth);
			}
			return false;
		}

		SF_UNITY_EXPORT bool SF_CALLING_CONV SF_AttachMovie(SFValueManaged* pmtarget, SFValueManaged* pmdst, const char* symbolName,
			const char* instanceName, SInt32 depth)
		{
			if (!pmtarget || !pmdst) return false;
			
			Lock::Locker locker(&SFUnityLock);

			if (pManager)
			{
				return pManager->AttachMovie(pmtarget, pmdst, symbolName, instanceName, depth);
			}
			return false;
		}

		SF_UNITY_EXPORT bool SF_CALLING_CONV SF_GotoAndPlayFrame(SFValueManaged* pmtarget, const char* frame)
		{
			if (!pmtarget || !frame) return false;
			
			Lock::Locker locker(&SFUnityLock);

			if (pManager)
			{
				return pManager->GotoAndPlayFrame(pmtarget, frame);
			}
			return false;
		}

		SF_UNITY_EXPORT bool SF_CALLING_CONV SF_GotoAndStopFrame(SFValueManaged* pmtarget, const char* frame)
		{
			if (!pmtarget || !frame) return false;
			
			Lock::Locker locker(&SFUnityLock);

			if (pManager)
			{
				return pManager->GotoAndStopFrame(pmtarget, frame);
			}
			return false;
		}

		SF_UNITY_EXPORT bool SF_CALLING_CONV SF_GotoAndPlay(SFValueManaged* pmtarget, unsigned int frame)
		{
			if (!pmtarget) return false;
			
			Lock::Locker locker(&SFUnityLock);

			if (pManager)
			{
				return pManager->GotoAndPlay(pmtarget, frame);
			}
			return false;	
		}

		SF_UNITY_EXPORT bool SF_CALLING_CONV SF_GotoAndStop(SFValueManaged* pmtarget, unsigned int frame)
		{
			if (!pmtarget) return false;

			Lock::Locker locker(&SFUnityLock);

			if (pManager)
			{
				return pManager->GotoAndStop(pmtarget, frame);
			}
			return false;
		}
#if !defined SF_OS_IPHONE && !defined SF_OS_ANDROID	
		SF_UNITY_EXPORT bool SF_CALLING_CONV SF_CreateFunction(SFValueManaged* context, char* funcName, SFManager::PFNDELEGATEDEF pdelegate, SFValueManaged* pargs)
		{
			if (pManager)
			{
				pdelegate();
				pManager->pfnDelegate = pdelegate;
				return pManager->CreateFunction(context, funcName, &pdelegate, pargs);
			}
			return false;
		}
#endif //defined SF_OS_IPHONE || defined SF_OS_ANDROID
		// MovieDef Exports
		SF_UNITY_EXPORT int	SF_CALLING_CONV	SF_GetVersion(MovieDef* pmovieDef)
		{
			if (pManager)
			{
				return pManager->GetSWFVersion(pmovieDef);
			}
			return Failure;
		}

		SF_UNITY_EXPORT float SF_CALLING_CONV	SF_GetMovieHeight(MovieDef* pmovieDef)
		{
			if (pManager)
			{
				return pManager->GetMovieHeight(pmovieDef);
			}
			return 0;
		}

		SF_UNITY_EXPORT float SF_CALLING_CONV	SF_GetMovieWidth(MovieDef* pmovieDef)
		{
			if (pManager)
			{
				return pManager->GetMovieWidth(pmovieDef);
			}
			return 0;
		}

		SF_UNITY_EXPORT int	SF_CALLING_CONV		SF_GetFrameCount(MovieDef* pmovieDef)
		{
			if (pManager)
			{
				return pManager->GetFrameCount(pmovieDef);
			}
			return 0;
		}

		SF_UNITY_EXPORT float SF_CALLING_CONV	SF_GetFrameRate2(MovieDef* pmovieDef)
		{
			if (pManager)
			{
				return pManager->GetFrameRate(pmovieDef);
			}
			return 0;
		}

		SF_UNITY_EXPORT const char* SF_CALLING_CONV	SF_GetFileURL(MovieDef* pmovieDef, int* size)
		{
			if (pManager)
			{
				const char* fileURL = pManager->GetFileURL(pmovieDef, size);
				return fileURL;
			}
			return NULL;
		}

#if defined (SF_OS_WPHONE8)
	};
}
#endif
}