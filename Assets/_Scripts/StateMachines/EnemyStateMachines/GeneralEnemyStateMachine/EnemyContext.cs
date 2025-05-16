using System.Collections.Generic;
using UnityEngine.AI;
using UnityEngine;
public class EnemyContext : EnemyAISMContext
{
    public EnemyContext(EnemyStateMachine enemyStateMachine, NavMeshAgent agent, Transform transform, LayerMask groundLayers, float walkSpeed, float runSpeed, HealthForRagdolls health, EnemyWeapon primaryWeapon, EnemyShield shield)
    {
        EnemyStateMachine = enemyStateMachine;
        Agent = agent;
        Transform = transform;
        //GroundLayers = groundLayers;
        Radius = Agent.radius * enemyStateMachine.transform.lossyScale.x;
        AttackRange = Radius * 4;
        BlockRange = Radius * 8;
        GroundedOffset = -(Agent.height/2 - GroundCheckRadius * 2);
        //WalkSpeed = walkSpeed;
        //RunSpeed = runSpeed;
        Health = health;
        PrimaryWeapon = primaryWeapon;
        StoppingDistance = agent.stoppingDistance;
        Shield = shield;
        AttackCooldown = MinTimeBetweenAttacks;
    }


    public EnemyStateMachine EnemyStateMachine { get; private set; }
    public NavMeshAgent Agent { get; private set; }
    public List<GameObject> NearPlayers { get; set; } = new List<GameObject>(); 
    public AiSensor AiSensor { get; private set; }
    public HealthForRagdolls Health { get; private set; }
    public LayerMask WhatIsPlayer { get; set; }
    public float VerticalVelocity { get; set; }
    public float WalkPointRange { get; private set; } = 10f;
    public float AttackRange { get; private set; }
    public float BlockRange { get; private set; }   
    public EnemyWeapon PrimaryWeapon { get; private set; }
    public float MaxTimeBetweenAttacks { get; set; } = 3f;
    public float MinTimeBetweenAttacks { get; set; } = .5f;
    public float AttackCooldown { get; set; }
    public bool LookAround {get; set; }
    public float Radius { get; set; }

    public EnemyShield Shield { get; set; }
    public bool ChargeAttack { get; set; }
    public bool LongRangeAttack { get; set; }
    public bool IsDamaged {  get; set; }

    //Current Target Player

    public PlayerContext ClosestPlayerContext { get; set; }
    public Transform ClosestPlayer { get; set; }
    public Vector3 LastPlayerPos { get; set; } 
    public Vector3 suspectedPos { get; set; }   
}
