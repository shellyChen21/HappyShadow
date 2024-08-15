using UnityEngine;

public class WallLightsManager : MonoBehaviour
{
    [SerializeField] private GameObject wall1;
    [SerializeField] private GameObject wall2;
    [SerializeField] private GameObject wall3;
    [SerializeField] private GameObject wall4;
    [SerializeField] private GameObject wall5;
    [SerializeField] private GameObject wall6;

    [SerializeField] bool useWall1;
    [SerializeField] bool useWall2;
    [SerializeField] bool useWall3;
    [SerializeField] bool useWall4;
    [SerializeField] bool useWall5;
    [SerializeField] bool useWall6;

    [HideInInspector] public Vector3 NowLightPos { get; private set; }
    void FixedUpdate()
    {
        if(useWall1) 
        {
            Wall1Light();
        }
        else if(useWall2)
        {
            Wall2Light();
        }
        else if (useWall3)
        {
            Wall3Light();
        }
        else if (useWall4)
        {
            Wall4Light();
        }

        else if (useWall5)
        {
            Wall5Light();
        }
        else if (useWall6)
        {
            Wall6Light();
        }
    }

    public void Wall1Light() 
    {
        useWall2 = false;
        useWall3 = false;
        useWall4 = false;
        useWall5 = false;
        useWall6 = false;
        NowLightPos = wall1.transform.position;
    }
    public void Wall2Light()
    {
        useWall1 = false;
        useWall3 = false;
        useWall4 = false;
        useWall5 = false;
        useWall6 = false;
        NowLightPos = wall2.transform.position;
    }

    public void Wall3Light()
    {
        useWall1 = false;
        useWall2 = false;
        useWall4 = false;
        useWall5 = false;
        useWall6 = false;
        NowLightPos = wall3.transform.position;
    }
    public void Wall4Light()
    {
        useWall1 = false;
        useWall2 = false;
        useWall3 = false;
        useWall5 = false;
        useWall6 = false;
        NowLightPos = wall4.transform.position;
    }
    public void Wall5Light()
    {
        useWall1 = false;
        useWall2 = false;
        useWall3 = false;
        useWall4 = false;
        useWall6 = false;
        NowLightPos = wall5.transform.position;
    }
    public void Wall6Light()
    {
        useWall1 = false;
        useWall2 = false;
        useWall3 = false;
        useWall4 = false;
        useWall5 = false;
        NowLightPos = wall6.transform.position;
    }
}
