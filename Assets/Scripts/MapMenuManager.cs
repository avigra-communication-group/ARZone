using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MapMenuManager : MonoBehaviour
{
    public Button backToHomeButton;

    private void Start() {
        Init();
    }

    private void Init() {
        backToHomeButton.onClick.AddListener(BackToHome);
    }

    public void BackToHome() {
        SceneManager.LoadScene("mainmenu");
    }
}
