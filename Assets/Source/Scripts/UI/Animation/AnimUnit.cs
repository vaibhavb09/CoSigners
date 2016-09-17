using UnityEngine;
using System.Collections;

public class AnimUnit : MonoBehaviour {

	public AnimUnitDelegate OnAnimationFinished;
	protected bool _isPlaying = false;
	protected float _currentTimer;
	protected float _animationLength = 1.0f; //default value
	protected int _timesWePlay = 0;
	protected int _playedTimes = 0;
	protected string _animationName;
	

	public void Play(string i_animName, int i_timesWePlay = 1)
	{
		_isPlaying = true;
		_currentTimer = 0.0f;
		_timesWePlay = i_timesWePlay;
		_playedTimes = 0;
		_animationName = i_animName;
	}

	public void StopAndRemoveFromDictionary()
	{
		Stop();
		AnimControl.RemoveAnimation(_animationName, this);
	}

	public void Stop()
	{
		_isPlaying = false;
		_currentTimer = 0.0f;
		_timesWePlay = 0;
		Destroy(this.gameObject);
	}

	public virtual void AnimationLoop(float i_tick)
	{

	}

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {

		if(_isPlaying)
		{
			AnimationLoop(_currentTimer);
			_currentTimer += Time.deltaTime;
			if(_currentTimer > _animationLength)
			{
				if(_timesWePlay == 0) //looping
				{
					_currentTimer = 0.0f;
				}
				else
				{
					++_playedTimes;
					if(_playedTimes >= _timesWePlay)
					{
						StopAndRemoveFromDictionary(); //perform self-descruct
					}
					else
					{
						_currentTimer = 0.0f;
					}
				}
				if(OnAnimationFinished != null)
					OnAnimationFinished();
			}
		}
	}
}
