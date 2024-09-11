using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;

public class PlayerController : MonoBehaviour
{
    public bool isDead { get; private set; } = false;

    [SerializeField] private float carveForce = 1000;
    [SerializeField] private float carveIntensity = 10;
    [SerializeField] private float jumpForce = 42;
    [SerializeField] private float pushForce = 10;
    [SerializeField] private float brakeForce = 1;
    [SerializeField] private float maxSpeed = 1.25f;
    [SerializeField] private float deadImpulse = 1000;
    [SerializeField] private float sprint = 2;
    [Space]
    [SerializeField] Rigidbody tubeRb;
    [Space]
    [SerializeField] PositionConstraint lookAt;
    [SerializeField] GameObject character;
    [SerializeField] Animator skateAnimator;
    [Space]
    [SerializeField] ParticleSystem splashParticles;
    [Space]
    [SerializeField] AudioSource waterAudio;
    [SerializeField] AudioSource playerAudio;
    [SerializeField] AudioClip rollClip;
    [SerializeField] AudioClip jumpInClip;
    [SerializeField] AudioClip jumpOutClip;
    [SerializeField] AudioClip deathClip;

    private float horizontalInput;
    private float verticalInput;
    private Collider playerCol;
    private Rigidbody playerRb;
    private Rigidbody[] ragdollRb;
    private GameObject mesh;
    private Animator animator;
    private Dictionary<string, Vector3> ragdollPos = new Dictionary<string, Vector3>();
    private Dictionary<string, Quaternion> ragdollRot = new Dictionary<string, Quaternion>();
    private bool isGrounded;
    private bool isSplashing;
    

    void Start()
    {
        playerCol = GetComponent<Collider>();
        playerRb = GetComponent<Rigidbody>();
        playerRb.isKinematic = false;
        isGrounded = true;

        if (!tubeRb) { tubeRb = GameObject.Find("CTRL_spin").GetComponent<Rigidbody>(); }
        if (!lookAt) { lookAt = GameObject.Find("Center").GetComponent<PositionConstraint>(); }
        if (!character) { character = GameObject.Find("Character"); }

        mesh = character.transform.Find("Orientation/Capsule/Skate/Anchor/Diego").gameObject;
        animator = mesh.GetComponent<Animator>();

        ragdollRb = character.GetComponentsInChildren<Rigidbody>();
        foreach (var rb in ragdollRb)
        {
            ragdollPos.Add(rb.gameObject.name, rb.gameObject.transform.localPosition);
            ragdollRot.Add(rb.gameObject.name, rb.gameObject.transform.localRotation);

            //Debug.Log(rb.gameObject.name + rb.gameObject.transform.localPosition);
            //Debug.Log(rb.gameObject.name + ragdollPos[rb.gameObject.name]);
        }

    }

    void Update()
    {
        if (isGrounded && !isDead && GameManager.Instance.isGameActive)
        {
            horizontalInput = Input.GetAxis("Horizontal");
            verticalInput = Input.GetAxis("Vertical");

            Carve();
            Push();
            Jump();
            Animation();
            Fx();
        }

    }

    private void Carve()
    {
        //Unbrake
        if(horizontalInput != 0)
        {
            playerRb.drag = 0;
            playerRb.angularDrag = 0.05f;
        }

        //Left to Right
        playerRb.AddForce(Vector3.right * horizontalInput * Time.deltaTime * carveForce);
        playerRb.AddTorque(Vector3.back * horizontalInput * Time.deltaTime * carveForce);

        //Pump
        tubeRb.AddRelativeTorque(Vector3.back * Math.Abs(horizontalInput) * Time.deltaTime * (pushForce / 4));

    }

    private void Push()
    {
        float force = pushForce;

        if (verticalInput >= 0)
        {
            tubeRb.angularDrag = 0.01f;

            if (tubeRb.angularVelocity.magnitude < maxSpeed)
            {
                if (Input.GetKeyDown(KeyCode.LeftShift)) { force = pushForce * sprint; animator.SetBool("isSprinting", true); }
                if (Input.GetKeyUp(KeyCode.LeftShift)) { force = pushForce; animator.SetBool("isSprinting", false); }
                tubeRb.AddRelativeTorque(Vector3.back * verticalInput * Time.deltaTime * force);

            }

        }
        //Brake
        else if (verticalInput < 0)
        {
            tubeRb.angularDrag = brakeForce;

            playerRb.drag = brakeForce;
            playerRb.angularDrag = brakeForce;

        }

    }

    private void Jump()
    {
        if (Input.GetButtonDown("Jump") && verticalInput >= 0)
        {
            lookAt.constraintActive = true;

            Physics.Raycast(transform.position, Vector3.down, out RaycastHit hit);
            playerRb.AddForce(hit.normal * jumpForce, ForceMode.Impulse);
            isGrounded = false;            

            animator.SetBool("isGrounded", false);
            animator.SetTrigger("Jump");
            skateAnimator.SetBool("isGrounded", false);
            skateAnimator.SetTrigger("Jump");
            PlayAudio(jumpInClip, false, 1); //TODO: fix volume bug
            AudioSource.PlayClipAtPoint(jumpInClip, transform.position, 0.07f);

        }

    }

