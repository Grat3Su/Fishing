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
        //낚기 실패
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
                // 낚기 실패
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
        // 마우스 포지션 (에디터 또는 PC 빌드)
        screenPos = Input.mousePosition;
#else
        // 터치 포지션 (모바일 빌드)
        if (Input.touchCount > 0)
        {
            screenPos = Input.GetTouch(0).position;
        }
        else
        {
            return Vector3.zero; // 터치가 없을 경우 처리
        }
#endif

        // 스크린 좌표를 월드 좌표로 변환
        screenPos.z = Camera.main.nearClipPlane; // z값은 카메라에서 거리 설정
        Vector3 worldPos = Camera.main.ScreenToWorldPoint(screenPos);

        return worldPos;
    }

    // 랜덤한 시간에 진동과 함께 이펙트
    // 1초 안에 화면을 터치하면 물고기 보여주기

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

        // 1초 뒤에 모두 끄기
        StartCoroutine(DisappearParticle());
    }

    IEnumerator DisappearParticle()
	{
        yield return new WaitForSeconds(1.0f);

        FishAppear.SetActive(false);

        // 랜덤한 물고기 보여주기
        int ranIdx = Random.Range(1, Fishes.transform.childCount);

        Fishes.transform.GetChild(ranIdx).gameObject.SetActive(true);

        yield return new WaitForSeconds(3.0f);
        Fishes.transform.GetChild(ranIdx).gameObject.SetActive(false);

    }


    public long[] pattern = { 0, 300, 100, 300 }; // [대기시간, 진동시간, 대기시간, 진동시간]
    public int[] amplitudes = { 0, 100, 0, 100 }; // 각 구간 진동 세기 (최대 255)
    void Vibrate()
	{
        VibrationManager.VibratePattern(pattern, amplitudes);

    }
}
