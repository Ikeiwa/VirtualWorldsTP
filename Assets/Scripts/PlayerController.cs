using System.Collections;
using System.Collections.Generic;
using UnityEngine;
 
 
[RequireComponent(typeof(CapsuleCollider))]
[RequireComponent(typeof(Rigidbody))]
 
public class PlayerController : MonoBehaviour {
 
    public float mouseSensitivity = 1.0f;
    [Tooltip("How fast the player moves.")]
    public float MovementSpeed = 7.0f;
    [Tooltip("Units per second acceleration")]
    public float AccelRate = 20.0f;
    [Tooltip("Units per second deceleration")]
    public float DecelRate = 20.0f;
    [Tooltip("Acceleration the player has in mid-air")]
    public float AirborneAccel = 5.0f;
    [Tooltip("The velocity applied to the player when the jump button is pressed")]
    public float JumpSpeed = 7.0f;
    [Tooltip("Extra units added to the player's fudge height... if you're rocketting off ramps or feeling too loosely attached to the ground, increase this. If you're being yanked down to stuff too far beneath you, lower this.")]
    // Extra height, can't modify this during runtime
    public float FudgeExtra = 0.5f;
    [Tooltip("Maximum slope the player can walk up")]
    public float MaximumSlope = 45.0f;
    public LayerMask groundedMask;
 
    private bool grounded = false;
 
    //Unity Components
    private Transform cameraT;
    private LineRenderer lineRenderer;
    private Rigidbody rb;
    private Collider coll;
 
    // Temp vars
    private float inputX;
    private float inputY;
    private Vector3 movement;
 
    // Acceleration or deceleration
    private float acceleration;
 
    /*
     * Keep track of falling
     */
    private bool falling;
    private float fallSpeed;
 
    /*
     * Jump state var:
     * 0 = hit ground since last jump, can jump if grounded = true
     * 1 = jump button pressed, try to jump during fixedupdate
     * 2 = jump force applied, waiting to leave the ground
     * 3 = jump was successful, haven't hit the ground yet (this state is to ignore fudging)
    */
    private byte doJump;
 
    // Average normal of the ground i'm standing on
    private Vector3 groundNormal;
 
    // If we're touching a dynamic object, don't prevent idle sliding
    private bool touchingDynamic;
 
    // Was i grounded last frame? used for fudging
    private bool groundedLastFrame;
 
    // The objects i'm colliding with
    private List<GameObject> collisions;
 
    // All of the collision contact points
    private Dictionary<int, ContactPoint[]> contactPoints;

    private bool isGrappling = false;
    private Vector3 grabPoint;
    private float grabDistance;
    private float verticalLookRotation;
 
    /*
     * Temporary calculations
     */
    private float halfPlayerHeight;
    private float fudgeCheck;
    private float bottomCapsuleSphereOrigin; // transform.position.y - this variable = the y coord for the origin of the capsule's bottom sphere
    private float capsuleRadius;
 
