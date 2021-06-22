using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [SerializeField] StageManager stageManager;
    PlayerManager playerManager;
    int nextStageNum = 0;

    enum DIRECTION
    {
        UP,
        DOWN,
        RIGHT,
        LEFT
    }

    private void Start()
    {
        CreatStage(true);
        playerManager = stageManager.GetPlayerManager();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            //Debug.Log("上矢印が押されました。");
            MoveToNextPosition(DIRECTION.UP);
        }
        else if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            //Debug.Log("DownArrow");
            MoveToNextPosition(DIRECTION.DOWN);
        }
        else if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            //Debug.Log("RightArrow");
            MoveToNextPosition(DIRECTION.RIGHT);
        }
        else if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            //Debug.Log("LeftArrow");
            MoveToNextPosition(DIRECTION.LEFT);
        }

        if (ClearCheck())
        {
            CreatStage();
        }
    }

    void MoveToNextPosition(DIRECTION direction)
    {
        Vector2Int currentPlayerPosition = stageManager.GetObjPosition(playerManager.gameObject);
        Vector2Int nextPlayerPosition = GetNextPositon(currentPlayerPosition, direction);
        if (stageManager.IsWall(nextPlayerPosition))
        {
            return;
        }
        else if (stageManager.IsBlock(nextPlayerPosition))
        {
            Vector2Int nextBlockPosition = GetNextPositon(nextPlayerPosition, direction);
            if (stageManager.IsWall(nextBlockPosition) || stageManager.IsBlock(nextBlockPosition))
            {
                return;
            }
            stageManager.UpdateObjectPosition(nextPlayerPosition, nextBlockPosition, StageManager.STAGE_TYPE.BLOCK);
        }
        playerManager.Move(stageManager.GetScreenPositionFromTileTable(nextPlayerPosition));
        stageManager.SetObjPositionOnStage(playerManager.gameObject, nextPlayerPosition);
        stageManager.UpdateStageTableForPlayer(currentPlayerPosition, nextPlayerPosition);
    }

    Vector2Int GetNextPositon(Vector2Int currentPosition, DIRECTION direction)
    {
        switch (direction)
        {
            case DIRECTION.UP:
                return currentPosition - Vector2Int.up;
                break;
            case DIRECTION.DOWN:
                return currentPosition - Vector2Int.down;
                break;
            case DIRECTION.RIGHT:
                return currentPosition + Vector2Int.right;
                break;
            case DIRECTION.LEFT:
                return currentPosition + Vector2Int.left;
                break;
            default:
                return currentPosition;
        }
    }

    bool ClearCheck()
    {
        if (stageManager.GetBlockNum() == stageManager.GetGoalBlockNum())
        {
            return true;
        }
        return false;
    }

    void CreatStage(bool First = false)
    {
        stageManager.DestroyNowStage();
        stageManager.LoadStageData(nextStageNum);
        stageManager.CreateStage(First);
        nextStageNum++;
        if (nextStageNum > stageManager.stageFile.Length)
        {
            nextStageNum = 0;
        }
    }
}
