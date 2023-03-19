using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Healthbar : MonoBehaviour
{
    private Image bar;
    private PlayerController player;

    private void Awake() {
        bar = GetComponent<Image>();
    }

    private void Update() {
        if(GameManager.instance.gameOver) return;

        if(!player)
            player = GameManager.instance.player.GetComponent<PlayerController>();
        bar.fillAmount = (float)player.hp / (float)player.maxHp;
    }
}
