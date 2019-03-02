using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using TMPro;

public class ServerUI : MonoBehaviour
{

    #region Events

    public event Action<ServerUI> OnMainButtonPressed;
    public event Action<ServerUI> OnClearButtonPressed;
    public event Action<ServerUI> OnSendButtonPressed;

    #endregion



    #region Fields

    [SerializeField] Button mainButton;
    [SerializeField] Button clearButton;
    [SerializeField] Button sendButton;
    [SerializeField] TextMeshProUGUI mainText;

    [SerializeField] InputField serverAddress;
    [SerializeField] InputField serverPort;
    [SerializeField] InputField inputTextField;
    [SerializeField] InputField outputTextField;
    [SerializeField] bool isInitedByUnity;

    #endregion



    #region Properties
    
    public string IpAddress
    {
        get
        {
            return serverAddress.text;
        }
        set
        {
            serverAddress.text = value;
        }
    }
    
    public int PortAddress
    {
        get
        {
            return Int32.Parse(serverPort.text);
        }
        set
        {
            serverPort.text = value.ToString();
        }
    }

    public string InputPlaceholder
    {
        get
        {
            return inputTextField.placeholder.GetComponent<Text>().text;
        }
        set
        {
            inputTextField.placeholder.GetComponent<Text>().text = value;
        }
    }

    
    public string MainButtonText
    {
        get
        {
            return mainButton.GetComponentInChildren<Text>().text;
        }
        set
        {
            mainButton.GetComponentInChildren<Text>().text = value;
        }
    }


    public string OutputPlaceholder
    {
        get
        {
            return outputTextField.placeholder.GetComponent<Text>().text;
        }
        set
        {
            outputTextField.placeholder.GetComponent<Text>().text = value;
        }
    }


    public string InputText
    {
        get
        {
            return inputTextField.text;
        }
        set
        {
            inputTextField.text = value;
        }
    }


    public string MainText
    {
        set
        {
            mainText.text = value;
        }
    }

    #endregion



    #region Unity lifecycle

    void OnEnable()
    {
        mainButton.onClick.AddListener(() => Button_OnClicked(mainButton));
        clearButton.onClick.AddListener(() => Button_OnClicked(clearButton));
        sendButton.onClick.AddListener(() => Button_OnClicked(sendButton)); 
    }


    void OnDisable()
    {
        mainButton.onClick.RemoveAllListeners();
        clearButton.onClick.RemoveAllListeners();
        sendButton.onClick.RemoveAllListeners(); 
    }

    #endregion



    #region  Public methods

    public void ClearOutput()
    {
        outputTextField.text = "";
    }
    
    public void ClearInput()
    {
        inputTextField.text = "";
    }


    public void AddToOutput(string newText)
    {
        outputTextField.text += newText + "/n";
    }

    #endregion



    #region Event handlers

    void Button_OnClicked(Button sender)
    {
        if(sender ==  mainButton)
        {
            if(OnMainButtonPressed != null)
            {
                OnMainButtonPressed(this);
            }
        }
        else if(sender ==  clearButton)
        {
            if(OnClearButtonPressed != null)
            {
                OnClearButtonPressed(this);
            }
            ClearOutput();
        }
        else if(sender ==  sendButton)
        {
            if(OnSendButtonPressed != null)
            {
                OnSendButtonPressed(this);
            }
        }
    }

    #endregion
}
