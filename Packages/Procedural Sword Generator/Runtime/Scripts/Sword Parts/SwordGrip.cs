using UnityEngine;

namespace ProceduralSwordGenerator {
    public class SwordGrip : SwordPart {
        public void Setup(string defaultMaterialPath) {
            SwordPartMesh partMesh = GetComponent<SwordPartMesh>(); if (!partMesh) partMesh = gameObject.AddComponent<SwordPartMesh>();

            partMesh.CreateMesh(ref swordData.swordData.grip.shape, -1, swordData.swordData.grip.name,
                swordData.swordData.grip.materialPath != null && swordData.swordData.grip.materialPath != "" ? swordData.swordData.grip.materialPath : defaultMaterialPath);
            partMesh.SetupMesh();

            // Calculates parts centre of mass
            swordData.swordData.grip.pob = partMesh.CalcCentreMass(swordData.swordData.grip.weight, swordData.swordData.grip.shape.totalLength);

            meshRenderers = GetComponentsInChildren<MeshRenderer>();

        }

    }

}