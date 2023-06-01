using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using Network;
using System.Linq;

[RequireComponent(typeof(InputButton))]
public class InputButton : MonoBehaviour
{
    private Button _button;
    private NetworkObject controlObj;
    public Vector3 MoveDirection;
    void Start()
    {
#if !PLATFORM_ANDROID
        gameObject.SetActive(false);
#endif
        _button = GetComponent<Button>();
        _button.onClick.AddListener(() => 
        {
            if (controlObj==null) TryFindObj();
            controlObj.transform.position += MoveDirection;
        });
    }

    private void TryFindObj()
    {
        var objs = FindObjectsOfType<NetworkObject>();
        controlObj = objs.FirstOrDefault(o => o.IsLocal);
    }

}
