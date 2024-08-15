using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PrimeTween;

public class WallManager : MonoBehaviour
{
    [SerializeField] private Transform[] walls;
    [SerializeField] private Light[] lights;
    [SerializeField] private bool[] isTurn;
    [SerializeField] private float time;

    private bool[] isUp;
    private bool[] isDown;

    private void Awake()
    {
        isUp = new bool[walls.Length];
        isDown = new bool[walls.Length];

        for (int i = 0; i < isDown.Length; i++) 
        {
            isDown[i] = true;
        }
    }


    // Update is called once per frame
    void Update()
    {
        if (walls == null)
            return;

        if (Input.GetKeyDown(KeyCode.U)) 
        {
            isTurn[0] = true;
            isTurn[1] = true;
            isTurn[2] = true;
        }

        for (int i = 0; i < walls.Length; i++) 
        {
            WallAction(i);
        }
    }

    public void WallAction(int index)
    {
        if (isTurn[index])
        {
            if (isUp[index])
                return;

            isDown[index] = false;
            isUp[index] = true;

            var originY = walls[index].transform.eulerAngles.y;

            Tween.LightIntensity(lights[index], 80, time, Ease.InOutCubic);
            Tween.Rotation(walls[index], new Vector3(-90, originY, 0), new Vector3(0, originY, 0), time,Ease.OutCubic);
        }
        else
        {
            if (isDown[index])
                return;

            isDown[index] = true;
            isUp[index] = false;


            var originY = walls[index].transform.eulerAngles.y;

            Tween.LightIntensity(lights[index], 0, time, Ease.InOutCubic);
            Tween.Rotation(walls[index], new Vector3(0, originY, 0), new Vector3(-90, originY, 0), time, Ease.OutCubic);
        }       
    }
}
