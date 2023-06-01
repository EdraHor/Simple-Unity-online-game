using Network;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private float _speed = 5;

    public void StartLocaleInputs() => StartCoroutine(UpdateInputs());

    IEnumerator UpdateInputs()
    {
        while (true)
        {
            Vector3 translate = new Vector3(Input.GetAxis("Horizontal"), 0,
                                            Input.GetAxis("Vertical")) * _speed;

            transform.Translate(translate);
            yield return new WaitForFixedUpdate();
        }
    }
}
