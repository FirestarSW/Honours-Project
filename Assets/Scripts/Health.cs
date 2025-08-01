using UnityEngine;

public class Health : MonoBehaviour
{
    [SerializeField] float minHealth = 0.0f;
    [SerializeField] float maxHealth = 100.0f;
    [SerializeField] float health;

    public event System.Action OnDeath;

    void Start() {
        health = maxHealth;
        
    }

    private void Update() {
        if (health <= minHealth) {
            OnDeath?.Invoke();

        }

    }

    private void AddHealth(float amountToAdd) {
        health += amountToAdd;

        if (health > maxHealth) {
            health = maxHealth;

        } else if (health <= minHealth) {
            health = minHealth;
            OnDeath?.Invoke();

        }

    }

}
