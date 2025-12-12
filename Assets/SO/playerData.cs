using UnityEngine;

[CreateAssetMenu(fileName = "playerData", menuName = "Scriptable Objects/playerData")]
public class playerData : ScriptableObject
{
    [Header("Health")]
    
    public int maxHealth = 100;

    [Header("Movement")]
    public float walkSpeed = 5.0f;
    public float sprintSpeed = 8.0f;
    public float crouchSpeed = 2.5f;
    public float rotationSpeed = 10.0f;
   
    public float standingHeight = 2.0f; 

    [Header("Stamina")]
    public float maxStamina = 100f;
    public float sprintDrainRate = 10f; 
    public float recoveryRate = 5f;
}
