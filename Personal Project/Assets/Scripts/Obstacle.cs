using UnityEngine;

public class Obstacle : MonoBehaviour
{
    /*Killzone*/
    private void OnTriggerEnter(Collider collision)
    {
        if (collision.gameObject.CompareTag("Destroy"))
        {
            Destroy(transform.parent.parent.gameObject);

        }

    }

    //ARCHIVES
    //Failed attempt at animating the swing by code
    //Using legacy Animation System for now

    //[SerializeField] private bool isVariant;
    //[SerializeField] private float swingForce;
    //[SerializeField] GameObject obstacleRb;
    //private Rigidbody rb;
    //private float horizontalInput = 1;
    //private float delay;

    //private void Start()
    //{
    //    rb= obstacleRb.GetComponent<Rigidbody>();
    //    rb.maxAngularVelocity = 1000;
    //    if (isVariant) 
    //    { 
    //        rb.isKinematic = false;
    //        //StartCoroutine("Swing", 2.1f); 

    //    }
    //    else 
    //    {
    //        rb.isKinematic = true;
    //        rb.constraints = RigidbodyConstraints.FreezeAll;

    //    }

    //}

    //private void FixedUpdate()
    //{
    //    if (isVariant) { MoveObstacle(); }        

    //}

    //private void MoveObstacle()
    //{
    //    /*if stalls push the other way*/
    //    if(rb.velocity.magnitude < 0.1f && Time.time > delay) 
    //    { 
    //        horizontalInput*= -1; 
    //        delay = Time.time + 0.5f;
    //    }

    //    /*Left to Right*/        
    //    rb.AddTorque(obstacleRb.transform.forward * horizontalInput * Time.deltaTime * swingForce, ForceMode.Acceleration );

    //    /*Local Position Constraint*/
    //    Vector3 localVelocity = obstacleRb.transform.InverseTransformDirection(rb.velocity);
    //    localVelocity.z *= -1;
    //    rb.velocity = obstacleRb.transform.TransformDirection(localVelocity);

    //}    
    //*Changes force direction by seconds //old system*/
    //IEnumerator Swing(float rate)
    //{
    //    while (GameManager.instance.isGameActive)
    //    {
    //        yield return new WaitForSeconds(rate);

    //        horizontalInput *= -1;

    //    }

    //}


}
