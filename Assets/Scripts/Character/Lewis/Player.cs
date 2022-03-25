using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : ObjectNormalToFloor
{

    SpriteRenderer SpriteRenderer;
    private Animator animator;

    private string currentAction_;
    const string IDLE = "Walk_Face";
    const string RIGHT = "Walk_Right";
    const string LEFT = "Walk_Left";
    const string BACK = "Walk_Back";
    const string FACE = "Walk_Face";

    const string IDLE_FACE = "Idle_Face";
    const string IDLE_RIGHT = "Idle_Right";
    const string IDLE_LEFT = "Idle_Left";
    const string IDLE_BACK = "Idle_Back";

    string currentState = "Idle_Face";
    private Direction LastDirectionFaced = Direction.FACE;

    float x, y;
    //Direction Faced by character
    private enum Direction
    {
        FACE = 0,
        BACK = 1,
        RIGHT = 2,
        LEFT = 3,
    }

    public void setCurrenAction(string currentAction)
    {
        currentAction_ = currentAction;
    }

    void Start()
    {
        InitilizeSprite();
        animator = GetComponent<Animator>();
        SpriteRenderer = GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {      
    
    }

    private void FixedUpdate() // Used for physics or coll detection
    {
        WalkAnimation();     
    }


    //Make sure animations don't overlap
    private void ChangeAnimationState(string newState)
    {
        if (currentState == newState) return;
        animator.Play(newState);
        currentState = newState;
    }

    //Not really useful for now.
    public void setAnimation(float x1, float y1)
    {
        x = x1;
        y = y1;
    }

    //Handles Walk animation according to the direction of the input
    private void WalkAnimation()
    {
        float temp = 10 * x + y; // Why ? Because I can - Katarina

        switch (temp)
        {
            /*
             * -9      1     11
             * <-      ^     ->  
             *
             * -10     0     10
             * <-      .     -> 
             * 
             * -11     -1     9
             * <-       v    -> 
             */
            case 1:
                ChangeAnimationState(BACK);
                LastDirectionFaced = Direction.BACK;
                break;
            case -9:
                ChangeAnimationState(BACK);
                LastDirectionFaced = Direction.BACK;
                break;
            case -10:
                ChangeAnimationState(LEFT);
                LastDirectionFaced = Direction.LEFT;
                break;
            case -11:
                ChangeAnimationState(LEFT);
                LastDirectionFaced = Direction.LEFT;
                break;
            case -1:
                ChangeAnimationState(FACE);
                LastDirectionFaced = Direction.FACE;
                break;
            case 9:
                ChangeAnimationState(FACE);
                LastDirectionFaced = Direction.FACE;
                break;
            case 10:
                ChangeAnimationState(RIGHT);
                LastDirectionFaced = Direction.RIGHT;
                break;
            case 11:
                ChangeAnimationState(RIGHT);
                LastDirectionFaced = Direction.RIGHT;
                break;

            default:
                switch (LastDirectionFaced)
                {
                    case Direction.FACE:
                        ChangeAnimationState(IDLE_FACE);
                        break;

                    case Direction.BACK:
                        ChangeAnimationState(IDLE_BACK);
                        break;

                    case Direction.RIGHT:
                        ChangeAnimationState(IDLE_RIGHT);
                        break;

                    case Direction.LEFT:
                        ChangeAnimationState(IDLE_LEFT);
                        break;

                    default:
                        ChangeAnimationState(IDLE_FACE);
                        break;
                }
                break;
        }
    }

    //Give the sprite a 2D look and handle the jump movement
    public void setPos(float x1, float y1, float z1)
    {
        this.transform.position = new Vector3(x1, y1 , z1);

    }
}
