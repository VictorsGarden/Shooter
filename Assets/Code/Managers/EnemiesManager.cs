﻿namespace Managers {
    using Structures;
    using UnityEngine;

    public class EnemiesManager : MonoBehaviour {
        public static EnemiesManager Instance;

        public Enemy[] Enemies;

        private void Awake() => Instance = this;

        public void Initialize(Transform playerT) {
            foreach (var enemy in this.Enemies) {
                enemy.Initialize(playerT);
            }
        }
    }
}