using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fsm;

/*
패턴 하나가 발동하면
쿨타임이 돈다
쿨타임 - 글로벌공유

패턴마다 각기다른 확률을 가지고 있다.

한 패턴을 연속으로 사용하지 못하도록 하기 위해 @번 이상 연속으로 사용되면
다음번 랜덤값계산에서
확률을 0%로 고정시킨다
(예를 들어 3번이상 연속으로 사용되면 다음번 랜덤값계산에서 안나오도록)

-상태-
1. 아이들 (1회성)
-인식 범위안에 들어오면(보스기준) (인식각도 360도) (인식 거리 미정)

2. 플레이어추적
2-1. 다음패턴 뭐 사용할지 랜던값 계산하기
3. 현재 정해진 패턴의 범위내에 플레이어가 있으면 패턴 사용

->다시 2 / 2-1 / 3 반복
-----------------------------------------------
[1번]
시작: 주먹 두번 휘두르기
(파생)
끝1: 기폭발
끝2: 한번 더 휘두르기
끝3: 아무것도안하기

TODO:
- [x] 1회성 Idle
- [x] 플레이어와 일정 거리 안에 들어오면 옆으로(약간 휘어지게) 움직인다.
    - [ ] 다른 대안: 거리가 가까워지면 다른 패턴을 무조건 사용할 수 있게 됨
- [x] 휴머노이드 모델 적용
- [x] 스킬 쿨다운이 0이고 스킬 공격 사거리에 들어오면 스킬을 사용한다.
    - [ ] 다음 스킬을 랜덤(개별 확률)으로 선택한다.
    - [ ] 스킬(공격) 애니메이션 적용
        https://forum.unity.com/threads/how-to-find-animation-clip-length.465751/
- [ ] 스킬 만들기 (임시)
    - [ ] 근거리 공격
    - [ ] 범위 공격
    - [ ] 원거리 공격 (150m)
    - [ ] 대쉬
    - [ ] 순간이동
*/

public static class MonsterAnim
{
    public const string Stand = "Stand";
    public const string Walk = "Walk";
    public const string StrafeLeft = "Walk Strafe Left";
    public const string StrafeRight = "Walk Strafe Right";
}

[System.Serializable]
public class MonsterData
{
    [HideInInspector]
    public GameObject gameObject;

    [HideInInspector]
    public Transform transform;

    public Animator animator;

    [HideInInspector]
    public string currentAnimation;

    [HideInInspector]
    public Rigidbody rb;

    [HideInInspector]
    public GameObject target;

    [HideInInspector]
    public float targetDistance;

    [HideInInspector]
    public Vector3 targetDirection;

    [Header("Follow Settings")]
    public float aggroDistance = 10f;
    public float followSpeed = 4f;

    [Header("Strafe Settings")]
    public float strafeEnterRange = 5f;
    public float strafeDuration = 3f;
    public float strafeSpeed = 2.5f;

    [Header("Skill Settings")]
    public float skillRange = 7f;
    public float skillCooldown = 5f;
    public GameObject prefabBullet;

    [HideInInspector]
    public float skillCooldownTimer = 0f;

    [HideInInspector]
    public bool isSkillActive = false;
}

static class MonsterFsm
{
    public static Fsm<MonsterData> create(MonsterData data)
    {
        // states
        var idle = new MonsterState.Idle();
        var follow = new MonsterState.Follow();
        var strafeIdle = new MonsterState.StrafeIdle();
        var strafe = new MonsterState.Strafe();
        var skill1 = new MonsterState.Skill1();

        // flows
        var flowStart = new Flow<MonsterData>();
        var flowNormal = new Flow<MonsterData>();
        var flowSkill = new Flow<MonsterData>();

        flowStart
            .Do(
                name: "idle",
                state: data => idle,
                next: data =>
                    data.targetDistance <= data.aggroDistance
                    ? "to normal"
                    : null
            )
            .To(
                name: "to normal",
                next: data => flowNormal
            );

        flowNormal
            .ForceTo(
                condition: data => (
                    !data.isSkillActive &&
                    data.skillCooldownTimer >= data.skillCooldown &&
                    data.targetDistance <= data.skillRange
                ),
                next: data => flowSkill
            )
            .Do(
                name: "follow",
                state: data => follow,
                next: data =>
                    // add padding
                    data.targetDistance - data.strafeEnterRange - 0.1f <= 0
                    ? "strafe idle"
                    : null
            )
            .Do(
                name: "strafe idle",
                state: data => strafeIdle,
                next: data =>
                    strafeIdle.IsDone
                    ? "strafe"
                    : null
            )
            .Do(
                name: "strafe",
                state: data => strafe,
                next: data =>
                    strafe.IsDone
                    ? "follow"
                    : null
            );

        flowSkill
            .Do(
                name: "skill1",
                    state: data => skill1,
                    next: data =>
                        skill1.IsDone
                        ? "to normal"
                        : null
                )
                .To(
                    name: "to normal",
                    next: data => flowNormal
                );

        // create fsm instance
        return new Fsm<MonsterData>(
            data: data,
            startingFlow: flowStart
        );
    }
}
