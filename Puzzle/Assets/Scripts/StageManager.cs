using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageManager : MonoBehaviour
{
    public TextAsset[] stageFile;
    [SerializeField] GameObject[] prefabs;
    private PlayerManager playerManager;

    float tileSize;
    Vector2 centerPosition;
    private Dictionary<GameObject, Vector2Int> moveObjPositionOnStage =
        new Dictionary<GameObject, Vector2Int>();
    int blockNum = 0;
    int goalBlockNum = 0;

    public enum STAGE_TYPE
    {
        WALL,
        GROUND,
        BLOCK_POINT,
        BLOCK,
        PLAYER,
        BLOCK_ON_POINT,
        PLAYER_ON_POINT
    }
    STAGE_TYPE[,] stageTable;
    
    public void LoadStageData(int i)
    {
        string[] lines = stageFile[i].text.Split(new[] { '\n', '\r' });
        int rows = lines.Length;
        int columns = lines[0].Split(new[] { ',' }).Length;
        stageTable = new STAGE_TYPE[rows, columns];

        for (int x = 0; x < rows; x++)
        {
            string[] values = lines[x].Split(new[] { ',' });
            for (int y = 0;y < columns; y++)
            {
                stageTable[x, y] = (STAGE_TYPE)int.Parse(values[y]);
                Debug.Log($"{x}:{y} => {stageTable[x, y]}");
            }
        }
    }

    public void CreateStage(bool First = false)
    {
        tileSize = prefabs[0].GetComponent<SpriteRenderer>().bounds.size.x;
        centerPosition.x = (stageTable.GetLength(0) / 2) * tileSize;
        centerPosition.y = (stageTable.GetLength(1) / 2) * tileSize;

        for (int x = 0; x < stageTable.GetLength(0); x++)
        {
            for (int y = 0; y < stageTable.GetLength(1); y++)
            {
                Vector2Int position = new Vector2Int(y, x);

                STAGE_TYPE groundStageType = STAGE_TYPE.GROUND;
                GameObject groundObj = Instantiate(prefabs[(int)groundStageType]);
                groundObj.transform.position = GetScreenPositionFromTileTable(position);

                STAGE_TYPE stageType = stageTable[x, y];
                GameObject obj = Instantiate(prefabs[(int)stageType]);
                obj.transform.position = GetScreenPositionFromTileTable(position);

                if (stageType == STAGE_TYPE.PLAYER)
                {
                    if (First)
                    {
                        playerManager = obj.GetComponent<PlayerManager>();
                        moveObjPositionOnStage.Add(obj, position);
                    }
                    else
                    {
                        moveObjPositionOnStage[obj] = position;
                    }
                }

                if (stageType == STAGE_TYPE.BLOCK)
                {
                    moveObjPositionOnStage.Add(obj, position);
                    blockNum++;
                }
            }
        }
    }

    public Vector2 GetScreenPositionFromTileTable(Vector2Int position)
    {
        return new Vector2(position.x * tileSize - centerPosition.x,
            -(position.y * tileSize - centerPosition.y));
    }

    public PlayerManager GetPlayerManager()
    {
        return playerManager;
    }

    public void SetObjPositionOnStage(GameObject gameObject, Vector2Int position)
    {
        moveObjPositionOnStage[gameObject] = position;
    }

    public Vector2Int GetObjPosition(GameObject gameObject)
    {
        return moveObjPositionOnStage[gameObject];
    }

    public bool IsWall(Vector2Int position)
    {
        if (stageTable[position.y, position.x] == STAGE_TYPE.WALL)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public bool IsBlock(Vector2Int position)
    {
        if (stageTable[position.y, position.x] == STAGE_TYPE.BLOCK || stageTable[position.y, position.x] == STAGE_TYPE.BLOCK_ON_POINT)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    private GameObject GetObjectByPosition(Vector2 position)
    {
        foreach(var objPosition in moveObjPositionOnStage)
        {
            if(objPosition.Value == position)
            {
                return objPosition.Key;
            }
        }
        return null;
    }

    public void UpdateObjectPosition(Vector2Int nowPosition,Vector2Int nextPosition,STAGE_TYPE stageType)
    {
        GameObject targetBlock = GetObjectByPosition(nowPosition);
        targetBlock.transform.position = GetScreenPositionFromTileTable(nextPosition);

        moveObjPositionOnStage[targetBlock] = nextPosition;
        if (stageTable[nextPosition.y,nextPosition.x] == STAGE_TYPE.BLOCK_POINT)
        {
            stageTable[nextPosition.y, nextPosition.x] = STAGE_TYPE.BLOCK_ON_POINT;
            goalBlockNum++;
        }
        else
        {
            stageTable[nextPosition.y, nextPosition.x] = stageType;
        }
    }

    public void UpdateStageTableForPlayer(Vector2Int nowPosition, Vector2Int nextPosition)
    {
        if (stageTable[nextPosition.y, nextPosition.x] == STAGE_TYPE.BLOCK_POINT)
        {
            stageTable[nextPosition.y, nextPosition.x] = STAGE_TYPE.PLAYER_ON_POINT;
        }
        else
        {
            stageTable[nextPosition.y, nextPosition.x] = STAGE_TYPE.PLAYER;
        }

        if (stageTable[nowPosition.y, nowPosition.x] == STAGE_TYPE.PLAYER_ON_POINT)
        {
            stageTable[nowPosition.y, nowPosition.x] = STAGE_TYPE.BLOCK_POINT;
        }
        else
        {
            stageTable[nowPosition.y, nowPosition.x] = STAGE_TYPE.GROUND;
        }
    }

    public int GetBlockNum()
    {
        return blockNum;
    }

    public int GetGoalBlockNum()
    {
        return goalBlockNum;
    }

    public void DestroyNowStage()
    {
        GameObject[] tiles = GameObject.FindGameObjectsWithTag("Tile"); 

        foreach(GameObject tile in tiles)
        {
            Destroy(tile);
        }
    }
}
