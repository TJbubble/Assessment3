using System;
using UnityEditor;
using UnityEngine;

namespace Controller
{
    [RequireComponent(typeof(CharacterController))]
    [RequireComponent(typeof(Animator))]
    [DisallowMultipleComponent]
    public class CreatureMover1 : MonoBehaviour
    {
        [Header("Movement")]
        [SerializeField] private float m_WalkSpeed = 1f;
        [SerializeField] private float m_RunSpeed = 4f;
        [SerializeField, Range(0f, 360f)] private float m_RotateSpeed = 10f; // 使用更平滑的角速度
        [SerializeField] private Space m_Space = Space.Self;
        [SerializeField] private float m_JumpHeight = 5f;

        [Header("Animator")]
        [SerializeField] private string m_VerticalID = "Vert";
        [SerializeField] private string m_StateID = "State";
        [SerializeField] private string m_JumpTrigger = "Jump";
        [SerializeField] private LookWeight m_LookWeight = new(1f, 0.3f, 0.7f, 1f);

        private Transform m_Transform;
        private CharacterController m_Controller;
        private Animator m_Animator;

        private MovementHandler m_Movement;
        private AnimationHandler m_Animation;

        private Vector2 m_Axis;
        private Vector3 m_Target;
        private bool m_IsRun;
        private bool m_IsJump;
        private bool m_IsMoving;

        public Vector2 Axis => m_Axis;
        public Vector3 Target => m_Target;
        public bool IsRun => m_IsRun;

        private void OnValidate()
        {
            m_WalkSpeed = Mathf.Max(m_WalkSpeed, 0f);
            m_RunSpeed = Mathf.Max(m_RunSpeed, m_WalkSpeed);
            m_Movement?.SetStats(m_WalkSpeed / 3.6f, m_RunSpeed / 3.6f, m_RotateSpeed, m_JumpHeight, m_Space);
        }

        private void Awake()
        {
            m_Transform = transform;
            m_Controller = GetComponent<CharacterController>();
            m_Animator = GetComponent<Animator>();

            m_Movement = new MovementHandler(m_Controller, m_Transform, m_WalkSpeed, m_RunSpeed, m_RotateSpeed, m_JumpHeight, m_Space, m_Animator, m_JumpTrigger);
            m_Animation = new AnimationHandler(m_Animator, m_VerticalID, m_StateID);
        }

        private void Update()
        {
            m_Movement.Move(Time.deltaTime, in m_Axis, in m_Target, m_IsRun, m_IsMoving, m_IsJump, out var animAxis, out var isAir);
            m_Animation.Animate(in animAxis, m_IsRun ? 1f : 0f, Time.deltaTime);
        }

        private void OnAnimatorIK()
        {
            m_Animation.AnimateIK(in m_Target, m_LookWeight);
        }

        public void SetInput(in Vector2 axis, in Vector3 target, in bool isRun, in bool isJump)
        {
            m_Axis = axis;
            m_Target = target;
            m_IsRun = isRun;
            m_IsJump = isJump;

            if (m_Axis.sqrMagnitude < Mathf.Epsilon)
            {
                m_Axis = Vector2.zero;
                m_IsMoving = false;
            }
            else
            {
                m_Axis = Vector3.ClampMagnitude(m_Axis, 1f);
                m_IsMoving = true;
            }
        }

        private void OnControllerColliderHit(ControllerColliderHit hit)
        {
            if (hit.normal.y > m_Controller.stepOffset)
            {
                m_Movement.SetSurface(hit.normal);
            }
        }

        [Serializable]
        private struct LookWeight
        {
            public float weight;
            public float body;
            public float head;
            public float eyes;

            public LookWeight(float weight, float body, float head, float eyes)
            {
                this.weight = weight;
                this.body = body;
                this.head = head;
                this.eyes = eyes;
            }
        }

        #region Handlers

        private class MovementHandler
        {
            private readonly CharacterController m_Controller;
            private readonly Transform m_Transform;
            private readonly Animator m_Animator;
            private readonly string m_JumpTrigger;

            private float m_WalkSpeed;
            private float m_RunSpeed;
            private float m_RotateSpeed;
            private float m_JumpHeight;

            private Space m_Space;
            private Vector3 m_Normal;
            private Vector3 m_GravityAcceleration = Physics.gravity;
            private float m_VerticalVelocity = 0f;

