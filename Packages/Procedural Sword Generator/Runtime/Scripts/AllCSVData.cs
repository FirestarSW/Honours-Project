using ProceduralSwordGenerator.CSVDataStructs;
using UnityEngine;

namespace ProceduralSwordGenerator { 
    [CreateAssetMenu(fileName = "AllCSVData", menuName = "Scriptable Objects/Procedural Sword Generator/AllCSVData")]
    
    public class AllCSVData : ScriptableObject {
        public CSVDataStructs.Sword[] swords;
 
        public CSVDataStructs.Blade[] blades;
        public CSVDataStructs.Tang[] tangs;
        public CSVDataStructs.Fuller[] fullers;

        public CSVDataStructs.Guard[] guards;

        public CSVDataStructs.Grip[] grips;

        public CSVDataStructs.Pommel[] pommels;

    }

}
