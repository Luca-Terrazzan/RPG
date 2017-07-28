using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ChangeScene : MonoBehaviour
{
    public enum levels { Fattoria, Becchino, Prigione, Bordello, Corridoio, Saloon, Cimitero, duplè,ChangeScene };
    public levels GoToScene;


    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.name == "Player")
            SceneManager.LoadScene(GoToScene.ToString());
    }
}
