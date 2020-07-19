using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class kutzooi : MonoBehaviour
{

    bool doehet = false;
    /*
    IEnumerator waardeloos()
    {
        new WaitForFixedUpdate();
        Debug.Log("siblingindex = " + transform.GetSiblingIndex());
        yield return null;
    }*/

    private void OnEnable()
    {
doehet = true;
        //Debug.Log("siblingindex = " + transform.GetSiblingIndex());
    }

    //dit is achterlijk, maar het geeft tenminste niet de hoogste childindex als wanneer het in onenable gedaan wordt
    private void Update()
    {
        
        if (doehet)
        {
            doehet = false;
            Debug.Log("siblingindex = " + transform.GetSiblingIndex());
        }
    }
}
