using DG.Tweening;
using UnityStandardAssets.ImageEffects;

public class ImageEffectsHandler : CacheBehaviour
{
	private SunShafts sunShafts;
	private float shaftIntensityAboveGround   = 0f;
	private float shaftIntensityBelowGround   = 1.63f;
	private float shaftTimeToFade             = 1.0f;

	void Start()
	{
		sunShafts = GetComponent<SunShafts>();
	}

	void OnPlayerAboveGround(bool aboveGround)
	{
		// uses the DOTween.To routine, which allows us to call a function of our choosing multiple times,
		// providing it a starting and ending value, and a time over which to tween between those values
		if (aboveGround)
		{
			DOTween.To(AdjustSunShaftIntensity,
					shaftIntensityBelowGround,
					shaftIntensityAboveGround,
					shaftTimeToFade);
		}
		else
		{
			DOTween.To(AdjustSunShaftIntensity,
					shaftIntensityAboveGround,
					shaftIntensityBelowGround,
					shaftTimeToFade);
		}
	}

	void AdjustSunShaftIntensity(float newValue)
	{
		sunShafts.sunShaftIntensity = newValue;
	}

	void OnEnable()
	{
		EventKit.Subscribe<bool>("player above ground", OnPlayerAboveGround);
	}

	void OnDisable()
	{
		EventKit.Unsubscribe<bool>("player above ground", OnPlayerAboveGround);
	}
}
