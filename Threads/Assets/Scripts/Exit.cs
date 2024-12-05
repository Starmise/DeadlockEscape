using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Exit : MonoBehaviour
{
    [SerializeField] string nextLevelName;

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            Debug.Log("Level complete");

            // ThreadMovement playerMovement = other.GetComponent<ThreadMovement>();

            //if (playerMovement != null)
            //{
            //    playerMovement.DisableInput();
            //}

            StartCoroutine("LoadLevel");
        }
    }

    IEnumerator LoadLevel()
    {
        yield return new WaitForSeconds(1);

        SceneManager.LoadScene(nextLevelName);
    }
}
