using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/DecorationAsset", order = 1)]
public class DecorationAsset : ScriptableObject{
        public GameObject prefab;
        public Vector2 area;
        public RoomGenerator.Zone zone;
}
