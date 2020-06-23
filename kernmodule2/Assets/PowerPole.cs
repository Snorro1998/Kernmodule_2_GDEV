using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class PowerPole : MonoBehaviour
{
    public PowerPole previousPowerPole;
    public PowerPole nextPowerPole;
    public GameObject wirePrefab;

    float wireLength = 0;
    Transform cableOrigins, cableDestinations;

    private void AdjustWireLength(GameObject _gm/*, float length*/, Vector3 startPos, Vector3 endPos)
    {
        float distToOther = Vector3.Distance(startPos, endPos);
        Vector3 tmpScale = _gm.transform.localScale;
        tmpScale.z *= distToOther / Mathf.Max(0.0001f, wireLength);
        _gm.transform.localScale = tmpScale;
    }

    public void CreateWires()
    {
        if (nextPowerPole != null && wirePrefab != null)
        {
            cableOrigins = transform.Find("StartPoints");
            cableDestinations = nextPowerPole.transform.Find("EndPoints");
            int iMax = Mathf.Min(cableOrigins.childCount, cableDestinations.childCount);

            for (int i = 0; i < iMax; i++)
            {
                //kabelpunt van zichzelf
                Transform tOrig = cableOrigins.GetChild(i);
                //kabelpunt van de ander
                Transform tDest = cableDestinations.GetChild(i);
                tOrig.LookAt(tDest);
                GameObject gm = Instantiate(wirePrefab, tOrig.position, tOrig.rotation);
                gm.transform.parent = tOrig;
                Vector3 tmpScale = gm.transform.localScale;
                tmpScale.z = Vector3.Distance(tOrig.position, tDest.position) / wireLength;
                gm.transform.localScale = tmpScale;
            }
        }
    }

    public void UpdatePreviousObjectsInLine()
    {
        if (previousPowerPole != null)
        {
            previousPowerPole.LookAtNext();
            /*
            Quaternion rot = transform.rotation;
            rot.y = previousPowerPole.transform.rotation.y;
            //transform.rotation = rot;*/
            previousPowerPole.UpdateWires();
            if (previousPowerPole.previousPowerPole != null)
            {
                previousPowerPole.previousPowerPole.UpdateWires();
            }
        }
    }

    //fix voor undo als je een paal verwijdert met delete
    private void OnEnable()
    {
        if (wireLength == 0)
        {
            wireLength = wirePrefab.GetComponent<Renderer>().bounds.size.z;
        }

        if (previousPowerPole != null)
        {
            if (previousPowerPole.nextPowerPole != this)
            {
                previousPowerPole.nextPowerPole = this;
                UpdatePreviousObjectsInLine();
            }
        }

        if (nextPowerPole != null)
        {
            if (nextPowerPole.previousPowerPole != this)
            {
                nextPowerPole.previousPowerPole = this;
            }
        }
    }

    private void OnDestroy()
    {
        if (previousPowerPole != null)
        {
            previousPowerPole.nextPowerPole = nextPowerPole != null ? nextPowerPole : null;
            UpdatePreviousObjectsInLine();
        }

        if (nextPowerPole != null)
        {
            nextPowerPole.previousPowerPole = previousPowerPole != null ? previousPowerPole : null;
            UpdatePreviousObjectsInLine();
        }
    }

    public void UpdateWires()
    {
        PowerPole nextPole = nextPowerPole;
        cableOrigins = transform.Find("StartPoints");
        if (nextPowerPole != null)
        {
            cableDestinations = nextPowerPole.transform.Find("EndPoints");
        }

        else
        {
            DestroyCables();
            return;
        }
        
        int iMax = Mathf.Min(cableOrigins.childCount, cableDestinations.childCount);

        for (int i = 0; i < iMax; i++)
        {
            //kabelpunt van zichzelf
            Transform tOrig = cableOrigins.GetChild(i);
            //kabelpunt van de ander
            Transform tDest = cableDestinations.GetChild(i);
            tOrig.LookAt(tDest);

            if (tOrig.childCount > 0)
            {
                GameObject gm = tOrig.GetChild(0).gameObject;
                //AdjustWireLength(tOrig.gameObject, tOrig.position, tDest.position);
                //gm.transform.parent = tOrig;

                Vector3 tmpScale = gm.transform.localScale;
                tmpScale.z = Vector3.Distance(tOrig.position, tDest.position) / Mathf.Max(0.00001f, wireLength);
                gm.transform.localScale = tmpScale;
            }
            
        }
    }

    public void DestroyCables()
    {
        foreach(Transform t in cableOrigins)
        {
            foreach(Transform tt in t)
            {
                DestroyImmediate(tt.gameObject);
            }
        }
    }

    public void LookAtNext()
    {
        if (nextPowerPole != null)
        {
            Vector3 tmpPos = nextPowerPole.transform.position;
            tmpPos.y = transform.position.y;
            transform.LookAt(tmpPos);
        }

        UpdateWires();
    }

    public void LookAtPrevious()
    {
        if (previousPowerPole != null)
        {
            transform.forward = previousPowerPole.transform.forward;
            /*
            Vector3 tmpPos = previousPowerPole.transform.position;
            tmpPos.y = transform.position.y;
            transform.LookAt(tmpPos);*/
        }    
    }
}
