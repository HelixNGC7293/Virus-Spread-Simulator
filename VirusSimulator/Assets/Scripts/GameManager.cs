using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//DGSpitzer 2020
public class GameManager : MonoBehaviour
{
    //你好呀！
    [SerializeField]
    Human humanPrefab;
    [SerializeField]
    Transform humanGroup;
    List<Human> population = new List<Human>();
    List<Human> population_Normal0 = new List<Human>();
    List<Human> population_Incubation1 = new List<Human>();
    public List<Human> population_Mild2 = new List<Human>();
    List<Human> population_Critical3 = new List<Human>();
    List<Human> population_Dead4 = new List<Human>();
    List<Human> population_Cured5 = new List<Human>();

    [SerializeField]
    InputField Input_Population;
    [SerializeField]
    InputField Input_R0;
    [SerializeField]
    InputField Input_DayLength;
    [SerializeField]
    InputField Input_InitInfected;
    [SerializeField]
    InputField Input_PStudent;
    [SerializeField]
    InputField Input_PAdult;
    [SerializeField]
    InputField Input_PElder;
    [SerializeField]
    InputField Input_PStudent_DeathRate;
    [SerializeField]
    InputField Input_PAdult_DeathRate;
    [SerializeField]
    InputField Input_PElder_DeathRate;
    [SerializeField]
    InputField Input_Bed;
    [SerializeField]
    InputField Input_AddInfected;
    [SerializeField]
    InputField Input_Critical;
    [SerializeField]
    Text text_Day;
    [SerializeField]
    Text[] text_DataGroup;
    [SerializeField]
    Text text_FatalityRate;
    [SerializeField]
    Text text_FatalityRate_Student;
    [SerializeField]
    Text text_FatalityRate_Adult;
    [SerializeField]
    Text text_FatalityRate_Elder;
    [SerializeField]
    Text text_EconomyLost;
    [SerializeField]
    Text text_Panic;
    [SerializeField]
    Text text_Bed;
    [SerializeField]
    Toggle[] toggles;

    [SerializeField]
    GameObject initUI;
    [SerializeField]
    GameObject simulationUI;

    [SerializeField]
    AnimationCurve curve_IncubationDay;
    [SerializeField]
    AnimationCurve curve_MildDay;
    [SerializeField]
    AnimationCurve curve_CriticalDay;
    [SerializeField]
    AnimationCurve curve_DeathDay;
    //[SerializeField]
    //AnimationCurve curve_IncubationDay;

    int num_Population;
    int num_Bed = 100;
    float num_R0;

    public float dayTime = 0;
    public float dayLength = 3;

    float infectionRadius = 0.3f;

    bool isStarted = false;

    float ageRate_Student = 0;
    float ageRate_Adult = 0;
    float ageRate_Elder = 0;

    int ageNum_Student = 0;
    int ageNum_Adult = 0;
    int ageNum_Elder = 0;

    int ageNum_Student_Death = 0;
    int ageNum_Adult_Death = 0;
    int ageNum_Elder_Death = 0;

    public float criticalRate = 0.2f;
    public float ageFatality_Student = 0.002f;
    public float ageFatality_Adult = 0.003f;
    public float ageFatality_Elder = 0.056f;

    float economyLost = 0;
    float panic = 0;
    int currentBed = 0;
    int currentBedTotal = 100;

    [HideInInspector]
    public bool rule0_School = false;
    [HideInInspector]
    public bool rule1_WFH = false;
    [HideInInspector]
    public bool rule2_Elder = false;
    [HideInInspector]
    public bool rule3_MildRefuse = false;
    [HideInInspector]
    public bool rule4_MildAtHome = false;
    [HideInInspector]
    public bool rule5_Independence = false;
    [HideInInspector]
    public bool rule6_TempHospital = false;

    [SerializeField]
    UILineRenderer lR_Incubation1;
    [SerializeField]
    UILineRenderer lR_Mild2;
    [SerializeField]
    UILineRenderer lR_Critical3;
    [SerializeField]
    UILineRenderer lR_Dead4;
    [SerializeField]
    UILineRenderer lR_Cured5;

