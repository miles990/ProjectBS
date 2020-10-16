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
            Debug.Log("Added Character");
        }

        if(Input.GetKeyDown(KeyCode.Q))
        {
            ProjectBS.PlayerManager.Instance.Player.Equipments.Add(ProjectBS.EquipmentUtility.CreateNewEquipment(1));
            Debug.Log("Added Equipment 1");
        }

        if (Input.GetKeyDown(KeyCode.W))
        {
            ProjectBS.PlayerManager.Instance.Player.Equipments.Add(ProjectBS.EquipmentUtility.CreateNewEquipment(2));
            Debug.Log("Added Equipment 2");
        }

        if (Input.GetKeyDown(KeyCode.E))
        {
            ProjectBS.PlayerManager.Instance.Player.Equipments.Add(ProjectBS.EquipmentUtility.CreateNewEquipment(3));
            Debug.Log("Added Equipment 3");
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            ProjectBS.PlayerManager.Instance.Player.Equipments.Add(ProjectBS.EquipmentUtility.CreateNewEquipment(4));
            Debug.Log("Added Equipment 4");
        }

        if(Input.GetKeyDown(KeyCode.A))
        {
            ProjectBS.PlayerManager.Instance.AddSkill(1);
            Debug.Log("Added Skill 1");
        }
    }
}
