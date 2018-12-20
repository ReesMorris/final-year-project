﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCamera : MonoBehaviour {

    public Transform target;
    public Vector3 distance;
    public Vector3 rotation;

    PhotonView photonView;
    float padding = 0.95f;
    float cameraSpeed = 0.17f;
    bool lockedToPlayer;

    void Start() {
        lockedToPlayer = true;
        photonView = target.GetComponent<PhotonView>();
    }

	void Update () {
        if (photonView.isMine) {
            CheckForInput();
            CheckForMouseOnCorner();
            if (lockedToPlayer)
                CenterCamera();
        }
	}

    // Recenter the camera on the player if input pressed
    void CheckForInput() {
        if(Input.GetKey(KeyCode.Space)) {
            CenterCamera();
        }
        if(Input.GetKeyDown(KeyCode.Y)) {
            lockedToPlayer = !lockedToPlayer;
        }
    }

    // Lock the camera to the player
    public void FocusOnPlayer(bool lockOn) {
        if(lockOn)
            lockedToPlayer = true;
        CenterCamera();
    }

    // Center the camera on the player
    void CenterCamera() {
        transform.localPosition = target.position + distance;
        transform.localEulerAngles = rotation;
    }

    // Check to see if the player is moving the screen
    void CheckForMouseOnCorner() {
        if(Application.isFocused && !lockedToPlayer) {
            if (Input.mousePosition.y >= Screen.height * padding) {
                lockedToPlayer = false;
                transform.position += (Vector3.forward * cameraSpeed);
            }
            if(Input.mousePosition.y <= padding) {
                lockedToPlayer = false;
                transform.position += (Vector3.back * cameraSpeed);
            }
            if(Input.mousePosition.x >= Screen.width * padding) {
                lockedToPlayer = false;
                transform.position += (Vector3.right * cameraSpeed);
            }
            if(Input.mousePosition.x <= padding) {
                lockedToPlayer = false;
                transform.position += (Vector3.left * cameraSpeed);
            }
        }
    }
}