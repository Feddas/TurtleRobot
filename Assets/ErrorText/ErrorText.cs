using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(CanvasGroup))]
[RequireComponent(typeof(InputField))]
public class ErrorText : MonoBehaviour
{
    [Tooltip("When checked, error text caused by IAP is populated into the ErrorText field")]
    public bool IsErrorTextActive;

    private CanvasGroup canvasGroup;
    private InputField inputField;

    void Start()
    {
        canvasGroup = this.GetComponent<CanvasGroup>();
        inputField = this.GetComponent<InputField>();
    }

    void Update()
    {
    }

    public void SetErrorText(string message)
    {
        if (IsErrorTextActive == false || inputField == null || string.IsNullOrEmpty(message))
            return;

        SetVisible(true);
        inputField.text = message;
    }

    public void SetException(System.Exception e)
    {
        SetErrorText(e.Message + System.Environment.NewLine + e.StackTrace);
    }

    public void SetVisible(bool isVisible)
    {
        canvasGroup.alpha = isVisible ? 1 : 0;
        canvasGroup.interactable = isVisible;
        canvasGroup.blocksRaycasts = isVisible;
    }
}
