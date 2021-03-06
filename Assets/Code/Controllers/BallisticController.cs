namespace Controllers {
    using UnityEngine;

    public abstract class BallisticController : MonoBehaviour {
        public Transform Comparer;

        public float Velocity, Angle, Gravity = 30f;

        [SerializeField] protected LayerMask layerMask;
        [SerializeField] protected float     radius = .5f;

        protected RaycastHit hit;
        protected float      currentVelocity;
        protected float      radianAngle;
        protected float      maxDistance;
        protected float      time;

        protected abstract Vector3 CalculateNextPoint(float timepoint);
    }
}