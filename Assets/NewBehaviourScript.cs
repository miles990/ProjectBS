using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewBehaviourScript : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        ProjectBS.GameDataLoader.StartLoad();
        ProjectBS.PlayerManager.Instance.Init();
    }
}
