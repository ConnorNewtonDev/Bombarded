using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Networking;
using UnityEngine;

public class BombCollection : MonoBehaviour
{
    public static BombCollection instance;
    [SerializeField] BombData[] collection;
    private void Awake()
    {
        if (!instance)
            instance = this;
        else if (instance != this)
            Destroy(gameObject);
    }

    public GameObject GetBomb(BombType bType)
    {
        return (from bombData in collection where bombData.bType == bType select bombData.prefab).FirstOrDefault();
    }

    public GameObject GetBomb(int bType)
    {
        return (from bombData in collection where bombData.bType == (BombType)bType select bombData.prefab).FirstOrDefault();
    }
    
    [Serializable]
    public struct BombData
    {
        public BombType bType;
        public GameObject prefab;
    }
}
