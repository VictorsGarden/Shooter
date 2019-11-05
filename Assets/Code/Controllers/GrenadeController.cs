﻿namespace Controllers {
    using Structures;
    using UnityEngine;

    public class GrenadeController : ThrowingController {
        protected void OnWeaponReachedTarget() {
            this.Blow();
        }

        private void Blow() {
            Collider[] overlapResults = new Collider[100];
            int        numFound       = Physics.OverlapSphereNonAlloc(this.weaponTransform.position, 10f, overlapResults);

            for (int i = 0; i < numFound; i++) {
                var lifer = overlapResults[i].gameObject.GetComponent<Lifer>();

                if (lifer != null) {
                    Debug.DrawLine(this.weaponTransform.position, lifer.CenterOfMass.position, Color.red, 10f);

                    if (Physics.Linecast(this.weaponTransform.position, lifer.CenterOfMass.position, out this.hit, this.attackingLayerMask)) {
                        lifer = this.hit.transform.gameObject.GetComponent<Lifer>();

                        if (lifer != null) {
                            lifer.Hit(this.damage, this.id_attacker, this.id_weapon, this.weaponName);
                            Debug.Log(this.hit.transform.gameObject);
                        }
                    }
                }
            }
        }
    }
}