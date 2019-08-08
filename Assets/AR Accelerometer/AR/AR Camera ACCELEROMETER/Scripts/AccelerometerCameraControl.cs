// =========================
// MAKAKA GAMES - MAKAKA.ORG
// =========================

using UnityEngine;
using System.Collections;
using UnityEngine.UI;

#if UNITY_EDITOR
using UnityEditor;
#endif

public enum AccelerometerTracking
{
	BackCamera,
	NoneCamera,
	NoneCameraXZRotation
}

[HelpURL("https://makaka.org/category/docs")]
[AddComponentMenu ("AR/AccelerometerCameraControl")]
public class AccelerometerCameraControl : MonoBehaviour 
{	
	// ==============
	// CAMERA OBJECTS
	// ==============

	public Transform cameraTransform;
	public CameraAsBackground cameraAsBackground;
	private WebCamTexture cameraYaw;

	// ===============
	// X & Z ROTATIONS
	// ===============		

	[Header("XZ Settings")]
	[Tooltip("1f => no vibrations")]
	[Range(1f, 20f)]
	public float sensitivityXZ = 5f;

	[Tooltip("if (sensitivityXZ > 1f) => use it for smooth motion")]

	[Range(0f, 5f)]
	public float smoothLimitXZ = 0.5f;

	private float rotationalAngleFactorXZ = -90f;
	private Vector3 currentRotationXZ;	
	private Quaternion resultSharpRotationXZ;

	// ============
	// PLAYER PREFS
	// ============
	public static string trackingWayPlayerPrefsKey = "AccelerometerTracking";	
	
	[Header("Y Settings")]
	public bool IsTrackingWayFromPlayerPrefs = true;

	// ================================
	// CAMERA PARAMETERS / YAW / PIXELS
	// ================================

	public AccelerometerTracking trackingWay = AccelerometerTracking.BackCamera;

	// Save resolutions values after camera initialization
	// For fast acccess 
	private int cameraWidth;
	private int cameraHeight;
	
	[HideInInspector]
	[Range(0.01f, 1f)]
	public float cameraResolutionFactor = 1f; 

	[HideInInspector]
	[Range(0.05f, 1f)]
	public float cameraInsensitivityYaw = 0.3f; 	

	[HideInInspector]
	public int dimensionSensitivityYaw = 64; 	
	private int dimensionSensitivityFactorYaw;

	private int[] dimensionYaw = new int[] {  -4, -3, -2, -1, 0, 1, 2, 3, 4 };
		
	private int pixelRowsForSkippingYaw = 16;	
	private long[,] pixelDifferenceYaw;
	private int minPixelDifferenceXindexYaw;
	private long minPixelDifferenceYaw;

	private int[] previusFrameYaw;
	private int[] currentFrameYaw;

	[HideInInspector]
	public int rotationalSpeedFactorYaw = 32;
	private float currentRotationYaw = 0f;
	private float flatteningYaw = 0f;

	// ========================
	// Y ROTATION - NONE CAMERA
	// ========================
	
	[HideInInspector] 
	public float sensitivityYNoneCamera = 0.11f;  

	// Rotational Speed: Left and Right
	[HideInInspector]
	public float rotationalSpeedFactorYNoneCamera = 50f;
	private float rotationalSpeedYNoneCamera;

	private Vector3 dirNormalized;

	public static void SetTrackingWayWithPlayerPrefs(int trackingWay)
	{
		PlayerPrefs.SetInt(trackingWayPlayerPrefsKey, trackingWay);
	}

	public void InitTrackingWayWithPlayerPrefs()
	{
		if (PlayerPrefs.HasKey(trackingWayPlayerPrefsKey))
		{
			trackingWay = (AccelerometerTracking) PlayerPrefs.GetInt(trackingWayPlayerPrefsKey);
		}
	}

	private void InitCamera()
	{
		cameraYaw = cameraAsBackground.GetWebCamTexture();
					
		cameraAsBackground.ChangeResolutionAndPlay(cameraResolutionFactor);

		cameraWidth = cameraYaw.width;
		cameraHeight = cameraYaw.height;

		pixelRowsForSkippingYaw = Mathf.CeilToInt((float)cameraWidth * cameraInsensitivityYaw);

		InitCameraSensivity();

		pixelDifferenceYaw = new long[dimensionYaw.Length, dimensionYaw.Length];
	}

