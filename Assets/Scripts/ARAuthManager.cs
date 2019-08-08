using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Firebase.Auth;

public static class ARAuthManager 
{
    public static void AuthStateChanged(object sender, System.EventArgs eventArgs)
    {
        if (FO.auth.CurrentUser != FO.user)
        {
            FO.isSignedIn = FO.user != FO.auth.CurrentUser && FO.auth.CurrentUser != null;

            if (!FO.isSignedIn && FO.user != null)
            {
                Debug.Log("Signed out " + FO.user.UserId);
                // if signed out tell user to login or sign-in
                // AuthUIManager.instance.authContainer.SetActive(true);
                // AuthUIManager.instance.DisplayPanel(AuthPanelType.SignUp, true);
                // GoToScene("auth");
                if(MainMenuManager.instance != null)
                {
                    MainMenuManager.instance.SetMessage("Signed out.");
                }
            }

            FO.user = FO.auth.CurrentUser;

            if (FO.isSignedIn)
            {
                // GoToScene("mainmenu");
                if (MainMenuManager.instance != null)
                {
                    MainMenuManager.instance.SetMessage("Signed in.");
                }
            }
        }
    }

}
