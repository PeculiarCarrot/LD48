using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeControl : Singleton<TimeControl>
{
	bool hitStopped;
	Coroutine hitstopCoroutine;
	float hitstopDurationLeft;
	float hitstopExitDuration;
	private static Tween timeScaleTween;

	public static void Hitstop(float duration, float exitDuration = 0)
	{
		Instance.DoHitstop(duration, exitDuration);
	}

	public static void SetTimescale(float f, float lerpDuration = 0)
	{
		if (timeScaleTween != null)
			timeScaleTween.Kill();

		if (lerpDuration == 0)
			Time.timeScale = f;
		else
			timeScaleTween = DOTween.To(() => Time.timeScale, x => Time.timeScale = x, f, lerpDuration).SetUpdate(true);
	}

	public void DoHitstop(float duration, float exitDuration = 0)
	{
		if (hitStopped)
		{
			if (duration > hitstopDurationLeft)
			{
				if (exitDuration > hitstopExitDuration)
					hitstopExitDuration = exitDuration;
				hitstopDurationLeft = duration;
			}
		}
		else
		{
			StartHitStop();
			hitstopExitDuration = exitDuration;
			hitstopDurationLeft = duration;
		}
	}

	void StartHitStop()
	{
		hitStopped = true;
		SetTimescale(.001f);
	}

	void EndHitstop()
	{
		hitstopDurationLeft = 0;
		SetTimescale(1f, hitstopExitDuration);
		hitstopExitDuration = 0;
		hitStopped = false;
	}

	private void Update()
	{
		if (hitStopped)
		{
			hitstopDurationLeft -= Time.unscaledDeltaTime;
			if (hitstopDurationLeft <= 0)
			{
				EndHitstop();
			}
		}

		Time.fixedDeltaTime = .02f * Time.timeScale;
	}


}
