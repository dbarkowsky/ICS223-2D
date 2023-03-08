using Newtonsoft.Json.Linq;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class SceneController : MonoBehaviour
{
    [SerializeField] private GameObject cardPrefab;
    [SerializeField] private Transform cardSpawnPoint;
    [SerializeField] private Sprite[] cardImages;
    [SerializeField] private TextMeshProUGUI scoreText;

    // Gameplay variables
    List<Card> cards; // stores all cards
    private Card card1; // first card guessed
    private Card card2; // second ""
    private int score = 0;
    private bool allowClicks = true;

    private void Start()
    {
        cards = CreateCards();
        AssignImagesToCards();

        foreach(Card card in cards)
        {
            card.SetFaceVisible(false);
        }
    }

    private void Awake()
    {
        Messenger<Card>.AddListener(GameEvent.CARD_CLICKED, this.OnCardClicked); // generic parameter is what's passed to CardClicked
    }

    private void OnDestroy()
    {
        Messenger<Card>.RemoveListener(GameEvent.CARD_CLICKED, this.OnCardClicked);
    }

    // Create and return a single card at the given position
    Card CreateCard(Vector3 pos)
    {
        // instatiate the card prefab
        GameObject obj = Instantiate(cardPrefab, pos, cardPrefab.transform.rotation);
        Card card = obj.GetComponent<Card>();
        return card;
    }

    // Create (and return) a List of cards organized in a grid layout
    private List<Card> CreateCards()
    {
        List<Card> newCards = new List<Card>();
        int rows = 2;           // # of rows
        int cols = 4;           // # of columns
        float xOffset = 2f;     // # of units between cards horizontally
        float yOffset = -2.5f;  // # of units between cards vertically

        // Create cards and position on a grid
        for (int y = 0; y < rows; y++)
        {
            for (int x = 0; x < cols; x++)
            {
                Vector3 offset = new Vector3(x * xOffset, y * yOffset, 0);    // calculate the offset                
                Card card = CreateCard(cardSpawnPoint.position + offset);     // create the card          
                newCards.Add(card);                                           // add the card to the list
            }
        }
        return newCards;
    }

    // Assign images to the cards in pairs
    private void AssignImagesToCards()
    {
        // create a list of paired image indices - the # of entries MUST match the # of cards.
        // eg: [0,0,1,1,2,2,3,3]
        List<int> imageIndices = new List<int>();
        for (int i = 0; i < cardImages.Length; i++)
        {
            imageIndices.Add(i);    // one index for the first card in the pair
            imageIndices.Add(i);    // one index for the second
        }

        // *** TODO: write code to shuffle the list of image indices
        //imageIndices.Sort((a, b) => Random.Range(0, cardImages.Length) > a);
        imageIndices = imageIndices.OrderBy(value => (value + 5) * Random.Range(0, 10)).ToList();

        // Go through each card in the game and assign it an image based on the (shuffled) list of indices.
        for (int i = 0; i < cards.Count; i++)
        {
            int imageIndex = imageIndices[i];           // use the card # to index into the imageIndices array
            cards[i].SetSprite(cardImages[imageIndex]); // set the image on the card
        }
    }

    public void OnCardClicked(Card card)
    {
        if (allowClicks && !card.IsFaceVisible())
        {
            allowClicks = false;
            if (card1 == null)
            { // if first card
                card1 = card;
                card1.SetFaceVisible(true);
                allowClicks = true;
            }
            else if (card2 == null) // if second card
            {
                card2 = card;
                card2.SetFaceVisible(true);

                StartCoroutine(EvaluatePair());
            }
        }
    }

    IEnumerator EvaluatePair()
    {
        yield return new WaitForSecondsRealtime(1f);
        // if cards match
        if (card1.GetSprite() == card2.GetSprite())
        {
            Debug.Log("match");
            score++;
            scoreText.text = "Score: " + score.ToString();
        } else
        {
            Debug.Log("not a match");
            // swap cards
            float duration = 0.75f;
            Vector3 card1Pos = card1.transform.position;
            Vector3 card2Pos = card2.transform.position;
            iTween.MoveTo(card1.gameObject, card2Pos, duration);
            iTween.MoveTo(card2.gameObject, card1Pos, duration);

            // set face down
            yield return new WaitForSecondsRealtime(0.25f);
            card1.SetFaceVisible(false);
            card2.SetFaceVisible(false);
        }
        allowClicks = true;

        // reset card memory
        card1 = null;
        card2 = null;
    }

    public void OnResetButtonPressed()
    {
        Reset();
    }

    private void Reset()
    {
        score = 0;
        scoreText.text = "Score: " + score.ToString();
        card1 = null;
        card2 = null;

        // turn cards face down
        foreach(Card card in cards)
        {
            card.SetFaceVisible(false);
        }

        // randomize card images
        AssignImagesToCards();
    }
}
