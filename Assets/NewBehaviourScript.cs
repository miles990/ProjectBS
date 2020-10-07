using System.Collections.Generic;
using UnityEngine;

public class NewBehaviourScript : MonoBehaviour
{
    // Start is called before the first frame update
    private void Start()
    {
        ProjectBS.GameManager.Instance.StartGame();
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Space))
        {
            ProjectBS.PlayerManager.Instance.Player.Characters.Add(ProjectBS.CharacterUtility.CreateNewCharacter());
            Debug.Log(ProjectBS.PlayerManager.Instance.Player.Characters.Count);
        }
    }
}
