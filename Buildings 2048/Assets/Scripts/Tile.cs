using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour
{
    public Vector3 Pos => transform.position;

    public Cube OccupiedTile;
}
