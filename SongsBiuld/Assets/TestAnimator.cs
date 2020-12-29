using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestAnimator : MonoBehaviour
{
    Animator animator;

    // Start is called before the first frame update
    void Awake()
    {
        animator = GetComponent<Animator>();
    }

    int index = 0;
    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            if(index >= animator.parameterCount)
            {
                index = 0;
            }
            AnimatorControllerParameter parameter = animator.GetParameter(index);
            if(parameter.type == AnimatorControllerParameterType.Float)
            {
                animator.SetFloat(parameter.name,1);
            }
            if (parameter.type == AnimatorControllerParameterType.Bool)
            {
                animator.SetBool(parameter.name, true);
            }
        }

        if (Input.GetKeyDown(KeyCode.T))
        {
            AnimatorControllerParameter parameter = animator.GetParameter(index);
            if (parameter.type == AnimatorControllerParameterType.Float)
            {
                animator.SetFloat(parameter.name, 0);
            }
            if (parameter.type == AnimatorControllerParameterType.Bool)
            {
                animator.SetBool(parameter.name, false);
            }
            index++;
        }
    }
}
