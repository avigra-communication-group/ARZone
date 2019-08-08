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
using UnityEngine.SceneManagement;

#if UNITY_EDITOR
using UnityEditor;
#endif

[HelpURL("https://makaka.org/category/docs")]
[AddComponentMenu ("Makaka Games/Everyday Tools/Scene Control/Menu Scene Control")]
public class MenuSceneControl : MonoBehaviour 
{	
	public static string menuSceneName = "Menu_ARSpaceShooter_Landscape";
	public static string gameSceneName = "Game_ARSpaceShooter_Landscape";

	public void LoadMenu()
	{
		LoadScene(menuSceneName);
	}

	public void LoadGame()
	{
		LoadScene(gameSceneName);
	}

	public void LoadScene(string sceneName)
	{
		LoadScreenControl.Instance.LoadScene(sceneName);
	}

	public void LoadSceneWithScreenOrientationLandscapeLeft(string sceneName)
	{
		Screen.orientation = ScreenOrientation.LandscapeLeft;

		LoadScene(sceneName);
	}

	public void LoadSceneWithScreenOrientationPortrait(string sceneName)
	{
		Screen.orientation = ScreenOrientation.Portrait;

		LoadScene(sceneName);
	}

	public void ReloadCurrentScene()
	{
		LoadScene(SceneManager.GetActiveScene().name);
	}
}