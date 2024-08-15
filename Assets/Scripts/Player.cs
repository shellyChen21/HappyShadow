using Character;
using UnityEngine;


public class Player : MonoBehaviour
{
    
    [SerializeField] private TrackerData trackData;
    
    private CharacterIKNetwork characterIKNetwork;
    
    public void SetCharacterIKNetwork(CharacterIKNetwork characterIKNetwork)
    {
        this.characterIKNetwork = characterIKNetwork;
    }
   
    void Update()
    {
        SyncTrackerTrans();
    }
    
    private void SyncTrackerTrans()
    {
        if (characterIKNetwork == null) return;

        characterIKNetwork.trackerData.SyncTrans(trackData);
    }
}
