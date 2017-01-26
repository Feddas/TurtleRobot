using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.UI;
using System;

enum DirectionEnum
{
    Up,
    Right,
    Down,
    Left,
}

public class RunCards : MonoBehaviour
{
    public event EventHandler<EventArgs> FinishedRun;

    public CardLayout CardsProgram;
    public CardLayout CardsFunction;
    public float SecondsPerCard;
    public CollideTurtle TurtleCollider;
    public Transform TurtleGhost;

    private Transform turtle
    {
        get
        {
            if (_turtle == null)
                _turtle = this.transform;
            return _turtle;
        }
    }
    private Transform _turtle;
    private RectTransform turtleRect;
    private List<Card> programToRun;
    private List<Card> function;
    private DirectionEnum facingDirection;
    private bool isRunning;
    private bool hasWon;
    private CollideTurtle turtleCollider;

    private Vector2 startPosition;
    private Quaternion startRotation;
    private DirectionEnum startDirection;

    private Image activeCardInProgram; // used to hightlight cards in the main program
    private Image activeCardInFunction; // used to hightlight cards in the function
    private float runningFunctionIndex; // used to determine how long a function frog card should stay highlighted

    void Start()
    {
        turtleRect = turtle as RectTransform;
        startPosition = turtleRect ? turtleRect.anchoredPosition : (Vector2)turtle.localPosition;
        startRotation = turtle.localRotation;
        startDirection = DirectionEnum.Up;

        turtleCollider = this.GetComponentInChildren<CollideTurtle>();
        turtleCollider.GemReached += TurtleCollider_GemReached;
    }

    void Update()
    {
    }

    private void OnDestroy()
    {
        turtleCollider.GemReached -= TurtleCollider_GemReached;
    }

    public bool OnClickRun()
    {
        isRunning = isRunning == false;
        if (isRunning == false)
            return false;

        // grab cards to run
        programToRun = CardsProgram.GetComponentsInChildren<Card>().ToList();
        function = CardsFunction.GetComponentsInChildren<Card>().ToList();

        // run them
        StartCoroutine(executeProgram());

        return true;
    }

    private void OnFinishedRun()
    {
        isRunning = false;

        // remove any hightlighting
        if (activeCardInProgram) activeCardInProgram.color = Color.white;
        if (activeCardInFunction) activeCardInFunction.color = Color.white;

        // move ghost
        if (hasWon == false)
        {
            TurtleGhost.position = this.transform.position;
            TurtleGhost.localRotation = this.transform.localRotation;
            resetTurtlePosition();
        }

        if (this.FinishedRun != null)
            this.FinishedRun(this, new EventArgs());
    }

    private void resetTurtlePosition()
    {
        // reset back to starting position
        if (turtleRect)
        {
            turtleRect.anchoredPosition = startPosition;
        }
        else
        {
            turtle.localPosition = startPosition;
        }
        turtle.localRotation = startRotation;
        facingDirection = startDirection;
    }

    private void TurtleCollider_GemReached(object sender, EventArgs e)
    {
        hasWon = true;
    }

    IEnumerator executeProgram()
    {
        foreach (var card in programToRun)
        {
            if (isRunning)
            {
                activeCardInProgram = card.GetComponent<Image>();
                activeCardInFunction = null; // erase value from any previous function cards
                yield return StartCoroutine(executeCard(card.Type));
            }
        }
        yield return null; // handles no cards in the program by finishing on a different frame than it started on.

        OnFinishedRun();
        yield return null;
    }

    IEnumerator executeFunction()
    {
        foreach (var card in function)
        {
            if (isRunning)
            {
                activeCardInFunction = card.GetComponent<Image>();
                yield return StartCoroutine(executeCard(card.Type));

                // prep for next card
                runningFunctionIndex++;
            }
        }
        runningFunctionIndex = 0;
        yield return null;
    }

    IEnumerator executeCard(CardTypeEnum cardType)
    {
        //yield return new WaitForSeconds(SecondsPerCard);

        Debug.Log("preforming a " + cardType.ToString());
        switch (cardType)
        {
            case CardTypeEnum.Forward:
                var newPosition = goTowards(facingDirection);
                yield return StartCoroutine(LerpTo((Vector3)newPosition, turtle.transform.localRotation, turtle.transform.localScale, turtle.transform));
                break;
            case CardTypeEnum.TurnLeft:
                updateDirection(ref facingDirection, towardsRight: false);
                yield return StartCoroutine(rotate(turtle.transform, 90)); // lerp turtle.rectTransform.Rotate(0, 0, 90);
                break;
            case CardTypeEnum.TurnRight:
                updateDirection(ref facingDirection, towardsRight: true);
                yield return StartCoroutine(rotate(turtle.transform, -90)); // lerp turtle.rectTransform.Rotate(0, 0, -90);
                break;
            case CardTypeEnum.Function:
                yield return StartCoroutine(executeFunction());
                break;
            case CardTypeEnum.Error:
            default:
                break;
        }
    }

