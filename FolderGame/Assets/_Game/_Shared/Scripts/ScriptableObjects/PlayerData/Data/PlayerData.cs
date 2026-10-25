using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[CreateAssetMenu(fileName = "PlayerData", menuName = "Player/BaseData")]
public class PlayerData : ScriptableObject
{
    [Header("Health")]
    public float health = 100;
    
    [Header("Speed")] 
    public float speed = 4f;
    public float speedMultiplier = 1f;
    
    [Header("Sprint")]
    public float sprintSpeed = 8f;
    public float sprintMultiplier = 1f;
    
    [Header("Crouch")]
    public float crouchSpeed = 2f;
    public float crouchMultiplier = 1f;
    
    [Header("Jump")]
    public float jumpForce = 5f;
    public int jumpCount = 1;
    
    [Header("Knockback")]
    public float invulnerabilityTime = 1f;
    public float knockbackMultiplier = 1f;
}