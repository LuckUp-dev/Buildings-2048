using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Cube : MonoBehaviour
{
    public int Value;
    public Tile Tile;
    public Cube MergingCube;
    public bool Merging;

    private Vector3 _maxScale;
    private Vector3 min;

   float timer = 1f;

    public Vector3 Pos => transform.position;
    [SerializeField] private TextMeshPro _text;
    [SerializeField] private MeshRenderer _renderer;
    [SerializeField] private Transform _scale;

    public void Scale(Vector3 scale)//
    {
        min = new Vector3(0.7f, scale.y, 0.7f);
        _maxScale = scale;
        timer = 0;
       
    }

    private void Update()
    {
        timer += Time.deltaTime;

        if (timer < 1)
        {
            transform.localScale = Vector3.Lerp(min, _maxScale, timer);
            min= transform.localScale;
        }
    }

    public void Init(CubeTypes type)
    {
        Value = type.Value;
        _renderer.material = type.material;
        _scale.localScale += type.vec;
        _text.text = type.Value.ToString();
        Scale(_scale.localScale);//
    }


    public void SetCube(Tile tile)
    {
        if (Tile != null)
            Tile.OccupiedTile = null;
        Tile= tile;
        Tile.OccupiedTile = this;
    }

    public void MergeCube(Cube cubeToMerge)
    {
        MergingCube = cubeToMerge;

        Tile.OccupiedTile= null;

        cubeToMerge.Merging = true;
    }

    public bool CanMerge(int value) => value == Value && !Merging && MergingCube == null;

}
