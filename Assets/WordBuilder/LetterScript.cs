using UnityEngine;

public class LetterScript : MonoBehaviour
{
    private static Sprite[] sprites;
    public static Sprite GetSprite(int index)
    {
        if (sprites == null)
            sprites = Resources.LoadAll<Sprite>("letterblocks");
        if (index >= sprites.Length) return null;
        return sprites[index];
    }

    public char letter = 'a';
    public bool isStatic = false;
    bool animating;

    void Start()
    {
        ChangeSprite();
    }


    void OnValidate()
    {
        ChangeSprite();
    }
    public void ChangeSprite()
    {
        if (char.IsLetter(letter))
        {
            char upper = char.ToUpper(letter);
            int offset = upper - 'A';
            Sprite s = GetSprite(offset);
            if (s != null)
                GetComponent<SpriteRenderer>().sprite = s;
        }

    }

    Vector3 offset = Vector3.zero;
    private Vector3 origin;
    private bool caught, shaking;

    void OnMouseDown()
    {
        if (isStatic || animating || !WordManager.instance.isInteractive) return;
        caught = true;
        offset = GetWorldPos() - transform.position;
        origin = transform.position;
    }
    void OnMouseDrag()
    {
        if (isStatic || animating || !caught || !WordManager.instance.isInteractive) return;
        Vector3 worldPos = GetWorldPos();
        transform.position = worldPos - offset;
    }
    void OnMouseUp()
    {
        if (!WordManager.instance.isInteractive) return;
        if (caught)
        {
            caught = false;
            shaking = !TryPlaceBlock();
            if (shaking)
            {
                startshake = Time.time;
                WordManager.instance.audio.clip = Resources.Load<AudioClip>("kidsounds/SuccessFailPrompt/oopsDoesntGoThere");
                WordManager.instance.audio.Play();
            }
            animating = true;
        }
    }
    float startshake;
    bool TryPlaceBlock()
    {
        int index = WordManager.instance.GetLetterIndex(transform.position);
        if (index == -1 || WordManager.instance.IsFull(index)) return true;
        if (!WordManager.instance.IsCorrect(index, letter)) return false;
        WordManager.instance.Fill(index);
        origin = WordManager.instance.GetLetterPos(index);
        isStatic = true;
        if (WordManager.instance.remainingLetters.Contains(this))
            WordManager.instance.remainingLetters.Remove(this);
        return true;
    }

    public float speed = 0.2f;
    void Update()
    {
        if (animating)
        {
            if (shaking)
            {
                float timediff = Time.time - startshake;
                float period = timediff * 30f;
                float rot = 30.0f * Mathf.Sin(period);
                transform.eulerAngles = new Vector3(0, 0, rot);
                int rotations = 3;
                if (period > Mathf.PI * 2f * rotations)
                {
                    shaking = false;
                    transform.eulerAngles = Vector3.zero;
                }
            }
            else
            {
                transform.position = Vector3.MoveTowards(transform.position, origin, speed);
                if (transform.position == origin)
                {
                    animating = false;
                    WordManager.instance.CheckWordComplete();
                }
            }
        }
    }
    Vector3 GetWorldPos()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        return ray.origin + ray.direction * -Camera.main.transform.position.z;
    }
}
