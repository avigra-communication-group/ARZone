// =========================
// MAKAKA GAMES - MAKAKA.ORG
// =========================

using UnityEngine;
using System.Collections;
using UnityEngine.UI;

#if UNITY_EDITOR
using UnityEditor;
#endif

public enum AccelerometerTrackingTest
{
	BackCamera,
	NoneCamera,
	NoneCameraXZRotation
}

[HelpURL("https://makaka.org/category/docs")]
//[AddComponentMenu ("Publisher/PublisherComponentExample")]
public class PublisherComponentExample : MonoBehaviour 
{	
	[Header("XZ Settings")]
	[Tooltip("1f => no vibrations")]
	[Range(1f, 20f)]
	public float sensitivityXZ = 5f;

	[Tooltip("if (sensitivityXZ > 1f) => use it for smooth motion")]

	[Range(0f, 5f)]
	public float smoothLimitXZ = 0.5f;

	public static string trackingWayPlayerPrefsKey = "AccelerometerTrackingTest";	
	
	[Header("Y Settings")]
	public bool IsLoadFromPlayerPrefs = true;

	public AccelerometerTrackingTest
 trackingWay = AccelerometerTrackingTest
.BackCamera;
	
	[HideInInspector]
	[Range(0.01f, 1f)]
	public float cameraResolutionFactor = 1f; 

	[HideInInspector]
	[Range(0.05f, 1f)]
	public float cameraInsensitivityYaw = 0.3f; 	

	[HideInInspector]
	public int dimensionSensitivityYaw = 64; 	

	[HideInInspector]
	public int rotationalSpeedFactorYaw = 32;
	
	[HideInInspector] 
	public float sensitivityYNoneCamera = 0.11f;  

	[HideInInspector]
	public float rotationalSpeedFactorYNoneCamera = 50f;
}

#if UNITY_EDITOR
[CustomEditor(typeof(PublisherComponentExample)), CanEditMultipleObjects]
public class PublisherComponent_Example_Editor : PublisherComponent_Editor
{	
	private PublisherComponentExample myTarget;
	
	private SerializedProperty cameraInsensitivityYaw;
	private SerializedProperty dimensionSensitivityYaw;
	private SerializedProperty rotationalSpeedFactorYaw;
	private SerializedProperty cameraResolutionFactor;
	private SerializedProperty sensitivityYNoneCamera;
	private SerializedProperty rotationalSpeedFactorYNoneCamera;

	private GUIContent cameraInsensitivityYawText;	
	private GUIContent dimensionSensitivityYawText;
	private GUIContent rotationalSpeedFactorYawText;
	private GUIContent cameraResolutionFactorText;
	private GUIContent sensitivityYNoneCameraText;
	private GUIContent rotationalSpeedFactorYNoneCameraText;
	
	public override void OnEnable()
    {
		base.OnEnable();

		myTarget = target as PublisherComponentExample;

		cameraInsensitivityYaw = serializedObject.FindProperty("cameraInsensitivityYaw");
		dimensionSensitivityYaw = serializedObject.FindProperty("dimensionSensitivityYaw");
		rotationalSpeedFactorYaw = serializedObject.FindProperty("rotationalSpeedFactorYaw");
    	cameraResolutionFactor = serializedObject.FindProperty("cameraResolutionFactor");
		sensitivityYNoneCamera = serializedObject.FindProperty("sensitivityYNoneCamera");
		rotationalSpeedFactorYNoneCamera = serializedObject.FindProperty("rotationalSpeedFactorYNoneCamera");

		cameraInsensitivityYawText = new GUIContent(
			"Insensitivity", iconSkip, "Camera Insensitivity for Yaw");
		dimensionSensitivityYawText = new GUIContent(
			"Dimension Sensitivity", iconTouch, "Dimension Sensitivity for Yaw");
		rotationalSpeedFactorYawText = new GUIContent(
			"Rotational Speed Y", iconCameraRotate, "Rotational Speed Factor for Yaw");
		cameraResolutionFactorText = new GUIContent(
			"Resolution Factor", iconCamera, "Camera Resolution Factor for Yaw and for Game Texture");
		sensitivityYNoneCameraText = new GUIContent(
			"Sensitivity Y", iconTouch);
		rotationalSpeedFactorYNoneCameraText = new GUIContent(
			"Rotational Speed Y", iconCameraRotate, "Rotational Speed Y Factor");
    }

	public override void OnInspectorGUI()
	{	
		serializedObject.UpdateIfRequiredOrScript();
		
		DrawHeaderByDefault();

		GUI.backgroundColor = backgroundColorByDefault;
		DrawDefaultInspector();
		
		DrawBody(Color.white);

		DrawHelpBoxByDefault();

		serializedObject.ApplyModifiedProperties();
	}

	private void DrawBody(Color backgroundColor)
	{
		GUI.backgroundColor = backgroundColor;

		switch (myTarget.trackingWay) 
		{
			case AccelerometerTrackingTest
		.BackCamera:	

				EditorGUILayout.BeginVertical(headerFlexibleStyle);
				
				EditorGUILayout.PropertyField(dimensionSensitivityYaw, dimensionSensitivityYawText);
				EditorGUILayout.PropertyField(rotationalSpeedFactorYaw, rotationalSpeedFactorYawText);
				EditorGUILayout.PropertyField(cameraResolutionFactor, cameraResolutionFactorText);
				EditorGUILayout.PropertyField(cameraInsensitivityYaw, cameraInsensitivityYawText);

				EditorGUILayout.EndVertical();

				break;

			case AccelerometerTrackingTest
		.NoneCamera:

				EditorGUILayout.BeginVertical(headerFlexibleStyle);
				
				EditorGUILayout.PropertyField(sensitivityYNoneCamera, sensitivityYNoneCameraText);
				EditorGUILayout.PropertyField(rotationalSpeedFactorYNoneCamera, rotationalSpeedFactorYNoneCameraText);

				EditorGUILayout.EndVertical();
				
				break;
		}
	}
}
#endif