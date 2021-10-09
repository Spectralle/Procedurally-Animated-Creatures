using UnityEngine;
using Sirenix.OdinInspector;
using System.Collections;

namespace IKProceduralAnimation_GeckoImplementation
{
    public class IKProceduralAnimator : MonoBehaviour
    {
        [Title("Entity Body")]
        [SerializeField] private Transform _body;
        [Title("Entity Head")]
        [SerializeField] private Transform _head;
        [SerializeField] private Transform _headLookAtTarget;
        [SerializeField, Range(1f, 20f)] private float _headTurnSpeed = 5f;
        [SerializeField, Range(0.1f, 360f)] float _headMaxTurnAngle;

        [Title("Entity Limbs")]
        [SerializeField] IKALegStepper _frontLeftLeg;
        [SerializeField] IKALegStepper _frontRightLeg;
        [Space]
        [SerializeField] IKALegStepper _middleFrontLeftLeg;
        [SerializeField] IKALegStepper _middleFrontRightLeg;
        [Space]
        [SerializeField] IKALegStepper _middleBackLeftLeg;
        [SerializeField] IKALegStepper _middleBackRightLeg;
        [Space]
        [SerializeField] IKALegStepper _backLeftLeg;
        [SerializeField] IKALegStepper _backRightLeg;


        private void Awake() => StartCoroutine(LegUpdateCoroutine());

        private void LateUpdate()
        {
            #region Head Rotation
            Quaternion currentLocalRotation = _head.localRotation;
            _head.localRotation = Quaternion.identity;

            Vector3 targetWorldLookDir = _headLookAtTarget.position - _head.position;
            Vector3 targetLocalLookDir = _head.InverseTransformDirection(targetWorldLookDir);

            targetLocalLookDir = Vector3.RotateTowards(
                Vector3.forward,
                targetLocalLookDir,
                Mathf.Deg2Rad * _headMaxTurnAngle,
                0
            );

            Quaternion targetLocalRotation = Quaternion.LookRotation(targetLocalLookDir, Vector3.up);
            _head.localRotation = Quaternion.Slerp(
                currentLocalRotation,
                targetLocalRotation,
                1 - Mathf.Exp(-_headTurnSpeed * Time.deltaTime)
            );
            #endregion
        }

        private void FixedUpdate()
        {
            UpdateBodyPosition();
        }

        private IEnumerator LegUpdateCoroutine()
        {
            while (true)
            {
                do
                {
                    _frontLeftLeg.TryToStep();
                    _middleFrontRightLeg.TryToStep();
                    _middleBackLeftLeg.TryToStep();
                    _backRightLeg.TryToStep();
                    
                    yield return null;
                }
                while (_frontLeftLeg.IsMoving || _middleFrontRightLeg.IsMoving || _middleBackLeftLeg.IsMoving || _backRightLeg.IsMoving);

                do
                {
                    _frontRightLeg.TryToStep();
                    _middleFrontLeftLeg.TryToStep();
                    _middleBackRightLeg.TryToStep();
                    _backLeftLeg.TryToStep();

                    yield return null;
                }
                while (_frontRightLeg.IsMoving || _middleFrontLeftLeg.IsMoving || _middleBackRightLeg.IsMoving || _backLeftLeg.IsMoving);
            }
        }

        private void UpdateBodyPosition()
        {
            if (!_body)
                return;

            Vector3 sumOfPositions = Vector3.zero;
            sumOfPositions += _frontLeftLeg.transform.position;
            sumOfPositions += _frontRightLeg.transform.position;
            sumOfPositions += _middleFrontLeftLeg.transform.position;
            sumOfPositions += _middleFrontRightLeg.transform.position;
            sumOfPositions += _middleBackLeftLeg.transform.position;
            sumOfPositions += _middleBackRightLeg.transform.position;
            sumOfPositions += _backLeftLeg.transform.position;
            sumOfPositions += _backRightLeg.transform.position;

            sumOfPositions /= 8;
            _body.position = Vector3.Lerp(_body.position, sumOfPositions, Time.deltaTime * 5);
        }
    }
}