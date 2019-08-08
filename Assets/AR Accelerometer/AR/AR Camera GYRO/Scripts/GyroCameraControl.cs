/*
============================
Unity Assets by MAKAKA GAMES
============================

Online Docs: https://makaka.org/unity-assets
Offline Docs: You have a PDF file in the package folder.

=======
SUPPORT
=======

First of all, read the docs. If it didn’t help, get the support.

Web: https://makaka.org/support
Email: info@makaka.org

If you find a bug or you can’t use the asset as you need, 
please first send email to info@makaka.org (in English or in Russian) 
before leaving a review to the asset store.

I am here to help you and to improve my products for the best.
*/

using UnityEngine;
using UnityEngine.Events;

[HelpURL("https://makaka.org/unity-assets")]
[AddComponentMenu ("AR/GyroCameraControl")]
public class GyroCameraControl : MonoBehaviour 
{
	private Gyroscope gyro;
	private bool gyroSupported;
	private Quaternion rotationFix = new Quaternion(0f, 0f, 1f, 0f);

	public Transform gyroCamera;

	[Tooltip("If it's 'true' then Gyro's 'Y' Rotation is resetted on Scene Closing or Reloading.")]
	public bool IsGyroDisabledOnDestroy = false;
	public UnityEvent OnGyroIsNotSupported;

	void Start () 
	{
		gyroSupported = SystemInfo.supportsGyroscope;

		gyroCamera.parent.transform.rotation = Quaternion.Euler (90f, 180f, 0f);

		if (gyroSupported) 
		{
			gyro = Input.gyro;
			gyro.enabled = true;
		}
		else
		{
			//Your Logic
			OnGyroIsNotSupported.Invoke();
		}
	}

	void Update () 
	{
		if (gyroSupported)
		{
			gyroCamera.localRotation = gyro.attitude * rotationFix;
		}
	}

	private void OnDestroy()
	{
		if (gyro != null && IsGyroDisabledOnDestroy)
		{
			gyro.enabled = false;
			
			//print("Reset Gyro!");
		} 
	}
}
