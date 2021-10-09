using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace IKProceduralAnimation
{
    public class IKCharacterController : MonoBehaviour
    {
        [SerializeField] private Transform rotatingBody;
        [SerializeField] private IIKAnimator IKAnimator;
        [Space]
        [SerializeField] private float moveSpeed = 5;

        private Rigidbody rb;


        void Awake() => rb = GetComponent<Rigidbody>();

        void Update()
        {
            rb.AddForce(new Vector3(0, 0, Input.GetAxis("Horizontal") * Time.deltaTime * moveSpeed));
        }
    }
}