            public MovementHandler(CharacterController controller, Transform transform,
                float walkSpeed, float runSpeed, float rotateSpeed, float jumpHeight, Space space,
                Animator animator, string jumpTrigger)
            {
                m_Controller = controller;
                m_Transform = transform;
                m_Animator = animator;
                m_JumpTrigger = jumpTrigger;

                m_WalkSpeed = walkSpeed;
                m_RunSpeed = runSpeed;
                m_RotateSpeed = rotateSpeed;
                m_JumpHeight = jumpHeight;
                m_Space = space;
            }

            public void SetStats(float walkSpeed, float runSpeed, float rotateSpeed, float jumpHeight, Space space)
            {
                m_WalkSpeed = walkSpeed;
                m_RunSpeed = runSpeed;
                m_RotateSpeed = rotateSpeed;
                m_JumpHeight = jumpHeight;
                m_Space = space;
            }

            public void SetSurface(in Vector3 normal)
            {
                m_Normal = normal;
            }

            public void Move(float deltaTime, in Vector2 axis, in Vector3 target, bool isRun, bool isMoving, bool isJump, out Vector2 animAxis, out bool isAir)
            {
                ConvertMovement(in axis, in target, out var movement);

                bool grounded = m_Controller.isGrounded;

                if (grounded)
                {
                    m_VerticalVelocity = 0f;
                    if (isJump)
                    {
                        m_VerticalVelocity = Mathf.Sqrt(2f * Mathf.Abs(Physics.gravity.y) * m_JumpHeight);
                        if (m_Animator && !string.IsNullOrEmpty(m_JumpTrigger))
                            m_Animator.SetTrigger(m_JumpTrigger);
                    }
                }
                else
                {
                    m_VerticalVelocity += Physics.gravity.y * deltaTime;
                }

                isAir = !grounded;

                Vector3 displacement = (isRun ? m_RunSpeed : m_WalkSpeed) * movement;
                displacement += Vector3.up * m_VerticalVelocity;
                displacement *= deltaTime;

                m_Controller.Move(displacement);

                // GTA风格：角色朝移动方向平滑转向（加“曲线滑动感”）
                if (movement.sqrMagnitude > 0.01f)
                {
                    Quaternion current = m_Transform.rotation;
                    Quaternion targetRot = Quaternion.LookRotation(movement, Vector3.up);
                    m_Transform.rotation = Quaternion.Slerp(current, targetRot, deltaTime * m_RotateSpeed);
                }

                GenAnimationAxis(in movement, out animAxis);
            }

            private void ConvertMovement(in Vector2 axis, in Vector3 targetForward, out Vector3 movement)
            {
                Vector3 forward, right;

                if (m_Space == Space.Self)
                {
                    forward = new Vector3(targetForward.x, 0f, targetForward.z).normalized;
                    right = Vector3.Cross(Vector3.up, forward).normalized;
                }
                else
                {
                    forward = Vector3.forward;
                    right = Vector3.right;
                }

                movement = axis.x * right + axis.y * forward;
                movement = Vector3.ProjectOnPlane(movement, m_Normal);
            }

            private void GenAnimationAxis(in Vector3 movement, out Vector2 animAxis)
            {
                if (m_Space == Space.Self)
                {
                    animAxis = new Vector2(Vector3.Dot(movement, m_Transform.right), Vector3.Dot(movement, m_Transform.forward));
                }
                else
                {
                    animAxis = new Vector2(Vector3.Dot(movement, Vector3.right), Vector3.Dot(movement, Vector3.forward));
                }
            }
        }

        private class AnimationHandler
        {
            private readonly Animator m_Animator;
            private readonly string m_VerticalID;
            private readonly string m_StateID;

            private readonly float k_InputFlow = 4.5f;

            private float m_FlowState;
            private Vector2 m_FlowAxis;

            public AnimationHandler(Animator animator, string verticalID, string stateID)
            {
                m_Animator = animator;
                m_VerticalID = verticalID;
                m_StateID = stateID;
            }

            public void Animate(in Vector2 axis, float state, float deltaTime)
            {
                m_Animator.SetFloat(m_VerticalID, m_FlowAxis.magnitude);
                m_Animator.SetFloat(m_StateID, Mathf.Clamp01(m_FlowState));

                m_FlowAxis = Vector2.ClampMagnitude(m_FlowAxis + k_InputFlow * deltaTime * (axis - m_FlowAxis).normalized, 1f);
                m_FlowState = Mathf.Clamp01(m_FlowState + k_InputFlow * deltaTime * Mathf.Sign(state - m_FlowState));
            }

            public void AnimateIK(in Vector3 target, in LookWeight lookWeight)
            {
                m_Animator.SetLookAtPosition(target);
                m_Animator.SetLookAtWeight(lookWeight.weight, lookWeight.body, lookWeight.head, lookWeight.eyes);
            }
        }

        #endregion
    }
}
    