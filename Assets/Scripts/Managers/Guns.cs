using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName="Data", menuName="ScriptableObjects/Guns", order=1)]
public class Guns : ScriptableObject
{
    public GameObject[] Weapons;
}
