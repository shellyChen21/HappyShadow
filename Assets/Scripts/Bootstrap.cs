using Character;
using Photon.Pun;
using UnityEngine;

public class Bootstrap : MonoBehaviourPunCallbacks
{
    [SerializeField] private Transform[] characterSpawnTrans;

    private void Start()
    {
        PhotonNetwork.ConnectUsingSettings();
    }

    public override void OnConnectedToMaster()
    {
        Debug.Log("connected to master server");

        PhotonNetwork.JoinRandomOrCreateRoom();
    }

    public override void OnJoinedRoom()
    {
        int playerOrder = PhotonNetwork.PlayerList.Length;

        Vector3    spawnPosition = characterSpawnTrans[playerOrder - 1].position;
        Quaternion spawnRotation = characterSpawnTrans[playerOrder - 1].rotation;

        PhotonNetwork.Instantiate("Character", spawnPosition, spawnRotation).GetComponent<CharacterIKNetwork>();
    }

    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        Debug.Log($"join room failed: {message}");
    }
}