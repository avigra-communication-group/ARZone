// Script ini mengontrol semua fungsi button di mainmenu

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Firebase.Auth;
using Firebase;
using Firebase.Database;
using Firebase.Unity.Editor;
using DG.Tweening;

public class MainMenuManager : MonoBehaviour
{

    public static MainMenuManager instance = null;

    public ARPointManager aRPointManager;

    public GameObject mainMenuContainer;

    public GameObject mainMenuButtonsContainer;
    public GameObject galleryContainer;
    public GameObject myPointContainer;
    public GameObject aboutContainer;
    public GameObject settingContainer;

    public Button scanButton;
    public Button galleryButton;
    public Button arMapButton;
    public Button myPointButton;
    public Button aboutButton;
    public Button settingButton;
    public Button quitButton;

    // for Debug only. Please disable or hide this on production!
    public Button signOutButton;
    public Button checkAuthButton;

    public Text pointText;
    public Text debugText;

    private Canvas canvas;
    private double m_Point = -1;

    private void Awake() {
        if(instance == null) {
            instance = this;
        } else if (instance != this) {
            Destroy(this.gameObject);
        }
    }

    private void Start() {
        Init();
    }

    private void Update()
    {
        if(FO.currentUser  != null && m_Point != FO.currentUser.point) {
            //update the point
            pointText.text = FO.currentUser.point.ToString();
            Debug.Log("point updaated.");
            m_Point = FO.currentUser.point;
        }
    }

    private void OnEnable() 
    {
        ARPointManager.onRegisterDelegate += SetMyPointButtonInterractibility;
        ARPointManager.onPointValueChanged += UpdatePointText;
        
    }

    private void OnDisable()
    {
        ARPointManager.onRegisterDelegate -= SetMyPointButtonInterractibility;
        ARPointManager.onPointValueChanged -= UpdatePointText;

    }

    private void Init() {

        // Component getters
        canvas = GetComponent<Canvas>();

        // Pasangkan listeners ke setiap event onClick button
        scanButton.onClick.AddListener(ScanButton);
        galleryButton.onClick.AddListener(GalleryButton);
        arMapButton.onClick.AddListener(ARMapButton);
        aboutButton.onClick.AddListener(AboutButton);
        settingButton.onClick.AddListener(SettingButton);
        myPointButton.onClick.AddListener(() => {
            StartCoroutine("PointRoutine");
        });
        quitButton.onClick.AddListener(QuitButton);
        
        // for Debug only!
        signOutButton.onClick.AddListener(() =>
        {
            FO.auth.SignOut();
            FirebaseAuth.DefaultInstance.SignOut();
            PlayerPrefs.DeleteKey(UserPrefType.PLAYER_ID);
            SetMessage("Signed out.");
        });

        checkAuthButton.onClick.AddListener(() => {
            if(FO.auth == null)
            {
                SetMessage("Auth is null");
            } 
            else if(FO.auth.CurrentUser == null)
            {
                SetMessage("Current user doesn't exist.");
            }
            else 
            {
                SetMessage("Current user is "+FO.auth.CurrentUser);
            }
        });

        
    }

    // Buttons function
    public void ScanButton() {
        SceneManager.LoadScene("scanviewer");
    }

    public void GalleryButton() {
        mainMenuButtonsContainer.SetActive(false);
        settingContainer.SetActive(false);
        aboutContainer.SetActive(false);
        galleryContainer.SetActive(true);
    }

    public void ARMapButton() {
        #if UNITY_ANDROID && !UNITY_EDITOR
        if(!Input.location.isEnabledByUser)
        {
            ModalPanelManager.instance.Choice(
                "",
                "Mohon untuk mengaktifkan GPS di perangkat Anda untuk menggunakan fitur ini.",
                false,
                "Ya",
                "Tidak",
                () => { OpenGPSSettings(); },
                () => {
                    ModalPanelManager.instance.ClosePanel();
                }
            );
            return;
        }
        #endif
        SceneManager.LoadScene("armap");
    }

