using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dot : MonoBehaviour
{
    public int column, row, targetX, targetY, previousColumn, previousRow;
    public bool isMatched = false;
    private Board board;
    private FindMatches findMatches;
    private GameObject otherDot;
    private HintManager hintManager;
    private Vector2 firstTouch, finalTouch, tempPosition;
    [SerializeField] float swipeAngle, swipeResist = 1f;

    void Start()
    {
        hintManager = FindObjectOfType<HintManager>();
        board = FindObjectOfType<Board>();
        findMatches = FindObjectOfType<FindMatches>();
        targetX = (int)transform.position.x;
        targetY = (int)transform.position.y;
    }

    void Update()
    {
        if (isMatched)
        {
            SpriteRenderer mySprite = GetComponent<SpriteRenderer>();
            mySprite.color = new Color(1f, 1f, 1f, .2f);
        }
        targetX = column;
        targetY = row;
        if(Mathf.Abs(targetX - transform.position.x) > .1)
        {
            tempPosition = new Vector2(targetX, transform.position.y);
            transform.position = Vector2.Lerp(transform.position, tempPosition, .4f);
            if (board.allDots[column, row] != this.gameObject){
                board.allDots[column, row] = this.gameObject;
            }
            findMatches.FindAllMatches();
        } else
        {
            tempPosition = new Vector2(targetX, transform.position.y);
            transform.position = tempPosition;
        }
        if (Mathf.Abs(targetY - transform.position.y) > .1)
        {
            tempPosition = new Vector2(transform.position.x, targetY);
            transform.position = Vector2.Lerp(transform.position, tempPosition, .4f);
            if (board.allDots[column, row] != this.gameObject)
            {
                board.allDots[column, row] = this.gameObject;
            }
            findMatches.FindAllMatches();
        }
        else
        {
            tempPosition = new Vector2(transform.position.x, targetY);
            transform.position = tempPosition;
        }
    }

    private IEnumerator CheckMoveCo()
    {
        yield return new WaitForSeconds(.5f);
        if(otherDot != null)
        {
            if (!isMatched && !otherDot.GetComponent<Dot>().isMatched)
            {
                otherDot.GetComponent<Dot>().row = row;
                otherDot.GetComponent<Dot>().column = column;
                row = previousRow;
                column = previousColumn;
                board.currentState = GameState.move;
            }
            else
            {
                board.DestroyMatches();
            }
            otherDot = null;
        }
        yield return new WaitForSeconds(.2f);
    }

    private void OnMouseDown()
    {
        if (hintManager != null)
        {
            hintManager.DestroyHint();
        }
        if (board.currentState == GameState.move)
        {
            firstTouch = Input.mousePosition;
        }
    }

    private void OnMouseUp()
    {
        if(board.currentState == GameState.move)
        {
            finalTouch = Input.mousePosition;
            CalculateAngle();
        }
    }
    public void MoveGem(int offsetX, int offsetY)
    {
        if (board.currentState == GameState.move)
        {
            firstTouch = gameObject.transform.position;
            finalTouch = new Vector2(gameObject.transform.position.x + offsetX, gameObject.transform.position.y + offsetY);
            CalculateAngle();
        }
        else
        {
            Debug.Log("Game board not in move state");
        }
    }

    private void CalculateAngle()
    {
        if(Mathf.Abs(finalTouch.y - firstTouch.y) > swipeResist || Mathf.Abs(finalTouch.x - firstTouch.x) > swipeResist)
        {
            board.currentState = GameState.wait;
            swipeAngle = Mathf.Atan2(finalTouch.y - firstTouch.y, finalTouch.x - firstTouch.x) * 180 / Mathf.PI;
            MovePieces();
        }
        else
        {
            board.currentState = GameState.move;
        }
    }

    void MovePiecesActual(Vector2 direction)
    {
        otherDot = board.allDots[column + (int)direction.x, row + (int)direction.y];
        previousRow = row;
        previousColumn = column;
        if (otherDot != null)
        {
            otherDot.GetComponent<Dot>().column += -1 * (int)direction.x;
            otherDot.GetComponent<Dot>().row += -1 * (int)direction.y;
            column += (int)direction.x;
            row += (int)direction.y;
            StartCoroutine(CheckMoveCo());
        }
        else
        {
            board.currentState = GameState.move;
        }
    }

    void MovePieces()
    {
        if (swipeAngle > -45 && swipeAngle <= 45 && column < board.width - 1)
        {
            MovePiecesActual(Vector2.right);
        }
        else if (swipeAngle > 45 && swipeAngle <= 135 && row < board.height - 1)
        {
            MovePiecesActual(Vector2.up);
        }
        else if ((swipeAngle > 135 || swipeAngle <= -135) && column > 0)
        {
            MovePiecesActual(Vector2.left);
        }
        else if (swipeAngle < -45 && swipeAngle >= -135 && row > 0)
        {
            MovePiecesActual(Vector2.down);
        }
        else
        {
            board.currentState = GameState.move;
        }
    }
}
