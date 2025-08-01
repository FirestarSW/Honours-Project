using UnityEngine;
using System;
using ProceduralSwordGenerator.SwordDataStructs;

namespace ProceduralSwordGenerator {

    namespace CSVDataStructs {
        [Serializable]
        public struct Sword {
            public string name;
            public int id;
            public string type;

            public int bladeId;
            public int tangId;

            public int[] fullerTopFaceIds;
            public float[] fullerTopFaceCentreX;
            public float[] fullerTopFaceStartY;

            public int[] fullerBottomFaceIds;
            public float[] fullerBottomFaceCentreX;
            public float[] fullerBottomFaceStartY;

            public int[] trueGuardIds;
            public int[] falseGuardIds;
            public int gripId;
            public int pommelId;

        }

        [Serializable]
        public struct Blade {
            public string name;
            public int id;

            public Shape shape;

            public float weight;
            public string material;

        }

        [Serializable]
        public struct Tang {
            public string name;
            public int id;

            public Shape shape;

            public float weight;
            public string material;

        }

        [Serializable]
        public struct Fuller {
            public string name;
            public int id;

            public float[] y;

            public float[] leftX;
            public float[] rightX;

            public float[] leftDepth;
            public float[] rightDepth;

        }

        [Serializable]
        public struct Guard {
            public string name;
            public int id;

            public float weight;
            public string material;

            public SwordDataStructs.Shape shape;

        }

        [Serializable]
        public struct Grip {
            public string name;
            public int id;

            public float weight;
            public string material;

            public SwordDataStructs.Shape shape;

        }

        [Serializable]
        public struct Pommel {
            public string name;
            public int id;

            public float weight;
            public string material;

            public SwordDataStructs.Shape shape;

        }

    }

}
