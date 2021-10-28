using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour 
{

	public float mouseSensitivityX = 1.0f;
	public float mouseSensitivityY = 1.0f;

	public float walkSpeed = 10.0f;
    public float jumpForce = 250.0f;
    public float groundFriction = 10;
    public float airFriction = 3;
    public float airControl = 0.25f;
    public LayerMask groundedMask;

    private Transform cameraT;
    private Rigidbody rigidbodyR;
    private LineRenderer lineRenderer;

    private Vector3 moveAmount;
    private float verticalLookRotation;
    private bool grounded = false;

    private bool isGrappling = false;
    private Vector3 grabPoint;
    private float grabDistance;

	// Use this for initialization
	void Start () {
		cameraT = Camera.main.transform;
		rigidbodyR = GetComponent<Rigidbody> ();
        lineRenderer = GetComponent<LineRenderer>();
		LockMouse ();
	}
	
	// Update is called once per frame
	void Update () {
		// rotation
		transform.Rotate (Vector3.up * (Input.GetAxis("Mouse X") * mouseSensitivityX));
		verticalLookRotation += Input.GetAxis ("Mouse Y") * mouseSensitivityY;
		verticalLookRotation = Mathf.Clamp (verticalLookRotation, -95, 95);
		cameraT.localEulerAngles = Vector3.left * verticalLookRotation;

		// movement
		Vector3 moveDir = Vector3.ClampMagnitude(new Vector3 (Input.GetAxisRaw ("Horizontal"), 0, Input.GetAxisRaw ("Vertical")),1);
		moveAmount = moveDir * walkSpeed;

        // jump
		if (Input.GetButtonDown("Jump")) {
			if (grounded) {
				rigidbodyR.AddForce (transform.up * jumpForce, ForceMode.Impulse);
            }
		}

        if (Input.GetMouseButtonDown(0) && !isGrappling)
        {
            if (Physics.Raycast(cameraT.position, cameraT.forward, out RaycastHit hit, 50, groundedMask))
            {
                isGrappling = true;
                grabPoint = hit.point;
                grabDistance = Vector3.Distance(transform.position, grabPoint);
                lineRenderer.enabled = true;
                lineRenderer.SetPosition(1,grabPoint);
            }
        }
        else if (Input.GetMouseButtonUp(0) && isGrappling)
        {
            isGrappling = false;
            lineRenderer.enabled = false;
        }

        if (Input.GetMouseButton(1) && isGrappling && grabDistance > 2)
        {
            Vector3 grabDir = (grabPoint - transform.position).normalized;
            rigidbodyR.AddForce(grabDir * 10);

            float currentDist = Vector3.Distance(transform.position, grabPoint);

            if (currentDist < grabDistance)
            {
                grabDistance = currentDist;
            }
        }

        if (isGrappling)
        {
            lineRenderer.SetPosition(0,transform.position);
        }
	}

	void FixedUpdate()
    {
        Vector3 force = moveAmount.x * transform.right + moveAmount.z * transform.forward;
        if (!grounded)
            force *= airControl;

        rigidbodyR.AddForce(force * Time.fixedDeltaTime);

        Vector3 drag = new Vector3(rigidbodyR.velocity.x, 0, rigidbodyR.velocity.z) * -(grounded ? groundFriction:airFriction) * 100;
		rigidbodyR.AddForce(drag * Time.fixedDeltaTime);

        if (isGrappling)
        {
            if (Vector3.Distance(transform.position, grabPoint) > grabDistance)
            {
                Vector3 grabDir = (transform.position - grabPoint).normalized;

                rigidbodyR.MovePosition(grabPoint + grabDir * grabDistance);
                rigidbodyR.velocity = Vector3.ProjectOnPlane(rigidbodyR.velocity, grabDir);
            }
                
        }


        Ray ray = new Ray (transform.position, -transform.up);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, 1 + .1f, groundedMask)) {
            grounded = true;
        }
        else {
            grounded = false;
        }
	}

	void LockMouse() {
		Cursor.lockState = CursorLockMode.Locked;
		Cursor.visible = false;
	}
}