using System.Collections;
using UnityEngine;

public class IKALegStepper : MonoBehaviour
{
    [SerializeField] private Transform _footHomeTransform;
    [SerializeField] private float _stepDistance;
    [SerializeField] private float stepOvershootFraction;
    [SerializeField] private float _moveDuration;

    [HideInInspector]
    public bool IsMoving;


    public void TryToStep()
    {
        if (IsMoving)
            return;

        float distFromHome = Vector3.Distance(transform.position, _footHomeTransform.position);

        if (distFromHome > _stepDistance)
            StartCoroutine(MoveToHome());
    }

    private IEnumerator MoveToHome()
    {
        IsMoving = true;

        Vector3 startPoint = transform.position;
        Quaternion startRot = transform.rotation;

        Vector3 rot = _footHomeTransform.rotation.eulerAngles + new Vector3(0, 90, 0);
        Quaternion endRot = Quaternion.Euler(rot);

        Vector3 towardHome = (_footHomeTransform.position - transform.position);
        float overshootDistance = _stepDistance * stepOvershootFraction;
        Vector3 overshootVector = towardHome * overshootDistance;
        overshootVector = Vector3.ProjectOnPlane(overshootVector, Vector3.up);

        Vector3 endPoint = _footHomeTransform.position + overshootVector;

        Vector3 centerPoint = (startPoint + endPoint) / 2;
        centerPoint += _footHomeTransform.up * Vector3.Distance(startPoint, endPoint) / 2f;

        float timeElapsed = 0;
        do
        {
            timeElapsed += Time.deltaTime;
            float normalizedTime = timeElapsed / _moveDuration;
            normalizedTime = EasingCubicInOut(normalizedTime);

            // Quadratic bezier curve
            transform.position =
                Vector3.Lerp(
                Vector3.Lerp(startPoint, centerPoint, normalizedTime),
                Vector3.Lerp(centerPoint, endPoint, normalizedTime),
                normalizedTime
                );

            transform.rotation = Quaternion.Slerp(startRot, endRot, normalizedTime);

            yield return null;
        }
        while (timeElapsed < _moveDuration);

        IsMoving = false;
    }

    public static float EasingCubicInOut(float k)
    {
        if ((k *= 2f) < 1f)
            return 0.5f * k * k * k;
        return 0.5f * ((k -= 2f) * k * k + 2f);
    }

    private void OnDrawGizmos() => Gizmos.DrawLine(_footHomeTransform.position, _footHomeTransform.position + (_footHomeTransform.forward * 0.1f));
}
