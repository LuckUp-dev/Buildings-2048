using Newtonsoft.Json.Linq;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Spawn : MonoBehaviour
{
    [SerializeField] GameObject _firstLevelCube;

    Transform[] _spawnPoints = new Transform[16];

    Vector3 spawnY = new Vector3(0f, 0.5f, 0);

    public float sphereRadious = 0.2f;
    bool _checkTile;



    private void Start()    //Adding spawn points to the array. Spawn of the first cube
    {
        int _firstCubeSpawn = Random.Range(0, 16);

        for (int i = 0; i < _spawnPoints.Length; i++)
        {
            _spawnPoints[i] = this.gameObject.transform.GetChild(i);
        }

        Instantiate(_firstLevelCube, _spawnPoints[_firstCubeSpawn].position + spawnY, Quaternion.identity); //Изменить начальный спавн на рандом

    }

    private void Update()   
    {
        if (Input.GetMouseButtonDown(0))
        {
            SpawnCube();
        }
    }

    private void SpawnCube()    //Spawn cubes if its possible
    {
        int _randomTile = Random.Range(0, 16);
        _checkTile = CheckSpawnPoint(_randomTile);

        if (_checkTile)
        {
            Instantiate(_firstLevelCube, _spawnPoints[_randomTile].position + spawnY, Quaternion.identity);
            IsPointEmpty();
        }
        else
            SpawnCube();

    }


    bool CheckSpawnPoint(int value) //Cheching on free spot
    {
        if (!Physics.CheckSphere(_spawnPoints[value].position + spawnY, sphereRadious))
            return true;
        else
            return false;
    }

    void IsPointEmpty() //Checking if no spots, then and for game
    {
        bool _emptyTile = false;

        for (int i = 0; i < _spawnPoints.Length; i++)
        {
            if (!Physics.CheckSphere(_spawnPoints[i].position + spawnY, sphereRadious))
                _emptyTile = true;
        }

        if (_emptyTile == false)
            print("нет места");
    }
}
