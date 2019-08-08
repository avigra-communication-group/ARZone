/*
============================
Unity Assets by MAKAKA GAMES
============================

Online Docs: https://makaka.org/category/docs/
Offline Docs: You have a PDF file in the package folder.

=======
SUPPORT
=======

First of all, read the docs. If it didn’t help, get the support.

Web: https://makaka.org/support/
Email: info@makaka.org

If you find a bug or you can’t use the asset as you need, 
please first send email to info@makaka.org (in English or in Russian) 
before leaving a review to the asset store.

I am here to help you and to improve my products for the best.
*/

using UnityEngine;

[HelpURL("https://makaka.org/category/docs/")]
[AddComponentMenu ("Makaka Games/Everyday Tools/MirrorFlipCamera")]
[RequireComponent(typeof(Camera))]
public class MirrorFlipCamera : MonoBehaviour 
{
	private Camera cameraCurrent;
	public bool flipHorizontal;
	private Vector3 currentScale;

	void Start () 
	{
		cameraCurrent = GetComponent<Camera>();
	}

	void OnPreCull() 
	{
		cameraCurrent.ResetWorldToCameraMatrix();
		cameraCurrent.ResetProjectionMatrix();

		currentScale = new Vector3(flipHorizontal ? -1 : 1, 1, 1);
		
		cameraCurrent.projectionMatrix = cameraCurrent.projectionMatrix * Matrix4x4.Scale(currentScale);
	}

	void OnPreRender () 
	{
		GL.invertCulling = flipHorizontal;
	}
	
	void OnPostRender () 
	{
		GL.invertCulling = false;
	}
}