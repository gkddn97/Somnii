﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

// 캐릭터들의 공통 움직임 컨트롤
public class ControllerScript : MonoBehaviour
{
    public JoyStickSetting joystick;                // JoyStick 스크립트
    public CameraController cameraScript;
    public float HP = 100.0f;                       // 체력(inspector에서 개별 조정 필요, CharacterSwitch.cs에서 체력바 조절)
    public float MoveSpeed = 4f;
    public float[] attackDamage = new float[3];     // [0]: FAttack, [1]: SAttack, [2]: TAttack Damage

    private Vector3 _moveVector;                    // 플레이어 이동벡터
    private Transform _transform;                   // 플레이어 트랜스폼
    private SpriteRenderer charRenderer;            // 캐릭터의 스프라이트 렌더러 가져옴
    private Animator animator;                      // 애니메이터 가져오기
    private Rigidbody2D rb2D;

    public static bool isClear = false;             // Portal 스크립트에서 참조
    public static bool hitCheck = false;            // 맞는 모션동안(ture)은 무적, 맞는 모션 끝나면 false
                                                    // enemy들의 각 스크립트에서 참조
    public static bool isAttack = false;            // t: 공격중, f: 공격안하는중, t일때는 움직임 제한하는 플래그, UIEvent.cs, AttackControl.cs
    public bool isDash = false;                     // t: 대쉬중
    private float dashTime;
    private float startDashTime = 0.05f;

    void OnEnable()
    {
        // 초기화
        _transform = transform; // 트랜스폼 캐싱
        _moveVector = Vector3.zero; // 플레이어 이동벡터 초기화
        charRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
        rb2D = GetComponent<Rigidbody2D>();
        // 카메라 컨트롤러 스크립트 초기화(플레이어 다시 연결)
        CameraController cameraScript = GameObject.Find("Main Camera").GetComponent<CameraController>();   
        joystick = GameObject.Find("JoyStickPanel").GetComponent<JoyStickSetting>();
        cameraScript.Init();
        gameObject.GetComponent<AttackControl>().AttackBtnInit();
        isClear = false;
        hitCheck = false;
        isAttack = false;

        dashTime = startDashTime;
    }

    // Start is called before the first frame update
    void Start()
    {
        // _transform = transform; // 트랜스폼 캐싱
        // _moveVector = Vector3.zero; // 플레이어 이동벡터 초기화
        // charRenderer = GetComponent<SpriteRenderer>();
        // animator = GetComponent<Animator>();
        // attackDamage[0] = 10f;
        // attackDamage[1] = 15f;
        // attackDamage[2] = 30f;
    }

    // Update is called once per frame
    void Update()
    {
        // 터치패드 입력받기
        HandleInput();

        if(isClear) {
            SetPosition();
        }
    }

    void FixedUpdate()
    {
        // 플레이어 이동
        if(!isAttack && !hitCheck && !isDash) {    // 공격중이거나 맞는중에는 이동 x
            Move();
        }

        if(isDash) {
            if(dashTime <= 0) {
                cameraScript.smoothTimeX = 0.15f;
                cameraScript.smoothTimeY = 0.1f;
                dashTime = startDashTime;
                rb2D.velocity = Vector2.zero;
                isDash = false;
            }
            else {
                cameraScript.smoothTimeX = 0.3f;
                cameraScript.smoothTimeY = 0.2f;
                dashTime -= Time.deltaTime;
                rb2D.velocity = _moveVector * 32f;
            }
        }
    }

    public void HandleInput()
    {
        _moveVector = poolInput();
    }

    public Vector3 poolInput()
    {
        float h = joystick.GetHorizontalValue();
        float v = joystick.GetVerticalValue();    // y축 사용시 활성화
        Vector3 moveDir = new Vector3(h, v, 0).normalized;  // y축사용시 인자(h, v, 0), normalized 정규화 해줘야 대각선때 안빨라짐

        return moveDir;
    }

    public void Move()
    {
        if(GameObject.Find("CameraCanvas").transform.GetChild(0).gameObject.activeSelf) {
            return;
        }

        if(_moveVector.x < 0) { // 이동하는 x축벡터값 음수면
            if(!WeponControl.nowAttack) {   // 공격중이 아니라면
                //charRenderer.flipX = true;  // 스프라이트 플립
                transform.localScale = new Vector3(-1f, 1f);
            }
            // Player의 애니메이션 parameter 불값 변경
            animator.SetBool("isWalking", true);
        }
        else if(_moveVector.x > 0) {    // 양수면
            if(!WeponControl.nowAttack) {   // 공격중이 아니라면
                //charRenderer.flipX = false; // 원래대로
                transform.localScale = new Vector3(1f, 1f);
            }
            animator.SetBool("isWalking", true);
        }
        else if(_moveVector.x == 0 && _moveVector.y == 0) {   // 안움직이면
            animator.SetBool("isWalking", false);
        }
        else {
            animator.SetBool("isWalking", true);
        }
        
        // 캐릭터 벡터값, 움직이는 힘 줘서 움직이게 하기
        _transform.Translate(_moveVector * MoveSpeed * Time.deltaTime);
    }

    // 맞았을 때 애니메이션 이벤트, 체력 처리
    public void OnHit(GameObject enemyObject)
    {
        hitCheck = true;
        animator.SetBool("isHit", true);
        Vector3 distance = new Vector3(enemyObject.transform.position.x- gameObject.transform.position.x, 0f);

        if(distance.x < 0 ) {    // 플레이어가 때린애보다 오른쪽에 있을 때
            transform.localScale = new Vector3(-1f, 1f);
            _transform.Translate(new Vector3(1f, 0f, 0f)); 
        }
        else {  // 플레이어가 때린애보다 왼쪽에 있을 때
            transform.localScale = new Vector3(1f, 1f);
            _transform.Translate(new Vector3(-1f, 0f, 0f));
        }
    }

    public void EndHit()
    {
        animator.SetBool("isHit", false);
        hitCheck = false;
    }

    // 룸 클리어시 알맞은 바운드 위치로 캐릭터 포지션 이동
    void SetPosition()
    {
        isClear = false;

        GameObject bound = GameObject.Find(ClearCheck.boundName);

        // 바운드에 맞는 포지션으로 캐릭터 이동
        this.transform.position = new Vector3(bound.transform.position.x, bound.transform.position.y, this.transform.position.z);
    }
}
