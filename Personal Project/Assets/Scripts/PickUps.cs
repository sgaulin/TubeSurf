using UnityEngine;

public class PickUps : MonoBehaviour
{
    public int Points = 100;
    [Header("Bonus")]
    [SerializeField] private AudioClip sfx;
    [SerializeField] private GameObject vfx;


    private void Start()
    {
        if (vfx != null)
        {
            vfx.SetActive(false);
        }

    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (sfx != null)
            {
                AudioSource.PlayClipAtPoint(sfx, transform.position, 0.3f);
                GetComponent<Collider>().enabled = false;
            }
            if (vfx != null)
            {
                vfx.SetActive(true);
            }
        }      

    }


}