    void Awake()
    {
        cameraT = Camera.main.transform;
        lineRenderer = GetComponent<LineRenderer>();
        rb = GetComponent<Rigidbody>();
        coll = GetComponent<Collider>();
 
        movement = Vector3.zero;
 
        grounded = false;
        groundNormal = Vector3.zero;
        touchingDynamic = false;
        groundedLastFrame = false;
 
        collisions = new List<GameObject>();
        contactPoints = new Dictionary<int, ContactPoint[]>();
 
        // do our calculations so we don't have to do them every frame
        CapsuleCollider capsule = (CapsuleCollider)coll;
        Debug.Log(capsule);
        halfPlayerHeight = capsule.height * 0.5f;
        fudgeCheck = halfPlayerHeight + FudgeExtra;
        bottomCapsuleSphereOrigin = halfPlayerHeight - capsule.radius;
        capsuleRadius = capsule.radius;
 
        PhysicMaterial controllerMat = new PhysicMaterial();
        controllerMat.bounciness = 0.0f;
        controllerMat.dynamicFriction = 0.0f;
        controllerMat.staticFriction = 0.0f;
        controllerMat.bounceCombine = PhysicMaterialCombine.Minimum;
        controllerMat.frictionCombine = PhysicMaterialCombine.Minimum;
        capsule.material = controllerMat;
 
        // just in case this wasn't set in the inspector
        rb.freezeRotation = true;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
 
    void FixedUpdate()
    {
        // check if we're grounded
        RaycastHit hit;
        grounded = false;
        groundNormal = Vector3.zero;
 
        foreach (ContactPoint[] contacts in contactPoints.Values)
            for (int i = 0; i < contacts.Length; i++)
                if (contacts[i].point.y <= rb.position.y - bottomCapsuleSphereOrigin && Physics.Raycast(contacts[i].point + Vector3.up, Vector3.down, out hit, 1.1f, ~0) && Vector3.Angle(hit.normal, Vector3.up) <= MaximumSlope) {
                    grounded = true;
                    groundNormal += hit.normal;
 
                }
 
        if (grounded) {
            // average the summed normals
            groundNormal.Normalize();
 
            if (doJump == 3)
                doJump = 0;
        } else if (doJump == 2)
            doJump = 3;
 
        // get player input
        inputX = Input.GetAxis("Horizontal");
        inputY = Input.GetAxis("Vertical");
 
        // limit the length to 1.0f
        float length = Mathf.Sqrt(inputX * inputX + inputY * inputY);
 
        if (length > 1.0f) {
            inputX /= length;
            inputY /= length;
        }
 
        if (grounded && doJump != 3) {
            if (falling) {
                // we just landed from a fall
                falling = false;
                this.DoFallDamage(Mathf.Abs(fallSpeed));
            }
 
            // align our movement vectors with the ground normal (ground normal = up)
            Vector3 newForward = transform.forward;
            Vector3.OrthoNormalize(ref groundNormal, ref newForward);
 
            Vector3 targetSpeed = Vector3.Cross(groundNormal, newForward) * inputX * MovementSpeed + newForward * inputY * MovementSpeed;
 
            length = targetSpeed.magnitude;
            float difference = length - rb.velocity.magnitude;
 
            // avoid divide by zero
            if (Mathf.Approximately(difference, 0.0f))
                movement = Vector3.zero;
 
            else {
                // determine if we should accelerate or decelerate
                if (difference > 0.0f)
                    acceleration = Mathf.Min(AccelRate * Time.deltaTime, difference);
 
                else
                    acceleration = Mathf.Max(-DecelRate * Time.deltaTime, difference);
 
                // normalize the difference vector and store it in movement
                difference = 1.0f / difference;
                movement = new Vector3((targetSpeed.x - rb.velocity.x) * difference * acceleration, (targetSpeed.y - rb.velocity.y) * difference * acceleration, (targetSpeed.z - rb.velocity.z) * difference * acceleration);
            }
 
            if (doJump == 1) {
                // jump button was pressed, do jump    
                movement.y = JumpSpeed - rb.velocity.y;
                doJump = 2;
            } else if (!touchingDynamic && Mathf.Approximately(inputX + inputY, 0.0f) && doJump < 2)
                // prevent sliding by countering gravity... this may be dangerous
                movement.y -= Physics.gravity.y * Time.deltaTime;
 
            rb.AddForce(new Vector3(movement.x, movement.y, movement.z), ForceMode.VelocityChange);
            groundedLastFrame = true;
        } else {
            // not grounded, so check if we need to fudge and do air accel
 
            // fudging
            if (groundedLastFrame && doJump != 3 && !falling) {
                // see if there's a surface we can stand on beneath us within fudgeCheck range
                if (Physics.Raycast(transform.position, Vector3.down, out hit, fudgeCheck + (rb.velocity.magnitude * Time.deltaTime), ~0) && Vector3.Angle(hit.normal, Vector3.up) <= MaximumSlope) {
                    groundedLastFrame = true;
 
                    // catches jump attempts that would have been missed if we weren't fudging
                    if (doJump == 1) {
                        movement.y += JumpSpeed;
                        doJump = 2;
                        return;
                    }
 
                    // we can't go straight down, so do another raycast for the exact distance towards the surface
                    // i tried doing exsec and excsc to avoid doing another raycast, but my math sucks and it failed horribly
                    // if anyone else knows a reasonable way to implement a simple trig function to bypass this raycast, please contribute to the thead!
                    if (Physics.Raycast(new Vector3(transform.position.x, transform.position.y - bottomCapsuleSphereOrigin, transform.position.z), -hit.normal, out hit, hit.distance, ~0)) {
                        rb.AddForce(hit.normal * -hit.distance, ForceMode.VelocityChange);
                        return; // skip air accel because we should be grounded
                    }
                }
            }
 
            // if we're here, we're not fudging so we're defintiely airborne
            // thus, if falling isn't set, set it
            if (!falling)
                falling = true;
 
            fallSpeed = rb.velocity.y;
 
            // air accel
            if (!Mathf.Approximately(inputX + inputY, 0.0f)) {
                // note, this will probably malfunction if you set the air accel too high... this code should be rewritten if you intend to do so
 
                // get direction vector
                movement = transform.TransformDirection(new Vector3(inputX * AirborneAccel * Time.deltaTime, 0.0f, inputY * AirborneAccel * Time.deltaTime));
 
                // add up our accel to the current velocity to check if it's too fast
                float a = movement.x + rb.velocity.x;
                float b = movement.z + rb.velocity.z;
 
                // check if our new velocity will be too fast
                length = Mathf.Sqrt(a * a + b * b);
                if (length > 0.0f) {
                    if (length > MovementSpeed) {
                        // normalize the new movement vector
                        length = 1.0f / Mathf.Sqrt(movement.x * movement.x + movement.z * movement.z);
                        movement.x *= length;
                        movement.z *= length;
 
                        // normalize our current velocity (before accel)
                        length = 1.0f / Mathf.Sqrt(rb.velocity.x * rb.velocity.x + rb.velocity.z * rb.velocity.z);
                        Vector3 rigidbodyDirection = new Vector3(rb.velocity.x * length, 0.0f, rb.velocity.z * length);
 
                        // dot product of accel unit vector and velocity unit vector, clamped above 0 and inverted (1-x)
                        length = (1.0f - Mathf.Max(movement.x * rigidbodyDirection.x + movement.z * rigidbodyDirection.z, 0.0f)) * AirborneAccel * Time.deltaTime;
                        movement.x *= length;
                        movement.z *= length;
                    }
 
                    // and finally, add our force
                    rb.AddForce(new Vector3(movement.x, 0.0f, movement.z), ForceMode.VelocityChange);
                }
            }
 
            groundedLastFrame = false;
        }

        rb.useGravity = !grounded;

        if (Input.GetMouseButton(1) && isGrappling && grabDistance > 2)
        {
            Vector3 grabDir = (grabPoint - transform.position).normalized;
            rb.AddForce(grabDir * 10);

            float currentDist = Vector3.Distance(transform.position, grabPoint);

            if (currentDist < grabDistance)
            {
                grabDistance = currentDist;
            }
        }

        if (isGrappling)
        {
            if (Vector3.Distance(transform.position, grabPoint) > grabDistance)
            {
                Vector3 grabDir = (transform.position - grabPoint).normalized;

                rb.MovePosition(grabPoint + grabDir * grabDistance);
                rb.velocity = Vector3.ProjectOnPlane(rb.velocity, grabDir);
            }
                
        }
    }
 
    void Update()
    {
        transform.Rotate (Vector3.up * (Input.GetAxis("Mouse X") * mouseSensitivity));
        verticalLookRotation += Input.GetAxis ("Mouse Y") * mouseSensitivity;
        verticalLookRotation = Mathf.Clamp (verticalLookRotation, -95, 95);
        cameraT.localEulerAngles = Vector3.left * verticalLookRotation;

        // check for input here
        if (groundedLastFrame && Input.GetButtonDown("Jump"))
            doJump = 1;

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

        if (isGrappling)
        {
            lineRenderer.SetPosition(0,transform.position);
        }
    }
 
    void DoFallDamage(float fallSpeed) // fallSpeed will be positive
    {
        // do your fall logic here using fallSpeed to determine how hard we hit the ground
        Debug.Log("Hit the ground at " + fallSpeed.ToString() + " units per second");
    }
 
    void OnCollisionEnter(Collision collision)
    {
        // keep track of collision objects and contact points
        collisions.Add(collision.gameObject);
        contactPoints.Add(collision.gameObject.GetInstanceID(), collision.contacts);
 
        // check if this object is dynamic
        if (!collision.gameObject.isStatic)
            touchingDynamic = true;
 
        // reset the jump state if able
        if (doJump == 3)
            doJump = 0;
    }
 
    void OnCollisionStay(Collision collision)
    {
        // update contact points
        contactPoints[collision.gameObject.GetInstanceID()] = collision.contacts;
    }
 
    void OnCollisionExit(Collision collision)
    {
        touchingDynamic = false;
 
        // remove this collision and its associated contact points from the list
        // don't break from the list once we find it because we might somehow have duplicate entries, and we need to recheck groundedOnDynamic anyways
        for (int i = 0; i < collisions.Count; i++) {
            if (collisions[i] == collision.gameObject)
                collisions.RemoveAt(i--);
 
            else if (!collisions[i].isStatic)
                touchingDynamic = true;
        }
 
        contactPoints.Remove(collision.gameObject.GetInstanceID());
    }
 
    public bool Grounded {
        get {
            return grounded;
        }
    }
 
    public bool Falling {
        get {
            return falling;
        }
    }
 
    public float FallSpeed {
        get {
            return fallSpeed;
        }
    }
 
    public Vector3 GroundNormal {
        get {
            return groundNormal;
        }
    }
}