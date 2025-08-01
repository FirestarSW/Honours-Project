using UnityEngine;
using System;

namespace ProceduralSwordGenerator {
    namespace SwordDataStructs {
        public enum SwordPartEnum { SWORD, BLADE, GUARD, GRIP, POMMEL };

        [Serializable]
        public struct Sword {
            public string name;
            public int id;
            public Blade blade;
            public Guard[] trueGuards;
            public Guard[] falseGuards;
            public Grip grip;
            public Pommel pommel;

            public float totalLength;
            public float totalWeight;

            public float pob;

        }

        [Serializable]
        public struct Shape {
            [Header("Totals")]
            public float totalLength;

            [Header("Width")]
            public float baseWidth;
            public float tipWidth;

            [Header("Thickness")]
            public float baseThickness;
            public float tipThickness;

            [Header("Profiles & Cross Sections")]
            public Profile[] leftProfiles;
            public Profile[] rightProfiles;

            public CrossSections[] crossSections;

            [Serializable]
            public struct Profile {
                public float baseX;
                public float topX;

                public float length;

                public float baseWidth;
                public float topWidth;

                public string shape;

            }

            [Serializable]
            public struct CrossSections {
                public float y;

                // Uses blade terminology
                public EdgeCrossSection[] trueEdges; // Left edges
                public EdgeCrossSection[] falseEdges; // Right edges

                [Serializable]
                public struct EdgeCrossSection {
                    public float x;

                    // Looking at 2d edge section base at bottom, top at top
                    public CrossSectionThickness left;
                    public CrossSectionThickness right;

                }

                [Serializable]
                public struct CrossSectionThickness {
                    public float baseThickness;
                    public float topThickness;

                    public string shape;

                }

            }

        }

        [Serializable]
        public struct Blade {
            public string name;

            [Header("Totals")]
            public float totalLength;
            public float totalWeight;

            [Header("Weight & Material")]
            public float bladeWeight;
            public string bladeMaterial;
            public string bladeMaterialPath;

            public float pob;

            [Header("Shape")]
            public Shape shape;

            [Header("Fullers")]
            public Fuller[] topFaceFullers;
            public Fuller[] bottomFaceFullers;

            [Header("Tang")]
            public Tang tang;

        }

        [Serializable]
        public struct Tang {
            [Header("Weight & Material")]
            public float weight;
            public string material;

            public float pob;

            [Header("Shape")]
            public Shape shape;

        }

        [Serializable]
        public struct Fuller {
            public float centreX;
            public float startY;

            [Header("Sides")]
            public FullerSide[] leftSide;
            public FullerSide[] rightSide;

            [Serializable]
            public struct FullerSide {
                public float y;

                public float x;
                public float depth;

            }

        }

        [Serializable]
        public struct Guard {
            public string name;

            [Header("Weight & Material")]
            public float weight;
            public string material;
            public string materialPath;

            public float pob;

            [Header("Shape")]
            public Shape shape;

        }

        [Serializable]
        public struct Grip {
            public string name;

            [Header("Weight & Material")]
            public float weight;
            public string material;
            public string materialPath;

            public float pob;

            [Header("Shape")]
            public Shape shape;

        }

        [Serializable]
        public struct Pommel {
            public string name;

            [Header("Weight & Material")]
            public float weight;
            public string material;
            public string materialPath;

            public float pob;

            [Header("Shape")]
            public Shape shape;

        }

    }

}
