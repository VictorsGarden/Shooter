﻿namespace Managers {
    using System.Collections.Generic;
    using Abilities;
    using Weapons.ConcreteWeapons;
    using Weapons.Options;
    using Weapons.WeaponTypes;
    using System;
    using System.Threading.Tasks;
    using System.Linq;
    using ScriptableObjects;
    using UnityEngine;

    public class GameManager : MonoBehaviour {
        public PoolOptions PoolOptions;

        [SerializeField] private PlayerManager playerManager;
        [SerializeField] private Transform     respawnPoint;

        [SerializeField] private List<Ability> startingPlayerAbilities;
        [SerializeField] private List<Ability> startingEnemyAIAbilities;

        // Singletons
        private PoolManager    poolManager;
        private EnemiesManager enemiesManager;

        private bool isRespawning;

        private void Awake()
            => this.playerManager.OnDeath += this.OnDeath;

        private async void Start() {
            this.GetSingletones();
            await Task.Delay(TimeSpan.FromSeconds(1));
            this.ParseJSONData();
            await Task.Delay(TimeSpan.FromSeconds(1));
            this.Initialize();
        }

        private void Initialize() {
            foreach (var ability in this.startingPlayerAbilities) {
                this.playerManager.AddAbility(ability);
            }

            this.poolManager.Initialize(this.PoolOptions.Pools);
            this.InitializeTheEnemies();
        }

        private void OnDeath() {
            this.isRespawning = true;
            this.playerManager.WeaponController.SetupArcRenderer(false);
        }

        private void ParseJSONData() {
            var asset = Resources.Load<TextAsset>("GameOptions/weapon_options");

            if (asset != null) {
                var weaponOptionsContainer = JsonUtility.FromJson<WeaponOptionsList>(asset.text);

                var weaponOptions = weaponOptionsContainer.weaponOptions;

                foreach (var weapon in this.playerManager.WeaponController.Weapons) {
                    var findedOption = weaponOptions.FirstOrDefault(option => option.id == weapon.Id);

                    weapon.WeaponName = findedOption.weaponName;
                    weapon.SerialRate = findedOption.serialRate;
                    weapon.Damage     = findedOption.damage;
                    weapon.Range      = findedOption.range;

                    if (weapon is Grenade grenade) {
                        grenade.BlowingTime = findedOption.blowingTime;

                        continue;
                    }


                    if (weapon is FiringWeapon firingWeapon) {
                        firingWeapon.AttackSpeed      = findedOption.attackSpeed;
                        firingWeapon.Splash           = findedOption.splash;
                        firingWeapon.MagazineCapacity = findedOption.magazineCapacity;
                        firingWeapon.ReloadRate       = findedOption.reloadRate;
                    }
                }

                // for not loosing after 2 seconds :)
                var additionalSerialRate = 1.4f;
                var enemyDamage          = 5;

                foreach (var enemy in this.enemiesManager.Enemies) {
                    enemy.SerialRate = (weaponOptions.FirstOrDefault(option => option.id == 1).serialRate + additionalSerialRate);

                    foreach (var weapon in enemy.WeaponController.Weapons) {
                        var findedOption = weaponOptions.FirstOrDefault(option => option.id == weapon.Id);

                        weapon.WeaponName = findedOption.weaponName;
                        weapon.SerialRate = findedOption.serialRate;
                        weapon.Damage     = enemyDamage;
                        weapon.Range      = findedOption.range;

                        if (weapon is Grenade grenade) {
                            grenade.BlowingTime = findedOption.blowingTime;

                            continue;
                        }

                        if (weapon is FiringWeapon firingWeapon) {
                            firingWeapon.AttackSpeed      = findedOption.attackSpeed;
                            firingWeapon.Splash           = findedOption.splash;
                            firingWeapon.MagazineCapacity = findedOption.magazineCapacity;
                            firingWeapon.ReloadRate       = findedOption.reloadRate;
                        }
                    }
                }
            }
        }

        private void Respawn() {
            this.playerManager.transform.position = Vector3.Lerp(
                this.playerManager.transform.position, this.respawnPoint.position, Time.deltaTime);

            if (Vector3.Distance(this.playerManager.transform.position, this.respawnPoint.position) <= 0.5f) {
                this.isRespawning                     = false;
                this.playerManager.transform.position = this.respawnPoint.position;
                this.playerManager.gameObject.SetActive(true);
                this.playerManager.Respawn();
                this.playerManager.WeaponController.DetectThrowingWeapon();
            }
        }

        private void GetSingletones() {
            this.poolManager    = PoolManager.Instance;
            this.enemiesManager = EnemiesManager.Instance;
        }

        private void InitializeTheEnemies()
            => this.enemiesManager.Initialize(this.playerManager.transform, this.startingEnemyAIAbilities);

        private void Update() {
            if (this.isRespawning) {
                this.Respawn();
            }
        }
    }
}