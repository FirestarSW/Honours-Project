using System;
using UnityEngine;

namespace ProceduralSwordGenerator {
    public class SwordPart : MonoBehaviour {
        [SerializeField] protected SwordData swordData;

        SwordDataStructs.SwordPartEnum part;

        protected MeshRenderer[] meshRenderers;

        public void SetData(ref SwordData data, SwordDataStructs.SwordPartEnum part) {
            swordData = data;
            this.part = part;

        }

        public void Render(SwordDataStructs.SwordPartEnum partToRender) {
            if (meshRenderers != null && meshRenderers.Length > 0) {
                foreach (var renderer in meshRenderers) {
                    renderer.enabled = partToRender == part;

                }

            }

        }

    }

}