using UnityEngine;

namespace ProceduralSwordGenerator {
    public class SwordPhysics : MonoBehaviour {

        [SerializeField] SwordData swordData;
        [SerializeField] float weightDamageFactor = 1.5f;

        float force = 50;
        float pivotPoint = -0.01f;

        float optimalPoint;
        float incrementBy = 0.01f;

        void Start() {
            Sword sword = gameObject.GetComponent<Sword>();

            swordData = sword.swordData;

            FindOptimalImpactPoint(swordData.swordData.blade.shape.totalLength * 0.01f, -0.01f);

        }

        public float Cut(float force, float time, float u, float impactPointFactor) {
            float impactPoint = 0;
            float maxKe = 0;
            float minKe = 0;
            while (impactPoint <= swordData.swordData.blade.shape.totalLength) {
                float ke = CalcKineticEngery(0, 0.1f, impactPoint, pivotPoint);

                if (ke >= maxKe) maxKe = ke;
                if (ke <= minKe) minKe = ke;

                impactPoint += incrementBy;

            }

            float damage = maxKe != 0 ? minKe / maxKe * CalcKineticEngery(u, time, impactPointFactor != 0 ? swordData.swordData.blade.shape.totalLength * impactPointFactor : 0, pivotPoint) : 0;

            return damage * swordData.swordData.totalWeight * weightDamageFactor;

        }

        float CalcKineticEngery(float u, float time, float impactPoint, float pivotPoint) {
            float centreMass = swordData.swordData.pob * 0.01f;
            float mass = swordData.swordData.totalWeight * 0.001f;

            float v = mass != 0 ? time * force / mass - u : -u;

            float effortDist = impactPoint - pivotPoint;
            float loadDist = centreMass - impactPoint;

            float vRatio = loadDist != 0 ? effortDist / loadDist : 0;

            v *= vRatio;

            float temp = Mathf.Pow(v, 2) * 0.5f;

            return Mathf.Abs(temp * mass - (loadDist != 0 ? mass * (effortDist / loadDist) * temp : 0));

        }

        void FindOptimalImpactPoint(float bladeLength, float pivotPoint) {
            float impactPoint = 0;
            float ke = 0;
            optimalPoint = 0;
            while (impactPoint <= bladeLength) {
                float temp = CalcKineticEngery(0, 0.1f, impactPoint, pivotPoint);
                
                if (temp >= ke) optimalPoint = impactPoint;

                ke = temp;

                impactPoint += incrementBy;

            }

            optimalPoint = bladeLength - optimalPoint;

        }

    }

}
