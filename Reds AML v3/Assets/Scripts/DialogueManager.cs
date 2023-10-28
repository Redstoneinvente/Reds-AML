using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DialogueManager : MonoBehaviour
{
    [SerializeField] TMP_Text title;
    [SerializeField] TMP_Text subTitle;
    [SerializeField] TMP_Text description;

    public delegate void OnOkButtonPressed();
    public OnOkButtonPressed onOkButtonPressed;

    public delegate void OnDismissed();
    public OnDismissed onDismissed;

    public DialogueObject dialogueObject;

    private void Awake()
    {
        onOkButtonPressed += OnOK;
        onDismissed += OnDismiss;
    }

    public void InitializeDialogue(DialogueObject dialogueObject)
    {
        this.dialogueObject = dialogueObject;

        title.text = dialogueObject.title;

        if (subTitle != default)
        {
            subTitle.text = dialogueObject.subTitle;
        }

        if (description != default)
        {
            description.text = dialogueObject.description;
        }

        onOkButtonPressed += dialogueObject.onOkayButtonPressed;
        onDismissed += dialogueObject.onDismissed;
    }

    public void Show()
    {
        gameObject.SetActive(true);
    }

    public void OkButton()
    {
        onOkButtonPressed();
    }

    public void DismissButton()
    {
        onDismissed();
    }

    void OnOK()
    {
        this.gameObject.SetActive(false);
        Debug.Log("Ok Pressed!");
    }

    void OnDismiss()
    {
        this.gameObject.SetActive(false);
        Debug.Log("Dismissed!");
    }

    [System.Serializable]
    public class DialogueObject
    {
        public string title;
        public string subTitle;
        public string description;

        public OnOkButtonPressed onOkayButtonPressed;
        public OnDismissed onDismissed;

        public DialogueObject(string title, string subTitle, string description, OnOkButtonPressed onOkButtonPressed, OnDismissed onDismissed)
        {
            this.title = title;
            this.subTitle = subTitle;   
            this.description = description;

            this.onOkayButtonPressed += onOkButtonPressed;   
            this.onDismissed += onDismissed;
        }

        public DialogueObject(DialogueObject dialogueObject)
        {
            this.title = dialogueObject.title;
            this.subTitle = dialogueObject.subTitle;
            this.description = dialogueObject.description;

            this.onOkayButtonPressed += dialogueObject.onOkayButtonPressed;
            this.onDismissed += dialogueObject.onDismissed;
        }
    }
}
