using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.UI;

enum DirectionEnum
{
    Up,
    Right,
    Down,
    Left,
}

public class RunCards : MonoBehaviour
{
    public CardLayout CardsProgram;
    public CardLayout CardsFunction;
    public float SecondsPerCard;

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

    private Vector2 startPosition;
    private Quaternion startRotation;
    private DirectionEnum startDirection;

    void Start()
    {
        turtleRect = turtle as RectTransform;
        startPosition = turtleRect ? turtleRect.anchoredPosition : (Vector2)turtle.localPosition;
        startRotation = turtle.localRotation;
        startDirection = DirectionEnum.Up;
    }

    void Update()
    {
    }

    public void OnClickRun()
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

        isRunning = isRunning == false;
        if (isRunning == false)
            return;

        // grab cards to run
        programToRun = CardsProgram.GetComponentsInChildren<Card>().ToList();
        function = CardsFunction.GetComponentsInChildren<Card>().ToList();

        // run them
        StartCoroutine(executeProgram());
    }

    IEnumerator executeProgram()
    {
        foreach (var card in programToRun)
        {
            yield return StartCoroutine(executeCard(card.Type));
        }

        isRunning = false;
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
                foreach (var card in function)
                {
                    yield return StartCoroutine(executeCard(card.Type));
                }
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
            yield return null;
        }
    }
}
