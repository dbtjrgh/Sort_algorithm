using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Sort_manager : MonoBehaviour
{
    [Header("막대기 개수")]
    public int BarMaxNum = 100;

    [Header("막대기 Prefabs")]
    public GameObject bar_Prefabs;
    [Header("처음 막대기")]
    public Transform startBar;

    [SerializeField] private Text Log_t;


    public Material[] bar_Mat;

    public int coefficientofCurvature = 20;

    private List<GameObject> barList;

    private bool isCompelete = true;
    
    private void Start()
    {
        transform.position = new Vector3(0f, (BarMaxNum * 0.5f) + 10f, 0f);

        barList = new List<GameObject>();

        barList.Add(startBar.gameObject);
        //블록생성...
        for (int i=1;i< BarMaxNum;i++)
        {
            GameObject tempobj = Instantiate(bar_Prefabs, transform);
            tempobj.transform.localScale = new Vector3(1f, 1f + i, 1f);
            tempobj.transform.position = startBar.position + new Vector3(i, Random.Range(0, coefficientofCurvature), 0);
            barList.Add(tempobj);
        }
    }

    private void Update()
    {
        if (!isCompelete) return;

        if(Input.GetKeyDown(KeyCode.Space))
        {
            Coroutine(RandomSuffle_co());
        }
        if(Input.GetKeyDown(KeyCode.Alpha1))
        {
            Coroutine(SelectionSort_co());
        }
        if(Input.GetKeyDown(KeyCode.Alpha2))
        {
            Coroutine(InsertSort_Co());
        }
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            Coroutine(BubbleSort_co());
        }
        if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            Coroutine(MargeSort_co(0,BarMaxNum-1));
        }
        if (Input.GetKeyDown(KeyCode.Alpha5))
        {
            Coroutine(QuickSort_co(0, BarMaxNum - 1));
        }
    }

    #region 실행을 위해...
    public void Coroutine(IEnumerator co)
    {
        StartCoroutine(Corutine_co(co));
    }


    private IEnumerator Corutine_co(IEnumerator co)
    {
        isCompelete = false;
        yield return StartCoroutine(co);
        yield return StartCoroutine(nomalrize_position());
        isCompelete = true;
        Log_t.text = "Compelete";
    }
    private IEnumerator nomalrize_position()
    {
        for (int i = 0; i < BarMaxNum; i++)
        {
            barList[i].transform.position = new Vector3(barList[i].transform.position.x, barList[i].transform.localScale.y, 0);
            if (barList[i].TryGetComponent(out Bar b))
            {
                b.set_Kinematic(false);
            }
            yield return null;
        }
    }

    private IEnumerator RandomSuffle_co()
    {
        Log_t.text = "RandomSuffle";
        WaitForSeconds wfs = new WaitForSeconds(0.05f);

        for (int i=0;i<BarMaxNum;i++)
        {
            int Select_index = Random.Range(0, BarMaxNum - 1);
            //Debug.Log(Select_index);
            //suffle
            GameObject tempObj = barList[i];
            barList[i]=barList[Select_index];
            barList[Select_index] = tempObj;
            //Matarial 섞을 준비...
            GameObject[] selectObj_Array = new GameObject[2] { barList[i], barList[Select_index] };
            foreach(GameObject ob in selectObj_Array)
            {
                if(ob.TryGetComponent(out Renderer r))
                {
                    r.material = bar_Mat[1];
                }
            }
            //위치 변경
            Vector3 tempPos = selectObj_Array[1].transform.position;
            selectObj_Array[1].transform.position= selectObj_Array[0].transform.position;
            selectObj_Array[0].transform.position = tempPos;

            yield return wfs;
            //Matarial 정상화
            foreach (GameObject ob in selectObj_Array)
            {
                if (ob.TryGetComponent(out Renderer r))
                {
                    r.material = bar_Mat[0];
                }
            }

        }

        
       

    }

    #endregion
    #region 정렬 알고리즘.
    //선택정렬
    private IEnumerator SelectionSort_co()
    {
        WaitForSeconds wfs = new WaitForSeconds(0.01f);
        Log_t.text = "선택정렬 진행중...";

        for (int i=0;i<BarMaxNum-1;i++)
        {
            GameObject Select_obj=null;
            /*
             ① 정렬 되지 않은 인덱스의 맨 앞에서 부터, 
                이를 포함한 그 이후의 배열값 중 가장 작은 값을 찾아간다.
                (정렬되지 않은 인덱스의 맨 앞은, 초기 입력에서는 배열의 시작위치일 것이다.)
             */
            int select_Num = i; //①

            for (int j=i;j< BarMaxNum;j++)
            {
                /*
                 * ② 가장 작은 값을 찾으면, 그 값을 현재 인덱스의 값과 바꿔준다.
                 */
                if (barList[j].TryGetComponent(out Renderer r))
                {
                    r.material = bar_Mat[2];
                }

                bool isSelection = false;
                if (Select_obj==null)
                {
                    Select_obj = barList[j];
                    if(Select_obj.TryGetComponent(out Renderer select_r))
                    {
                        select_r.material= bar_Mat[1];
                    }
                }
                else if(Select_obj.transform.localScale.y>barList[j].transform.localScale.y)
                {
                    Renderer select_r;
                    if (Select_obj.TryGetComponent(out select_r))
                    {
                        select_r.material = bar_Mat[0];
                    }
                    Select_obj = barList[j];
                    select_Num = j;
                    if (Select_obj.TryGetComponent(out select_r))
                    {
                        select_r.material = bar_Mat[1];
                    }
                    isSelection = true;
                }
                yield return wfs;
                if(!isSelection)
                {
                    if(barList[j].TryGetComponent(out Renderer j_r))
                    {
                        j_r.material = bar_Mat[0];
                    }
                }
            }
            GameObject tempobj = barList[i];
            barList[i] = barList[select_Num];
            barList[select_Num] = tempobj;

            GameObject[] select_arr = { barList[i], barList[select_Num] };

            Vector3 temppos = select_arr[0].transform.position;
            select_arr[0].transform.position = select_arr[1].transform.position;
            select_arr[1].transform.position= temppos;

            if(select_arr[0].TryGetComponent(out Renderer s0_r))
            {
                s0_r.material = bar_Mat[3];
            }

            if (select_arr[1].TryGetComponent(out Renderer s1_r))
            {
                s1_r.material = bar_Mat[0];

            }
        }
    }

    //삽입정렬
    private IEnumerator InsertSort_Co()
    {
        Log_t.text = "삽입정렬 진행중...";
        /*
            ① 삽입 정렬은 두 번째 인덱스부터 시작한다. 
        현재 인덱스는 별도의 변수에 저장해주고, 비교 인덱스를 현재 인덱스 -1로 잡는다.

            ② 별도로 저장해 둔 삽입을 위한 변수와, 비교 인덱스의 배열 값을 비교한다.
            
            ③ 삽입 변수의 값이 더 작으면 현재 인덱스로 비교 인덱스의 값을 저장해주고,
        비교 인덱스를 -1하여 비교를 반복한다.
            
            ④ 만약 삽입 변수가 더 크면, 비교 인덱스+1에 삽입 변수를 저장한다.  X
         */
        WaitForSeconds wfs = new WaitForSeconds(0.01f);
        for (int i=1; i < BarMaxNum;i++)
        {
            /*
              ① 삽입 정렬은 두 번째 인덱스부터 시작한다. 
                현재 인덱스는 별도의 변수에 저장해주고, 
                비교 인덱스를 현재 인덱스 -1로 잡는다.
            */
            GameObject Select_obj = barList[i];
            if(Select_obj.TryGetComponent(out Renderer r))
            {
                r.material = bar_Mat[2];
            }
            for(int j=i-1;j>=0;j--)
            {
                //② 별도로 저장해 둔 삽입을 위한 변수와, 비교 인덱스의 배열 값을 비교한다.
                if (barList[j+1].transform.localScale.y<barList[j].transform.localScale.y)
                {
                    //③ 삽입 변수의 값이 더 작으면 현재 인덱스로 비교 인덱스의 값을 저장해주고,
                    //   비교 인덱스를 -1하여 비교를 반복한다.
                    GameObject tempobj = barList[j + 1];
                    barList[j + 1] = barList[j];
                    barList[j] = tempobj;

                    GameObject[] selectobj_arr = { barList[j + 1], barList[j] };

                    Vector3 tempPos = selectobj_arr[0].transform.position;
                    selectobj_arr[0].transform.position = selectobj_arr[1].transform.position;
                    selectobj_arr[1].transform.position=tempPos;

                    yield return wfs;

                }
                else
                {
                    break;
                }
            }
            r.material= bar_Mat[0];
        }
    }

    //버블정렬
    private IEnumerator BubbleSort_co()
    {
        Log_t.text = "버블정렬 진행중...";
        WaitForSeconds wfs = new WaitForSeconds(0.01f);

        for (int i=BarMaxNum-1;i>0;i--)
        {
            //왼쪽부터 탐색하여 인접한 요소를 비교하고 더 높은 요소를 오른쪽에 배치합니다.
            for (int j=0;j<i;j++)
            {
                //다음과 같은 방식으로 가장 큰 요소가 가장 오른쪽 끝으로 이동됩니다.
                GameObject[] Selectobj_arr = { barList[j], barList[j + 1] };
                foreach(GameObject ob in Selectobj_arr)
                {
                    if(ob.TryGetComponent(out Renderer r))
                    {
                        r.material = bar_Mat[2];
                    }
                }

                if(barList[j+1].transform.localScale.y<barList[j].transform.localScale.y)
                {//그다음 이 프로세스는 데이터가 정렬될 때까지 두 번째로 큰 것을 찾아 배치하는 등의 작업을 계속합니다.
                    GameObject tempobj = barList[j + 1];
                    barList[j + 1] = barList[j];
                    barList[j] = tempobj;

                    Vector3 temppos = Selectobj_arr[0].transform.position;
                    Selectobj_arr[0].transform.position = Selectobj_arr[1].transform.position;
                    Selectobj_arr[1].transform.position = temppos;

                    foreach (GameObject ob in Selectobj_arr)
                    {
                        if (ob.TryGetComponent(out Renderer r))
                        {
                            r.material = bar_Mat[1];
                        }
                    }

                }
                yield return wfs;
                foreach (GameObject ob in Selectobj_arr)
                {
                    if (ob.TryGetComponent(out Renderer r))
                    {
                        r.material = bar_Mat[0];
                    }
                }
            }
        }

    }

    //병합정렬
    private IEnumerator MargeSort_co(int s, int e)
    {
        Log_t.text = "병합정렬 진행중...";
        if (s<e)
        {
            int m = (s + e)/2;

            yield return StartCoroutine(MargeSort_co(s, m));
            yield return StartCoroutine(MargeSort_co(m+1, e));
            yield return StartCoroutine(Marge_co(s,e,m));

        }
    }
    private IEnumerator Marge_co(int s,int e,int m)
    {
        WaitForSeconds wfs = new WaitForSeconds(0.01f);

        List<GameObject> tempList = new List<GameObject>();

        int i = s;
        int j = m + 1;
        int copy = 0;
        int pivot = s;

        while(i<=m&&j<=e)
        {
            if(barList[i].transform.localScale.y<barList[j].transform.localScale.y)
            {
                tempList.Add(barList[i++]);
                pivot++;
            }
            else if(barList[i].transform.localScale.y>barList[j].transform.localScale.y)
            {
                tempList.Add(barList[j++]);
                pivot++;
            }
        }

        while (i <= m)
        {
            tempList.Add(barList[i++]);
            pivot++;
        }
        while (j<=e)
        {
            tempList.Add(barList[j++]);
            pivot++;
        }

        for(int k=s;k<=e;k++)
        {
            barList[k] = tempList[copy++];
            barList[k].transform.position =
                new Vector3(0, 0, 0) + Vector3.right * (k - 1);

            if(barList[k].TryGetComponent(out Renderer r))
            {
                r.material = bar_Mat[2];
                yield return wfs;
                r.material = bar_Mat[0];
            }


        }


    }

    //퀵정렬
    private IEnumerator QuickSort_co(int s,int e)
    {
        Log_t.text = "퀵정렬 진행중...";


        WaitForSeconds wfs = new WaitForSeconds(0.01f);


        int pivot = (int)barList[s].transform.localScale.y;
        int bs = s;
        int be = e;

        while(s<e)
        {
            while(pivot <= (int)barList[e].transform.localScale.y&&s<e)
            {
                e--;
            }
            if (s > e) break;

            while(pivot>=(int)barList[s].transform.localScale.y&&s<e)
            {
                s++;
            }
            if (s > e) break;

            if(barList[s].TryGetComponent(out Renderer s_R))
            {
                s_R.material = bar_Mat[2];
            }
            if (barList[e].TryGetComponent(out Renderer e_R))
            {
                e_R.material = bar_Mat[2];
            }


            GameObject tempobj = barList[s];
            barList[s] = barList[e];
            barList[e] = tempobj;

            Vector3 tempPos = barList[s].transform.position;
            barList[s].transform.position = barList[e].transform.position;
            barList[e].transform.position = tempPos;

            yield return wfs;

            s_R.material = bar_Mat[0];
            e_R.material = bar_Mat[0];

        }

        if(barList[s].TryGetComponent(out Renderer s_R2))
        {
            s_R2.material = bar_Mat[2];
        }
        if (barList[e].TryGetComponent(out Renderer e_R2))
        {
            e_R2.material = bar_Mat[2];
        }

        GameObject tempobj_2 = barList[bs];
        barList[bs] = barList[s];
        barList[s]=tempobj_2;

        Vector3 tempPos_2 = barList[s].transform.position;
        barList[s].transform.position = barList[bs].transform.position;
        barList[bs].transform.position=tempPos_2;

        yield return wfs;

        s_R2.material = bar_Mat[0];
        e_R2.material = bar_Mat[0];

        if(bs<s)
        {
            yield return StartCoroutine(QuickSort_co(bs, s - 1));
        }
        if(be>e)
        {
            yield return StartCoroutine(QuickSort_co(s+1,be));

        }

    }


    #endregion
}