    private void Animation()
    {
        //Skate rotation
        var velocity = playerRb.velocity.x;
        var lookAt = character.GetComponentInChildren<LookAtConstraint>();
        lookAt.rotationOffset = new Vector3(0, velocity * carveIntensity, 0);

        //Character Animation
        animator.SetFloat("verticalInput", verticalInput);
        animator.SetFloat("horizontalInput", horizontalInput);
        animator.SetFloat("magnitude", tubeRb.angularVelocity.magnitude);

    }

    private void Fx()
    {
        //Particles
        if (isSplashing)
        {   
            //Moves with player
            splashParticles.transform.localPosition = new Vector3 (transform.position.x, 0, -0.3f);
        }
        //Audio
        if (!playerAudio.isPlaying && tubeRb.angularVelocity.magnitude > 0 && isGrounded)
        {
            playerAudio.clip = rollClip;
            playerAudio.loop= true;
            playerAudio.Play();
        }
        if (tubeRb.angularVelocity.magnitude <= 0 && isGrounded)
        {
            playerAudio.Stop();
        }
        playerAudio.volume = tubeRb.angularVelocity.magnitude / (maxSpeed*10);
    }

    private void PlayAudio(AudioClip clip, bool loop)
    {
        playerAudio.Stop();
        playerAudio.clip = clip;
        playerAudio.loop = loop;
        playerAudio.Play();
    }

    private void PlayAudio(AudioClip clip, bool loop, float volume)
    {        
        PlayAudio(clip, loop);
        playerAudio.volume = volume;

    }

    private void Dead()
    {
        isDead = true;
        playerCol.isTrigger = true;
        playerRb.isKinematic = true;

        ScoreManager manager = GameManager.Instance.scoreManager;
        manager.SubstractLife();
        if(manager.lives > 0)
        {
            StartCoroutine(Respawn(3));
        }

        //animation
        character.GetComponent<PositionConstraint>().constraintActive = false;
        character.GetComponentInChildren<LookAtConstraint>().constraintActive = false;
        mesh.GetComponent<Animator>().enabled = false;
        //skateAnimator.gameObject.GetComponent<Animator>().enabled = false;

        StartCoroutine(Ragdoll());

        PlayAudio(deathClip, false, 0.35f);

    }
    
    IEnumerator Ragdoll()
    {
        yield return new WaitForSeconds(0);
        
        mesh.transform.parent = null;

        foreach (Rigidbody rb in ragdollRb)
        {
            rb.isKinematic = false;
            rb.AddForce(new Vector3(0, 2, 0.5f) * tubeRb.angularVelocity.magnitude * deadImpulse, ForceMode.VelocityChange);

        }

    }

    IEnumerator Respawn(float delay)
    {
        yield return new WaitForSeconds(delay);

        isDead = false;
        playerCol.isTrigger = false;
        playerRb.isKinematic = false;        

        /*reset ragdoll*/
        Transform skate = character.transform.Find("Orientation/Capsule/Skate/Anchor");
        mesh.transform.SetParent(skate);
        mesh.transform.localPosition= Vector3.zero;
        mesh.transform.localRotation= Quaternion.identity;
        mesh.transform.localScale = Vector3.one;

        for (int i = 0; i < ragdollRb.Length; i++)
        {
            ragdollRb[i].isKinematic = true;

            GameObject obj = ragdollRb[i].gameObject;
            string name = obj.name;

            //Debug.Log(name + ragdollPos[name]);

            obj.transform.localPosition = ragdollPos[name];
            obj.transform.localRotation = ragdollRot[name];
        }

        /*animation*/
        character.GetComponent<PositionConstraint>().constraintActive = true;
        character.GetComponentInChildren<LookAtConstraint>().constraintActive = true;
        character.GetComponent<Animation>().Play();
        mesh.GetComponent<Animator>().enabled = true;

    }

    private void OnCollisionEnter(Collision collision)
    {
        /*Checks if isGrounded*/
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = true;
            lookAt.constraintActive = false;

            PlayAudio(rollClip, true);
            AudioSource.PlayClipAtPoint(jumpOutClip, transform.position, 0.07f);

            animator.SetBool("isGrounded", true);
            skateAnimator.SetBool("isGrounded", true);
        }

        else if (collision.gameObject.CompareTag("Obstacle"))
        {
            Dead();            

        }

    }

    private void OnTriggerEnter(Collider other)
    {          
        if (other.gameObject.CompareTag("Splash") && !isDead)
        {
            isSplashing = true;
            splashParticles.Play();
            waterAudio.Play();
        }

        if (other.gameObject.CompareTag("Pickup") && !isDead)
        {
            int points = other.GetComponent<PickUps>().Points;
            GameManager.Instance.scoreManager.AddPoints(points);

        }

    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Splash"))
        {
            isSplashing = false;
            splashParticles.Stop();
            waterAudio.Stop();
        }

    }


}
