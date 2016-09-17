using UnityEngine;
using System.Collections;

public class TracerAnimation : MonoBehaviour {
	
	float _delayTime;
	float _calibratingTime;
	float _activeTime;
	float _warningIntervel = 0.5f;
	
	float _line1CalibratingTime;
	float _line2CalibratingaTime;
	
	float _line1DelayTime;
	float _line2DelayTime;
	
	public GameObject Accent01;
	public GameObject Accent02;
	public GameObject Line01;
	public GameObject Line02;
	public GameObject Line03;
	public GameObject[] LoadBar;
	
	public float[] _loadBarProgress = {0,0,0,0,0,0,0,0};
	
	float _lastCalibratePercentage;
	float _line1ColorChangingFactor = 0.95f;
	float _line2ColorChangingFactor = 0.97f;
	float _line3ColorChangingFactor = 0.98f;
	
	//Line colors
	Color _colorShutdown = new Color(0.0f, 0.5f, 1.0f, 1.0f);
	Color _colorInactive = new Color(0.0f, 1.0f, 1.0f, 1.0f);
	Color _colorPartialActive =  new Color(0.5f, 1.0f, 1.0f, 1.0f);
	Color _colorActive = new Color(1.0f, 1.0f, 1.0f, 1.0f);
	Color _colorInactiveInvisible = new Color(0.0f, 1.0f, 1.0f, 0.0f);
	
	//LineRotation
	Vector3 _rotationOffset = new Vector3(0.0f, 0.0f, 0.0f);
	Quaternion _initRotation;
	
	
	float _accent1RotationSpeed = -60.0f;
	float _accent2RotationSpeed = 100.0f;
	
	float _currentTime;
	bool _delayState = true;
	bool _calState = false;
	bool _activeState = false;
	
	public void SetTimer(float i_delayTime, float i_calibrateTime, float i_activeTime)
	{
		_delayTime = i_delayTime;
		_line1DelayTime = i_delayTime/3;
		_line2DelayTime = 2 * i_delayTime/3;
		
		_calibratingTime = i_calibrateTime;
		_line1CalibratingTime = i_calibrateTime/3;
		_line2CalibratingaTime = 2 * i_calibrateTime/3;
		
		_activeTime = i_activeTime;
		_initRotation = Line01.transform.rotation;
		//_initRotation = gameObject.transform.rotation.eulerAngles;
		//_finalRotation = _initRotation + new Vector3(0.0f, 360.0f, 0.0f);
	}
	
	public void SetDelayAnimation(float i_delayTicker)
	{
		float delayPercentage = i_delayTicker/_delayTime;
		float line1Delay = delayPercentage * 3;
		float line2Delay = delayPercentage * 1.5f;
		line1Delay = line1Delay > 1? 1: line1Delay;
		line2Delay = line2Delay > 1? 1: line2Delay;
		
		Line01.renderer.material.color = Color.Lerp(_colorShutdown, _colorInactive, line1Delay);
		Line02.renderer.material.color = Color.Lerp(_colorShutdown, _colorInactive, line2Delay);
		Line03.renderer.material.color = Color.Lerp(_colorShutdown, _colorInactive, delayPercentage);
		
		SetLoadBarProgress(delayPercentage);
	
		//for(int i = 0; i < 16 ; ++i)
		//{
		//	LoadBar[i].renderer.material.color = Color.Lerp(_colorInactiveInvisible, _colorInactive, _loadBarProgress[(int)(i/2)]);
		//}
			
	}
	
	// I can make this dynamic but that would need a lot of math operation, since we are not gonna change it, 
	// let's just hardcode this so it doesn't cost that much performance
	private void SetLoadBarProgress(float i_percentage)
	{
		if(i_percentage < 0.125f)
			_loadBarProgress[0] = i_percentage * 8;
		else if(i_percentage >= 0.125f && i_percentage < 0.25f)
			_loadBarProgress[1] = (i_percentage - 0.125f) * 8;
		else if(i_percentage >= 0.25f && i_percentage < 0.375f)
			_loadBarProgress[2] = (i_percentage - 0.25f) * 8;
		else if(i_percentage >= 0.375f && i_percentage < 0.5f)
			_loadBarProgress[3] = (i_percentage - 0.375f) * 8;
		else if(i_percentage >= 0.5f && i_percentage < 0.625f)
			_loadBarProgress[4] = (i_percentage - 0.5f) * 8;
		else if(i_percentage >= 0.625f && i_percentage < 0.75f)
			_loadBarProgress[5] = (i_percentage - 0.625f) * 8;
		else if(i_percentage >= 0.75f && i_percentage < 0.875f)
			_loadBarProgress[6] = (i_percentage - 0.75f) * 8;
		else if(i_percentage >= 0.875f && i_percentage < 0.99f)
			_loadBarProgress[7] = (i_percentage - 0.875f) * 8;
	}
	
	public void ReSetLoadBarProgress()
	{
		for(int i = 0;  i< 8; i++)
		{
			_loadBarProgress[i] = 0;
		}
	}
	
