using UnityEngine;
using UnityEngine.Animations.Rigging;
using Sirenix.OdinInspector;


namespace IKProceduralAnimation
{
    public class IKOctapedAnimator : MonoBehaviour
    {
        [SerializeField] private Rig _rig;
        [Space]
        [SerializeField] private LayerMask _walkableTerrain;
        [Space, Header("General Parameters:")]
        [SerializeField] private float _moveSpeed = 5f;
        [Space, Header("Foot Parameters:")]
        [SerializeField, Min(0.2f)] private float _stepDistance = 0.6f;
        [SerializeField, Min(0.1f)] private float _stepHeight = 0.3f;
        [SerializeField, Min(1f)] private float _stepSpeed = 2f;
        [SerializeField] private float _raycastDistance = 1.5f;
        [SerializeField] private bool _showDebugGizmos = true;
        [SerializeField] private Vector3[] _distanceFromJoints = new Vector3[4];
        [SerializeField] private float[] _offset = new float[4];
        [Space, Header("IK Legs")]
        [SerializeField] private ChainIKConstraint _frontLeftFoot;
        [SerializeField] private ChainIKConstraint _middleFrontLeftFoot;
        [SerializeField] private ChainIKConstraint _middleBackLeftFoot;
        [SerializeField] private ChainIKConstraint _backLeftFoot;
        [SerializeField] private ChainIKConstraint _frontRightFoot;
        [SerializeField] private ChainIKConstraint _middleFrontRightFoot;
        [SerializeField] private ChainIKConstraint _middleBackRightFoot;
        [SerializeField] private ChainIKConstraint _backRightFoot;

        public AnimationCurve graph;
        private bool stepped;

        private Vector2 _input = new Vector2();
        private FootData[,] _feetData = new FootData[2,4];
        private Rigidbody rb;
        private Vector3[,] _footRayOriginPositions = new Vector3[2,4];
        private bool _initialized;
        public bool offset;


        [Button]
        private void InitializeArrays()
        {
            // Left feet (Front -> Back)
            _feetData[0, 0] = new FootData(_frontLeftFoot.data.target, _frontLeftFoot.data.root, new Vector3(-_distanceFromJoints[0].x, _distanceFromJoints[0].y, _distanceFromJoints[0].z));
            _feetData[0, 1] = new FootData(_middleFrontLeftFoot.data.target, _frontLeftFoot.data.root, new Vector3(-_distanceFromJoints[1].x, _distanceFromJoints[1].y, _distanceFromJoints[1].z));
            _feetData[0, 2] = new FootData(_middleBackLeftFoot.data.target, _backLeftFoot.data.root, new Vector3(-_distanceFromJoints[2].x, _distanceFromJoints[2].y, -_distanceFromJoints[2].z));
            _feetData[0, 3] = new FootData(_backLeftFoot.data.target, _backLeftFoot.data.root, new Vector3(-_distanceFromJoints[3].x, _distanceFromJoints[3].y, -_distanceFromJoints[3].z));
            // Right feet (Front -> Back)
            _feetData[1, 0] = new FootData(_frontRightFoot.data.target, _frontRightFoot.data.root, new Vector3(_distanceFromJoints[0].x, _distanceFromJoints[0].y, _distanceFromJoints[0].z));
            _feetData[1, 1] = new FootData(_middleFrontRightFoot.data.target, _frontRightFoot.data.root, new Vector3(_distanceFromJoints[1].x, _distanceFromJoints[1].y, _distanceFromJoints[1].z));
            _feetData[1, 2] = new FootData(_middleBackRightFoot.data.target, _backRightFoot.data.root, new Vector3(_distanceFromJoints[2].x, _distanceFromJoints[2].y, -_distanceFromJoints[2].z));
            _feetData[1, 3] = new FootData(_backRightFoot.data.target, _backRightFoot.data.root, new Vector3(_distanceFromJoints[3].x, _distanceFromJoints[3].y, -_distanceFromJoints[3].z));



            // Left feet (Front -> Back)
            _footRayOriginPositions[0, 0] = new Vector3(_feetData[0, 0].relativeRayOrigin.x, 0, _feetData[0, 0].relativeRayOrigin.z + _offset[0]);
            _footRayOriginPositions[0, 1] = new Vector3(_feetData[0, 1].relativeRayOrigin.x, 0, _feetData[0, 1].relativeRayOrigin.z - _offset[1]);
            _footRayOriginPositions[0, 2] = new Vector3(_feetData[0, 2].relativeRayOrigin.x, 0, _feetData[0, 2].relativeRayOrigin.z + _offset[2]);
            _footRayOriginPositions[0, 3] = new Vector3(_feetData[0, 3].relativeRayOrigin.x, 0, _feetData[0, 3].relativeRayOrigin.z - _offset[3]);
            // Right feet (Front -> Back)
            _footRayOriginPositions[1, 0] = new Vector3(_feetData[1, 0].relativeRayOrigin.x, 0, _feetData[1, 0].relativeRayOrigin.z - _offset[0]);
            _footRayOriginPositions[1, 1] = new Vector3(_feetData[1, 1].relativeRayOrigin.x, 0, _feetData[1, 1].relativeRayOrigin.z + _offset[1]);
            _footRayOriginPositions[1, 2] = new Vector3(_feetData[1, 2].relativeRayOrigin.x, 0, _feetData[1, 2].relativeRayOrigin.z - _offset[2]);
            _footRayOriginPositions[1, 3] = new Vector3(_feetData[1, 3].relativeRayOrigin.x, 0, _feetData[1, 3].relativeRayOrigin.z + _offset[3]);

            Debug.Log($"{_feetData[0,1].relativeRayOrigin} ({_feetData[0,1].baseBodyJoint.position} | {_feetData[0,1].bodyPositionOffset}) | {_feetData[0,1].currentRaycastHitPosition}");

            _initialized = true;
        }

