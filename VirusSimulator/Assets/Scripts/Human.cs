using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//DGSpitzer 2020
public class Human : MonoBehaviour
{
    [HideInInspector]
    public GameManager gameManager;
    //0 = student; 1 = Adult; 2 = Elder
    [HideInInspector]
    public int age = -1;
    //0 = Normal; 1 = Incubation; 2 = Mild; 3 = Critical; 4 = Dead; 5 = Cured
    [HideInInspector]
    public int infectedType = 0;
    [HideInInspector]
    public int infectedPeopleNum = 0;
    Vector3 currentDirection = Vector3.zero;
    float moveTimer = 1;
    float moveTimerTotal = 1;

    [SerializeField]
    GameObject[] gO_infectedType;

    [HideInInspector]
    public bool onlyMild = true;
    [HideInInspector]
    public bool isSurvived = true;
    [HideInInspector]
    public bool isQuarantined = false;
    [HideInInspector]
    public bool useTempBed = false;
    [HideInInspector]
    public float timeStamp_Incubation = 0;
    [HideInInspector]
    public float timeStamp_Mild = 0;
    [HideInInspector]
    public float timeStamp_Critical = 0;

    public void ChangeInfectedType(int type)
    {
        if(type != infectedType)
        {
            gO_infectedType[infectedType].SetActive(false);
            infectedType = type;
            gO_infectedType[infectedType].SetActive(true);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        if(moveTimer > moveTimerTotal)
        {
            moveTimer = 0;
            moveTimerTotal = Random.value * 3 + 0.3f;

            float dx = 0;
            float dy = 0;
            //if(transform.position.x > 40)
            //{
            //    dx = -Random.value * 2.5f;
            //}
            //else if (transform.position.x < -40)
            //{
            //    dx = Random.value * 2.5f;
            //}
            //else
            //{
            //    dx = Random.value * 5 - 2.5f;
            //}

            //if (transform.position.z > 20)
            //{
            //    dy = -Random.value * 2.5f;
            //}
            //else if (transform.position.z < -20)
            //{
            //    dy = Random.value * 2.5f;
            //}
            //else
            //{
            //    dy = Random.value * 5 - 2.5f;
            //}
            if (transform.position.magnitude > 20)
            {
                Vector3 normalPos = -transform.position.normalized;
                dx = normalPos.x * (Random.value + 0.5f) * 2.5f;
                dy = normalPos.z * (Random.value + 0.5f) * 2.5f;
            }
            else
            {
                dx = Random.value * 5 - 2.5f;
                dy = Random.value * 5 - 2.5f;
            }

            //dx /= 3;
            //dy /= 3;
            dx /= gameManager.dayLength;
            dy /= gameManager.dayLength;

            if(age == 1)
            {
                dx *= 2f;
                dy *= 2f;
            }
            else if(age == 2)
            {
                dx *= 0.5f;
                dy *= 0.5f;
            }
            if(infectedType == 2)
            {
                dx *= 0.8f;
                dy *= 0.8f;
            }
            else if (infectedType == 3)
            {
                dx *= 0.2f;
                dy *= 0.2f;
            }

            if (gameManager.rule4_MildAtHome && gameManager.population_Mild2.Contains(this))
            {
                if (gameManager.rule5_Independence)
                {
                    if(Random.value < 0.01f)
                    {
                        dx *= 2f;
                        dy *= 2f;
                    }
                    else
                    {
                        dx *= 0.5f;
                        dy *= 0.5f;
                    }
                }
                else 
                {
                    dx = 0;
                    dy = 0;
                }
            }
            if ((gameManager.rule0_School && age == 0) || (gameManager.rule1_WFH && age == 1) || (gameManager.rule2_Elder && age == 2))
            {
                if (gameManager.rule5_Independence)
                {
                    if (Random.value < 0.01f)
                    {
                        dx *= 2f;
                        dy *= 2f;
                    }
                    else
                    {
                        dx *= 0.5f;
                        dy *= 0.5f;
                    }
                }
                else
                {
                    dx = 0;
                    dy = 0;
                }
            }
            currentDirection = new Vector3(dx, 0, dy);
        }
        else
        {
            moveTimer += Time.deltaTime;
        }

        if (infectedType != 4)
        {
            transform.Translate(currentDirection * Time.deltaTime);
            if(transform.position.magnitude >= 30)
            {
                Vector2 pos2D = Random.insideUnitCircle * 20;
                transform.position = new Vector3(pos2D.x, 0, pos2D.y);
            }
        }
    }

    //*EddyV*
    //void OnTriggerEnter(Collider other)
    //{
    //    if (other.CompareTag("Human"))
    //    {
    //        //Infected

    //    }
    //}
}
