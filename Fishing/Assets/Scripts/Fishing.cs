using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fishing : MonoBehaviour
{
    public GameObject FishAppear;
    public GameObject WaterDrop;
    public GameObject Fishes;

    public float Timer;
    public float DelayTimer;

    bool appear;
    float appearTimer;
    // Start is called before the first frame update
    void Start()
    {
        Timer = Random.Range(10, 50f);
        //���� ����
        FishAppear.SetActive(false);
        WaterDrop.SetActive(false);
        appearTimer = 0;
        appear = false;
    }

    void Update()
    {
        if (appear)
        {
            appearTimer += Time.deltaTime;
            if (appearTimer > 5.0f)
            {
                // ���� ����
                FishAppear.SetActive(false);
                WaterDrop.SetActive(false);
                appear = false;
                appearTimer = 0;
            }
        }
        else
        {
            TimeChecker();
        }

#if UNITY_EDITOR
        if (Input.GetMouseButtonDown(0))
#else
    if (Input.touchCount > 0)
#endif
        {
            if (appear)
            {
                appear = false;
                appearTimer = 0;
                GetFish();
            }
        }
    }


    Vector3 InputPosition()
    {
        Vector3 screenPos;

#if UNITY_EDITOR || UNITY_STANDALONE
        // ���콺 ������ (������ �Ǵ� PC ����)
        screenPos = Input.mousePosition;
#else
        // ��ġ ������ (����� ����)
        if (Input.touchCount > 0)
        {
            screenPos = Input.GetTouch(0).position;
        }
        else
        {
            return Vector3.zero; // ��ġ�� ���� ��� ó��
        }
#endif

        // ��ũ�� ��ǥ�� ���� ��ǥ�� ��ȯ
        screenPos.z = Camera.main.nearClipPlane; // z���� ī�޶󿡼� �Ÿ� ����
        Vector3 worldPos = Camera.main.ScreenToWorldPoint(screenPos);

        return worldPos;
    }

    // ������ �ð��� ������ �Բ� ����Ʈ
    // 1�� �ȿ� ȭ���� ��ġ�ϸ� ����� �����ֱ�

    void TimeChecker()
	{
        if (appear) return;
        DelayTimer += Time.deltaTime;

		if (DelayTimer > Timer)
		{
            WaterDrop.SetActive(true);
            DelayTimer = 0;
            appear = true;
            Timer = Random.Range(10, 50f);

            Vibrate();

        }

    }

    void GetFish()
	{
        FishAppear.SetActive(true);
        WaterDrop.SetActive(false);
        VibrationManager.Vibrate(1000, 150);

        // 1�� �ڿ� ��� ����
        StartCoroutine(DisappearParticle());
    }

    IEnumerator DisappearParticle()
	{
        yield return new WaitForSeconds(1.0f);

        FishAppear.SetActive(false);

        // ������ ����� �����ֱ�
        int ranIdx = Random.Range(1, Fishes.transform.childCount);

        Fishes.transform.GetChild(ranIdx).gameObject.SetActive(true);

        yield return new WaitForSeconds(3.0f);
        Fishes.transform.GetChild(ranIdx).gameObject.SetActive(false);

    }


    public long[] pattern = { 0, 300, 100, 300 }; // [���ð�, �����ð�, ���ð�, �����ð�]
    public int[] amplitudes = { 0, 100, 0, 100 }; // �� ���� ���� ���� (�ִ� 255)
    void Vibrate()
	{
        VibrationManager.VibratePattern(pattern, amplitudes);

    }
}
