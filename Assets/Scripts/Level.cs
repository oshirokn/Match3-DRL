using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "World", menuName = "Level")]
public class Level : ScriptableObject
{

    public int width;
    public int height;
    public float sTime;

    public GameObject[] dots;
}
