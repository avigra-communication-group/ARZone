using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Firebase.Database;

public class ExchangeWindiowController : MonoBehaviour
{
    public Text messageDisplay;
    public InputField pinInput;
    public Button tukarButton;


    private DatabaseReference pointReference;
    private double pointToExchange;
    // private bool pointGathered = false;
    // private bool transactionSucceeded = false;

    private void Start() 
    {
        tukarButton.onClick.AddListener(TukarReward);
    }

    public void OpenExchangeWindow(double pointPrice, string reward)
    {

        // Get or update point from db async
        FirebaseHelper.GetUserPoint(FO.userId, (userPoint) =>
        {
            FO.userPoint = userPoint;
            pointToExchange = pointPrice;
            
            if (FO.userPoint < pointPrice)
            {
                ModalPanelManager.instance.Choice(
                    "",
                    "Point Anda tidak mencukupi untuk mendapatkan reward ini. Kumpulkan point dengan menemukan lokasi-lokasi tertentu di AR Map!. Pergi ke AR Map sekarang?",
                    true,
                    "Ya",
                    "Tidak",
                    () => { UnityEngine.SceneManagement.SceneManager.LoadScene("armap"); },
                    () => { ModalPanelManager.instance.ClosePanel(); },
                    false
                );
                return;
            }
            gameObject.SetActive(true);
            tukarButton.interactable = true;
            messageDisplay.text =
            string.Format(
                "Anda dapat menukarkan {0} point milik Anda untuk mendapatkan reward {1}. \nTunjukan layar ini ke merchant untuk mendapatkan reward!",
                FO.userPoint,
                reward
            );
        });

        
    }

    public void CloseExchangeWindow()
    {
        pinInput.text = "";
        tukarButton.interactable = false;
        this.gameObject.SetActive(false);
    }

    public void TukarReward()
    {
        Debug.Log("Menukarkan reward.");

        if(pinInput.text == "")
        {
            Debug.Log("Tukar reward belum berhasil. PIN belum terisi.");
            ModalPanelManager.instance.Choice(
                "",
                "Masukan pin terlebih dahulu. (Dilakukan oleh merchant)",
                false,
                "",
                "",
                null,
                null,
                false
            );
            return;
        }
        else if(pinInput.text != PointShopManager.PIN)
        {
            Debug.Log("Tukar reward belum berhasil. PIN salah.");
            ModalPanelManager.instance.Choice(
                "",
                "PIN anda salah",
                false,
                "",
                "",
                null,
                null,
                false
            );
            pinInput.text = "";
            return;
        }

        tukarButton.interactable = false;

        Debug.Log("Mengambil point dari database.");
        SubstractUserPoint(FO.userId, pointToExchange);
        
    }

    void SubstractUserPoint(string userID, double point)
    {
        Debug.Log("Mengajukan transaksi point.");

        double result = FO.userPoint - point;
        
        if(result < 0)
        {
            Debug.Log("Transaksi gagal. Jumlah point anda tidak mencukupi.");

            ModalPanelManager.instance.Choice(
                "",
                "point Anda tidak mencukupi",
                false,
                "",
                "",
                null,
                null,
                false
            );
            tukarButton.interactable = true;
            return;
        }

        Debug.Log("Melakukan update point kedalam database.");

        FirebaseHelper.AddUserPoint(FO.userId, -pointToExchange, () => {
            FO.userPoint = result;
            Debug.Log("Transaksi berhasil.");
            OnTransactionSucceedEvent();
        });
    }

    public void OnTransactionSucceedEvent()
    {
        pinInput.text = "";
        tukarButton.interactable = false;

        ModalPanelManager.instance.Choice(
                "Sukses",
                "Transaksi berhasil. Point Anda kini tersisa " + FO.userPoint + " point. Jangan lupa untuk mengambil reward Anda di Merchant.",
                true,
                "Kembali ke menu Utama",
                "",
                () =>
                {
                    ModalPanelManager.instance.ClosePanel();
                    MainMenuManager.instance.BackToMainMenuButtonContainer();
                },
                () => { },
                true
            );
    }
}
