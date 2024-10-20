using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Stair : MonoBehaviour
{
    void OnTriggerEnter2D(Collider2D coll)
    {
        if (coll.gameObject.CompareTag("Player"))
        {
            int currentStage = int.Parse(SceneManager.GetActiveScene().name.Substring(5));
            string nextStage = "Stage" + (currentStage+1);
            SceneManager.LoadScene(nextStage);
        }
    }
}
