using UnityEngine;
using UnityEngine.SceneManagement;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class AccelerometerTrackingMenuControl : MonoBehaviour 
{
	public void SetAccelerometerTrackingWay(int tracking)
	{
		AccelerometerCameraControl.SetTrackingWayWithPlayerPrefs(tracking);

		Screen.orientation = ScreenOrientation.Portrait;
		
		SceneManager.LoadScene("Demo_ARCameraACCELEROMETER");
	}

	public void BackToMenu()
	{
		Screen.orientation = ScreenOrientation.Portrait;

		SceneManager.LoadScene("Menu_ARCameraACCELEROMETER");
	}
}

#if UNITY_EDITOR
[CustomEditor(typeof(AccelerometerTrackingMenuControl)), CanEditMultipleObjects]
public class AccelerometerTrackingMenuControl_Editor : PublisherComponent_Editor
{	
	
}
#endif