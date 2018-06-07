﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Enemy6DamageDetection : MonoBehaviour {

    public float life;
    private float maxLife;
    public float multiplier;

    public Image healthBar;


    public float worth = GameStats.moneyForBoss;

    public static float damage;


    public bool sendInfo = false;

    public bool isChecked = false;
    private bool isAlive = true;



    void Start () {
        worth = GameStats.moneyForBoss;

        //life = GameStats.Enemy2Life;

        //life = GameStats.allEnemyStartLife;

        //multiplier = GameStats.Enemy1LifeMultiplier;

        life = Mathf.Round(GameStats.allEnemyLastLife * GameStats.allEnemyLifeMultiplier) * 10;

        maxLife = life;

        GameStats.actualLife = life;

        Debug.Log("boss worth: " + worth);

    }
	

	void Update () {
        if (sendInfo && MenuButtons.UIStatsEnemyEnabled)
        {
            MenuButtons.UIStatsEnabled = true;
            MenuButtons.UpdateUIStatsInfo(GameStats.Enemy6AvatarStatic, GameStats.Enemy6Name, "", "", "", "Life: " + life + "/" + maxLife, "", "Speed: " + gameObject.GetComponent<Enemy6Movement>().enemySpeed.ToString(), "", "Worth: " + worth + "$", "", "", "", "", "");
        }
        else
        {
            sendInfo = false;
        }

        if (!MenuButtons.UIStatsEnemyEnabled)
        {
            isChecked = false;
        }
    }


    private bool spawnedAOE = false;
    private bool spawnedAOEFreeze = false;


    private void OnTriggerStay(Collider other)
    {
        if (other.tag == "AOEBOOM")
        {
            spawnedAOE = false;
        }
        if (other.tag == "AOEFREEZE")
        {
            spawnedAOEFreeze = false;
        }

    }



    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "checkingGO")
        {
            sendInfo = true;
            MenuButtons.UIStatsEnemyEnabled = true;
            isChecked = true;
        }
        else
        {

        }

        if (other.tag == "AOEBOOM")
        {
            if (!spawnedAOE)
            {
                float damageAOE = int.Parse(other.name);


                Vector3 AOEBOOMCenter = other.GetComponent<Renderer>().bounds.center;

                Vector3 distanceToBOOMCenter = transform.position - AOEBOOMCenter;

                float damageScaleX = Mathf.Abs(distanceToBOOMCenter.x / GameStats.Tower2AOEScaleSize);
                float damageScaleZ = Mathf.Abs(distanceToBOOMCenter.z / GameStats.Tower2AOEScaleSize);

                float damageScale = Mathf.Pow(damageScaleX, 2) + Mathf.Pow(damageScaleZ, 2);
                damageScale = Mathf.Sqrt(damageScale);

                damageScale = 1 - damageScale;

                //Debug.Log("vektor: " + distanceToBOOMCenter);
                //Debug.Log("damageScaleX: " + damageScaleX);
                //Debug.Log("damageScaleZ: " + damageScaleZ);

                //Debug.Log("mnoznik: " + damageScale);

                life -= Mathf.Round(damageAOE * damageScale);
                //Debug.Log("odjeto: " + Mathf.Round(damageAOE * damageScale));
                spawnedAOE = true;
            }
            else
            {

            }

        }


        if (other.tag == "AOEFREEZE")
        {
            if (!spawnedAOEFreeze)
            {
                //Debug.Log("freezing");
                StartCoroutine(SlowSpeed());

                float damageAOEFreeze = int.Parse(other.name);


                Vector3 AOEFreezeCenter = other.GetComponent<Renderer>().bounds.center;

                Vector3 distanceToFreezeCenter = transform.position - AOEFreezeCenter;

                float damageScaleX = Mathf.Abs(distanceToFreezeCenter.x / GameStats.Tower3AOEFreezeScaleSize);
                float damageScaleZ = Mathf.Abs(distanceToFreezeCenter.z / GameStats.Tower3AOEFreezeScaleSize);

                float damageScale = Mathf.Pow(damageScaleX, 2) + Mathf.Pow(damageScaleZ, 2);
                damageScale = Mathf.Sqrt(damageScale);

                damageScale = 1 - damageScale;

                //Debug.Log("vektor: " + distanceToBOOMCenter);
                //Debug.Log("damageScaleX: " + damageScaleX);
                //Debug.Log("damageScaleZ: " + damageScaleZ);

                //Debug.Log("mnoznik: " + damageScale);

                life -= Mathf.Round(damageAOEFreeze * damageScale);
                //Debug.Log("odjeto: " + Mathf.Round(damageAOE * damageScale));
                spawnedAOEFreeze = true;
            }
            else
            {

            }
        }


        if (other.tag == "Tower1Missile")
        {
            //damage = Tower1.Tower1DamageStatic;
            damage = int.Parse(other.name);
            //Debug.Log(damage);

            life -= damage;
            Destroy(other.gameObject);
        }
        else if (other.tag == "Tower2Missile")
        {
            damage = int.Parse(other.name);
            life -= damage;
            Destroy(other.gameObject);

            GameObject AoEMissile = Instantiate(PublicGameobjects.AoEDamageGOStatic, transform.position, transform.rotation);
            AoEMissile.name = Mathf.Round((GameStats.Tower2AOEDamageMultiplier * damage)).ToString();
            AoEMissile.transform.localScale = GameStats.Tower2AOEScale;
            spawnedAOE = true;
            Destroy(AoEMissile, 0.1f);
        }
        else if (other.tag == "Tower3Missile")
        {
            damage = int.Parse(other.name);
            StartCoroutine(SlowSpeed());

            GameObject AoEFreeze = Instantiate(PublicGameobjects.AoEFreezeGOStatic, transform.position, transform.rotation);
            AoEFreeze.name = Mathf.Round((GameStats.Tower3AOEFreezeDamageMultiplier * damage)).ToString();
            AoEFreeze.transform.localScale = GameStats.Tower3AOESFreezecale;
            spawnedAOEFreeze = true;
            Destroy(AoEFreeze, 0.1f);


            life -= damage;
            Destroy(other.gameObject);
        }
        else if (other.tag == "Tower4Missile")
        {
            damage = int.Parse(other.name);
            life -= damage;
            Destroy(other.gameObject);
        }

        healthBar.fillAmount = life / maxLife;
        CheckIfAlive();

    }


    IEnumerator SlowSpeed()
    {
        float lastSpeed = gameObject.GetComponent<Enemy6Movement>().enemySpeed;
        if (gameObject.GetComponent<Enemy6Movement>().enemySpeed == GameStats.Enemy6Speed)
        {
            float newSpeed = gameObject.GetComponent<Enemy6Movement>().enemySpeed *= GameStats.Tower3AOEFreezeSpeedScale;
            Mathf.Round(newSpeed);
            gameObject.GetComponent<Enemy6Movement>().enemySpeed = newSpeed;
        }
        else
        {
            //Debug.Log("juz spowolniony");
        }
        yield return new WaitForSeconds(GameStats.Tower3AOEFreezeTime);
        gameObject.GetComponent<Enemy6Movement>().enemySpeed = lastSpeed;

    }



    void CheckIfAlive()
    {
        if (life <= 0)
        {
            sendInfo = false;

            /*
            if (MenuButtons.UIStatsEnemyEnabled)
            {
                MenuButtons.UIStatsEnabled = false;
                
            }
            MenuButtons.UIStatsEnemyEnabled = false;
            */

            if (!MenuButtons.UIStatsTowerEnabled && isChecked)
            {
                MenuButtons.UpdateUIStatsInfo(GameStats.emptyAvatarStatic, "", "", "", "", "", "", "", "", "", "", "", "", "", "");
                MenuButtons.UIStatsEnabled = false;
            }

            Destroy(transform.gameObject);


            if (isAlive)
            {
                GameStats.money += worth;
                isAlive = false;
            }




            //TODO show animated text + x $$
            AddDollarsAnimation();

        }
        else
        {

        }
    }

    private GameObject Dollars;
    private TextMesh DollarsText;

    public void AddDollarsAnimation()
    {
        Vector3 pos = new Vector3(transform.position.x, transform.position.y + 3f, transform.position.z);
        DollarsText = PublicGameobjects.DollarsIncomeGOStatic.transform.GetChild(0).GetComponent<TextMesh>();
        DollarsText.text = "+" + worth + "$";
        Dollars = Instantiate(PublicGameobjects.DollarsIncomeGOStatic, pos, Quaternion.identity);



        Destroy(Dollars, GameStats.dollarsShowTime);

    }

    public static void CheckDamage(float dmg)
    {
        damage = dmg;
    }

}
