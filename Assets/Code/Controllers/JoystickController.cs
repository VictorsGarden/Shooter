﻿namespace Controllers {
    using UI;
    using System.Threading.Tasks;
    using UI.Interfaces;
    using UnityEngine.UI;
    using Interfaces;
    using System;
    using UnityEngine;

    public class JoystickController : MonoBehaviour, InputController, Joystick {
        public float ForwardMoving     => this.movingJoystick.GetAxisY();
        public float HorizontalMoving  => this.movingJoystick.GetAxisX();
        public float GetAxisX          => this.movingJoystick.GetAxisX();
        public float GetAxisY          => this.movingJoystick.GetAxisY();
        public float GetBallisticValue => this.grenadeBar.GetAxisY();

        public AttackButton AttackBtn       => this.attackBtn;
        public Button       ReloadBtn       => this.reloadBtn;
        public Button       ChangeWeaponBtn => this.changeWeaponBtn;

        public bool  IsSneak    { get; }
        public bool  IsJumping  { get; }
        public float SerialRate { get; set; }

        public event Action OnFireOnce;
        public event Action OnReload;
        public event Action OnChangingWeapon;

        public ETCJoystick MovingJoystick => this.movingJoystick;
        public ETCJoystick GrenadeBar     => this.grenadeBar;

        [SerializeField] private AttackButton attackBtn;
        [SerializeField] private Button       reloadBtn;
        [SerializeField] private Button       changeWeaponBtn;

        [SerializeField] private ETCJoystick movingJoystick;
        [SerializeField] private ETCJoystick grenadeBar;

        private bool canInputFire = true;
        private bool isAttackHolding = false;

        public void RearrangeAttackUI(Sprite sprite, bool isThrowable) {
            this.ChangeWeaponSprite(sprite);

            this.grenadeBar.gameObject.SetActive(isThrowable);
            this.reloadBtn.gameObject.SetActive(!isThrowable);
        }

        private void ChangeWeaponSprite(Sprite sprite)
            => this.AttackBtn.GetComponent<Image>().sprite = sprite;

        private void Awake() {
            this.changeWeaponBtn.onClick.AddListener(() => this.OnChangingWeapon?.Invoke());
            this.reloadBtn.onClick.AddListener(() => this.OnReload?.Invoke());

            this.attackBtn.OnPointerDownEvent += () => {
                this.isAttackHolding = true;
                
                if (this.canInputFire) {
                    this.PerformFireInputWithFireRate();
                }
            };
            this.attackBtn.OnPointerUpEvent += () => this.isAttackHolding = false;
        }

        private void Update() {
            if (this.isAttackHolding && this.canInputFire) {
                this.PerformFireInputWithFireRate();
            }
        }

        private async void PerformFireInputWithFireRate() {
            this.canInputFire = false;
            this.OnFireOnce?.Invoke();
            await Task.Delay(TimeSpan.FromSeconds(this.SerialRate));
            this.canInputFire = true;
        }
    }
}