using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.Characters.FirstPerson;

public class PlayerAnimationController : MonoBehaviour
{
    [SerializeField] private FirstPersonController firstPersonController;
    [SerializeField] private Animator anim;

    enum PlayerAnimState
    {
        IDLE,
        FORWARD,
        BACKWARD,
        RIGHT,
        LEFT
    };

    // Update is called once per frame
    void Update()
    {
        SetPlayerAnimation();
    }

    void SetPlayerAnimation()
    {
        float horizontal = firstPersonController.GetHorizontalInput();
        float vertical = firstPersonController.GetVerticalInput();

        if (vertical == 0 && horizontal == 0)
        {
            anim.SetBool("move_forard", false);
            anim.SetBool("move_backward", false);
            anim.SetBool("move_right", false);
            anim.SetBool("move_left", false);
            return;
        }

        if(vertical > 0)
        {
            anim.SetBool("move_backward", false);
            anim.SetBool("move_right", false);
            anim.SetBool("move_left", false);

            anim.SetBool("move_forard", true);
            return;
        }
        else if(vertical < 0)
        {
            anim.SetBool("move_forward", false);
            anim.SetBool("move_right", false);
            anim.SetBool("move_left", false);

            anim.SetBool("move_backward", true);
            return;
        }

        if(horizontal > 0)
        {
            anim.SetBool("move_forward", false);
            anim.SetBool("move_backward", false);
            anim.SetBool("move_left", false);

            anim.SetBool("move_right", true);
        }
        else if(horizontal < 0)
        {
            anim.SetBool("move_forward", false);
            anim.SetBool("move_backward", false);
            anim.SetBool("move_right", false);

            anim.SetBool("move_left", true);
        }
    }
}
