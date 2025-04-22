using UnityEngine;

namespace Controller
{
    [RequireComponent(typeof(CreatureMover1))] // ✅ 改为新版类名
    public class MovePlayerInput1 : MonoBehaviour
    {
        [Header("Character")]
        [SerializeField] private string m_HorizontalAxis = "Horizontal";
        [SerializeField] private string m_VerticalAxis = "Vertical";
        [SerializeField] private string m_JumpButton = "Jump";
        [SerializeField] private KeyCode m_RunKey = KeyCode.LeftShift;

        [Header("Camera")]
        [SerializeField] private Transform m_Camera;

        private CreatureMover1 m_Mover; // ✅ 改为新版类型

        private Vector2 m_Axis;
        private bool m_IsRun;
        private bool m_IsJump;
        private Vector3 m_Target;

        private bool _isMovingForward = true;
        public bool IsMovingForward { get { return _isMovingForward; } }

        private void Awake()
        {
            m_Mover = GetComponent<CreatureMover1>(); // ✅ 改为新版类名
        }

        private void Update()
        {
            GatherInput();
            SetInput();
        }

        public void GatherInput()
        {
            float h = Input.GetAxis(m_HorizontalAxis);
            float v = Input.GetAxis(m_VerticalAxis);
            m_Axis = new Vector2(h, v);
            m_IsRun = Input.GetKey(m_RunKey);
            m_IsJump = Input.GetButton(m_JumpButton);

            if (m_Camera != null)
            {
                Vector3 camForward = m_Camera.forward;
                Vector3 camRight = m_Camera.right;

                camForward.y = 0f;
                camRight.y = 0f;
                camForward.Normalize();
                camRight.Normalize();

                Vector3 moveDir = (camRight * h + camForward * v).normalized;
                m_Target = moveDir.sqrMagnitude > 0.01f ? moveDir : transform.forward;

                _isMovingForward = v > 0.01f;
            }
            else
            {
                m_Target = transform.forward;
                _isMovingForward = true;
            }
        }

        public void BindMover(CreatureMover1 mover) // ✅ 也同步改名
        {
            m_Mover = mover;
        }

        public void SetInput()
        {
            if (m_Mover != null)
            {
                m_Mover.SetInput(in m_Axis, in m_Target, in m_IsRun, m_IsJump);
            }
        }
    }
}