	private void InitCameraSensivity()
	{
		dimensionSensitivityFactorYaw = (int) (cameraWidth / dimensionSensitivityYaw);

		if (dimensionSensitivityFactorYaw < 2) 
		{
			dimensionSensitivityFactorYaw = 2;
		}
	}

	void Start ()
	{
		if (IsTrackingWayFromPlayerPrefs)
		{
			InitTrackingWayWithPlayerPrefs();
		}

		try
		{
			switch (trackingWay) 
			{
				case AccelerometerTracking.BackCamera:

					InitCamera();

					break;
			}
		}
		catch (System.Exception e)
		{
			Debug.LogWarning("Camera 🎥 is not available: " + e);
		}
	}

	void Update ()
	{	
		RotateY ();
		RotateXZ ();
	}

	private void RotateXZ() 
	{	
		currentRotationXZ.y = cameraTransform.localEulerAngles.y;
		currentRotationXZ.x = Input.acceleration.z * rotationalAngleFactorXZ;
		currentRotationXZ.z = Input.acceleration.x * rotationalAngleFactorXZ;

		resultSharpRotationXZ = Quaternion.Slerp(
			cameraTransform.localRotation, 
			Quaternion.Euler(currentRotationXZ), 
			Time.deltaTime * sensitivityXZ
			);

		if (Quaternion.Angle(cameraTransform.rotation, resultSharpRotationXZ) > smoothLimitXZ)
		{
			cameraTransform.localRotation = resultSharpRotationXZ;
		}
		else
		{
			cameraTransform.localRotation = Quaternion.Slerp(
				cameraTransform.localRotation, 
				Quaternion.Euler(currentRotationXZ), 
				Time.deltaTime
				);
		}
	}

	private void RotateY () 
	{
		switch (trackingWay) 
		{
			case AccelerometerTracking.BackCamera:

				RotateYYaw ();

				#if UNITY_EDITOR

				Debug.LogWarning("Run the build on the mobile device 📲 for the testing camera 🎥 tracking. ");

				#endif

				break;

			case AccelerometerTracking.NoneCamera:

				RotateYNoneCam();
				break;
		}
	}

	private void RotateYNoneCam() 
	{
		rotationalSpeedYNoneCamera = Input.acceleration.x * rotationalSpeedFactorYNoneCamera;

		dirNormalized = Input.acceleration.normalized;

		if (dirNormalized.x >= sensitivityYNoneCamera || dirNormalized.x <= -sensitivityYNoneCamera) 
		{  
			cameraTransform.Rotate (0f, sensitivityYNoneCamera * rotationalSpeedYNoneCamera, 0f); 
		}
	}

	// Calculate Y rotation with camera and analyzing of frames (current and previous)
	// Compare frames to find the correct direction of rotation
	private void RotateYYaw() 
	{
		currentFrameYaw = GetMonochromeColorsFrom(cameraYaw.GetPixels32());

		if (previusFrameYaw != null) 
		{
			currentRotationYaw = CalculateCurrentRotationYaw();
			currentRotationYaw = GetFlatteningAngle (ref flatteningYaw, 10f, currentRotationYaw);            

			cameraTransform.Rotate(Vector3.up, currentRotationYaw * Time.deltaTime * rotationalSpeedFactorYaw);
		}

		previusFrameYaw = currentFrameYaw;
	}

	private float CalculateCurrentRotationYaw()
	{
		minPixelDifferenceXindexYaw = 0;
		minPixelDifferenceYaw = long.MaxValue;

		for (int x = 0; x < dimensionYaw.Length; x++) 
		{
			for (int y = 0; y < dimensionYaw.Length; y++) 
			{
				pixelDifferenceYaw[x,y] = AccumulatePixelDifference(
					dimensionYaw[x] * dimensionSensitivityFactorYaw, 
					dimensionYaw[y] * dimensionSensitivityFactorYaw);

				if (pixelDifferenceYaw[x,y] < minPixelDifferenceYaw) 
				{
					minPixelDifferenceYaw = pixelDifferenceYaw[x,y];
					minPixelDifferenceXindexYaw = x;
				}
			}
		}

		return minPixelDifferenceYaw == 0L ? 0f : dimensionYaw[minPixelDifferenceXindexYaw];
	}

