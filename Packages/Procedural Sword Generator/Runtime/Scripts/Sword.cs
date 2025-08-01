using ProceduralSwordGenerator.CSVDataStructs;
using ProceduralSwordGenerator.SwordDataStructs;
using System;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

namespace ProceduralSwordGenerator {
    public class Sword : MonoBehaviour {
        // In cm's and g's
        [SerializeField] SwordPommel pommel;
        [SerializeField] SwordGrip grip;
        [SerializeField] SwordGuard guard;
        [SerializeField] SwordBlade blade;

        [SerializeField] public SwordData swordData;

        [SerializeField] string[] defaultMaterialPaths = new string[4];

        bool setupCompleted = false;

        MeshRenderer meshRenderer;

        [SerializeField] SwordDataStructs.SwordPartEnum partToRender = SwordDataStructs.SwordPartEnum.SWORD;

        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start() {
            if (swordData && !setupCompleted) {
                Setup(swordData);

            }

        }

        public void Setup(SwordData swordData) {
            this.swordData = swordData;
           
            // Get parts in children
            pommel = GetComponentInChildren<SwordPommel>();
            grip = GetComponentInChildren<SwordGrip>();
            guard = GetComponentInChildren<SwordGuard>();
            blade = GetComponentInChildren<SwordBlade>();

            // If missing any - add them
            if (!blade) { GameObject bladeObject = new GameObject("Blade"); bladeObject.transform.position = transform.position; bladeObject.transform.parent = gameObject.transform; blade = bladeObject.AddComponent<SwordBlade>(); }
            if (!guard) { GameObject guardObject = new GameObject("Guards"); guardObject.transform.position = transform.position; guardObject.transform.parent = gameObject.transform; guard = guardObject.AddComponent<SwordGuard>(); }
            if (!grip) { GameObject gripObject = new GameObject("Grip"); gripObject.transform.position = transform.position; gripObject.transform.parent = gameObject.transform; grip = gripObject.AddComponent<SwordGrip>(); }
            if (!pommel) { GameObject pommelObject = new GameObject("Pommel"); pommelObject.transform.position = transform.position; pommelObject.transform.parent = gameObject.transform; pommel = pommelObject.AddComponent<SwordPommel>(); }

            // If theres sword data - doesnt work for struct
            if (swordData) {
                // Move parts according to swordData
                Vector3 pommelPosition = pommel.transform.position;

                float tangLength = swordData.swordData.blade.tang.shape.totalLength;

                pommelPosition.y = -(tangLength != 0 ? tangLength : grip.transform.position.y + swordData.swordData.grip.shape.totalLength);

                pommel.transform.position = pommelPosition;

                // Give parts reference of data
                blade.SetData(ref swordData, SwordDataStructs.SwordPartEnum.BLADE);
                guard.SetData(ref swordData, SwordDataStructs.SwordPartEnum.GUARD);
                grip.SetData(ref swordData, SwordDataStructs.SwordPartEnum.GRIP);
                pommel.SetData(ref swordData, SwordDataStructs.SwordPartEnum.POMMEL);

                // Setup parts
                blade.Setup(defaultMaterialPaths[0]);
                guard.Setup(defaultMaterialPaths[1]);
                grip.Setup(defaultMaterialPaths[2]);
                pommel.Setup(defaultMaterialPaths[3]);

                // Get sword mesh - if missing, add and create mesh
                SwordMesh swordMesh = GetComponent<SwordMesh>();
                if (!swordMesh) swordMesh = gameObject.AddComponent<SwordMesh>(); swordMesh.CreateMesh(swordData.swordData.name);

                meshRenderer = swordMesh.GetComponent<MeshRenderer>();

                // Calculates swords total length - has same problem with grip positions possibly being off when tang isnt used
                swordData.swordData.totalLength = swordData.swordData.blade.shape.totalLength
                    + (swordData.swordData.blade.tang.shape.totalLength > swordData.swordData.grip.shape.totalLength ? swordData.swordData.blade.tang.shape.totalLength : swordData.swordData.grip.shape.totalLength)
                    + swordData.swordData.pommel.shape.totalLength;

                // Calculates swords total weight
                swordData.swordData.totalWeight = swordData.swordData.blade.totalWeight + guard.totalWeight + swordData.swordData.grip.weight + swordData.swordData.pommel.weight;

                // Done incorrectly, start from pommel and move along instead of starting from base of blade
                swordData.swordData.pob = swordData.swordData.totalWeight != 0 ? (swordData.swordData.blade.pob * swordData.swordData.blade.bladeWeight
                    + -swordData.swordData.grip.pob * swordData.swordData.grip.weight
                    + -(swordData.swordData.pommel.pob - pommel.gameObject.transform.position.y) * swordData.swordData.pommel.weight) / swordData.swordData.totalWeight : 0;

            }

            setupCompleted = true;

        }

        private void Update() {
            RenderParts(partToRender);

        }

        void RenderParts(SwordDataStructs.SwordPartEnum partToRender) {
            meshRenderer.enabled = partToRender == SwordDataStructs.SwordPartEnum.SWORD;

            blade.Render(partToRender);
            guard.Render(partToRender);
            grip.Render(partToRender);
            pommel.Render(partToRender);

        }

    }

}