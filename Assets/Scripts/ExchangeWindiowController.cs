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
    private bool pointGathered = false;
    private bool transactionSucceeded = false;

    private void Start() 
    {
        tukarButton.onClick.AddListener(TukarReward);
    }

    public void OnTransactionSucceedEvent()
    {
        pinInput.text = "";
        tukarButton.interactable = false;

        ModalPanelManager.instance.Choice(
                "Sukses",
                "Transaksi berhasil. Point Anda kini tersisa " + FO.userPoint + " point.",
                true,
                "Kembali ke menu Utama",
                "",
                () => {

                    ModalPanelManager.instance.ClosePanel();
                    MainMenuManager.instance.BackToMainMenuButtonContainer();
                },
                ()=>{},
                true
            );
    }

    private void OnEnable() 
    {

        pointGathered = false;
        transactionSucceeded = false;

        // Get or update point from db
        pointReference = FO.fdb.GetReference("users").Child(FO.userId).Child("point");
        
        pointReference
            .GetValueAsync()
            .ContinueWith(task =>
            {
                if (task.IsFaulted)
                {
                    Debug.Log("Retrieving point from firebase failed " + task.Exception);
                    pinInput.text = "";
                }
                else if (task.IsCompleted)
                {
                    Debug.Log("Data berhasil diambil. Mengolah data...");
                    FO.userPoint = Convert.ToDouble(task.Result.Value.ToString());
                    pointGathered = true;
                }
            });
    }

    public void OpenExchangeWindow(double point, string reward)
    {
        if(FO.userPoint < point)
        {
            ModalPanelManager.instance.Choice(
                "",
                "Point Anda tidak mencukupi untuk mendapatkan reward ini.",
                false,
                "",
                "",
                null,
                null,
                false
            );
            return;
        }
        gameObject.SetActive(true);
        tukarButton.interactable = false;
        messageDisplay.text = 
        string.Format(
            "Anda dapat menukarkan {0} point milik Anda untuk mendapatkan reward {1}. \nTunjukan layar ini ke merchant untuk mendapatkan reward!"
            ,point, reward
        );
        pointToExchange = point;
    }

    private void Update() 
    {
        tukarButton.interactable = pointGathered;
        if(transactionSucceeded)
        {
            OnTransactionSucceedEvent();
            transactionSucceeded = false;
        }
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

        pointReference
            .SetValueAsync(result)
            .ContinueWith(task =>
            {
                if (task.IsFaulted)
                {
                    Debug.Log("Point update failed. " + task.Exception);
                }
                else if (task.IsCompleted)
                {
                    Debug.Log("Point updated to database successfully");
                    FO.userPoint = result;
                    transactionSucceeded = true;
                }
            });
        
    }
}
