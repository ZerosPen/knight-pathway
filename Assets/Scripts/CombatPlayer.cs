using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class CombatPlayer : MonoBehaviour
{
    [SerializeField] private float attackcooldown;
    [SerializeField] private int ATK;
    private Animator anim;
    private Player playerMovement;
    private float cooldownTimer =  Mathf.Infinity;
    public Transform attackPoint;
    public float attackRange = 0.5f;
    [SerializeField] private LayerMask Enemy;


    private void Awake()
    {
        anim = GetComponent<Animator>();
        playerMovement = GetComponent<Player>();
    }

    private void Update()
    {
       if (Input.GetKeyDown(KeyCode.J) && cooldownTimer > attackcooldown && playerMovement.canAttack())
            Attack();

        cooldownTimer += Time.deltaTime;
    }

    private void Attack()
    {
        anim.SetTrigger("Attack");
        //hit enemy
        Collider2D[] hitenemys = Physics2D.OverlapCircleAll(attackPoint.position, attackRange, Enemy);
        // DMG
        foreach (Collider2D enemy in hitenemys)
        {
            enemy.GetComponent<EnemyMove>().takingDamage(ATK);
        }
        cooldownTimer = 0;
    }

    private void OnDrawGizmosSelected()
    {
        if (attackPoint == null)
            return;
        Gizmos.DrawSphere(attackPoint.position, attackRange);
    }

}
