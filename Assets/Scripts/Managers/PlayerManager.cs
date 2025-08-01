using UnityEngine;
using System;

public class PlayerManager : MonoBehaviour
{

    public Player[] players;

    private void Start() {
        players = FindObjectsByType<Player>(FindObjectsSortMode.None); // Change to tag

    }

}