    float lR_MaxHeight = 350;
    float lR_MaxWidth = 450;

    float lRTimer = 0;

    [SerializeField]
    GameObject[] cameras;

    // Start is called before the first frame update
    void Start()
    {
        initUI.SetActive(true);
        simulationUI.SetActive(false);
    }

    public void RuleChanged()
    {
        rule0_School = toggles[0].isOn;
        rule1_WFH = toggles[1].isOn;
        rule2_Elder = toggles[2].isOn;
        rule3_MildRefuse = toggles[3].isOn;
        rule4_MildAtHome = toggles[4].isOn;
        rule5_Independence = toggles[5].isOn;
        rule6_TempHospital = toggles[6].isOn;

        CalculatePanic();
    }

    void CalculatePanic()
    {
        float p = 0;
        if (rule0_School)
        {
            p += 10;
        }
        if (rule1_WFH)
        {
            p += 10;
        }
        if (rule2_Elder)
        {
            p += 10;
        }
        if (rule3_MildRefuse)
        {
            p += 10;
        }
        if (rule4_MildAtHome)
        {
            p += 10;
        }
        if (rule5_Independence)
        {
            p += 10;
        }
        if (rule6_TempHospital)
        {
            p += 10;
        }
        panic += p;
    }

    //Controlled by button
    public void StartSimulation()
    {
        num_Population = Mathf.Min(1000, int.Parse(Input_Population.text));
        num_R0 = float.Parse(Input_R0.text);
        currentBed = 0;
        currentBedTotal = int.Parse(Input_Bed.text);
        int initInfected = int.Parse(Input_InitInfected.text);
        dayLength = Mathf.Max(0.5f, float.Parse(Input_DayLength.text));
        lRTimer = dayLength;
        infectionRadius = 0.2f + 0.02f * 3f / dayLength + Mathf.Max(0, (num_R0 - 3.77f) * 0.01f);
        criticalRate = float.Parse(Input_Critical.text) * 0.01f;
        //dayLength = 3;
        //Time.timeScale = float.Parse(Input_DayLength.text);
        dayTime = 0;
        economyLost = 0;
        ageNum_Student = 0;
        ageNum_Adult = 0;
        ageNum_Elder = 0;

        ageNum_Student_Death = 0;
        ageNum_Adult_Death = 0;
        ageNum_Elder_Death = 0;

        panic = 0;
        CalculatePanic();

        lR_Incubation1.Points.Clear();
        lR_Incubation1.Points.Add(Vector2.zero);
        lR_Incubation1.Points.Add(Vector2.zero);
        lR_Mild2.Points.Clear();
        lR_Mild2.Points.Add(Vector2.zero);
        lR_Mild2.Points.Add(Vector2.zero);
        lR_Critical3.Points.Clear();
        lR_Critical3.Points.Add(Vector2.zero);
        lR_Critical3.Points.Add(Vector2.zero);
        lR_Dead4.Points.Clear();
        lR_Dead4.Points.Add(Vector2.zero);
        lR_Dead4.Points.Add(Vector2.zero);
        lR_Cured5.Points.Clear();
        lR_Cured5.Points.Add(Vector2.zero);
        lR_Cured5.Points.Add(Vector2.zero);

        float pS = float.Parse(Input_PStudent.text);
        float pA = float.Parse(Input_PAdult.text);
        float pE = float.Parse(Input_PElder.text);
        float pT = pS + pA + pE;
        ageRate_Student = pS / pT;
        ageRate_Adult = pA / pT;
        ageRate_Elder = pE / pT;
        ageFatality_Student = float.Parse(Input_PStudent_DeathRate.text) * 0.01f;
        ageFatality_Adult = float.Parse(Input_PAdult_DeathRate.text) * 0.01f;
        ageFatality_Elder = float.Parse(Input_PElder_DeathRate.text) * 0.01f;

        foreach (Human human in population)
        {
            Destroy(human.gameObject);
        }
        population.Clear();
        population_Normal0.Clear();
        population_Incubation1.Clear();
        population_Mild2.Clear();
        population_Critical3.Clear();
        population_Dead4.Clear();
        population_Cured5.Clear();

        //Restart
        if (num_Population > 0 && num_R0 > 0 && dayLength > 0 && initInfected >= 0 && criticalRate >= 0 && currentBedTotal > 0 && ageFatality_Student >= 0 && ageFatality_Adult >= 0 && ageFatality_Elder >= 0)
        {
            isStarted = true;
            initUI.SetActive(false);
            simulationUI.SetActive(true);
            for (int i = 0; i < num_Population; i++)
            {
                Human human = Instantiate(humanPrefab, humanGroup);
                //float r = Random.value * 40 - 20;
                //float angle = Random.value * 360;
                //Vector3 pos = new Vector3(r * Mathf.Cos(angle), 0, r * Mathf.Sin(angle));
                //human.transform.position = pos;
                Vector2 pos2D = Random.insideUnitCircle * 20;
                human.transform.position = new Vector3(pos2D.x, 0, pos2D.y);
                human.gameManager = this;
                //float ranAge = Random.value;
                //float ranSurvive = Random.value;
                //if (ranAge <= ageRate_Student)
                //{
                //    human.age = 0;
                //    ageNum_Student++;
                //    if (ranSurvive <= ageFatality_Student)
                //    {
                //        human.isSurvived = false;
                //    }
                //}
                //else if (ranAge <= ageRate_Student + ageRate_Adult)
                //{
                //    human.age = 1;
                //    ageNum_Adult++;
                //    if (ranSurvive <= ageFatality_Adult)
                //    {
                //        human.isSurvived = false;
                //    }
                //}
                //else
                //{
                //    human.age = 2;
                //    ageNum_Elder++;
                //    if (ranSurvive <= ageFatality_Elder)
                //    {
                //        human.isSurvived = false;
                //    }
                //}
                //if (ranSurvive <= criticalRate)
                //{
                //    human.onlyMild = false;
                //}

                //human.transform.position = new Vector3(Random.value * 50 - 25, 0, Random.value * 50 - 25);
                population.Add(human);
            }

            List<Human> tempPopulation = new List<Human>();
            List<Human> tempPopulation_Student = new List<Human>();
            List<Human> tempPopulation_Adult = new List<Human>();
            List<Human> tempPopulation_Elder = new List<Human>();
            tempPopulation.AddRange(population);
            int criticalNum = Mathf.RoundToInt(num_Population * criticalRate);
            int currentCriticalNum = 0;
            int ageNum = Mathf.RoundToInt(num_Population * ageRate_Student);
            int ageNum_Fatality = Mathf.RoundToInt(population.Count * ageFatality_Student);
            currentCriticalNum += ageNum_Fatality;
            for (int i = 0; i < ageNum; i++)
            {
                if (tempPopulation.Count > 0)
                {
                    Human human = tempPopulation[Random.Range(0, tempPopulation.Count)];
                    if (human.age == -1)
                    {
                        human.age = 0;
                        ageNum_Student++;
                        tempPopulation_Student.Add(human);
                        tempPopulation.Remove(human);
                    }
                }
                else
                {
                    break;
                }
            }
            for (int i = 0; i < ageNum_Fatality; i++)
            {
                if (tempPopulation_Student.Count > 0)
                {
                    Human human = tempPopulation_Student[Random.Range(0, tempPopulation_Student.Count)];
                    tempPopulation_Student.Remove(human);
                    human.isSurvived = false;
                    human.onlyMild = false;
                }
                else
                {
                    break;
                }
            }

            ageNum = Mathf.RoundToInt((float)population.Count * ageRate_Adult);
            ageNum_Fatality = Mathf.RoundToInt((float)population.Count * ageFatality_Adult);
            currentCriticalNum += ageNum_Fatality;
            for (int i = 0; i < ageNum; i++)
            {
                if (tempPopulation.Count > 0)
                {
                    Human human = tempPopulation[Random.Range(0, tempPopulation.Count)];
                    if (human.age == -1)
                    {
                        human.age = 1;
                        ageNum_Adult++;
                        tempPopulation_Adult.Add(human);
                        tempPopulation.Remove(human);
                    }
                }
                else
                {
                    break;
                }
            }
            for (int i = 0; i < ageNum_Fatality; i++)
            {
                if (tempPopulation_Adult.Count > 0)
                {
                    Human human = tempPopulation_Adult[Random.Range(0, tempPopulation_Adult.Count)];
                    tempPopulation_Adult.Remove(human);
                    human.isSurvived = false;
                    human.onlyMild = false;
                }
                else
                {
                    break;
                }
            }

            //ageNum = Mathf.RoundToInt((float)population.Count * ageRate_Elder);
            ageNum = tempPopulation.Count;
            ageNum_Fatality = Mathf.RoundToInt((float)population.Count * ageFatality_Elder);
            currentCriticalNum += ageNum_Fatality;
            for (int i = 0; i < ageNum; i++)
            {
                if (tempPopulation.Count > 0)
                {
                    Human human = tempPopulation[Random.Range(0, tempPopulation.Count)];
                    if (human.age == -1)
                    {
                        human.age = 2;
                        ageNum_Elder++;
                        tempPopulation_Elder.Add(human);
                        tempPopulation.Remove(human);
                    }
                }
                else
                {
                    break;
                }
            }
            for (int i = 0; i < ageNum_Fatality; i++)
            {
                if (tempPopulation_Elder.Count > 0)
                {
                    Human human = tempPopulation_Elder[Random.Range(0, tempPopulation_Elder.Count)];
                    tempPopulation_Elder.Remove(human);
                    human.isSurvived = false;
                    human.onlyMild = false;
                }
                else
                {
                    break;
                }
            }
            //Random Critical
            //tempPopulation.AddRange(tempPopulation_Student);
            tempPopulation.AddRange(tempPopulation_Adult);
            tempPopulation.AddRange(tempPopulation_Elder);
            int moreCriticalNum = criticalNum - currentCriticalNum;
            if (moreCriticalNum > 0)
            {
                for(int i = 0; i < moreCriticalNum; i++)
                {
                    if (tempPopulation.Count > 0)
                    {
                        Human human = tempPopulation[Random.Range(0, tempPopulation.Count)];
                        tempPopulation.Remove(human);
                        human.onlyMild = false;
                    }
                    else
                    {
                        break;
                    }
                }
            }

            //Random Infection
            population_Normal0.AddRange(population);
            for (int i = 0; i < initInfected; i++)
            {
                Human human = population_Normal0[Random.Range(0, population_Normal0.Count)];
                Infected(human);
                if (population_Normal0.Count <= 0)
                {
                    break;
                }
            }
        }
        else
        {
            isStarted = false;
            initUI.SetActive(true);
            simulationUI.SetActive(false);
        }
    }

