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

[RequireComponent(typeof(Image))]
public class RunCards : MonoBehaviour
{
    public CardLayout CardsProgram;
    public CardLayout CardsFunction;
    public float SecondsPerCard;

    private Image turtle;
    private List<Card> programToRun;
    private List<Card> function;
    private DirectionEnum facingDirection;
    private bool isRunning;

    private Vector2 startPosition;
    private Quaternion startRotation;
    private DirectionEnum startDirection;

    void Start()
    {
        turtle = this.GetComponent<Image>();
        startPosition = turtle.rectTransform.anchoredPosition;
        startRotation = turtle.rectTransform.localRotation;
        startDirection = DirectionEnum.Down;
    }

    void Update()
    {
    }

    public void OnClickRun()
    {
        // reset back to starting position
        turtle.rectTransform.anchoredPosition = startPosition;
        turtle.rectTransform.localRotation = startRotation;
        facingDirection = startDirection;

        isRunning = isRunning == false;
        if (isRunning == false)
            return;

        // grab cards to run
        programToRun = new List<Card>();
        programToRun = CardsProgram.GetComponentsInChildren<Card>().ToList();

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

        //Debug.Log("preforming a " + cardType.ToString());
        switch (cardType)
        {
            case CardTypeEnum.Forward:
                var newPosition = goTowards(facingDirection);
                yield return StartCoroutine(LerpTo((Vector3)newPosition, turtle.rectTransform.localRotation, turtle.rectTransform.localScale, turtle.rectTransform));
                break;
            case CardTypeEnum.TurnLeft:
                updateDirection(ref facingDirection, towardsRight: false);
                yield return StartCoroutine(rotate(turtle.rectTransform, 90)); // lerp turtle.rectTransform.Rotate(0, 0, 90);
                break;
            case CardTypeEnum.TurnRight:
                updateDirection(ref facingDirection, towardsRight: true);
                yield return StartCoroutine(rotate(turtle.rectTransform, -90)); // lerp turtle.rectTransform.Rotate(0, 0, -90);
                break;
            case CardTypeEnum.Function:
            case CardTypeEnum.Error:
            default:
                break;
        }
    }

    IEnumerator rotate(RectTransform model, float eulerDegreesInZ)
    {
        var euler = turtle.rectTransform.localRotation.eulerAngles;
        var newRotation = Quaternion.Euler(euler.x, euler.y, euler.z + eulerDegreesInZ);
        yield return StartCoroutine(LerpTo(turtle.rectTransform.localPosition, newRotation, turtle.rectTransform.localScale, turtle.rectTransform));
    }

    private void updateDirection(ref DirectionEnum toUpdate, bool towardsRight)
    {
        //Debug.Log("turning right?" + towardsRight + " on " + toUpdate.ToString());
        int increment = towardsRight ? 1 : -1;
        toUpdate = (DirectionEnum)(((int)toUpdate + increment) % 4);
        //Debug.Log("new direction " + toUpdate.ToString());
    }

    private Vector2 goTowards(DirectionEnum direction)
    {
        Vector2 newPosition = turtle.rectTransform.anchoredPosition;
        switch (direction)
        {
            case DirectionEnum.Up:
                newPosition.y += turtle.rectTransform.sizeDelta.y;
                break;
            case DirectionEnum.Right:
                newPosition.x += turtle.rectTransform.sizeDelta.x;
                break;
            case DirectionEnum.Down:
                newPosition.y += -turtle.rectTransform.sizeDelta.y;
                break;
            case DirectionEnum.Left:
                newPosition.x += -turtle.rectTransform.sizeDelta.x;
                break;
            default:
                break;
        }

        //Debug.Log("Moving from " + turtle.rectTransform.anchoredPosition + " to " + (Vector3)newPosition);
        //turtle.rectTransform.localPosition = (Vector3)newPosition;
        return newPosition;
    }

    IEnumerator LerpTo(Vector3 endPosition, Quaternion endRotation, Vector3 endScale, RectTransform model)
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
