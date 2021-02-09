using UnityEngine;

public class ExplodeCubes : MonoBehaviour
{
    public GameObject restartButton, explosion;
    private bool collisionSet;
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Cube" && !collisionSet)
        {
            for (int i = collision.transform.childCount - 1; i >= 0; i--)
            {
                Transform child = collision.transform.GetChild(i);
                child.gameObject.AddComponent<Rigidbody>().AddExplosionForce(70f, Vector3.up, 5f);
                child.SetParent(null);
            }
            Destroy(collision.gameObject);
            collisionSet = true;
            Camera.main.transform.localPosition -= new Vector3(0, 0, 5f);
            Camera.main.gameObject.AddComponent<CameraShake>();

            GameObject explosObj = Instantiate(explosion, collision.contacts[0].point, Quaternion.identity);
            Destroy(explosObj, 2.5f);
            if (PlayerPrefs.GetString("music") == "On")
                GetComponent<AudioSource>().Play();

            restartButton.SetActive(true);
        }
    }
}