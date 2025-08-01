using UnityEngine;

namespace ProceduralSwordGenerator {
    public class SwordMesh : MonoBehaviour {
        string meshName = "Sword";

        Mesh mesh;
        MeshRenderer meshRenderer;

        private void SetupMesh() {
            // Get Mesh Filter, if there is not one, add one
            if (!GetComponent<MeshFilter>()) gameObject.AddComponent<MeshFilter>();

            // Get Mesh Renderer, if there is not one, add one
            meshRenderer = GetComponent<MeshRenderer>(); if (!meshRenderer) meshRenderer = gameObject.AddComponent<MeshRenderer>();

            MeshFilter[] meshFilters = GetComponentsInChildren<MeshFilter>();

            CombineInstance[] combine = new CombineInstance[meshFilters.Length - 1];

            Material[] materials = new Material[meshFilters.Length - 1];

            for (int i = 1; i < meshFilters.Length; i++) {
                // Add mesh to meshes to combine
                combine[i - 1].mesh = meshFilters[i].sharedMesh;

                // Set added mesh to swords world transform * mesh's local transform
                combine[i - 1].transform = meshFilters[0].transform.worldToLocalMatrix * meshFilters[i].transform.localToWorldMatrix;

                // Get mesh's material from the mesh renderer of the mesh's game object
                MeshRenderer childRenderer = meshFilters[i].gameObject.GetComponent<MeshRenderer>();
                materials[i - 1] = childRenderer.material;

                // Disable the mesh's renderer
                childRenderer.enabled = false;

            }

            meshRenderer.materials = materials;

            mesh = new Mesh();

            mesh.name = meshName;

            mesh.CombineMeshes(combine, false);

            mesh.RecalculateNormals();

            meshFilters[0].sharedMesh = mesh;
            transform.gameObject.SetActive(true);
        }

        public void CreateMesh(string name) {
            meshName = name;

            SetupMesh();

        }


    }

}
