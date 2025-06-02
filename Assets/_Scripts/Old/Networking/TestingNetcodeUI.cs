using System;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class TestingNetcodeUI : MonoBehaviour
{
    public event EventHandler OnHost;

    [SerializeField] private Button hostButton;
    [SerializeField] private Button clientButton;

    private void Start()
    {
        //Debug.Log("Host");
        NetworkManager.Singleton.StartHost();
        OnHost?.Invoke(this, EventArgs.Empty);
        Hide();
        return;

        hostButton.onClick.AddListener(() =>
        {
            Debug.Log("Host");
            NetworkManager.Singleton.StartHost();
            OnHost?.Invoke(this, EventArgs.Empty);
            Hide();
        });
        clientButton.onClick.AddListener(() =>
        {
            Debug.Log("Client");
            NetworkManager.Singleton.StartClient();
            Hide();
        });
    }

    private void Hide()
    {
        gameObject.SetActive(false);
    }
}
