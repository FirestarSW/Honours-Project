using UnityEngine;
using System;

namespace ProceduralSwordGenerator {
    [CreateAssetMenu(fileName = "SwordData", menuName = "Scriptable Objects/Procedural Sword Generator/SwordData")]
    public class SwordData : ScriptableObject {
        public SwordDataStructs.Sword swordData;

    }

}
