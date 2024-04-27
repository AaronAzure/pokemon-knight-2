using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class CinemachineShake : MonoBehaviour
{
	public static CinemachineShake Instance {get; private set;}
	[SerializeField] CinemachineVirtualCamera cm;
	private CinemachineBasicMultiChannelPerlin bmcp;
	private float shakeTimer;
	private float shakeTotalTimer;
	private float startingIntensity;

	private void Awake() {
		Instance = this;
	}

    // Start is called before the first frame update
    void Start()
    {
		bmcp = cm.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
    }

	public void ShakeCam(float intensity, float duration)
	{
		if (bmcp != null)
		{
			bmcp.m_AmplitudeGain = startingIntensity = intensity;
			shakeTimer = shakeTotalTimer = duration;
		}
	}

	void FixedUpdate() 
	{
		if (shakeTimer > 0)
		{
			shakeTimer -= Time.fixedDeltaTime;

			bmcp.m_AmplitudeGain = 
				Mathf.Lerp(startingIntensity, 0f, 1 - (shakeTimer/shakeTotalTimer));
		}	
	}
}
