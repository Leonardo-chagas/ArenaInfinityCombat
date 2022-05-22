using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName="Data", menuName="ScriptableObjects/Animator Controllers", order=1)]
public class AnimatorControllers : ScriptableObject
{
    public RuntimeAnimatorController[] animatorControllers;
}