    private void OpenGPSSettings() 
    {
        try
        {
        #if UNITY_ANDROID
            using (var unityClass = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
            using (AndroidJavaObject currentActivityObject = unityClass.GetStatic<AndroidJavaObject>("currentActivity"))
            {
                string packageName = currentActivityObject.Call<string>("getPackageName");

                using (var uriClass = new AndroidJavaClass("android.net.Uri"))
                using (AndroidJavaObject uriObject = uriClass.CallStatic<AndroidJavaObject>("fromParts", "package", packageName, null))
                using (var intentObject = new AndroidJavaObject("android.content.Intent", "android.settings.ACTION_LOCATION_SOURCE_SETTINGS", uriObject))
                {
                    intentObject.Call<AndroidJavaObject>("addCategory", "android.intent.category.DEFAULT");
                    intentObject.Call<AndroidJavaObject>("setFlags", 0x10000000);
                    currentActivityObject.Call("startActivity", intentObject);
                }
            }
        #endif
        }
        catch (Exception ex)
        {
            Debug.LogException(ex);
        }
    }

    IEnumerator PointRoutine() {
        FO.fdb
            .GetReference("users")
            .Child(FO.userId)
            .Child("point")
            .GetValueAsync()
            .ContinueWith(task => {
                if(task.IsFaulted)
                {

                }
                else if (task.IsCompleted)
                {
                    pointText.text = task.Result.Value.ToString();
                }
            });
        yield return new WaitForSeconds(2f);
        OpenPointPanel();
    }

    public void UpdatePointUI(object sender, ValueChangedEventArgs args)
    {
        string json = args.Snapshot.GetRawJsonValue();
        User user = JsonUtility.FromJson<User>(json);

        pointText.text = ""+user.point;
    }

    public void SetMyPointButtonInterractibility(bool state)
    {
        myPointButton.interactable = state;
    }

    public void UpdatePointText(double value)
    {
        pointText.text = value.ToString() ?? "...";
        Debug.Log("Updating point text with value = "+value);
        //Canvas.ForceUpdateCanvases();
    }

    public void MyPointButton() {
        // connect to the firebase first
        Debug.Log("Connecting to database...");
        SetMessage("Connecting to database...");
    }

    private void OpenPointPanel() {
        myPointContainer.SetActive(true);
        mainMenuButtonsContainer.SetActive(false);
        settingContainer.SetActive(false);
        aboutContainer.SetActive(false);
        galleryContainer.SetActive(false);
    }

    public void AboutButton() {
        mainMenuButtonsContainer.SetActive(false);
        galleryContainer.SetActive(false);
        settingContainer.SetActive(false);
        myPointContainer.SetActive(false);
        aboutContainer.SetActive(true);
    }

    public void SettingButton() {
        mainMenuButtonsContainer.SetActive(false);
        galleryContainer.SetActive(false);
        aboutContainer.SetActive(false);
        myPointContainer.SetActive(false);
        settingContainer.SetActive(true);
    }

    public void BackToMainMenuButtonContainer() {
        galleryContainer.SetActive(false);
        aboutContainer.SetActive(false);
        settingContainer.SetActive(false);
        myPointContainer.SetActive(false);
        mainMenuButtonsContainer.SetActive(true);
    }

    public void QuitButton() {
        ModalPanelManager.instance.Choice(
            "Quit",
            "Anda yakin ingin keluar dari Aplikasi?",
            true,
            "ya",
            "tidak",
            () => { Application.Quit(); },
            () => { ModalPanelManager.instance.ClosePanel(); }
        );
    }

    public void SetMessage(string text)
    {
        CancelInvoke("ClearMessage");
        string currentText = debugText.text;
        debugText.text = currentText + "\n" + text;
        Invoke("ClearMessage", 2f);
    }

    private void ClearMessage()
    {
        Color currentTextColor = debugText.color;

        debugText.DOFade(0f, 2f).OnComplete(() =>
        {
            debugText.text = "";
            debugText.color = currentTextColor;
        });
    }
}
