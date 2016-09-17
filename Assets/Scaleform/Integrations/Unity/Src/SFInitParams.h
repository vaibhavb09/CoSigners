
/**************************************************************************

Filename    :   SFManager.cpp
Content     :   Implementation of SFInitParams
Created     :   Sep 01, 2011
Authors     :	Ankur
Copyright   :   Copyright 2011 Autodesk, Inc. All Rights reserved.

Use of this software is subject to the terms of the Autodesk license
agreement provided at the time of installation or download, or which
otherwise accompanies this software in either electronic or hard copy form.

**************************************************************************/

#ifndef INC_SF_INITPARAMS_H
#define INC_SF_INITPARAMS_H

namespace NMVideoSoundSystem
{
	// Note that some compilers support strongly typed enums (VideoSoundSystem:char), but not all,
	// so we'll stick with int sized enums (the default)
	enum VideoSoundSystem
	{
		SystemSound = 0,
		FMod,
		WWise,
		Default
	};
}

namespace NMInitIME
{
	enum InitIME
	{
		Yes = 0,
		No
	};
}

namespace NMEnableProgressiveLoading
{
	enum EnableProgressiveLoading
	{
		Yes = 0,
		No
	};
}

namespace NMEnableAmpProfiling
{
	enum EnableAmpProfiling
	{
		Yes = 0,
		No
	};
}

class SFInitParams
{
public:
	enum ASVersion
	{
		AS2 = 0,
		AS3,
		Both
	};
	ASVersion TheASVersion;

	bool InitVideo;

	bool InitSound;

	bool UseDynamicShaderCompilation;

	NMVideoSoundSystem::VideoSoundSystem TheVideoSoundSystem;

	bool InitIME;

	NMEnableAmpProfiling::EnableAmpProfiling IfEnableAmpProfiling;

	NMEnableProgressiveLoading::EnableProgressiveLoading ProgLoading;
	class FontCacheConfig
	{
	public:
		int TextureHeight;
		int TextureWidth;
		int MaxNumTextures;
		int MaxSlotHeight;
	} ;
	FontCacheConfig TheFontCacheConfig;
	bool SetFontCacheParams;
	bool EnableDynamicCache;

	class FontPackParams
	{
	public:
		int NominalSize;
		int PadPixels;
		int TextureWidth;
		int TextureHeight;
	};
	FontPackParams mFontPackParams;
	bool	IsSetFontPackParams;
	int		GlyphCountLimit;
	float	SoundVolume;
	bool	IsMute;
	
	enum SupportImageFormat
	{
		Default = 0xffffffff,
		JPG = 1,
		PNG = 1<<1, 
		DDS = 1<<2,
		TGA = 1<<3, 
		SIF = 1<<4, 
		PVR = 1<<5, 
		KTX = 1<<6, 
		GXT = 1<<7,
		GTX = 1<<8,
		GNF = 1<<9
	};
	int TheImageFormat;
	int		Sentinal;
};

#endif