    public void EndSimulation()
    {
        currentBed = 0;
        dayTime = 0;
        economyLost = 0;

        panic = 0;


        foreach (Human human in population)
        {
            Destroy(human.gameObject);
        }
        population.Clear();
        population_Normal0.Clear();
        population_Incubation1.Clear();
        population_Mild2.Clear();
        population_Critical3.Clear();
        population_Dead4.Clear();
        population_Cured5.Clear();

        isStarted = false;
        initUI.SetActive(true);
        simulationUI.SetActive(false);
    }

    public void AddInfected()
    {
        int num_Add = Mathf.Max(0, int.Parse(Input_AddInfected.text));
        for (int i = 0; i < num_Add; i++)
        {
            Human human = Instantiate(humanPrefab, humanGroup);
            float r = Random.value * 40 - 20;
            float angle = Random.value * 360;
            Vector3 pos = new Vector3(r * Mathf.Cos(angle), 0, r * Mathf.Sin(angle));
            human.transform.position = pos;
            human.gameManager = this;
            float ranAge = Random.value;
            float ranSurvive = Random.value;
            if (ranAge <= ageRate_Student)
            {
                human.age = 0;
                ageNum_Student++;
                if (ranSurvive <= ageFatality_Student)
                {
                    human.isSurvived = false;
                }
            }
            else if (ranAge <= ageRate_Student + ageRate_Adult)
            {
                human.age = 1;
                ageNum_Adult++;
                if (ranSurvive <= ageFatality_Adult)
                {
                    human.isSurvived = false;
                }
            }
            else
            {
                human.age = 2;
                ageNum_Elder++;
                if (ranSurvive <= ageFatality_Elder)
                {
                    human.isSurvived = false;
                }
            }

            if (ranSurvive <= criticalRate)
            {
                human.onlyMild = false;
            }
            //human.transform.position = new Vector3(Random.value * 50 - 25, 0, Random.value * 50 - 25);
            population.Add(human);
            Infected(human);
        }
    }

