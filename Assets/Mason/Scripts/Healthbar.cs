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

    private void Start() {
        player = GameManager.instance.player.GetComponent<PlayerController>();
    }

    private void Update() {
        bar.fillAmount = (float)player.hp / (float)player.maxHp;
    }
}
