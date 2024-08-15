using System.Collections;
using Photon.Pun;
using UnityEngine;

namespace Character
{
    [System.Serializable]
    public class TrackerData
    {
        public Transform head;
        public Transform leftHand;
        public Transform rightHand;

        public void SyncTrans(TrackerData trackerData)
        {
            head.position = trackerData.head.position;
            head.rotation = trackerData.head.rotation;

            leftHand.position  = trackerData.leftHand.position;
            leftHand.rotation  = trackerData.leftHand.rotation;
            rightHand.position = trackerData.rightHand.position;
            rightHand.rotation = trackerData.rightHand.rotation;

        }
    }

    public class CharacterIKNetwork : MonoBehaviourPun, IPunObservable
    {
        public TrackerData trackerData;

        private Vector3    syncHeadPosition;
        private Quaternion syncHeadRotation;

        private Vector3    syncHandLPosition;
        private Quaternion syncHandLRotation;
        private Vector3    syncHandRPosition;
        private Quaternion syncHandRRotation;

        private IEnumerator Start()
        {
            if (photonView.IsMine)
            {
                FindObjectOfType<Player>().SetCharacterIKNetwork(this);
            }

            yield return new WaitForSeconds(2);
        }

        private void Update()
        {
            if (photonView.IsMine)
                return;

            SyncIKTrans();
        }

        private void SyncIKTrans()
        {

            trackerData.head.localPosition = syncHeadPosition;
            trackerData.head.localRotation = syncHeadRotation;

            trackerData.leftHand.localPosition  = syncHandLPosition;
            trackerData.leftHand.localRotation  = syncHandLRotation;
            trackerData.rightHand.localPosition = syncHandRPosition;
            trackerData.rightHand.localRotation = syncHandRRotation;
        }

        public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
        {
            if (stream.IsWriting)
            {
                stream.SendNext(trackerData.head.localPosition);
                stream.SendNext(trackerData.head.localRotation);

                stream.SendNext(trackerData.leftHand.localPosition);
                stream.SendNext(trackerData.leftHand.localRotation);
                stream.SendNext(trackerData.rightHand.localPosition);
                stream.SendNext(trackerData.rightHand.localRotation);

            }
            else
            {
                syncHeadPosition = (Vector3)stream.ReceiveNext();
                syncHeadRotation = (Quaternion)stream.ReceiveNext();

                syncHandLPosition = (Vector3)stream.ReceiveNext();
                syncHandLRotation = (Quaternion)stream.ReceiveNext();
                syncHandRPosition = (Vector3)stream.ReceiveNext();
                syncHandRRotation = (Quaternion)stream.ReceiveNext();

            }
        }
    }
}