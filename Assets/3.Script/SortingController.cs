using System.Collections;
using UnityEngine;

public class SortingController : MonoBehaviour
{
    public GameObject cubePrefab; // 큐브 프리팹
    public int arraySize = 10;    // 배열 크기
    public int maxValue = 20;     // 최대 값
    public float cubeSpacing = 1.5f; // 큐브 간격
    public float sortSpeed = 0.5f;  // 정렬 속도

    private int[] array;         // 정렬할 배열
    private GameObject[] cubes;  // 큐브 오브젝트 배열
    private bool isSorting = false; // 정렬 중인지 확인

    void Start()
    {
        ResetArray();
    }

    void Update()
    {
        if (isSorting)
        {
            return;
        }

        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            Debug.Log("선택 정렬 시작!");
            StartCoroutine(SelectionSort());
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            Debug.Log("삽입 정렬 시작!");
            StartCoroutine(InsertionSort());
        }

        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            Debug.Log("버블 정렬 시작!");
            StartCoroutine(BubbleSort());
        }

        if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            Debug.Log("퀵 정렬 시작!");
            StartCoroutine(QuickSort(0, array.Length - 1));
        }

        if (Input.GetKeyDown(KeyCode.Alpha5))
        {
            Debug.Log("병합 정렬 시작!");
            StartCoroutine(MergeSort(0, array.Length - 1));
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            ResetArray();
        }
    }

    // 배열 초기화 및 큐브 생성
    void ResetArray()
    {
        StopAllCoroutines();
        isSorting = false;

        if (cubes != null)
        {
            foreach (var cube in cubes) Destroy(cube);
        }

        array = new int[arraySize];
        cubes = new GameObject[arraySize];

        for (int i = 0; i < arraySize; i++)
        {
            array[i] = Random.Range(1, maxValue + 1);
            GameObject cube = Instantiate(cubePrefab, new Vector3(i * cubeSpacing, array[i] / 2f, 0), Quaternion.identity);
            cube.transform.localScale = new Vector3(1, array[i], 1);
            cube.name = $"Cube {i}";
            cubes[i] = cube;
        }
    }

    // 큐브 위치 및 크기 업데이트
    IEnumerator UpdateCubes()
    {
        for (int i = 0; i < array.Length; i++)
        {
            Vector3 position = cubes[i].transform.position;
            cubes[i].transform.position = new Vector3(i * cubeSpacing, array[i] / 2f, 0);
            cubes[i].transform.localScale = new Vector3(1, array[i], 1);
        }
        yield return new WaitForSeconds(sortSpeed);
    }

    // 큐브 교환
    IEnumerator SwapCubes(int index1, int index2)
    {
        int temp = array[index1];
        array[index1] = array[index2];
        array[index2] = temp;

        Vector3 tempPosition = cubes[index1].transform.position;
        cubes[index1].transform.position = cubes[index2].transform.position;
        cubes[index2].transform.position = tempPosition;

        GameObject tempCube = cubes[index1];
        cubes[index1] = cubes[index2];
        cubes[index2] = tempCube;

        yield return UpdateCubes();
    }


    // 선택 정렬
    IEnumerator SelectionSort()
    {
        isSorting = true;
        int n = array.Length;
        for (int i = 0; i < n - 1; i++)
        {
            int minIndex = i;
            for (int j = i + 1; j < n; j++)
            {
                if (array[j] < array[minIndex])
                {
                    minIndex = j;
                }
            }
            if (minIndex != i)
            {
                yield return SwapCubes(i, minIndex);
            }
        }
        isSorting = false;
        Debug.Log("선택 정렬 완료!");
    }

    // 삽입 정렬
    IEnumerator InsertionSort()
    {
        isSorting = true;
        int n = array.Length;
        for (int i = 1; i < n; i++)
        {
            int key = array[i];
            int j = i - 1;

            while (j >= 0 && array[j] > key)
            {
                yield return SwapCubes(j + 1, j);
                j--;
            }
        }
        isSorting = false;
        Debug.Log("삽입 정렬 완료!");
    }

    // 버블 정렬
    IEnumerator BubbleSort()
    {
        isSorting = true;
        int n = array.Length;
        for (int i = 0; i < n - 1; i++)
        {
            for (int j = 0; j < n - i - 1; j++)
            {
                if (array[j] > array[j + 1])
                {
                    yield return SwapCubes(j, j + 1);
                }
            }
        }
        isSorting = false;
        Debug.Log("버블 정렬 완료!");
    }
    // 퀵 정렬
    IEnumerator QuickSort(int low, int high)
    {
        if (low < high)
        {
            // 피벗 위치를 계산
            int pivotIndex = Partition(low, high);

            // 작업 중인 배열 범위를 강조
            yield return HighlightRange(low, high, Color.yellow);

            // 왼쪽과 오른쪽 배열 재귀적으로 정렬
            yield return QuickSort(low, pivotIndex - 1);
            yield return QuickSort(pivotIndex + 1, high);

            // 작업 완료된 범위를 원래 색상으로 복원
            yield return HighlightRange(low, high, Color.white);
        }
    }

    int Partition(int low, int high)
    {
        // 피벗 값은 배열의 마지막 값
        int pivot = array[high];
        int i = low - 1;

        // 피벗 요소 강조 표시
        HighlightCube(high, Color.red);

        for (int j = low; j < high; j++)
        {
            // 현재 요소가 피벗보다 작으면 교환
            if (array[j] < pivot)
            {
                i++;
                SwapImmediate(i, j);
            }
        }

        // 피벗을 올바른 위치로 이동
        SwapImmediate(i + 1, high);

        return i + 1; // 피벗 위치 반환
    }

    void SwapImmediate(int index1, int index2)
    {
        // 배열 값 교환
        int temp = array[index1];
        array[index1] = array[index2];
        array[index2] = temp;

        // 큐브 위치 교환
        Vector3 tempPosition = cubes[index1].transform.position;
        cubes[index1].transform.position = cubes[index2].transform.position;
        cubes[index2].transform.position = tempPosition;

        // 큐브 참조 교환
        GameObject tempCube = cubes[index1];
        cubes[index1] = cubes[index2];
        cubes[index2] = tempCube;
    }

    IEnumerator HighlightRange(int low, int high, Color color)
    {
        // 특정 범위의 큐브 색상을 변경
        for (int i = low; i <= high; i++)
        {
            cubes[i].GetComponent<Renderer>().material.color = color;
        }
        yield return new WaitForSeconds(sortSpeed);
    }

    void HighlightCube(int index, Color color)
    {
        // 특정 큐브를 강조
        cubes[index].GetComponent<Renderer>().material.color = color;
    }



    // 병합 정렬
    IEnumerator MergeSort(int left, int right)
    {
        if (left < right)
        {
            int mid = (left + right) / 2;

            yield return MergeSort(left, mid);
            yield return MergeSort(mid + 1, right);

            yield return HighlightRange(left, right, Color.yellow); // 작업 영역 강조
            yield return Merge(left, mid, right);
            yield return HighlightRange(left, right, Color.white);  // 작업 완료된 영역 복원
        }
    }

    IEnumerator Merge(int left, int mid, int right)
    {
        int n1 = mid - left + 1;
        int n2 = right - mid;

        int[] leftArray = new int[n1];
        int[] rightArray = new int[n2];

        for (int i = 0; i < n1; i++) leftArray[i] = array[left + i];
        for (int i = 0; i < n2; i++) rightArray[i] = array[mid + 1 + i];

        int iLeft = 0, iRight = 0, k = left;

        while (iLeft < n1 && iRight < n2)
        {
            if (leftArray[iLeft] <= rightArray[iRight])
            {
                array[k] = leftArray[iLeft];
                iLeft++;
            }
            else
            {
                array[k] = rightArray[iRight];
                iRight++;
            }
            UpdateCube(k); // 병합 결과를 즉시 업데이트
            k++;
            yield return new WaitForSeconds(sortSpeed);
        }

        while (iLeft < n1)
        {
            array[k] = leftArray[iLeft];
            UpdateCube(k);
            iLeft++;
            k++;
            yield return new WaitForSeconds(sortSpeed);
        }

        while (iRight < n2)
        {
            array[k] = rightArray[iRight];
            UpdateCube(k);
            iRight++;
            k++;
            yield return new WaitForSeconds(sortSpeed);
        }
    }

    void UpdateCube(int index)
    {
        cubes[index].transform.localScale = new Vector3(1, array[index], 1);
        cubes[index].transform.position = new Vector3(index * cubeSpacing, array[index] / 2f, 0);
    }


}
