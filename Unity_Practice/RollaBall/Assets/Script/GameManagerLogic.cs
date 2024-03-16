using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class GameManagerLogic : MonoBehaviour
{
    public int TotalItemCount;
    public int stage;
    public GameObject TotalItem;
    public GameObject CurrentItem;
    TMP_Text TotalItemText;
    TMP_Text CurrentItemText;

    void Awake()
    {
        CurrentItemText = CurrentItem.GetComponent<TMP_Text>();
        TotalItemText = TotalItem.GetComponent<TMP_Text>();
        TotalItemText.text = "/ " + TotalItemCount.ToString();
    }

    public void GetItem(int count)
    {
        CurrentItemText.text = count.ToString();
    }

    void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag == "Player")
        {
            SceneManager.LoadScene(stage);
        }
    }
}
