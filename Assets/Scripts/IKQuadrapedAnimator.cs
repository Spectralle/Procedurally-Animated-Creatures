using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace IKProceduralAnimation
{
    public class IKQuadrapedAnimator : MonoBehaviour, IIKAnimator
    {
//        [SerializeField] private LayerMask walkableTerrain;
//        [Space]
//        [SerializeField, Min(0.2f)] private float stepDistance = 0.6f;
//        [SerializeField, Min(0.1f)] private float stepHeight = 0.5f;
//        [SerializeField, Min(1f)] private float stepSpeed = 2f;
//        [SerializeField, Min(0f)] private float xDistanceFromBody = 0.3f;
//        [Space]
//        [SerializeField] private Transform frontLeftLegJoint;
//        [SerializeField] private Transform frontRightLegJoint;
//        [SerializeField] private Transform backLeftLegJoint;
//        [SerializeField] private Transform backRightLegJoint;
//        [Space]
//        [SerializeField] private Transform frontLeftFoot;
//        [SerializeField] private Transform frontRightFoot;
//        [SerializeField] private Transform backLeftFoot;
//        [SerializeField] private Transform backRightFoot;
//        [Space]
//        [SerializeField] private List<Transform> Legs = new List<Transform>();

//        private FootData[] feetData = new FootData[4];


//        private void Awake()
//        {
//            feetData[0] = new FootData(FootData.Foot.FrontLeft, frontLeftFoot, frontLeftLegJoint, new Vector3(-xDistanceFromBody, 0, 0));
//            feetData[1] = new FootData(FootData.Foot.FrontRight, frontRightFoot, frontRightLegJoint, new Vector3(xDistanceFromBody, 0, 0));
//            feetData[2] = new FootData(FootData.Foot.BackLeft, backLeftFoot, backLeftLegJoint, new Vector3(-xDistanceFromBody, 0, 0));
//            feetData[3] = new FootData(FootData.Foot.BackRight, backRightFoot, backRightLegJoint, new Vector3(xDistanceFromBody, 0, 0));
//        }

//        private void Start()
//        {
//            for (int fdi = 0; fdi < feetData.Length; fdi++)
//            {
//                Ray ray = new Ray(feetData[fdi].relativeRayOrigin, -transform.up);
//                if (Physics.Raycast(ray, out RaycastHit hit, 10, walkableTerrain))
//                {
//                    feetData[fdi].currentRaycastPosition = hit.point;
//                    feetData[fdi].currentFootPosition = hit.point;
//                    feetData[fdi].footTransform.position = hit.point;
//                }
//            }
//        }

//        private void Update()
//        {
//            for (int f = 0; f < feetData.Length; f++)
//            {
//                feetData[f].relativeRayOrigin = feetData[f].baseBodyJoint.position + feetData[f].bodyPositionOffset;
//                if (!feetData[f].isStepping)
//                    feetData[f].footTransform.position = feetData[f].currentFootPosition;

//                Ray ray = new Ray(feetData[f].relativeRayOrigin, -transform.up);
//                if (Physics.Raycast(ray, out RaycastHit hit, 10, walkableTerrain))
//                {
//                    feetData[f].currentRaycastPosition = hit.point;

//                    if (!feetData[f].isStepping && Vector3.Distance(feetData[f].currentFootPosition, hit.point) > stepDistance)
//                        /*StartCoroutine(*/
//                        TakeAStep(feetData[f], hit.point);//);
//                }
//            }
//        }

//        private void TakeAStep(FootData foot, Vector3 newPosition)
//        {
//            Debug.Log($"{foot.foot} takes a step");

//            foot.isStepping = true;
//            //float lerp = 0;
//            //while (Vector3.Distance(foot.footTransform.position, newPosition) > 0.015f)
//            //{
//            //    Vector3 footPosition = Vector3.Lerp(foot.footTransform.position, newPosition, lerp);
//            //    footPosition.y += Mathf.Sin(lerp * Mathf.PI) * stepHeight;

//            //    foot.footTransform.position = footPosition;
//            //    lerp += Time.deltaTime * stepSpeed;
//            //    yield return new WaitForEndOfFrame();
//            //}
//            Debug.Log($"{foot.currentFootPosition} should equal {newPosition}");

//            foot.currentFootPosition = newPosition;
//            //foot.footTransform.position = foot.currentFootPosition;
//            foot.isStepping = false;
//            //yield break;
//        }

//        private void OnDrawGizmos()
//        {
//            foreach (FootData foot in feetData)
//            {
//                Gizmos.color = Color.blue;
//                Gizmos.DrawSphere(foot.relativeRayOrigin, 0.05f);

//                Gizmos.color = Color.yellow;
//                Gizmos.DrawSphere(foot.currentFootPosition, 0.1f);

//                Gizmos.color = Color.red;
//                Gizmos.DrawSphere(foot.currentRaycastPosition, 0.05f);

//                Gizmos.DrawLine(foot.currentRaycastPosition, foot.currentFootPosition);
//            }
//        }
    }
}