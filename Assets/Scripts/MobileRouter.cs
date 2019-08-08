using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MobileRouter : MonoBehaviour
{
    private void Update() {
        if(Input.GetKeyDown(KeyCode.Escape)) {
            Back();
        }
    }

    private void Back() {
        // make sure it doesn't run on main menu scene
        if ((SceneManager.GetActiveScene().name == "mainmenu")) {
            ModalPanelManager.instance.Choice(
                "Quit",
                "Anda yakin ingin keluar dari Aplikasi?",
                true,
                "ya",
                "tidak",
                () => { Application.Quit(); },
                () => { ModalPanelManager.instance.ClosePanel(); }
            );
            return;
        } else if (SceneManager.GetActiveScene().name == "Demo_ARCameraACCELEROMETER" || SceneManager.GetActiveScene().name == "Demo_ARCameraGYRO") {
            SceneManager.LoadScene("armap");
            return;
        }
        SceneManager.LoadScene("mainmenu");
    }
}
