using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using UnityEngine.Networking;
using UnityEngine.Networking.Match;

public class JoinGame : MonoBehaviour {

    List<GameObject> roomList = new List<GameObject>();

    [SerializeField]
    private Text status;

    [SerializeField]
    private GameObject roomListItemPrefab;

    [SerializeField]
    private Transform roomListParent;

    private NetworkManager networkManager;

    private void Start()
    {
        networkManager = NetworkManager.singleton;
        if (networkManager.matchMaker == null) {
            networkManager.StartMatchMaker();
        }

        RefreshRoomList();
    }

    public void RefreshRoomList () {
        ClearRoomList();
        networkManager.matchMaker.ListMatches(0, 10, "", true, 0, 0, OnMatchList);
        status.text = "Загрузка...";
    }

    public void OnMatchList (bool success, string extendedInfo, List <MatchInfoSnapshot> matchList) {
        status.text = "";

        if (!success || matchList == null) {
            status.text = "Ошибка при получении списка комнат";
            return;
        }

        foreach (MatchInfoSnapshot match in matchList) {
            GameObject _roomListItemGO = Instantiate(roomListItemPrefab);
            _roomListItemGO.transform.SetParent(roomListParent);

            RoomListItem _roomListItem = _roomListItemGO.GetComponent<RoomListItem>();
            if (_roomListItem != null) {
                _roomListItem.Setup(match, JoinRoom);
            }
            // Have a component sit on a gameobj that'll take care 
            // of setting up the name/amount of users as well as
            // setting up a callback function that'll join the game.

            roomList.Add(_roomListItemGO);
        }

        if (roomList.Count == 0) {
            status.text = "В данный момент комнат нет";
        }
    }

    void ClearRoomList() {
        for (int i = 0; i < roomList.Count; i++)
        {
            Destroy(roomList[i]);
        }

        roomList.Clear();
    }

    public void JoinRoom (MatchInfoSnapshot _match) {
        networkManager.matchMaker.JoinMatch(_match.networkId, "", "", "", 0, 0, networkManager.OnMatchJoined);
        ClearRoomList();
        Debug.Log("Присоединяюсь... ");
    }

}