    IEnumerator rotate(Transform model, float eulerDegreesInZ)
    {
        var euler = model.localRotation.eulerAngles;
        var newRotation = Quaternion.Euler(euler.x, euler.y, euler.z + eulerDegreesInZ);
        yield return StartCoroutine(LerpTo(model.localPosition, newRotation, model.localScale, model));
    }

    private void updateDirection(ref DirectionEnum toUpdate, bool towardsRight)
    {
        //Debug.Log("turning right?" + towardsRight + " on " + toUpdate.ToString());
        int increment = towardsRight ? 1 : 3;  // using 3 instead of -1 because % of -1 was failing
        toUpdate = (DirectionEnum)(((int)toUpdate + increment) % 4);
        //Debug.Log("new direction " + toUpdate.ToString());
    }

    private Vector2 goTowards(DirectionEnum direction)
    {
        Vector2 newPosition = turtleRect ? turtleRect.anchoredPosition : (Vector2)turtle.localPosition;
        switch (direction)
        {
            case DirectionEnum.Up:
                newPosition.y += turtleRect ? turtleRect.sizeDelta.y : turtle.localScale.y;
                break;
            case DirectionEnum.Right:
                newPosition.x += turtleRect ? turtleRect.sizeDelta.x : turtle.localScale.x;
                break;
            case DirectionEnum.Down:
                newPosition.y += turtleRect ? -turtleRect.sizeDelta.y : -turtle.localScale.y;
                break;
            case DirectionEnum.Left:
                newPosition.x += turtleRect ? -turtleRect.sizeDelta.x : -turtle.localScale.x;
                break;
            default:
                break;
        }

        //Debug.Log(turtle.rectTransform.anchoredPosition + "'s newPosition " + (Vector3)newPosition + " delta:" + turtle.rectTransform.sizeDelta + " direction:" + direction);
        //Debug.Log("Moving from " + turtle.rectTransform.anchoredPosition + " to " + (Vector3)newPosition);
        //turtle.rectTransform.localPosition = (Vector3)newPosition;
        return newPosition;
    }

    IEnumerator LerpTo(Vector3 endPosition, Quaternion endRotation, Vector3 endScale, Transform model)
    {
        //Debug.Log(model.anchoredPosition + "Moving from " + turtle.rectTransform.anchoredPosition + " to " + endPosition);
        //Debug.Log("LerpTo scale " + endScale + this.name);
        float percent = 0.0f;

        Vector3 startPosition = model.localPosition;
        Quaternion startRotation = model.localRotation;
        Vector3 startScale = model.localScale;
        while (percent <= 1.0 && isRunning)
        {
            percent += Time.deltaTime / SecondsPerCard;
            float percentEased = Mathf.SmoothStep(0, 1, percent);
            model.localPosition = Vector3.Lerp(startPosition, endPosition, percentEased);
            model.localRotation = Quaternion.Lerp(startRotation, endRotation, percentEased);
            model.localScale = Vector3.Lerp(startScale, endScale, percentEased);

            highlightActiveCards(percent);
            yield return null;
        }
    }

    void highlightActiveCards(float lerpPercent)
    {
        // It's a function card, highlight function frog card and card inside of function
        if (activeCardInFunction)
        {
            // highlight the function card
            highlightCard(ref activeCardInFunction, lerpPercent);

            // keep the card highlighted in the main program until the function is completed
            if (function.Count <= 1) // note: this function should never be called if function.Count == 0, but "< 1" is there just incase
                highlightCard(ref activeCardInProgram, lerpPercent);
            else if (runningFunctionIndex == 0 && lerpPercent < .5)
                highlightCard(ref activeCardInProgram, lerpPercent);
            else if (runningFunctionIndex == function.Count - 1 && lerpPercent >= .5)
                highlightCard(ref activeCardInProgram, lerpPercent);
        }

        // Not a function frog card, highlight it for it's single movement
        else
        {
            highlightCard(ref activeCardInProgram, lerpPercent);
        }
    }

    void highlightCard(ref Image imageToHightlight, float lerpPercent)
    {
        if (lerpPercent < .5)
        {
            imageToHightlight.color = Color.Lerp(Color.white, Color.green, lerpPercent * 2);
        }
        if (lerpPercent >= .5)
        {
            imageToHightlight.color = Color.Lerp(Color.green, Color.white, (lerpPercent - 0.5f) * 2);
        }
    }
}
