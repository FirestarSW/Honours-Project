using UnityEngine;

namespace ProceduralSwordGenerator {
    public class SwordGuard : SwordPart {
        public float totalWeight;

        public void Setup(string defaultMaterialPath) {
            SwordPartMesh[] guardMeshes = GetComponentsInChildren<SwordPartMesh>();

            if (guardMeshes == null || guardMeshes.Length < 1) {
                guardMeshes = new SwordPartMesh[swordData.swordData.trueGuards.Length + swordData.swordData.falseGuards.Length];

                for (int i = 0; i < guardMeshes.Length; i++) {
                    GameObject guardObject = new GameObject("Guard");

                    guardObject.transform.position = transform.position;

                    guardObject.transform.parent = transform;

                    guardMeshes[i] = guardObject.AddComponent<SwordPartMesh>();

                }

            }

            for (int i = 0; i < swordData.swordData.trueGuards.Length; i++) {
                guardMeshes[i].CreateMesh(ref swordData.swordData.trueGuards[i].shape, 2, swordData.swordData.trueGuards[i].name,
                    swordData.swordData.trueGuards[i].materialPath != null && swordData.swordData.trueGuards[i].materialPath != "" ? swordData.swordData.trueGuards[i].materialPath : defaultMaterialPath);
                guardMeshes[i].SetupMesh();

            }

            for (int i = 0; i < swordData.swordData.falseGuards.Length; i++) {
                guardMeshes[i + swordData.swordData.trueGuards.Length].CreateMesh(ref swordData.swordData.falseGuards[i].shape, -2, swordData.swordData.falseGuards[i].name,
                    swordData.swordData.falseGuards[i].materialPath != null && swordData.swordData.falseGuards[i].materialPath != "" ? swordData.swordData.falseGuards[i].materialPath : defaultMaterialPath);
                guardMeshes[i + swordData.swordData.trueGuards.Length].SetupMesh();

            }

            // Calculates total weight of guards
            totalWeight = 0;
            foreach (var guard in swordData.swordData.trueGuards) totalWeight += guard.weight;
            foreach (var guard in swordData.swordData.falseGuards) totalWeight += guard.weight;

            meshRenderers = GetComponentsInChildren<MeshRenderer>();

        }

    }

}
