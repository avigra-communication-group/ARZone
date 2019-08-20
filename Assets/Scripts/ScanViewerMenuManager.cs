// Script ini mengatur kontrol ui untuk scene scanviewer.

using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using DG.Tweening;

public class ScanViewerMenuManager : MonoBehaviour
{
    private const string V = "target";

    // UI elements.
    public Canvas canvas;
    public Canvas watermarkCanvas;

    public GameObject screenshotPanel;
    public RectTransform socialMediaButtonsPanel;

    public Button backHomeButton;
    public Button screenshotButton;
    public Button saveSSButton;
    public Button backFromSSButton;

    public Button twitterShareButton;
    public Button instagramShareButton;
    public Button facebookShareButton;

    public RawImage screenshotPlaceHolder;

    private Texture2D ss;
    private bool screenshotPanelActive = false;

    private const string TWITTER = "com.twitter.android";
    private const string FACEBOOK = "com.facebook.katana";
    private const string INSTAGRAM = "com.instagram.android";

    private void Start() {
        Init();
    }

    private void Init() {
        // Important!       
        ARCameraTargetPicker.togglePick = false;
        
        screenshotPanelActive = screenshotPanel.activeInHierarchy;
        socialMediaButtonsPanel.DOMoveY(-Screen.width / 2, 0f, false);

        saveSSButton.onClick.AddListener(SaveSSToGallery);
        backFromSSButton.onClick.AddListener(CloseScreenshotPanel);
        backHomeButton.onClick.AddListener(BackToHome);
        screenshotButton.onClick.AddListener(ScreenshootButton);

        twitterShareButton.onClick.AddListener(() => ShareSSButton("twitter"));
        instagramShareButton.onClick.AddListener(() => ShareSSButton("instagram"));
        facebookShareButton.onClick.AddListener(() => ShareSSButton("facebook"));
    }

    public void ScreenshootButton() {
        // Deactivate if media picker running;
        if (NativeGallery.IsMediaPickerBusy())
        {
            return;
        }

        // Take screenshot
        StartCoroutine(TakeScreenShotAndShare());

        // Activate the social media buttons panel
        ViewSocialMediaButtonsPanel(true);
    }

    public void ShareSSButton(string type) {
        if(ss == null) {
            return;
        }
        StartCoroutine(ShareSSCoroutine(type));
    }

    private IEnumerator TakeScreenShotAndShare() {
        canvas.enabled = false;
        watermarkCanvas.enabled = true;

        yield return new WaitForEndOfFrame();

        ss = new Texture2D(Screen.width, Screen.height, TextureFormat.RGB24, false);
        ss.ReadPixels(new Rect(0, 0, Screen.width, Screen.height), 0, 0  );
        ss.Apply();
        
        canvas.enabled = true;
        watermarkCanvas.enabled = false;
        
        yield return new WaitForEndOfFrame();

        // Open screenshot panel
        screenshotPanel.SetActive(true);
        screenshotPlaceHolder.texture = ss;
    }

    private IEnumerator ShareSSCoroutine(string type) {
        yield return new WaitForEndOfFrame();

        string filePath = Path.Combine( Application.temporaryCachePath, "shared img.png" );
        File.WriteAllBytes(filePath, ss.EncodeToPNG());

        string target = type;
        
        switch(type) {
            case "twitter":
                target = TWITTER;
            break;
            case "instagram":
                target = INSTAGRAM;
            break;
            case "facebook":
                target= FACEBOOK;
            break;
            default:
            break;
        }

        if (NativeShare.TargetExists(target)) {
            new NativeShare().AddFile(filePath).SetTarget(target).Share();
        } else {
            string appName = target;

            switch(target) {
                case TWITTER:
                    appName = "Twitter";
                    break;
                case INSTAGRAM:
                    appName = "Instagram";
                    break;
                case FACEBOOK:
                    appName = "Twitter";
                    break;
                default:
                break;
            }

            ModalPanelManager.instance.Choice(
                "",
                ("Maaf, Anda harus menginstall aplikasi "+appName+" agar dapat menggunakan fitur ini."),
                false,
                "",
                "",
                null,
                null,
                false
            );
            Debug.Log("target not exist");
        }

        ss = null;
        
        screenshotPanel.SetActive(false);
        ViewSocialMediaButtonsPanel(false);
    }

    public void SaveSSToGallery() {
        if(ss == null) {
            Debug.Log("No screenshot taken.");
            return;
        }
        NativeGallery.SaveImageToGallery(ss, "AR Zone", "Screenshot{0}.png");
        
        ModalPanelManager.instance.Choice(
            "",
            "Screenshot Anda berhasil tersimpan di Gallery!",
            false,
            "",
            "",
            null,
            null,
            false
        );

    }

    public void CloseScreenshotPanel() {
        ss = null;
        screenshotPanel.SetActive(false);
        ViewSocialMediaButtonsPanel(false);
    }

    public void BackToHome() {
        SceneManager.LoadScene("mainmenu");
    }

    public void ViewSocialMediaButtonsPanel(bool state) {
        if(state) {
            socialMediaButtonsPanel.DOMoveY(0, 0.5f, false);
        } else {
            socialMediaButtonsPanel.DOMoveY(-Screen.height / 2, 1f, false);
        }
        
    }
}
