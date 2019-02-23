﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChampionXP : MonoBehaviour {

    public delegate void OnChampionLevelUp(Champion champion, PhotonPlayer player, int level);
    public static OnChampionLevelUp onChampionLevelUp;
    public delegate void OnChampionReceiveXP(PhotonPlayer player, float progress);
    public static OnChampionReceiveXP onChampionReceiveXP;

    int maxLevel = 18;
    int firstLevelXP = 280;
    int levelIncrement = 100;

    int currentXP = 0;
    int currentLevel = 1;
    int nextLevelXP;
    int previousLevelXP = 0;
    Champion champion;
    public PhotonView photonView { get; protected set; }

    void Start() {
        photonView = GetComponent<PhotonView>();
        nextLevelXP = firstLevelXP;
        champion = GetComponent<PlayerChampion>().Champion;

        Turret.onTurretDestroyed += OnTurretDestroyed;
    }

    // Called when a turret is destroyed; give global XP to every player on the team who destroyed it
    void OnTurretDestroyed(Turret t) {
        if(photonView.isMine) {
            if (t.team != PhotonNetwork.player.GetTeam()) {
                photonView.RPC("GiveXP", PhotonTargets.AllBuffered, t.XPOnDeath);
            }
        }
    }

    [PunRPC]
    public void GiveXP(int amount) {
        if(currentLevel < maxLevel) {

            // Give the XP
            currentXP += amount;

            // Have we levelled up?
            while (currentXP > nextLevelXP) {
                previousLevelXP = nextLevelXP;
                currentLevel = Mathf.Min(currentLevel + 1, maxLevel);
                nextLevelXP += levelIncrement;

                if (onChampionLevelUp != null && photonView.isMine)
                    onChampionLevelUp(champion, photonView.owner, currentLevel);
            }

            // Make a call saying we've awarded XP (after checking level)
            float progress = ((float)(currentXP- previousLevelXP) / (float)(nextLevelXP-previousLevelXP));
            if (currentLevel == maxLevel) progress = 1;
            if (onChampionReceiveXP != null)
                onChampionReceiveXP(photonView.owner, progress);
        }
    }
}
