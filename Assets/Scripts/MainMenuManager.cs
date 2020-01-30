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

    public GameObject mainMenuContainer;

    public GameObject mainMenuButtonsContainer;
    public GameObject galleryContainer;
    public GameObject myPointContainer;
    public GameObject aboutContainer;
    public GameObject settingContainer;
    public GameObject exchangeContainer;

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

    public Transform galleryParent;
    public RawImage galleryImagePrefab;

    private Canvas canvas;

    private void Awake() 
    {
        if(instance == null) {
            instance = this;
        } else if (instance != this) {
            Destroy(this.gameObject);
        }

        FirebaseHelper.Init();
        // Get the galleryUrls
        FirebaseHelper.GetGalleryImagesFromDB();
    }

    private void Start() {
        Init();
    }

    private void Init() {
        // Component getters
        canvas = GetComponent<Canvas>();
        //galleryImages = galleryParent.GetComponentsInChildren<RawImage>();
        // Pasangkan listeners ke setiap event onClick button

        scanButton.onClick.AddListener(ScanButton);
        galleryButton.onClick.AddListener(() => {
            StartCoroutine(LoadImagesIntoGallery());
        });

        arMapButton.onClick.AddListener(ARMapButton);
        aboutButton.onClick.AddListener(AboutButton);
        settingButton.onClick.AddListener(SettingButton);
        myPointButton.onClick.AddListener(PointRoutine);
        quitButton.onClick.AddListener(QuitButton);

        // Check if user registered
        FirebaseHelper.CheckIfUserIsRegistered(FO.userId, (isRegistered) =>
        {
            Debug.Log("Validating user registration.");
            if (isRegistered)
            {
                Debug.Log("User registered.");
                myPointButton.interactable = true;
                Debug.Log("Gathering user visited places.");
                FirebaseHelper.GetUserVisitedPlaces(FO.userId, (visPlaces) => {
                    FO.visitedPlace = new List<string>(visPlaces);
                    arMapButton.interactable = true;
                    Debug.Log("Visited places gathered.");
                });
            }
            else
            {
                Debug.Log("User not registered. Creating new user account...");
                FirebaseHelper.CreateNewUser(FO.userId, () => {
                    Debug.Log("New user created");
                    myPointButton.interactable = true;
                    Debug.Log("Gathering user visited places.");
                    FirebaseHelper.GetUserVisitedPlaces(FO.userId, (visPlaces) =>
                    {
                        FO.visitedPlace = new List<string>(visPlaces);
                        arMapButton.interactable = true;
                        Debug.Log("Visited places gathered.");
                    });
                });
            }
        });

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
        SceneManager.LoadScene("scanviewerwithtimeline");
    }

    public void GalleryButton() {
        // mainMenuButtonsContainer.SetActive(false);
        // settingContainer.SetActive(false);
        // aboutContainer.SetActive(false);
        // exchangeContainer.SetActive(false);
        // galleryContainer.SetActive(true);
        ModalPanelManager.instance.Choice(
            "",
            "Silahkan Buka Folder AR Zone Anda",
            false,
            "",
            "",
            null,
            null,
            false
        );
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
                },
                false
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

    public void PointRoutine() 
    {
        FirebaseHelper.GetUserPoint(FO.userId, (p) => {
            pointText.text = p.ToString();
            OpenPointPanel();
        });
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

    private void OpenPointPanel() {
        myPointContainer.SetActive(true);
        mainMenuButtonsContainer.SetActive(false);
        settingContainer.SetActive(false);
        aboutContainer.SetActive(false);
        galleryContainer.SetActive(false);
        exchangeContainer.SetActive(false);
    }

    public void AboutButton() {
        mainMenuButtonsContainer.SetActive(false);
        galleryContainer.SetActive(false);
        settingContainer.SetActive(false);
        myPointContainer.SetActive(false);
        aboutContainer.SetActive(true);
        exchangeContainer.SetActive(false);
    }

    public void SettingButton() {
        mainMenuButtonsContainer.SetActive(false);
        galleryContainer.SetActive(false);
        aboutContainer.SetActive(false);
        myPointContainer.SetActive(false);
        settingContainer.SetActive(true);
        exchangeContainer.SetActive(false);
    }

    public void BackToMainMenuButtonContainer() {
        galleryContainer.SetActive(false);
        aboutContainer.SetActive(false);
        settingContainer.SetActive(false);
        myPointContainer.SetActive(false);
        mainMenuButtonsContainer.SetActive(true);
        exchangeContainer.SetActive(false);
    }

    public void QuitButton() {
        ModalPanelManager.instance.Choice(
            "Quit",
            "Anda yakin ingin keluar dari Aplikasi?",
            true,
            "ya",
            "tidak",
            () => { Application.Quit(); },
            () => { ModalPanelManager.instance.ClosePanel(); },
            false
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

    public IEnumerator LoadImagesIntoGallery()
    {
        // clean the gallery
        foreach (Transform t in galleryParent)
        {
            Destroy(t.gameObject);
        }

        GalleryButton();

        for (int i = 0; i < FO.galleryImages.Count; i++)
        {
            if (i > 9)
            {
                break;
            }
            using (WWW www = new WWW(FO.galleryImages[i]))
            {
                yield return www;
                RawImage r = Instantiate(galleryImagePrefab);
                r.transform.SetParent(galleryParent, false);
                r.transform.localScale = Vector3.one;
                Material m = Instantiate(r.material);
                Texture2D tex;
                tex = new Texture2D(545, 462, TextureFormat.DXT1, false);
                www.LoadImageIntoTexture(tex);
                m.mainTexture = tex;
                r.material = m;
            }
        }

        ModalPanelManager.instance.ClosePanel();   
    }
}
