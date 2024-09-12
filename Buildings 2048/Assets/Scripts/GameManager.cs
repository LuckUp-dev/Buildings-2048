using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UIElements;
using Random = UnityEngine.Random;

public class GameManager : MonoBehaviour
{
    [SerializeField] private int _width = 4;
    [SerializeField] private int _height = 4;
    [SerializeField] private Tile _tilePrefab;
    [SerializeField] private Cube _cubePrefab;
    [SerializeField] private float _travelTime = 0.2f;
    [SerializeField] private AudioSource _swipeSound;
    [SerializeField] private AudioSource _mergeSound;
  //  [SerializeField] private int _winCondition = 2048;
    [SerializeField] private List<CubeTypes> _types;

 //   [SerializeField] private GameObject _winScreen, _loseScreen;

    private List<Tile> _tiles;
    private List<Cube> _cubes;
    private GameState _state;
    private int _round;

    private CubeTypes GetCubeTypeByValue(int value) => _types.First(t => t.Value == value);

    private void Start()
    {
        _round = 0;
        _tiles = new List<Tile>();
        _cubes = new List<Cube>();

        for (int x = 0; x < _width; x++)
        {
            for (int z = 0; z < _height; z++)
            {
                var tile = Instantiate(_tilePrefab, new Vector3(x, -0.5f, z) , Quaternion.identity );
                _tiles.Add(tile);
            }
        }
        var center = new Vector3((float)_width / 2 -0.5f, 0f, (float)_height /2 -0.5f);

        Camera.main.transform.position = new Vector3(1.5f, 5.5f, -2.2f);
        ChangeState(GameState.SpawnCubes);     //Меняет статус на спавн блоков

    }

    private void ChangeState(GameState newState)
    {
        _state = newState;

        switch (newState)
        {
            case GameState.SpawnCubes:
                SpawnCubes(_round++ == 0 ? 2 : 1);
                break;
            case GameState.WaitingInput:
                break;
            case GameState.Moving:
                break;
            case GameState.Win:
                break;
            case GameState.Lose:
                break;
            default:
                break;
        }
    }

    private void Update()
    {
        if (_state != GameState.WaitingInput)
            return;

        if (Input.GetKeyDown(KeyCode.LeftArrow))
            MoveCubes(Vector3.left);
        if (Input.GetKeyDown(KeyCode.RightArrow))
            MoveCubes(Vector3.right);
        if (Input.GetKeyDown(KeyCode.UpArrow))
            MoveCubes(Vector3.forward);
        if (Input.GetKeyDown(KeyCode.DownArrow))
            MoveCubes(Vector3.back);
    }

    void SpawnCubes(int amount)
    {
        var freeTiles = _tiles.Where(t => t.OccupiedTile == null).OrderBy(c => Random.value).ToList();      // u заменить на что-то нормальное и разобраться что это вообще

        foreach (var tile in freeTiles.Take(amount))
        {
            SpawnCube(tile, Random.value > 0.8f ? 4 : 2);
        }

        if (freeTiles.Count == 1)
        {
            //lose      //поставить статус проиграл
        }
        //поставить статус + проверка на победу
        ChangeState(GameState.WaitingInput);
    }

    void SpawnCube(Tile tile, int value)
    {
        var cube = Instantiate(_cubePrefab, new Vector3(tile.Pos.x, tile.Pos.y + 0.5f, tile.Pos.z), Quaternion.identity);
        cube.Init(GetCubeTypeByValue(value));
        cube.SetCube(tile);
        _cubes.Add(cube);
    }

    void MoveCubes(Vector3 dir)
    {
        _swipeSound.Play();
        ChangeState(GameState.Moving);
        var existingCubes = _cubes.OrderBy(c => c.Pos.x).ThenBy(c => c.Pos.z).ToList();
        if (dir == Vector3.right || dir == Vector3.forward)
            existingCubes.Reverse();

        foreach (var cube in existingCubes)
        {
            var next = cube.Tile;
            do
            {
                cube.SetCube(next);

                var possibleTile = GetTileAtPosition(next.Pos + dir);
                if (possibleTile != null)
                {
                    if (possibleTile.OccupiedTile != null && possibleTile.OccupiedTile.CanMerge(cube.Value))
                        cube.MergeCube(possibleTile.OccupiedTile);
                    else if (possibleTile.OccupiedTile == null)
                        next = possibleTile;
                }

            } while (next != cube.Tile);
        }

        var sequence = DOTween.Sequence();

        foreach (var cube in existingCubes)
        {
            var movepoint = cube.MergingCube != null ? cube.MergingCube.Tile.Pos : cube.Tile.Pos;

            sequence.Insert(0, cube.transform.DOMove(cube.Tile.Pos + new Vector3(0, 0.5f, 0), _travelTime));
        }

        sequence.OnComplete(() =>
        {
            foreach (var cube in existingCubes.Where(c => c.MergingCube != null))
            {
                MergeCube(cube.MergingCube, cube);
            }
            ChangeState(GameState.SpawnCubes);
        });
    }

    void MergeCube(Cube baseCube, Cube mergingCube)
    {
        SpawnCube(baseCube.Tile, baseCube.Value * 2);
        MergeSound();

        RemoveCube(baseCube);
        RemoveCube(mergingCube);

    }

    void RemoveCube(Cube cube)
    {
        _cubes.Remove(cube);
        Destroy(cube.gameObject);
    }


    Tile GetTileAtPosition(Vector3 pos)
    {
        return _tiles.FirstOrDefault(t => t.Pos == pos);
    }

    void MergeSound()
    {
        _mergeSound.Play();
    }

}


[Serializable]

public struct CubeTypes
{
    public int Value;
    public Material material;
    public Vector3 vec;
}

public enum GameState
{
    SpawnCubes,
    WaitingInput,
    Moving,
    Win,
    Lose
}