    void Infected(Human human)
    {
        human.ChangeInfectedType(1);
        population_Incubation1.Add(human);
        human.timeStamp_Incubation = dayTime + curve_IncubationDay.Evaluate(Random.value) * dayLength;
        if (population_Normal0.Contains(human))
        {
            population_Normal0.Remove(human);
        }
    }

    void TryQuarantined(Human human)
    {
        if(currentBed < currentBedTotal)
        {
            human.isQuarantined = true;
            human.gameObject.SetActive(false);
            currentBed++;
        }
        else if (rule6_TempHospital)
        {
            human.isQuarantined = true;
            human.useTempBed = true;
            human.gameObject.SetActive(false);
        }
    }
    void EndQuarantine(Human human)
    {
        human.isQuarantined = false;
        human.gameObject.SetActive(true);
        if(!human.useTempBed) currentBed--;
    }

    // Update is called once per frame
    void Update()
    {
        if (isStarted)
        {
            //Line Renderer
            if(lRTimer >= dayLength)
            {
                lRTimer = Time.deltaTime;
                lR_Incubation1.Points.Add(new Vector2(0, (float)population_Incubation1.Count / population.Count * lR_MaxHeight));
                int pNum = (lR_Incubation1.Points.Count - 1);
                float intervalX = lR_MaxWidth / pNum;
                for (int i = 0; i < lR_Incubation1.Points.Count; i++)
                {
                    Vector2 v = lR_Incubation1.Points[i];
                    v = new Vector2(intervalX * i, v.y);
                    lR_Incubation1.Points[i] = v;
                }
                lR_Incubation1.SetVerticesDirty();

                lR_Mild2.Points.Add(new Vector2(0, (float)population_Mild2.Count / population.Count * lR_MaxHeight));
                for (int i = 0; i < lR_Mild2.Points.Count; i++)
                {
                    Vector2 v = lR_Mild2.Points[i];
                    v = new Vector2(intervalX * i, v.y);
                    lR_Mild2.Points[i] = v;
                }
                lR_Mild2.SetVerticesDirty();

                lR_Critical3.Points.Add(new Vector2(0, (float)population_Critical3.Count / population.Count * lR_MaxHeight));
                for (int i = 0; i < lR_Critical3.Points.Count; i++)
                {
                    Vector2 v = lR_Critical3.Points[i];
                    v = new Vector2(intervalX * i, v.y);
                    lR_Critical3.Points[i] = v;
                }
                lR_Critical3.SetVerticesDirty();

                lR_Dead4.Points.Add(new Vector2(0, (float)population_Dead4.Count / population.Count * lR_MaxHeight));
                for (int i = 0; i < lR_Dead4.Points.Count; i++)
                {
                    Vector2 v = lR_Dead4.Points[i];
                    v = new Vector2(intervalX * i, v.y);
                    lR_Dead4.Points[i] = v;
                }
                lR_Dead4.SetVerticesDirty();

                lR_Cured5.Points.Add(new Vector2(0, (float)population_Cured5.Count / population.Count * lR_MaxHeight));
                for (int i = 0; i < lR_Cured5.Points.Count; i++)
                {
                    Vector2 v = lR_Cured5.Points[i];
                    v = new Vector2(intervalX * i, v.y);
                    lR_Cured5.Points[i] = v;
                }
                lR_Cured5.SetVerticesDirty();

            }
            else
            {
                lRTimer += Time.deltaTime;
            }

            //Daily Properties Change
            dayTime += Time.deltaTime;
            text_Day.text = "Day " + (Mathf.FloorToInt(dayTime / dayLength) + 1);
            text_DataGroup[0].text = "" + population_Normal0.Count;
            text_DataGroup[1].text = "" + population_Incubation1.Count;
            text_DataGroup[2].text = "" + population_Mild2.Count;
            text_DataGroup[3].text = "" + population_Critical3.Count;
            text_DataGroup[4].text = "" + population_Dead4.Count;
            text_DataGroup[5].text = "" + population_Cured5.Count;
            
            //text_FatalityRate.text = PercentageConverter((float)population_Dead4.Count / Mathf.Max(1, (population.Count))) + "%";
            text_FatalityRate.text = PercentageConverter((float)population_Dead4.Count / Mathf.Max(1, (population.Count - population_Normal0.Count))) + "%";
            text_FatalityRate_Student.text = PercentageConverter((float)ageNum_Student_Death / Mathf.Max(1, ageNum_Student)) + "%";
            text_FatalityRate_Adult.text = PercentageConverter((float)ageNum_Adult_Death / Mathf.Max(1, ageNum_Adult)) + "%";
            text_FatalityRate_Elder.text = PercentageConverter((float)ageNum_Elder_Death / Mathf.Max(1, ageNum_Elder)) + "%";
            text_EconomyLost.text = "" + Mathf.RoundToInt(economyLost);
            text_Panic.text = "" + Mathf.RoundToInt(panic);
            text_Bed.text = currentBed + "/" + currentBedTotal;

            for (int i = 0; i < population_Incubation1.Count; i++)
            {
                Human human = population_Incubation1[i];
                if(human.timeStamp_Incubation < dayTime)
                {
                    //Exposed
                    human.ChangeInfectedType(2);
                    human.timeStamp_Mild = dayTime + curve_MildDay.Evaluate(Random.value) * dayLength;
                    population_Mild2.Add(human);
                    population_Incubation1.Remove(human);
                    panic += 0.1f;
                    i--;
                    continue;
                }
                //Infect check
                float basicInfectRate = Random.value * 1000 / dayLength * Time.deltaTime;
                if (!human.isQuarantined && human.infectedPeopleNum <= num_R0 * 0.5f && basicInfectRate < num_R0 * 1.5f / dayLength)
                {
                    //Start infect
                    foreach(Human humanNormal in population_Normal0)
                    {
                        if(Vector3.Distance(human.transform.position, humanNormal.transform.position) < infectionRadius)
                        {
                            Infected(humanNormal);
                            break;
                        }
                    }
                }
            }

            for (int i = 0; i < population_Mild2.Count; i++)
            {
                Human human = population_Mild2[i];
                if (human.timeStamp_Mild < dayTime)
                {
                    //Become critical?
                    if (human.onlyMild)
                    {
                        human.ChangeInfectedType(5);
                        population_Mild2.Remove(human);
                        population_Cured5.Add(human);
                        panic -= 0.05f;

                        if (human.isQuarantined)
                        {
                            EndQuarantine(human);
                        }
                    }
                    else
                    {
                        human.ChangeInfectedType(3);
                        if(!human.isQuarantined) TryQuarantined(human);

                        if (human.isSurvived)
                        {
                            //Check if he's on bed
                            if (!human.isQuarantined || human.useTempBed)
                            {
                                float criticalInHome = 0.5f;
                                if(human.age == 0)
                                {
                                    criticalInHome = ageFatality_Student * 16;
                                }
                                else if (human.age == 1)
                                {
                                    criticalInHome = ageFatality_Adult * 16;
                                }
                                else if (human.age == 2)
                                {
                                    criticalInHome = ageFatality_Elder * 16;
                                }
                                if (Random.value < criticalInHome)
                                {
                                    human.isSurvived = false;
                                }
                                else
                                {
                                    human.timeStamp_Critical = dayTime + curve_CriticalDay.Evaluate(Random.value) * dayLength;
                                }
                            }
                        }
                        if (!human.isSurvived)
                        {
                            human.timeStamp_Critical = dayTime + curve_DeathDay.Evaluate(Random.value) * dayLength;
                        }
                        panic += 0.2f;
                        population_Critical3.Add(human);

                    }
                    population_Mild2.Remove(human);
                    i--;
                    continue;
                }
                //Infect check
                float basicInfectRate = Random.value * 1000 / dayLength * Time.deltaTime;
                if (!human.isQuarantined && human.infectedPeopleNum <= num_R0 && basicInfectRate < num_R0 * 30 / dayLength)
                {
                    //Start infect
                    foreach (Human humanNormal in population_Normal0)
                    {
                        if (Vector3.Distance(human.transform.position, humanNormal.transform.position) < infectionRadius)
                        {
                            Infected(humanNormal);
                            break;
                        }
                    }
                }


                if (!rule3_MildRefuse && !human.isQuarantined)
                {
                    TryQuarantined(human);
                }
            }

            for (int i = 0; i < population_Critical3.Count; i++)
            {
                Human human = population_Critical3[i];
                if (human.timeStamp_Critical < dayTime)
                {
                    if (!human.isSurvived && human.isQuarantined)
                    {
                        if (Random.value < 0.7f){
                            human.isSurvived = true;
                        }
                    }
                    //Become critical?
                    if (human.isSurvived)
                    {
                        human.ChangeInfectedType(5);
                        population_Cured5.Add(human);
                    }
                    else
                    {
                        //Dead
                        human.ChangeInfectedType(4);
                        population_Dead4.Add(human);
                        economyLost += 1;
                        panic += 1;

                        if(human.age == 0)
                        {
                            ageNum_Student_Death++;
                        }
                        else if (human.age == 1)
                        {
                            ageNum_Adult_Death++;
                        }
                        else if (human.age == 2)
                        {
                            ageNum_Elder_Death++;
                        }
                    }

                    if (human.isQuarantined)
                    {
                        EndQuarantine(human);
                    }
                    population_Critical3.Remove(human);
                    i--;
                    continue;
                }
                //Infect check
                float basicInfectRate = Random.value * 1000 / dayLength * Time.deltaTime;
                if (!human.isQuarantined && human.infectedPeopleNum <= num_R0 * 2f && basicInfectRate < num_R0 * 3 / dayLength)
                {
                    //Start infect
                    foreach (Human humanNormal in population_Normal0)
                    {
                        if (Vector3.Distance(human.transform.position, humanNormal.transform.position) < infectionRadius * 2)
                        {
                            Infected(humanNormal);
                            break;
                        }
                    }
                }

                if (!human.isQuarantined)
                {
                    TryQuarantined(human);
                }
            }

            economyLost += (0.1f * population_Mild2.Count + 0.5f * population_Critical3.Count + 0.1f * panic) * Time.deltaTime / dayLength;
            if (rule6_TempHospital)
            {
                economyLost += (10 * Time.deltaTime / dayLength);
            }
            panic = Mathf.Max(0, panic - Time.deltaTime / dayLength);
        }

        if (Input.GetButtonDown("Cancel"))
        {
            Application.Quit();
        }
    }

    string PercentageConverter(float rate)
    {
        float fR0 = rate * 100;
        int fR1 = Mathf.FloorToInt(fR0);
        int fR2 = Mathf.RoundToInt((fR0 - fR1) * 100);
        string text_FR = "" + fR1;
        if (fR2 != 0)
        {
            text_FR += "." + fR2;
        }
        return text_FR;
    }

    public void ChangeCamera(int id)
    {
        if(id == 0)
        {
            cameras[0].SetActive(true);
            cameras[1].SetActive(false);
            cameras[2].SetActive(false);
        }
        else if (id == 1)
        {
            cameras[0].SetActive(false);
            cameras[1].SetActive(true);
            cameras[2].SetActive(false);
        }
        else if (id == 2)
        {
            cameras[0].SetActive(false);
            cameras[1].SetActive(false);
            cameras[2].SetActive(true);
        }
    }
}

//大谷Spitzer 2020
