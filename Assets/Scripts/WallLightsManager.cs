using UnityEngine;

public class WallLightsManager : MonoBehaviour
{
    [SerializeField] private GameObject wall1;
    [SerializeField] private GameObject wall2;
    [SerializeField] private GameObject wall3;
    [SerializeField] private GameObject wall4;
    [SerializeField] private GameObject wall5;
    [SerializeField] private GameObject wall6;

    // [SerializeField] bool useWall1;
    // [SerializeField] bool useWall2;
    // [SerializeField] bool useWall3;
    // [SerializeField] bool useWall4;
    // [SerializeField] bool useWall5;
    // [SerializeField] bool useWall6;

    [HideInInspector] public Vector3 NowLightPos { get; private set; }

    public void ChangeLightPos(string wallName)
    {
        switch (wallName)
        {
            case "Hexagon_Wall_1":
                NowLightPos = wall1.transform.position;
                break;
            case "Hexagon_Wall_2":
                NowLightPos = wall2.transform.position;
                break;
            case "Hexagon_Wall_3":
                NowLightPos = wall3.transform.position;
                break;
            case "Hexagon_Wall_4":
                NowLightPos = wall4.transform.position;
                break;
            case "Hexagon_Wall_5":
                NowLightPos = wall5.transform.position;
                break;
            case "Hexagon_Wall_6":
                NowLightPos = wall6.transform.position;
                break;
        }
    }
}
