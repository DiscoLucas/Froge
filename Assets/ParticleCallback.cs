using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleCallback : MonoBehaviour
{
    Die die;
    // Start is called before the first frame update
    void Start()
    {
        // get the die component of the parent object
        die = transform.parent.GetComponent<Die>();
        
        var main = GetComponent<ParticleSystem>().main;
        main.stopAction = ParticleSystemStopAction.Callback;
    }

    private void OnParticleSystemStopped()
    {
        try
        {
            // call the despawn method of the parent object
            die.Despawn();
        }
        catch (System.Exception e)
        {
            die = transform.parent.GetComponent<Die>();
            die.Despawn();
            Debug.Log(e);
        }
    }
}
