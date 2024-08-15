using UnityEngine;
using Photon.Pun;
using PrimeTween;

public class LittleGirlDecalController : MonoBehaviourPun
{
    [Header("Decal Target Setting")] [SerializeField]
    private LayerMask targetLayer;

    [SerializeField] private Transform decalTarget;

    [Header("Little Girl Setting")] [SerializeField]
    private float movePower = 10f;

    [SerializeField] private float jumpPower = 15f;
    [SerializeField] private CharacterController cc;
    [SerializeField] private Animator girlAnimator;
    [SerializeField] private Transform girl;
    [SerializeField] private SpriteRenderer girlSkirt;

    [Header("Auto Move Setting")] [SerializeField]
    private bool isAutoMove;

    private Collider preCollider;
    private float gravity = -4f;
    private float verticalVelocity;
    private int lastDirection = 1;
    private bool isWearSkirt;

    private bool isCanMove = true;

    private void Update()
    {
        if (!PhotonNetwork.IsMasterClient)
            return;

        WalkAndJump();
        WearSkirt();
        AutoMove();
    }

    private void AutoMove()
    {
        if(!isAutoMove)
            return;

        isCanMove = false;
        girl.localScale = new Vector3(-1, 1, 1);

        var moveDir = -decalTarget.right;
        moveDir = moveDir.normalized * movePower;

        verticalVelocity += -9.81f * Time.deltaTime;

        moveDir.y = verticalVelocity;


        girlAnimator.SetBool("isWalk", true);
        cc.Move(moveDir * Time.deltaTime);
    }

    void FixedUpdate()
    {
        Vector3 rayDirection = (decalTarget.position - transform.position).normalized;
        Ray ray = new Ray(transform.position, rayDirection);

        if (!Physics.Raycast(ray, out RaycastHit hit, 40f, targetLayer))
            return;
        Debug.DrawLine(transform.position, hit.point, Color.red);

        // Debug.Log(hit.transform.name);

        // if (preCollider == hit.collider)
        // {
        //     decalTarget.localRotation = Quaternion.Euler(0, 0, 0);
        //
        //     decalTarget.localPosition = new Vector3(decalTarget.localPosition.x, decalTarget.localPosition.y, 0);
        //     return;
        // }


        decalTarget.SetParent(hit.transform);

        decalTarget.localRotation = Quaternion.Euler(0, 0, 0);

        decalTarget.localPosition = new Vector3(decalTarget.localPosition.x, decalTarget.localPosition.y, 0);
        
        decalTarget.SetParent(null);

        // preCollider = hit.collider;
    }

    void WearSkirt()
    {
        if (!Input.GetKeyDown(KeyCode.T) || isWearSkirt)
            return;

        isWearSkirt = true;

        photonView.RPC(nameof(RPC_SkirtAnim), RpcTarget.All, "isSkirt");

        Tween.Custom(0, 1, 2,
            onValueChange: newflaot => { photonView.RPC(nameof(RPC_ChangeSkirt), RpcTarget.All, newflaot); });
    }

    void WalkAndJump()
    {
        if(!isCanMove)
            return;
        
        var input_V = Input.GetAxis("Vertical");
        var input_H = Input.GetAxis("Horizontal");

        var isWalk = input_H == 0 ? false : true;
        if (girlAnimator.GetBool("isWalk") != isWalk)
        {
            girlAnimator.SetBool("isWalk", isWalk);
            photonView.RPC(nameof(RPC_Walk), RpcTarget.Others, isWalk);
        }

        if (input_H != 0)
        {
            var newDirection = input_H < 0 ? 1 : -1;
            if (lastDirection != newDirection)
            {
                lastDirection = newDirection;
                girl.localScale = new Vector3(lastDirection, 1, 1);
                photonView.RPC(nameof(RPC_GirlScale), RpcTarget.Others, lastDirection);
            }
        }

        if (Input.GetKeyDown(KeyCode.W))
        {
            verticalVelocity = jumpPower;
        }
        else
        {
            verticalVelocity += gravity * Time.deltaTime;
        }

        if (cc.isGrounded)
        {
            girlAnimator.SetBool("isJump", false);
            photonView.RPC(nameof(RPC_Jump), RpcTarget.Others, false);
        }
        else
        {
            girlAnimator.SetBool("isJump", true);
            photonView.RPC(nameof(RPC_Jump), RpcTarget.Others, true);
        }

        Vector3 moveDir = decalTarget.right * -input_H + decalTarget.up * input_V;
        moveDir = moveDir.normalized * movePower;

        moveDir.y = verticalVelocity;

        cc.Move(moveDir * Time.deltaTime);
    }

    public void SetAutoMoveFalse()
    {
        isAutoMove = false;
        isCanMove = true;
        girlAnimator.SetBool("isWalk", false);
    }

    [PunRPC]
    public void RPC_Jump(bool isJump)
    {
        girlAnimator.SetBool("isJump", isJump);
    }

    [PunRPC]
    public void RPC_Walk(bool isWalk)
    {
        girlAnimator.SetBool("isWalk", isWalk);
    }

    [PunRPC]
    public void RPC_GirlScale(int girlScale)
    {
        girl.localScale = new Vector3(girlScale, 1, 1);
    }

    [PunRPC]
    public void RPC_SkirtAnim(string triggerName)
    {
        girlAnimator.SetTrigger(triggerName);
    }

    [PunRPC]
    public void RPC_ChangeSkirt(float skirtShowUp)
    {
        girlSkirt.material.SetFloat("_Cutoff_Height", skirtShowUp);
    }
}