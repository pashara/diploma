using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ClientController : MonoBehaviour
{

    #region Fields

    [SerializeField] ServerUI clientUiPrefab;
    [SerializeField] Client clientPrefab;
    [SerializeField] Button buttonPrefab;
    [SerializeField] RectTransform buttonsParentTransform;
    [SerializeField] RectTransform clientsParentTransform;
    [SerializeField] Button addButton;

    Dictionary<Client, ServerUI> uiByClient = new Dictionary<Client, ServerUI>();
    Dictionary<Button, Client> clientByButton  = new Dictionary<Button, Client>();

    #endregion


    #region Properties



    #endregion



    #region Unity lifecycle

    void OnEnable()
    {
        addButton.onClick.AddListener(()=>OnAddButtonClicked());
    }


    void OnDisable()
    {
        // addButton.onClick.RemoveListener(OnAddButtonClicked);
    }

    #endregion



    #region Public methods

    public Client AddClient()
    {
        Debug.Log("add");
        ServerUI ui = Instantiate<ServerUI>(clientUiPrefab, clientsParentTransform.position, Quaternion.identity, clientsParentTransform);
        Client client = Instantiate<Client>(clientPrefab, Vector3.zero, Quaternion.identity, transform);

        client.Initialize(ui);
        (ui.transform as RectTransform).offsetMax = Vector2.zero;
        (ui.transform as RectTransform).offsetMin = Vector2.zero;
        (ui.transform as RectTransform).anchorMin = new Vector2(0, 0);
        (ui.transform as RectTransform).anchorMax = new Vector2(1, 1);
        (ui.transform as RectTransform).pivot = new Vector2(0.5f, 0.5f);


        uiByClient.Add(client, ui);
        return client;

    }


    public Button CreateNewButton(string text)
    {
        Button button = Instantiate<Button>(buttonPrefab, buttonsParentTransform.position, Quaternion.identity, buttonsParentTransform);
        button.GetComponentInChildren<Text>().text = text;
        return button;
    }

    #endregion


    #region Event handlers

    void OnAddButtonClicked()
    {
        Client client = AddClient();
        Button button = CreateNewButton((clientByButton.Count + 1).ToString());

        ServerUI ui;
        uiByClient.TryGetValue(client, out ui);
        ui.MainText = (clientByButton.Count + 1).ToString();

        clientByButton.Add(button, client);


        button.onClick.AddListener(() => OnClientButtonClicked(client));
        foreach(var keyPairValue in uiByClient)
        {
            bool isActiveTab = keyPairValue.Key == client;
            Debug.Log(isActiveTab);
            keyPairValue.Value.gameObject.SetActive(isActiveTab);
        }
    }


    void OnClientButtonClicked(Client client)
    {
        foreach(var keyPairValue in uiByClient)
        {
            bool isCurrentClient = keyPairValue.Key == client;
            keyPairValue.Value.gameObject.SetActive(isCurrentClient);
        }
    }

    #endregion
}
