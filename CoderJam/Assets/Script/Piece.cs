using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Piece : MonoBehaviour
{
    public bool isShot = false; 

    public void ShotPiece()
    {
        isShot = true;
    }

    public bool IsShot()
    {
        return isShot;
    }
}