	public void SetCalibrateAnimation(float i_calibratingTicker)
	{
		float calPercentage = i_calibratingTicker/_calibratingTime;
		float line1Cal = calPercentage * 3;
		float line2Cal = calPercentage * 1.5f;
		float line3Cal = calPercentage;
		line1Cal = line1Cal > 1? 1: line1Cal;
		line2Cal = line2Cal > 1? 1: line2Cal;
		
		float Diff = calPercentage - _lastCalibratePercentage;
		float line1Diff = (line1Cal == 1? 0: Diff * 3);
		float line2Diff = (line2Cal == 1? 0: Diff * 1.5f);
		
		line1Cal = (line1Cal - _line1ColorChangingFactor) / (1- _line1ColorChangingFactor);
		line1Cal = line1Cal < 0? 0: line1Cal;
		line2Cal = (line2Cal - _line2ColorChangingFactor) / (1- _line2ColorChangingFactor);
		line2Cal = line2Cal < 0? 0: line2Cal;
		line3Cal = (line3Cal - _line3ColorChangingFactor) / (1- _line3ColorChangingFactor);
		line3Cal = line3Cal < 0? 0: line3Cal;		
		


		Line01.renderer.material.color = Color.Lerp(_colorInactive, _colorActive, line1Cal);
		_rotationOffset.z = 360 * line1Diff;
		Line01.transform.Rotate(_rotationOffset);// = Vector3.Lerp(_initRotation, _finalRotation, line1Cal);
		
		Line02.renderer.material.color = Color.Lerp(_colorInactive, _colorActive, line2Cal);
		_rotationOffset.z = -360 * line2Diff;
		Line02.transform.Rotate(_rotationOffset);// =  Vector3.Lerp(_initRotation, _finalRotation, line2Cal);
		
		Line03.renderer.material.color = Color.Lerp(_colorInactive, _colorActive, line3Cal);
		_rotationOffset.z = 360 * Diff;
		Line03.transform.Rotate(_rotationOffset);// = Vector3.Lerp(_initRotation, _finalRotation, calPercentage);
		
		Accent01.renderer.material.color = Color.Lerp(_colorInactive, _colorActive, calPercentage);
		Accent02.renderer.material.color = Color.Lerp(_colorInactive, _colorActive, calPercentage);
		
		SetLoadBarProgress(calPercentage);
		if(_delayTime < 0.01f)
		{
			for(int i = 0; i < 16 ; ++i)
			{
				LoadBar[i].renderer.material.color = Color.Lerp(_colorInactiveInvisible, _colorInactive, _loadBarProgress[(int)(i/2)]);
			}
		}
		else
		{
			for(int i = 0; i < 16 ; ++i)
			{
				LoadBar[i].renderer.material.color = Color.Lerp(_colorInactiveInvisible, _colorInactive, _loadBarProgress[(int)(i/2)]);
			}
		}
		
		_lastCalibratePercentage = calPercentage;
	}
	
	public void SetActiveAnimation(float i_activeTicker)
	{
		Line01.transform.rotation = _initRotation;
		Line02.transform.rotation = _initRotation;
		Line03.transform.rotation = _initRotation;
		
		float activePercentage = i_activeTicker/_activeTime;
		float warningPercentage = 1 - (i_activeTicker%_warningIntervel) / _warningIntervel;
		
		Line01.renderer.material.color = Color.Lerp(_colorPartialActive, _colorActive, warningPercentage);
		
		Line02.renderer.material.color = Color.Lerp(_colorPartialActive, _colorActive, warningPercentage);
		
		Line03.renderer.material.color = Color.Lerp(_colorPartialActive, _colorActive, warningPercentage);
		
		Accent01.renderer.material.color = Color.Lerp(_colorActive, _colorInactive, activePercentage);
		Accent02.renderer.material.color = Color.Lerp(_colorActive, _colorInactive, activePercentage);
		
		SetLoadBarProgress(activePercentage);
	
		for(int i = 0; i < 16 ; ++i)
		{
			LoadBar[i].renderer.material.color = Color.Lerp(_colorActive, _colorInactiveInvisible, _loadBarProgress[(int)(i/2)]);
		}		
	}
	
	// Use this for initialization
	void Start () {
		if(Application.loadedLevel == 0)
		{
			SetTimer(3.0F, 4.0f, 4.0f);
			_currentTime = 0.0f;
		}
	}
	
	// Update is called once per frame
	void Update () {

		Accent01.transform.Rotate(0, 0, _accent1RotationSpeed*Time.deltaTime);
		Accent02.transform.Rotate(0, 0, _accent2RotationSpeed*Time.deltaTime);
		
		#region ForTesting
		if(Application.loadedLevel == 0)
		{
			_currentTime += Time.deltaTime;
			if(_currentTime > _delayTime && _delayState)
			{
				_currentTime = 0.0f;
				_delayState = false;
				_calState = true;
				ReSetLoadBarProgress();
			}
			
			
			if(_currentTime > _calibratingTime && _calState)
			{
				_currentTime = 0.0f;
				_calState = false;
				_activeState = true;
				ReSetLoadBarProgress();
			}
			
			if(_currentTime > _activeTime && _activeState)
			{
				_currentTime = 0.0f;
				_delayState = true;
				_activeState = false;
				ReSetLoadBarProgress();
			}		
			
			if(_delayState)
			{
				SetDelayAnimation(_currentTime);
			}
			
			if(_calState)
			{
				SetCalibrateAnimation(_currentTime);
			}
			
			if(_activeState)
			{
				SetActiveAnimation(_currentTime);
				_lastCalibratePercentage = 0.0f;
			}
		}
		#endregion
	}
}
