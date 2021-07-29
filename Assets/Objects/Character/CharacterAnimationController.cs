using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterAnimationController : MonoBehaviour
{
    private Animator characterAnimator;
    private string currentAnimation = "character_iddle";
    
    void Awake()
    {
        characterAnimator = transform.Find("sprite").GetComponent<Animator>();
    }

    void ChangeAnimation(string animation) {
        if (currentAnimation != animation) {
            characterAnimator.Play(animation);
            currentAnimation = animation;
        }
    }

    public void AnimateIddle() {
        ChangeAnimation("character_iddle");
    }
    public void AnimateWalk() {
        ChangeAnimation("character_walk");
    }
}