	private float GetFlatteningAngle(ref float flatteningAngle, float speed, float currentAngle) 
	{
		flatteningAngle = flatteningAngle * Mathf.Clamp01(1 - Time.deltaTime * speed) 
			+ currentAngle * Mathf.Clamp01(Time.deltaTime * speed);

		return flatteningAngle;
	}

	// Put the colors in monochrome for tracking
	private int[] GetMonochromeColorsFrom(Color32[] colors) 
	{
		int[] monochromeColors = new int[colors.Length];

		for (int i = 0; i < colors.Length; i++) 
		{
			monochromeColors[i] = colors[i].r * 3 + colors[i].g * 6 + colors[i].b;
		} 
		
		return monochromeColors;
	}

	private long AccumulatePixelDifference(int xCurrentDimension, int yCurrentDimension) 
	{
		long pixelDifferenceAccumulator = 0L;
		const long pixelDifferenceAccumulatorFactor = 1000L;

		int pixelDifferenceAccumulationNumber = 1;
		int index1 = 0;
		int index2 = 0;
		int y = 0;

		while(true) 
		{
			for (int x = 0; x < cameraWidth; x++) 
			{
				index1 = x + y * cameraWidth;

				// If last pixel		
				if (index1 >= currentFrameYaw.Length) 
				{
					return pixelDifferenceAccumulatorFactor * pixelDifferenceAccumulator 
						/ pixelDifferenceAccumulationNumber;
				}

				index2 = x + xCurrentDimension + (y + yCurrentDimension) * cameraWidth; 
				
				// If last pixel
				if (index2 >= previusFrameYaw.Length) 
				{
					return pixelDifferenceAccumulatorFactor * pixelDifferenceAccumulator 
						/ pixelDifferenceAccumulationNumber; 		
				}

				// If shift created position after end => X
				if (x + xCurrentDimension > cameraWidth) 
				{
					break; 					
				}
				
				// If shift created position before start => X
				if (x + xCurrentDimension < 0) 
				{ 
					x -= xCurrentDimension; 
					continue; 
				} 	

				// If shift created position after end => Y
				if (y + yCurrentDimension > cameraHeight) 
				{
					break; 					
				}

				// If shift created position before start => Y
				if (y + yCurrentDimension < 0) 
				{ 
					y -= yCurrentDimension; 
					continue; 
				}

				if (index1 < 0 || index2 < 0) 
				{
					continue;		
				}	

				// Accumulation
				pixelDifferenceAccumulator += Mathf.Abs(currentFrameYaw[index1] - previusFrameYaw[index2]);

				pixelDifferenceAccumulationNumber++;		
			}

			// Skip rows
			y += pixelRowsForSkippingYaw; 										
		}
	}
}

#if UNITY_EDITOR
[CustomEditor(typeof(AccelerometerCameraControl)), CanEditMultipleObjects]
public class AccelerometerCameraControl_Editor : PublisherComponent_Editor
{	
	private AccelerometerCameraControl myTarget;
	
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

		myTarget = target as AccelerometerCameraControl;

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
			case AccelerometerTracking.BackCamera:	

				EditorGUILayout.BeginVertical(headerFlexibleStyle);
				
				EditorGUILayout.PropertyField(dimensionSensitivityYaw, dimensionSensitivityYawText);
				EditorGUILayout.PropertyField(rotationalSpeedFactorYaw, rotationalSpeedFactorYawText);
				EditorGUILayout.PropertyField(cameraResolutionFactor, cameraResolutionFactorText);
				EditorGUILayout.PropertyField(cameraInsensitivityYaw, cameraInsensitivityYawText);

				EditorGUILayout.EndVertical();

				break;

			case AccelerometerTracking.NoneCamera:

				EditorGUILayout.BeginVertical(headerFlexibleStyle);
				
				EditorGUILayout.PropertyField(sensitivityYNoneCamera, sensitivityYNoneCameraText);
				EditorGUILayout.PropertyField(rotationalSpeedFactorYNoneCamera, rotationalSpeedFactorYNoneCameraText);

				EditorGUILayout.EndVertical();
				
				break;
		}
	}
}
#endif