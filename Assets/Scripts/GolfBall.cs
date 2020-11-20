using UnityEngine.Assertions;
using UnityEngine;

[RequireComponent(typeof(FGPhysicsBody))]
public class GolfBall : MonoBehaviour
{
    public enum BallStatus
    {
        WaitingForStroke,
        AimingStroke,
        Rolling
    }

    [SerializeField] private float m_AimRotationSpeed = 2.5f;
    [SerializeField] private float m_StrokeForceMultiplier = 3.0f;
    [SerializeField] private float m_MaxStrokeForce = 20.0f;
    [SerializeField] private GameObject m_AimTool = default;

    private bool m_CurrentlyAiming = false;
    private bool m_ReleasedAimButton = false;
    public float m_StrokeForce = 0.0f;
    private Camera m_MainCamera = default;
    private Vector3 m_LastBallPosition = Vector3.zero;
    private FGPhysicsBody m_PhysicBody = default;

    public BallStatus m_CurrentBallStatus = BallStatus.WaitingForStroke;

    private const string MOUSEXAXISNAME = "Mouse X";


    void Start()
    {
        m_MainCamera = Camera.main;
        m_PhysicBody = GetComponent<FGPhysicsBody>();
        Assert.IsNotNull(m_PhysicBody, "No FGPhysicBody script found on golfball");
        Assert.IsNotNull(m_AimTool, "No aimtool assigned in editor");
        m_LastBallPosition = transform.position;

        Cursor.lockState = CursorLockMode.Locked;
    }

    void Update()
    {
        UpdateBallStatus();

        HandlePlayerInput();
    }

    private void HandlePlayerInput()
    {
        if (m_CurrentBallStatus == BallStatus.Rolling)
        {
            HandleMouseMovement();
            return;
        }

        if (m_CurrentBallStatus == BallStatus.WaitingForStroke)
        {
            UpdateWhenWaitingForStroke();
        }
        else if (m_CurrentBallStatus == BallStatus.AimingStroke)
        {
            UpdateWhenAimingStroke();
        }
    }

    private void UpdateWhenWaitingForStroke()
    {
        HandleMouseMovement();

        if (Input.GetMouseButtonDown(0))
        {
            HandleMouseButtonDown();
        }
    }

    private void UpdateWhenAimingStroke()
    {
        HandleMouseMovement();

        if (Input.GetMouseButton(0) && m_ReleasedAimButton)
        {
            HandleMouseButtonDown();
        }

        if (Input.GetMouseButtonUp(0))
        {
            HandleMouseButtonUp();
        }
    }

    private void HandleMouseMovement()
    {
        float mouseMoveX = Input.GetAxis(MOUSEXAXISNAME);

        if (m_CurrentBallStatus == BallStatus.AimingStroke)
        {

            m_AimTool.transform.RotateAround(transform.position, transform.up, mouseMoveX * m_AimRotationSpeed);
            return;
        }

        m_MainCamera.transform.RotateAround(transform.position, transform.up, mouseMoveX * m_AimRotationSpeed);
    }

    private void HandleMouseButtonUp()
    {
        if (m_ReleasedAimButton)
        {
            m_PhysicBody.Velocity = m_AimTool.transform.forward * m_StrokeForce;
            m_AimTool.SetActive(false);
            m_ReleasedAimButton = false;
            m_StrokeForce = 0.0f;
            m_CurrentlyAiming = false;
        }
        else
        {
            m_ReleasedAimButton = true;
        }
    }

    private void HandleMouseButtonDown()
    {
        if (m_CurrentBallStatus == BallStatus.WaitingForStroke)
        {
            m_CurrentlyAiming = true;
            m_AimTool.SetActive(true);
        }
        else if (m_CurrentBallStatus == BallStatus.AimingStroke)
        {
            m_StrokeForce += Time.deltaTime * m_StrokeForceMultiplier;
            m_StrokeForce = Mathf.Clamp(m_StrokeForce, 0.0f, m_MaxStrokeForce);
        }
    }

    public void MoveBackBall()
    {
        transform.position = m_LastBallPosition;
        transform.position += new Vector3(0.0f, 0.01f, 0.0f);
        m_PhysicBody.Velocity = Vector3.zero;
    }

    private void UpdateBallStatus()
    {
        if (!m_PhysicBody.CurrentlyStaticOnAPlane())
        {
            if (m_CurrentBallStatus != BallStatus.Rolling)
            {
                m_CurrentBallStatus = BallStatus.Rolling;
                m_CurrentlyAiming = false;
                m_AimTool.SetActive(false);
                m_StrokeForce = 0.0f;
                m_ReleasedAimButton = false;
            }
            return;
        }

        if (m_CurrentlyAiming == true)
        {
            m_CurrentBallStatus = BallStatus.AimingStroke;
            return;
        }

        if (m_CurrentBallStatus == BallStatus.Rolling)
        {
            m_LastBallPosition = transform.position;
        }

        m_CurrentBallStatus = BallStatus.WaitingForStroke;
    }
}
