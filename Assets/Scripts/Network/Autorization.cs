using LiteNetLib;
using Network;
using OnlineGame.Networking;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

//TODO: таймаут на ожидание ответа от сервера
//TODO: проблема с получением никнейма при логине для отправки сообщений

public class Autorization : MonoBehaviour
{
    public static AccessData LocalAccessData { get; private set; }
    private bool _isLoginWindow = true; //Вводится ли сейчас логин
    [SerializeField] private int _nickNameMinLenght = 3;
    [SerializeField] private int _loginMinLenght = 2;
    [SerializeField] private int _passwordMinLenght = 6;
    [SerializeField] private int _MaxLenght = 50;
    [SerializeField] private Color DefaultInputsColor = Color.white;
    [SerializeField] private Color ErrorInputColor = Color.red;

    [SerializeField] private GameObject _autorizationPanel;
    [Header("Login")]
    [SerializeField] private GameObject _loginPanel;
    [SerializeField] private TMP_InputField _loginInputInLogin;
    [SerializeField] private TMP_InputField _passwordInputInLogin;
    [Header("Registration")]
    [SerializeField] private GameObject _registrationPanel;
    [SerializeField] private TMP_InputField _nickNameInputInRegister;
    [SerializeField] private TMP_InputField _loginInputInRegister;
    [SerializeField] private TMP_InputField _passwordInputInRegister;
    [SerializeField] private Button _loginButton;
    [SerializeField] private Button _registerButton;
    [SerializeField] private Button _switchButton;
    [SerializeField] private GameObject _errorLabelLogin;
    [SerializeField] private GameObject _errorLabelRegister;
    [SerializeField] private GameObject _LoadDataPanel;
    private TextMeshProUGUI _switchButtonText;

    private List<TMP_InputField> InputFields = new List<TMP_InputField>();

    private void Start()
    {
        _switchButtonText = _switchButton.GetComponentInChildren<TextMeshProUGUI>();
        InputFields = new List<TMP_InputField>() { _loginInputInLogin, _passwordInputInLogin, _nickNameInputInRegister, _loginInputInRegister, _passwordInputInRegister };
        ClearInputs();
    }

    public void Login()
    {
        if (IsCorrectLoginValues())
        {
            var loginData = new AccessData("null", _loginInputInLogin.text, _passwordInputInLogin.text);
            NetworkManager.SendEventFromServer(PacketType.Login, loginData, DeliveryMethod.ReliableOrdered);
            LocalAccessData = loginData;
            foreach (var input in InputFields)
            {
                input.interactable = false;
            }
            SetInputsInteractable(false);
            NetworkManager.ServerAnswerHandler.OnAutorizationAnswer += OnAutorizationAnswer;
        }
    }

    public void Register()
    {
        if (IsCorrectRegisterValues())
        {
            var registrationData = new AccessData(_nickNameInputInRegister.text, _loginInputInRegister.text,
                _passwordInputInRegister.text);
            LocalAccessData = registrationData;
            NetworkManager.SendEventFromServer(PacketType.Registration, registrationData, DeliveryMethod.ReliableOrdered);

            foreach (var input in InputFields)
            {
                input.interactable = false;
            }
            SetInputsInteractable(false);
            NetworkManager.ServerAnswerHandler.OnAutorizationAnswer += OnAutorizationAnswer;
        }
    }

    private void OnAutorizationAnswer(bool isSuccess)
    {
        ClearInputs();
        if (isSuccess)
        {
            if (_isLoginWindow)
            {
                Debug.Log("You are LogIn!");
                _LoadDataPanel.SetActive(true); //REWORK!!!
                NetworkManager.SendEventFromServer(PacketType.RequestData, "", DeliveryMethod.ReliableOrdered);
            }
            else
            {
                Debug.Log("You are Register!");
                ChangeAutorizationPanel();
            }
        }
        else
        {
            ErrorsSetActive(true);
            Debug.Log("You are not entered");
        }
        SetInputsInteractable(true);
        NetworkManager.ServerAnswerHandler.OnAutorizationAnswer -= OnAutorizationAnswer;
    }
    /// <summary>
    /// Ждет получения текущих данных пользователя с сервера
    /// </summary>
    public void OnLocalDataReceived()
    {
        _autorizationPanel.SetActive(false);
        NetworkManager.Instance.IsLogIn = true;
        gameObject.SetActive(false);
    }

    public void ChangeAutorizationPanel() //Меняем ввод между логином и паролем
    {
        _isLoginWindow = !_isLoginWindow;
        _loginPanel.SetActive(_isLoginWindow);
        _registrationPanel.SetActive(!_isLoginWindow);
        if (!_isLoginWindow) _switchButtonText.text = "Вход";
        else _switchButtonText.text = "Регистрация";
        ClearInputs();
    }

    private bool IsCorrectLoginValues()
    {
        ResetInputsColor();
        var login = _loginInputInLogin.text;
        var password = _passwordInputInLogin.text;
        if (login.Length < _loginMinLenght || login.Length > _MaxLenght)
        {
            SetInputColor(_loginInputInLogin, true);
            return false;
        }

        if (password.Length < _passwordMinLenght || password.Length > _MaxLenght)
        {
            SetInputColor(_passwordInputInLogin, true);
            return false;
        }
        return true;
    }

    private bool IsCorrectRegisterValues()
    {
        ResetInputsColor();
        var nickName = _nickNameInputInRegister.text;
        var login = _loginInputInRegister.text;
        var password = _passwordInputInRegister.text;

        if(nickName.Length < _nickNameMinLenght || password.Length > _MaxLenght)
        {
            SetInputColor(_nickNameInputInRegister, true);

            return false;
        }

        if (login.Length < _loginMinLenght || login.Length > _MaxLenght)
        {
            SetInputColor(_loginInputInRegister, true);
            return false;
        }
        
        if (password.Length < _passwordMinLenght || password.Length > _MaxLenght)
        {
            SetInputColor(_passwordInputInRegister, true);
            return false;
        }

        return true;
    }


    private void ClearInputs()
    {
        foreach (var input in InputFields)
        {
            input.text = "";
        }
        ResetInputsColor();
        ErrorsSetActive(false);
    }

    private void SetInputsInteractable(bool isActive)
    {
        foreach (var input in InputFields)
        {
            input.interactable = isActive;
        }
    }

    private void ErrorsSetActive(bool isActive)
    {
        _errorLabelLogin.SetActive(isActive);
        _errorLabelRegister.SetActive(isActive);
    }

    private void ResetInputsColor()
    {
        foreach (var input in InputFields)
        {
            SetInputColor(input, false);
        }
    }

    private void SetInputColor(TMP_InputField input, bool isErrorColor)
    {
        if (isErrorColor)
        {
            var normalColor = input.colors;
            normalColor.normalColor = ErrorInputColor;
            input.colors = normalColor;
        }
        else
        {
            var normalColor = input.colors;
            normalColor.normalColor = DefaultInputsColor;
            input.colors = normalColor;
        }
    }
}

