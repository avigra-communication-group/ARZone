#if UNITY_EDITOR

using UnityEngine;
using UnityEditor;
using System.Diagnostics;

public class PublisherComponent_Editor : Editor
{
	private string messageHelp = 
		"You can find docs in Assets folder. Online version is also available with help icon.";
	
	private int headerWidthCorrectionForScaling = 38;
	public string headerFlexibleStyle = "Box";

	private Texture2D header;

	public Color backgroundColorByDefault;

	public Texture2D iconPlay;
	public Texture2D iconBlur;
	public Texture2D iconGradientRainbow;
	public Texture2D iconGradientRed;
	public Texture2D iconGradientTurquoise;
	public Texture2D iconGradientYellow;
	public Texture2D iconBlue;
	public Texture2D iconBrightness;
	public Texture2D iconClipDown;
	public Texture2D iconClipLeft;
	public Texture2D iconClipRight;
	public Texture2D iconClipUp;
	public Texture2D iconColor;
	public Texture2D iconContrast;
	public Texture2D iconCorner1;
	public Texture2D iconCorner2;
	public Texture2D iconCorner3;
	public Texture2D iconCorner4;
	public Texture2D iconDistortion;
	public Texture2D iconEdge;
	public Texture2D iconFade;
	public Texture2D iconGreen;
	public Texture2D iconInout;
	public Texture2D iconPixel;
	public Texture2D iconRed;
	public Texture2D iconSeed;
	public Texture2D iconSize;
	public Texture2D iconSizeX;
	public Texture2D iconSizeY;
	public Texture2D iconTime;
	public Texture2D iconValue;
	public Texture2D iconSkip;
	public Texture2D iconCameraRotate;
	public Texture2D iconCamera;
	public Texture2D iconGear;
	public Texture2D iconDoubleTap;
	public Texture2D iconFingerprint;
	public Texture2D iconTouch;

	public virtual void OnEnable()
	{
		backgroundColorByDefault = EditorGUIUtility.isProSkin
     		? new Color(56, 56, 56, 255)
     		: Color.white;

		header = Resources.Load ("ComponentHeader") as Texture2D;

		iconPlay = Resources.Load ("icon-play") as Texture2D;
		iconBlur = Resources.Load ("icon-blur") as Texture2D;
		iconGradientRainbow = Resources.Load ("icon-gradient-rainbow") as Texture2D;
		iconGradientRed = Resources.Load ("icon-gradient-red") as Texture2D;
		iconGradientTurquoise = Resources.Load ("icon-gradient-turquoise") as Texture2D;
		iconGradientYellow = Resources.Load ("icon-gradient-yellow") as Texture2D;
		iconBlue = Resources.Load ("icon-blue") as Texture2D;
		iconBrightness= Resources.Load ("icon-brightness") as Texture2D;
		iconClipDown = Resources.Load ("icon-clip_down") as Texture2D;
		iconClipLeft = Resources.Load ("icon-clip_left") as Texture2D;
		iconClipRight = Resources.Load ("icon-clip_right") as Texture2D;
		iconClipUp = Resources.Load ("icon-clip_up") as Texture2D;
		iconColor = Resources.Load ("icon-color") as Texture2D;
		iconContrast = Resources.Load ("icon-contrast") as Texture2D;
		iconCorner1 = Resources.Load ("icon-corner-1") as Texture2D;
		iconCorner2 = Resources.Load ("icon-corner-2") as Texture2D;
		iconCorner3 = Resources.Load ("icon-corner-3") as Texture2D;
		iconCorner4 = Resources.Load ("icon-corner-4") as Texture2D;
		iconDistortion = Resources.Load ("icon-distortion") as Texture2D;
		iconEdge = Resources.Load ("icon-edge") as Texture2D;
		iconFade = Resources.Load ("icon-fade") as Texture2D;
		iconGreen = Resources.Load ("icon-green") as Texture2D;
		iconInout = Resources.Load ("icon-inout") as Texture2D;
		iconPixel = Resources.Load ("icon-pixel") as Texture2D;
		iconRed = Resources.Load ("icon-red") as Texture2D;
		iconSeed = Resources.Load ("icon-seed") as Texture2D;
		iconSize = Resources.Load ("icon-size") as Texture2D;
		iconSizeX = Resources.Load ("icon-size_x") as Texture2D;
		iconSizeY = Resources.Load ("icon-size_y") as Texture2D;
		iconTime = Resources.Load ("icon-time") as Texture2D;
		iconValue = Resources.Load ("icon-value") as Texture2D;
		iconSkip = Resources.Load ("icon-skip") as Texture2D;
		iconCameraRotate = Resources.Load ("icon-camera-rotate") as Texture2D;
		iconCamera = Resources.Load ("icon-camera") as Texture2D;
		iconGear = Resources.Load ("icon-gear") as Texture2D;
		iconDoubleTap = Resources.Load ("icon-double-tap") as Texture2D;
		iconFingerprint = Resources.Load ("icon-fingerprint") as Texture2D;
		iconTouch = Resources.Load ("icon-touch") as Texture2D;
	}

	public override void OnInspectorGUI()
	{	
		DrawEditorByDefaultWithHeaderAndHelpBox();
	}

	public void DrawEditorByDefaultWithHeaderAndHelpBox()
	{
		serializedObject.UpdateIfRequiredOrScript();

		DrawHeaderByDefault();

		GUI.backgroundColor = backgroundColorByDefault;
		DrawDefaultInspector();

		DrawHelpBoxByDefault();

		serializedObject.ApplyModifiedProperties();
	}

	public void DrawHeaderByDefault()
	{
		DrawHeaderFlexible(header, Color.black);
	}
	
	public void DrawHeaderFlexible(Texture2D header, Color backgroundColor)
	{
		if (header)
		{
			GUI.backgroundColor = backgroundColor;

			if (header.width + headerWidthCorrectionForScaling < EditorGUIUtility.currentViewWidth)
			{
				EditorGUILayout.BeginVertical(headerFlexibleStyle);
				
				DrawHeader(header);
				
				EditorGUILayout.EndVertical();
			}
			else
			{
				DrawHeaderIfScrollbar(header);
			}
		}
	}

	public void DrawHeaderIfScrollbar(Texture2D header)
	{
		EditorGUI.DrawTextureTransparent(
					GUILayoutUtility.GetRect(
						EditorGUIUtility.currentViewWidth - headerWidthCorrectionForScaling, 
						header.height), 
					header,
					ScaleMode.ScaleToFit);
	}

	public void DrawHeader(Texture2D header)
	{
		EditorGUI.DrawTextureTransparent(
					GUILayoutUtility.GetRect(
						header.width, 
						header.height), 
					header,
					ScaleMode.ScaleToFit);
	}

	public void DrawHelpBoxByDefault()
	{
		DrawHelpBox(messageHelp, backgroundColorByDefault);
	}

	public void DrawHelpBox(string message, Color color)
	{
		GUI.backgroundColor = color;

		EditorGUILayout.HelpBox(
			message,
			MessageType.Info);
	}
}
#endif