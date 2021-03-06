using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using StateBase = Fsm.State<MonsterData>;

namespace MonsterState
{
    class Idle : StateBase { }

    class Follow : StateBase
    {
        public override void OnEnter(MonsterData data)
        {
            data.animator.CrossFade(MonsterAnim.Walk, 0.5f);
        }
        public override void OnExit(MonsterData data)
        {
            data.rb.velocity = Vector3.zero;
        }
        public override void OnFixedUpdate(MonsterData data)
        {
            data.transform.SmoothLookAt(data.target.transform, 4f);

            var speed = data.followSpeed;

            // clamp speed if the movement overshots the strafe range
            if (data.targetDistance > data.strafeEnterRange)
            {
                if (data.targetDistance - (speed * Time.fixedDeltaTime) < data.strafeEnterRange)
                {
                    speed = (data.targetDistance - data.strafeEnterRange) / Time.fixedDeltaTime;
                }
            }
            else
            {
                speed = 0f;
            }

            // apply velocity
            data.rb.velocity = data.targetDirection * speed;
        }
    }

    class StrafeIdle : StateBase
    {
        private float duration = 0.5f;
        private float timer = 0f;
        public bool IsDone => timer >= duration;

        public override void OnEnter(MonsterData data)
        {
            data.animator.CrossFade(MonsterAnim.Stand, 0.2f);
        }
        public override void OnExit(MonsterData data)
        {
            timer = 0f;
        }
        public override void OnFixedUpdate(MonsterData data)
        {
            timer += Time.deltaTime;
            data.transform.SmoothLookAt(data.target.transform, 4f);
        }
    }

    class Strafe : StateBase
    {
        private Vector3 anchor;
        private Vector3 anchorDir;
        private float anchorDist = 7f;

        private float duration = float.MaxValue;
        private float timer = 0f;
        public bool IsDone => timer >= duration;

        public override void OnEnter(MonsterData data)
        {
            duration = data.strafeDuration;

            // to make a bigger circle, set the anchor position further than the target position
            anchor = data.target.transform.position + data.targetDirection * anchorDist;

            data.animator.CrossFade(MonsterAnim.StrafeLeft, 0.2f);
        }
        public override void OnExit(MonsterData data)
        {
            data.rb.velocity = Vector3.zero;
            timer = 0f;
        }
        public override void OnFixedUpdate(MonsterData data)
        {
            timer += Time.deltaTime;

            data.transform.SmoothLookAt(data.target.transform, 4f);

            // get the orthogonal vector of anchorDir
            anchorDir = (anchor - data.transform.position).normalized;
            var dir = Vector3.Cross(anchorDir, Vector3.up).normalized;

            // move
            data.rb.velocity = dir * data.strafeSpeed;
        }
    }

    // TODO: ??????????????? ?????? ??? ??????????????? ??????
    class Skill1 : StateBase
    {
        private float duration = 1f;
        private float timer = 0f;
        public bool IsDone => timer >= duration;

        public override void OnEnter(MonsterData data)
        {
            data.isSkillActive = true;
            data.skillCooldownTimer = 0;

            var bullet = GameObject.Instantiate(data.prefabBullet);
            bullet.transform.position = data.transform.position;
            bullet.transform.forward = data.targetDirection;

            data.animator.CrossFade(MonsterAnim.Stand, 0.2f);
        }
        public override void OnExit(MonsterData data)
        {
            data.isSkillActive = false;
            timer = 0f;
        }
        public override void OnUpdate(MonsterData data)
        {
            timer += Time.deltaTime;
        }
    }
}
