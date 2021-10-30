﻿using System;
using Gameplay.Player;
using Shared;
using UnityEngine;

namespace Gameplay
{
    [RequireComponent(typeof(Collider2D))]
    public class KillZone : MonoBehaviour
    {
        private void OnTriggerEnter2D(Collider2D col)
        {
            switch (col.gameObject.tag)
            {
                case Tags.Player:
                {
                    var playerBehaviour = col.gameObject.GetComponent<PlayerBehaviour>();
                    Helpers.AssertIsNotNullOrQuit(playerBehaviour, "GameObject tagged with Player does not have PlayerBehaviour component");
                    playerBehaviour.ResetPosition();
                    break;
                }
            }
        }
    }
}