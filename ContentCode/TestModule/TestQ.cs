using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestQ : MonoBehaviour
{
    public Transform targetTr;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        //남이 만든거
        Quaternion result = transform.rotation * Quaternion.Inverse(targetTr.rotation);

        //내거
        //대신 Quaternion.euler 각도를 사용하세요. 이 함수는 각도 대신 라디안을 사용하므로 더 이상 사용되지 않습니다.
        var result2 = result.ToEuler();        
        var result3 = result2 * 57.29578f;
        var result4 = Internal_MakePositive(result3);
        var result5 = ToEuler(result);

        Debug.Log($"{result.eulerAngles},{result2}=={result5}, {result3}, {result4}");
        
        //Internal_ToEulerRad
        //Internal_ToEulerRad(this) * 57.29578f;
    }
    private static Vector3 Internal_MakePositive(Vector3 euler)
    {
        float num = -0.005729578f;//그냥임의값
        float num2 = 360f + num;
        if (euler.x < num)
        {
            euler.x += 360f;
        }
        else if (euler.x > num2)
        {
            euler.x -= 360f;
        }

        if (euler.y < num)
        {
            euler.y += 360f;
        }
        else if (euler.y > num2)
        {
            euler.y -= 360f;
        }

        if (euler.z < num)
        {
            euler.z += 360f;
        }
        else if (euler.z > num2)
        {
            euler.z -= 360f;
        }

        return euler;
    }
    Vector3 ToEuler(Quaternion rotation)
    {
        Vector3 euler = Vector3.zero;
        //pitch계산
        float sinp = 2 * (rotation.w * rotation.x - rotation.y * rotation.z);
        if (Mathf.Abs(sinp) >= 1)
            euler.x = Mathf.Sign(sinp) * Mathf.PI / 2;
        else
            euler.x = Mathf.Asin(sinp);
        //float sinr_cosp = 2 * (rotation.w * rotation.x + rotation.y * rotation.z);
        //float cosr_cosp = 1 - 2 * (rotation.x * rotation.x + rotation.y * rotation.y);
        //euler.x = Mathf.Atan2(sinr_cosp, cosr_cosp);

        // yaw 계산
        sinp = 2 * (rotation.w * rotation.y - rotation.z * rotation.x);
        if (Mathf.Abs(sinp) >= 1)
            euler.y = Mathf.Sign(sinp) * Mathf.PI / 2;
        else
            euler.y = Mathf.Asin(sinp);
        //float sinp = 2 * (rotation.w * rotation.y - rotation.z * rotation.x);
        //if (Mathf.Abs(sinp) >= 1)
        //    euler.y = Mathf.Sign(sinp) * Mathf.PI / 2; // 절대값이 1 이상인 경우 엣지 케이스 처리
        //else
        //    euler.y = Mathf.Asin(sinp);

        // roll 계산
        double siny_cosp = 2 * (rotation.w * rotation.z + rotation.x * rotation.y);
        double cosy_cosp = 1 - 2 * (rotation.y * rotation.y + rotation.z * rotation.z);
        //euler.z = Mathf.Atan2(siny_cosp, cosy_cosp);
        euler.z = (float)System.Math.Atan2(siny_cosp, cosy_cosp);

        //sinp = 2 * (rotation.w * rotation.z - rotation.x * rotation.y);
        //if (Mathf.Abs(sinp) >= 1)
        //    euler.z = Mathf.Sign(sinp) * Mathf.PI / 2;
        //else
        //    euler.z = Mathf.Asin(sinp);

        return euler;
    }
}
