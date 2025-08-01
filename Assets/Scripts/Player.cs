using UnityEngine;

public class Player : MonoBehaviour
{
    Health health;

    private void Awake() {
        health = GetComponent<Health>();

    }

    private void OnEnable() {
        if (health) {
            health.OnDeath += Death;

        }

    }

    private void OnDisable() {
        if (health) {
            health.OnDeath -= Death;

        }

    }

    private void Death() {
        Debug.Log("Player died");

    }

}
