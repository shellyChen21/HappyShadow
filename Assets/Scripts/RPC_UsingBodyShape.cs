using UnityEngine;
using Photon.Pun;

public class RPC_UsingBodyShape : MonoBehaviourPunCallbacks
{
    [SerializeField] private GameObject[] bodys;
    [SerializeField] private ObjectDeformer ObjectDeformer;

    private int playerID;

    void Start()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            int id = CharacterRepository.AddCharacter(photonView.ViewID);
            
            photonView.RPC(nameof(RPC_BodyShapes), RpcTarget.AllBuffered,  id);
        }
    }

    public override void OnPlayerLeftRoom(Photon.Realtime.Player player) 
    {
        if (photonView.Owner == player && PhotonNetwork.IsMasterClient)
        {
            CharacterRepository.RemoveCharacter(playerID);            
        }
    }

    [PunRPC]
    private void RPC_BodyShapes(int bodysID)
    {
        playerID = bodysID;

        for (int i = 0; i < bodys.Length; i++)
        {
            bodys[i].SetActive(i == bodysID);
            SetBody();
        }
    }

    private void SetBody()
    {
        ObjectDeformer.SetSourceObject(bodys[playerID].transform);
    }
}