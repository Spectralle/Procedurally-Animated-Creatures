using System.Collections;
using UnityEngine;

namespace IKProceduralAnimation
{
    public struct FootData
    {
        public bool isStepping;
        public Transform footTarget;
        public Transform baseBodyJoint;
        public Vector3 bodyPositionOffset;
        public Vector3 relativeRayOrigin;
        public Vector3 currentFootPosition;
        public Vector3 currentRaycastHitPosition;
        public float lerp;
        public Vector3 oldPosition;

        public FootData(Transform footTransform, Transform baseBodyJoint, Vector3 bodyPositionOffset)
        {
            this.isStepping = false;
            this.footTarget = footTransform;
            this.baseBodyJoint = baseBodyJoint;
            this.bodyPositionOffset = bodyPositionOffset;
            this.relativeRayOrigin = baseBodyJoint.position + bodyPositionOffset;
            this.currentFootPosition = relativeRayOrigin;
            this.currentRaycastHitPosition = relativeRayOrigin;
            this.lerp = 1;
            this.oldPosition = this.currentFootPosition;
        }
    }
}