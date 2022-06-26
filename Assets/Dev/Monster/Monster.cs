using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Monster : MonoBehaviour
{
    [SerializeField]
    public MonsterData data;
    private Fsm.Fsm<MonsterData> fsm;

    private void Awake()
    {
        data.gameObject = gameObject;
        data.transform = transform;
        data.rb = GetComponent<Rigidbody>();
        data.target = GameObject.Find("Player");

        fsm = MonsterFsm.create(data);
    }

    private void Update()
    {
        data.targetDistance = Vector3.Distance(transform.position, data.target.transform.position);
        data.targetDirection = (data.target.transform.position - transform.position).normalized;

        if (!data.isSkillActive)
            data.skillCooldownTimer += Time.deltaTime;

        fsm.Update();
    }

    private void FixedUpdate()
    {
        fsm.FixedUpdate();
    }
}
