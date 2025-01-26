using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using UnityEngine;

public class BallBalanceAgent : Agent
{
    [SerializeField] private Transform ball; // �{�[����Transform
    [SerializeField] private Rigidbody ballRigidbody; // �{�[����Rigidbody
    [SerializeField] private Transform platform; // �y���Transform
    [SerializeField] private float forceMultiplier = 10f; // �y��̓�������

    private Vector3 initialBallPosition;
    private Vector3 initialPlatformPosition;

    public override void Initialize()
    {
        // �����ʒu��ۑ�
        initialBallPosition = ball.position;
        initialPlatformPosition = platform.position;
    }

    public override void OnEpisodeBegin()
    {
        // �G�s�\�[�h�J�n���Ƀ{�[���Ɠy��������ʒu�ɖ߂�
        ballRigidbody.velocity = Vector3.zero;
        ball.position = initialBallPosition;
        platform.position = initialPlatformPosition;
        platform.rotation = Quaternion.identity;
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        // �{�[���̈ʒu�Ƒ��x���ϑ�
        sensor.AddObservation(ball.localPosition); // �{�[���̈ʒu
        sensor.AddObservation(ballRigidbody.velocity); // �{�[���̑��x

        // �y��̌X�����ϑ�
        sensor.AddObservation(platform.rotation.eulerAngles); // �y��̉�]�p
    }

    public override void OnActionReceived(ActionBuffers actions)
    {
        // �y��̌X���𐧌䂷��A�N�V�������擾
        float moveX = actions.ContinuousActions[0]; // X������
        float moveZ = actions.ContinuousActions[1]; // Z������

        // �y��ɗ͂������ē�����
        platform.Rotate(new Vector3(moveX, 0, moveZ) * forceMultiplier * Time.deltaTime);

        // ��V�v�Z
        if (Mathf.Abs(ball.localPosition.x) > 5 || Mathf.Abs(ball.localPosition.z) > 5)
        {
            // �{�[�����y�䂩�痎������y�i���e�B��^���ăG�s�\�[�h�I��
            SetReward(-1f);
            EndEpisode();
        }
        else
        {
            // �{�[���������t�߂ɂ���قǕ�V��^����
            float distanceToCenter = Vector3.Distance(ball.localPosition, Vector3.zero);
            SetReward(1f - distanceToCenter / 5f); // ���S�ɋ߂��قǍ����_
        }
    }
}