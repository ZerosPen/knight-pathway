using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMove : MonoBehaviour
{

    public int maxHealth = 100;
    int currentHealt;
    [SerializeField]private Animator anim; 

    // Start is called before the first frame update
    void Start()
    {
        currentHealt = maxHealth;
    }

    public void takingDamage(int dmg)
    {
        currentHealt -= dmg;

        anim.SetTrigger("Hurt");

        if (currentHealt <= 0)
        {
            EnemyDefeat();
        }
    }

    void EnemyDefeat()
    {
        Debug.Log("Enemy Die");
        anim.SetBool("IsDead", true);
        GetComponent<Collider2D>().enabled = false;
        this.enabled = false;
    }

}