        private void Awake()
        {
            rb = GetComponent<Rigidbody>();
            InitializeArrays();
        }

        private void Start()
        {
            for (int x = 0; x < 2; x++)
            {
                for (int y = 0; y < 4; y++)
                {
                    Ray ray = new Ray(_feetData[x,y].relativeRayOrigin, -transform.up);
                    if (Physics.Raycast(ray, out RaycastHit hit, _raycastDistance, _walkableTerrain))
                    {
                        _feetData[x, y].currentRaycastHitPosition = hit.point;
                        _feetData[x, y].currentFootPosition = hit.point;
                        _feetData[x, y].footTarget.position = hit.point;
                    }
                }
            }
        }

        private void LateUpdate()
        {
            _input.x = Input.GetAxis("Horizontal");
            _input.y = Input.GetAxis("Vertical");
            rb.velocity = new Vector3(_input.x, 0, _input.y) * _moveSpeed;

            for (int x = 0; x < 2; x++)
            {
                for (int y = 0; y < 4; y++)
                {
                    _feetData[x, y].relativeRayOrigin = _feetData[x, y].baseBodyJoint.position + _feetData[x, y].bodyPositionOffset;
                    if (!_feetData[x, y].isStepping)
                        _feetData[x, y].footTarget.position = _feetData[x, y].currentFootPosition;

                    Ray ray = new Ray(_feetData[x, y].relativeRayOrigin, -transform.up);
                    if (Physics.Raycast(ray, out RaycastHit hit, _raycastDistance, _walkableTerrain))
                    {
                        _feetData[x, y].currentRaycastHitPosition = hit.point;

                        if (!_feetData[x, y].isStepping)
                        {
                            if (Vector3.Distance(_feetData[x, y].currentFootPosition, hit.point) > _stepDistance)
                            {
                                _feetData[x,y].lerp = 0;
                                _feetData[x, y].currentFootPosition = hit.point;
                            }
                        }
                    }

                    if (_feetData[x, y].lerp < 1)
                    {
                        _feetData[x, y].isStepping = true;
                        Vector3 footPosition = Vector3.Lerp(_feetData[x, y].footTarget.position, _feetData[x, y].currentFootPosition, _feetData[x, y].lerp);
                        footPosition.y += Mathf.Sin(_feetData[x, y].lerp * Mathf.PI) * _stepHeight;
                        //Debug
                        if (x == 0 && y == 0 && !stepped)
                            graph.AddKey(_feetData[x, y].lerp, Mathf.Sin(_feetData[x, y].lerp * Mathf.PI) * _stepHeight);

                        _feetData[x, y].footTarget.position = footPosition;
                        _feetData[x, y].lerp += Time.deltaTime * _stepSpeed;
                    }
                    else
                    {
                        _feetData[x, y].isStepping = false;
                        _feetData[x, y].oldPosition = _feetData[x, y].currentFootPosition;
                    }
                }
            }
        }


        private void OnDrawGizmos()
        {
            if (_showDebugGizmos)
            {
                if (!_initialized)
                    InitializeArrays();

                if (Application.isPlaying)
                {
                    foreach (FootData foot in _feetData)
                    {
                        Gizmos.color = Color.blue;
                        Gizmos.DrawSphere(foot.relativeRayOrigin, 0.02f);
                        Gizmos.DrawLine(foot.relativeRayOrigin, foot.currentRaycastHitPosition);

                        Gizmos.color = Color.red;
                        Gizmos.DrawSphere(foot.currentRaycastHitPosition, 0.03f);

                        Gizmos.color = Color.yellow;
                        Gizmos.DrawSphere(foot.currentFootPosition, 0.06f);

                        if (!foot.isStepping && Vector3.Distance(foot.currentFootPosition, foot.currentRaycastHitPosition) > _stepDistance)
                        {
                            Gizmos.color = Color.magenta;
                            Gizmos.DrawLine(foot.currentRaycastHitPosition, foot.currentFootPosition);
                        }
                        else
                        {
                            Gizmos.color = Color.black;
                            Gizmos.DrawLine(foot.currentRaycastHitPosition, foot.currentFootPosition);
                        }
                    }
                }
                else
                {
                    if (!_initialized)
                    {
                        Debug.LogWarning("IK Arrays not initialized");
                        return;
                    }

                    Gizmos.color = Color.cyan;

                    Gizmos.DrawSphere(_footRayOriginPositions[0, 0], 0.03f);
                    Gizmos.DrawSphere(_footRayOriginPositions[0, 1], 0.03f);
                    Gizmos.DrawSphere(_footRayOriginPositions[0, 2], 0.03f);
                    Gizmos.DrawSphere(_footRayOriginPositions[0, 3], 0.03f);

                    Gizmos.DrawSphere(_footRayOriginPositions[1, 0], 0.03f);
                    Gizmos.DrawSphere(_footRayOriginPositions[1, 1], 0.03f);
                    Gizmos.DrawSphere(_footRayOriginPositions[1, 2], 0.03f);
                    Gizmos.DrawSphere(_footRayOriginPositions[1, 3], 0.03f);

                    Gizmos.color = Color.red;
                    Gizmos.DrawSphere(_feetData[0,1].baseBodyJoint.position + _feetData[0,1].bodyPositionOffset + new Vector3(0, 0, offset ? _offset[0]: 0), 0.03f);
                }
            }
        }
    } 
}