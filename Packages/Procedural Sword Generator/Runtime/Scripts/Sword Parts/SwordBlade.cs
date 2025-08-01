using UnityEngine;
using UnityEngine.UIElements;

namespace ProceduralSwordGenerator {
    public class SwordBlade : SwordPart {
        public void Setup(string defaultMaterialPath) {
            SwordPartMesh partMesh = GetComponent<SwordPartMesh>(); if (!partMesh) partMesh = gameObject.AddComponent<SwordPartMesh>();

            partMesh.CreateMesh(ref swordData.swordData.blade.shape, 1, swordData.swordData.blade.name,
                swordData.swordData.blade.bladeMaterialPath != null && swordData.swordData.blade.bladeMaterialPath != "" ? swordData.swordData.blade.bladeMaterialPath : defaultMaterialPath);
            partMesh.SetupMesh();

            // Calculates parts centre of mass
            swordData.swordData.blade.pob = partMesh.CalcCentreMass(swordData.swordData.blade.bladeWeight, swordData.swordData.blade.shape.totalLength);

            meshRenderers = GetComponentsInChildren<MeshRenderer>();

        }

    }

}