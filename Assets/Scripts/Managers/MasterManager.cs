using ProceduralSwordGenerator;
using UnityEngine;
using static UnityEditor.Experimental.GraphView.GraphView;

public class MasterManager : MonoBehaviour
{
    
    public static MasterManager instance {  get; private set; }

    public PlayerManager playerManager;

    SwordCreator swordCreator;
    Sword[] swords;


    void Awake() {
        // If script is a duplicate, destroy
        if (instance != null && instance != this) {
            Destroy(this);
            return;

        }

        instance = this;

        playerManager = GetComponentInChildren<PlayerManager>();
    }

    private void Start() {
        
        swordCreator = FindFirstObjectByType<SwordCreator>();
        swordCreator.swordScale = 0.08f;

        swords = new Sword[6];

        Vector3 position = new Vector3(-12.5f, 4, 15);

        for (int i = 0; i < swords.Length; i++) {
            swords[i] = swordCreator.CreateSword();

            swords[i].transform.position = position;

            position.x += 5;

        }

        


    }

}
