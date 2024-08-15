using Photon.Pun;
using UnityEngine;

namespace ClearSky
{
    public class SimplePlayerController : MonoBehaviourPun
    {
        public float movePower = 10f;
        public float jumpPower = 15f;

        private Rigidbody2D rb;
        private Animator anim;
        Vector3 movement;
        private int direction = 1;
        bool isJumping = false;

        void Start()
        {
            rb = GetComponent<Rigidbody2D>();
            anim = GetComponent<Animator>();
        }

        private void Update()
        {
            if (!PhotonNetwork.IsMasterClient)
                return;

            Jump();
            Walk();
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            anim.SetBool("isJump", false);
        }


        void Walk()
        {
           

            if (Input.GetAxisRaw("Horizontal") == 0)
            {
                anim.SetBool("isWalk", false);
                photonView.RPC(nameof(RPC_Animator), RpcTarget.Others, false);
                return;
            }

            Vector3 moveVelocity = Vector3.zero;

            if (Input.GetAxisRaw("Horizontal") < 0)
            {
                direction = -1;
                moveVelocity = Vector3.left;

                transform.localScale = new Vector3(direction, 1, 1);
                if (!anim.GetBool("isJump"))
                {
                    anim.SetBool("isRun", true);
                    photonView.RPC(nameof(RPC_Animator), RpcTarget.Others, true);
                }
            }

            if (Input.GetAxisRaw("Horizontal") > 0)
            {
                direction = 1;
                moveVelocity = Vector3.right;

                transform.localScale = new Vector3(direction, 1, 1);
                if (!anim.GetBool("isJump"))
                {
                    anim.SetBool("isWalk", true);
                    photonView.RPC(nameof(RPC_Animator), RpcTarget.Others, true);
                }
            }

            transform.position += moveVelocity * movePower * Time.deltaTime;
        }

        void Jump()
        {
            if ((Input.GetButtonDown("Jump") || Input.GetAxisRaw("Vertical") > 0)
                && !anim.GetBool("isJump"))
            {
                isJumping = true;
                anim.SetBool("isJump", true);
            }

            if (!isJumping)
            {
                return;
            }

            rb.velocity = Vector2.zero;

            Vector2 jumpVelocity = new Vector2(0, jumpPower);
            rb.AddForce(jumpVelocity, ForceMode2D.Impulse);

            isJumping = false;
        }


        [PunRPC]
        public void RPC_Animator(bool isOn)
        {
            anim.SetBool("isWalk", isOn);
        }
    }
}