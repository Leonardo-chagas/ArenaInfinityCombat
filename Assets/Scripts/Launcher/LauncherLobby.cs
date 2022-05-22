using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using TMPro;
using Photon.Realtime;
using UnityEngine.UI;

public class LauncherLobby : MonoBehaviourPunCallbacks
{
    public static LauncherLobby Instance;

    public GameObject sceneLobby;
    public GameObject sceneLoading;

    [SerializeField] TMP_Text roomNameText;
    [SerializeField] Transform playerListContent;
    [SerializeField] GameObject playerListItemPrefab;
    [SerializeField] GameObject startGameButton;
    [SerializeField] TMP_Dropdown selectScene;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        if(PhotonNetwork.IsConnected == false)
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene("menu"); 
        }

        roomNameText.text = PhotonNetwork.CurrentRoom.Name;

        Debug.Log("Entrei Aqui");

        Player[] players = PhotonNetwork.PlayerList;
        
        if (players.Length < 4) {
            Debug.Log("Aqui menor");
            PhotonNetwork.CurrentRoom.IsOpen = true;
            PhotonNetwork.CurrentRoom.IsVisible = true;
        } else {
            Debug.Log("Aqui maior");
            PhotonNetwork.CurrentRoom.IsOpen = false;
            PhotonNetwork.CurrentRoom.IsVisible = false;  
        }

        foreach(Transform child in playerListContent)
        {
            Destroy(child.gameObject);
        }

        for(int i = 0; i < players.Length; i++)
        {
            Instantiate(playerListItemPrefab, playerListContent).GetComponent<PlayerListItem>().SetUp(players[i]);
        }

        startGameButton.SetActive(PhotonNetwork.IsMasterClient);
        selectScene.gameObject.SetActive(PhotonNetwork.IsMasterClient);
    }

    void Update()
    {
        Player[] players = PhotonNetwork.PlayerList;
        
        if (players.Length < 4) {
            PhotonNetwork.CurrentRoom.IsOpen = true;
            PhotonNetwork.CurrentRoom.IsVisible = true;
        } else {
            PhotonNetwork.CurrentRoom.IsOpen = false;
            PhotonNetwork.CurrentRoom.IsVisible = false;  
        }
    }

    public override void OnConnectedToMaster()
    {
        Debug.Log("Connected to Master");
        PhotonNetwork.JoinLobby();
        PhotonNetwork.AutomaticallySyncScene = true;
    }

    public override void OnMasterClientSwitched(Player newMasterClient)
    {
        startGameButton.SetActive(PhotonNetwork.IsMasterClient);
        selectScene.gameObject.SetActive(PhotonNetwork.IsMasterClient);
    }

    public void StartGame()
    {
        sceneLobby.gameObject.SetActive(false);
        sceneLoading.gameObject.SetActive(true);
        string[] array = new string[5] {"arena", "construction", "installation", "platforms", "plaza"};

        Debug.Log(array);
        Debug.Log(selectScene.value);

        PhotonNetwork.CurrentRoom.IsOpen = false;
        PhotonNetwork.CurrentRoom.IsVisible = false;
        PhotonNetwork.LoadLevel(array[selectScene.value]);
        
        Debug.Log("Estou aqui no Start");
    }

    public void LeaveRoom()
    {
        Debug.Log("Sai Aqui");
        PhotonNetwork.LeaveRoom();
    }

    public override void OnLeftRoom()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("menu");
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        Instantiate(playerListItemPrefab, playerListContent).GetComponent<PlayerListItem>().SetUp(newPlayer);
    }
}
