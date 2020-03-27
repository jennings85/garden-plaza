using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    private GameObject cameraObject;
    public Animator fadeAnim;

    // Start is called before the first frame update
    void Start()
    {
        cameraObject = GameObject.Find("Camera Object");
        //fadeAnim = GameObject.Find("Fade").GetComponent<Animator>();
    }

    private void Update()
    {

        if (Input.GetButtonDown("A"))
        {
            fadeAnim.SetBool("Fade", true);
        }
        else if (Input.GetButtonDown("X"))
        {
            Application.Quit();
        }

        if (fadeAnim.GetCurrentAnimatorStateInfo(0).IsName("Change"))
        {
            SceneManager.LoadScene(1);
        }
    } 

    void FixedUpdate()
    {
        cameraObject.transform.Rotate(new Vector3(0, 20 * Time.deltaTime, 0));
    }
}
