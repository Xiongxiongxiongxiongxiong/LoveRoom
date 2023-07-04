using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneLoader : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        DontDestroyOnLoad(this.gameObject);

      

    }


    bool isClicked = false;
    float dt = 0;
    // Update is called once per frame
    void Update()
    {
        isClicked = false;
#if UNITY_EDITOR
        if (Input.GetMouseButtonDown(0))
#else
        if(Input.GetTouch(0).phase== TouchPhase.Began)
#endif
        {
            isClicked = false;
            dt = Time.time;
        }

#if UNITY_EDITOR
        if (Input.GetMouseButtonUp(0))
            
#else
        if(Input.GetTouch(0).phase== TouchPhase.Ended)
#endif
        {
            dt = Time.time-dt;
            if (dt <= 0.1f) isClicked = true;
            else
                isClicked = false;
        }


        if (isClicked) {

            var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out var hitinfo))
            {
                if (hitinfo.collider.name.Contains("love"))
                {
                    UnityEngine.SceneManagement.SceneManager.LoadScene(1);
                }
                else if (hitinfo.collider.name.Contains("muma"))
                {
                    UnityEngine.SceneManagement.SceneManager.LoadScene(2);
                }
                else if (hitinfo.collider.name.Contains("class"))
                {
                    UnityEngine.SceneManagement.SceneManager.LoadScene(3);
                }
            }
        }
    }

        
        
}
