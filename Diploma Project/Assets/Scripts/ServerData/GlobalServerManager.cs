using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using System;

public class GlobalServerManager : InitializableMonobehaviour
{

    #region Fields

    [SerializeField] string serverUrl = "";

    [Header("UrlPartes")]
    [SerializeField] string authUrl = "";

    #endregion



    #region Properties

    public static GlobalServerManager Instance
    {
        get;
        private set;
    }


    public string AuthURI
    {
        get
        {
            return $"{serverUrl}{authUrl}";
        }
    }


    public string PlayeInfoURI
    {
        get
        {
            return $"{serverUrl}{authUrl}";
        }
    }

    #endregion



    #region Public methods

    public override void Initialize()
    {
        Instance = this;
    }


    public void LoadDataFromUrl(string url, Action<bool, string> callback)
    {
        StartCoroutine(IELoadDataFromUrl(url, callback));
    }


    public Coroutine LoadTexture(string url, Action<bool, Texture> callback)
    {
        return StartCoroutine(IELoadTexture(url, callback));
    }

    #endregion



    #region IEnumerators

    IEnumerator IELoadDataFromUrl(string url, Action<bool, string> callback)
    {
        UnityWebRequest request = UnityWebRequest.Get(url);
        yield return request.SendWebRequest();

        if (!request.isHttpError && !request.isNetworkError)
        {
            callback?.Invoke(true, request.downloadHandler.text);
        }
        else
        {
            callback?.Invoke(false, string.Empty);
            Debug.LogErrorFormat("error request [{0}, {1}]", url, request.error);
        }
        request.Dispose();
    }


    IEnumerator IELoadTexture(string url, Action<bool, Texture> callback)
    {
        UnityWebRequest request = UnityWebRequestTexture.GetTexture(url);
        yield return request.SendWebRequest();

        if (request.isNetworkError || request.isHttpError)
        {
            callback?.Invoke(false, null);
            Debug.Log(request.error);
        }
        else
        {
            Texture myTexture = ((DownloadHandlerTexture)request.downloadHandler).texture;
            callback?.Invoke(true, myTexture);
        }
    }

    #endregion
}
