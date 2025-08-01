using UnityEngine;

namespace ProceduralSwordGenerator {
    public class SwordPommel : SwordPart {
        public void Setup(string defaultMaterialPath) {
            SwordPartMesh partMesh = GetComponent<SwordPartMesh>(); if (!partMesh) partMesh = gameObject.AddComponent<SwordPartMesh>();

            partMesh.CreateMesh(ref swordData.swordData.pommel.shape, -1, swordData.swordData.pommel.name,
                swordData.swordData.pommel.materialPath != null && swordData.swordData.pommel.materialPath != "" ? swordData.swordData.pommel.materialPath : defaultMaterialPath);
            partMesh.SetupMesh();

            // Calculates parts centre of mass
            swordData.swordData.pommel.pob = partMesh.CalcCentreMass(swordData.swordData.pommel.weight, swordData.swordData.pommel.shape.totalLength);

            meshRenderers = GetComponentsInChildren<MeshRenderer>();

        }

    }

}