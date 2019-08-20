using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class ModalPanelManager : MonoBehaviour
{

    public static ModalPanelManager instance = null;

    public GameObject modalPanel;

    public Text titleDisplay;
    public Text  textDisplay;
    public Text yesButtonText;
    public Text noButtonText;

    public GameObject choiceButtonPanel;
    public Button yesButton;
    public Button noButton;
    public EventTrigger backgroundClicked;

    private void Awake() {
        if (instance == null) {
            instance = this;
        }
    }

    private void Start() {
        Init();
    }

    private void Init() {

    }

    public void Choice (string title, string text, bool showQuestionPanel, string yesText, string noText, UnityAction yesEvent, UnityAction noEvent, bool disableBackgroundExit) {
        Debug.Log(title+" panel called.");
        this.ShowPanel();
        this.titleDisplay.text = title;
        this.textDisplay.text = text;
        this.choiceButtonPanel.SetActive(showQuestionPanel);
        this.yesButtonText.text = yesText;
        this.noButtonText.text = noText;
        this.yesButton.onClick.RemoveAllListeners();
        this.noButton.onClick.RemoveAllListeners();
        this.yesButton.onClick.AddListener(yesEvent);
        this.noButton.onClick.AddListener(noEvent);

        if(showQuestionPanel)
        {
            yesButton.gameObject.SetActive(true);
            noButton.gameObject.SetActive(true);
            
            if(yesButtonText.text == "")
            {
                yesButton.gameObject.SetActive(false);
            }
            else if(noButtonText.text == "")
            {
                noButton.gameObject.SetActive(false);
            }
        }

        if(disableBackgroundExit)
        {
            EventTrigger trigger = modalPanel.GetComponent<EventTrigger>();
            if(trigger != null)
            {
                trigger.enabled = false;
            }
        }
    }

    public void ShowPanel() {
        modalPanel.SetActive(true);
    }

    public void ClosePanel() {
        modalPanel.SetActive(false);
    }
    
}
