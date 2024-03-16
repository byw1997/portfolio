using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Player : MonoBehaviour
{
    Rigidbody rigid;
    bool singlejump;
    bool doublejump;
    public int score;
    public float jumpforce;
    public GameManagerLogic Manager;
    new AudioSource audio;

    void Awake()
    {
        singlejump = false;
        doublejump = false;
        rigid = GetComponent<Rigidbody>();
        audio = GetComponent<AudioSource>();
    }

    void Update()
    {
        if (Input.GetButtonDown("Jump"))
        {
            if (!singlejump)
            {
                singlejump = true;
                rigid.AddForce(new Vector3(0, jumpforce, 0), ForceMode.Impulse);
            }
            else if (!doublejump)
            {
                doublejump = true;
                rigid.AddForce(new Vector3(0, jumpforce, 0), ForceMode.Impulse);
            }
        }
    }

    void FixedUpdate()
    {
        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");
        rigid.AddForce(new Vector3(h, 0, v), ForceMode.Impulse);
    }

    void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.tag == "Floor")
        {
            singlejump = false;
            doublejump = false;
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Item")
        {
            score++;
            Manager.GetItem(score);
            audio.Play();
            other.gameObject.SetActive(false);
        }
        else if(other.tag == "Goal")
        {
            if(score == Manager.TotalItemCount)
            {
                if(Manager.stage == 1)
                {
                    SceneManager.LoadScene(0);
                }
                else
                {
                    SceneManager.LoadScene(Manager.stage + 1);
                }
                //clear
                
            }
            else
            {
                //restart
                SceneManager.LoadScene(Manager.stage);
            }
        }
    }
}
