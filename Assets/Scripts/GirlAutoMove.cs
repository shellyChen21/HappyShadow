using UnityEngine;

public class GirlAutoMove : MonoBehaviour
{
    [SerializeField] private LittleGirlDecalController littleGirlDecalController;


    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("MoveTarget"))
            return;

        littleGirlDecalController.SetAutoMoveFalse();

        other.GetComponent<BoxCollider>().enabled = false;
    }
}