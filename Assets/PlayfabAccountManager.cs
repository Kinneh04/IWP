using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PlayFab.ClientModels;
using PlayFab.ServerModels;
using PlayFab;

public class PlayfabAccountManager : MonoBehaviour
{
    [Header("AccountOptions")]
    public GameObject SignedInAccountOptions;
    public GameObject GuestAccountOptions;

    [Header("LoginPage")]
    public GameObject LoginPage;

    [Header("SignUpPage")]
    public GameObject SignUpPage;

    public void OpenAccountOptions()
    {
        if(PlayFabClientAPI.IsClientLoggedIn())
        {
            SignedInAccountOptions.SetActive(true);
        }
        else
        {
            GuestAccountOptions.SetActive(true);
        }
    }

    public void OpenLoginPage()
    {
        LoginPage.SetActive(true);
    }

    public void OpenSignUpPage()
    {
        SignUpPage.SetActive(true);
    }
}
