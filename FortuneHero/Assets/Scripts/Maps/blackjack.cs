using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Cinemachine;            // Cinemachine v3
using UnityEngine;
using UnityEngine.UI;

public class BlackjackComponent : MonoBehaviour, IInteractable
{
    public float exitTime { get => throw new System.NotImplementedException(); set => new System.NotImplementedException(); }

    [Header("UI Panels")]
    [SerializeField] GameObject panelBlackjack;   // comme panelSlot
    [SerializeField] GameObject panelPlayer;      // comme panelPlayer de la slot

    [Header("UI Elements")]
    [SerializeField] Button btnDeal;
    [SerializeField] Button btnHit;
    [SerializeField] Button btnStand;
    [SerializeField] Button btnIncreaseBet;
    [SerializeField] Button btnDecreaseBet;

    [SerializeField] TextMeshProUGUI playerHandText;
    [SerializeField] TextMeshProUGUI dealerHandText;
    [SerializeField] TextMeshProUGUI resultText;
    [SerializeField] TextMeshProUGUI betText;
    [SerializeField] TextMeshProUGUI balanceText;

    [Header("Cinemachine (même pattern que la slot)")]
    [SerializeField] CinemachineCamera mainVcam;     // caméra de jeu par défaut
    [SerializeField] CinemachineCamera tableVcam;    // caméra de la table de blackjack
    [SerializeField] int mainPriority = 10;          // priorité par défaut de la caméra principale
    [SerializeField] int tableInactivePriority = 0;  // priorité quand la caméra table est idle
    [SerializeField] int tableActivePriority = 20;   // priorité quand on veut focus la table
                                                     // [NOUVEAU] – juste sous "Blackjack Settings"
    [Header("Tempo & Anim du croupier")]
    [SerializeField] float dealerDrawDelay = 0.25f;    // délai entre tirages du croupier
    [SerializeField] float dealerFlipDuration = 0.6f;  // durée de l'anim (deal / flip)
    [SerializeField] float dealerArcHeight = 0.12f;    // hauteur de l'arc de la carte
    [SerializeField] float dealerSideOffset = 0.35f;   // décalage latéral (comme la main du croupier)


    private CinemachineBrain brain;
    private Coroutine blendWaitCo;
    private bool pendingOpen;
    private bool dealerHoleRevealed = false;


    [Header("Blackjack Settings")]
    [SerializeField] int currentBet = 10;
    [SerializeField] int minBet = 5;
    [SerializeField] int maxBet = 100;

    [Header("Cartes en PREFABS (par enseigne)")]
    [Tooltip("13 prefabs chacun, index 0..12 = A..K")]
    public GameObject[] clubsPrefabs;    // ♣
    public GameObject[] diamondsPrefabs; // ♦
    public GameObject[] heartsPrefabs;   // ♥
    public GameObject[] spadesPrefabs;   // ♠
    public GameObject backCardPrefab;    // Dos de carte (pour cacher la 2e du croupier)

    [Header("Containers UI pour afficher les cartes")]
    [SerializeField] Transform playerCardsContainer;  // Empty + HorizontalLayoutGroup
    [SerializeField] Transform dealerCardsContainer;  // Empty + HorizontalLayoutGroup

    // --- État du jeu ---
    private readonly List<int> deck = new List<int>(52);
    private readonly List<int> playerCards = new List<int>(10);
    private readonly List<int> dealerCards = new List<int>(10);
    private bool roundActive = false;
    private bool playerTurn = false;

    void Start()
    {
        brain = PlayerComponent.Instance.transform.root.GetComponentInChildren<CinemachineBrain>();

        // Assure des priorités connues au démarrage
        if (mainVcam != null) mainVcam.Priority = mainPriority;
        if (tableVcam != null) tableVcam.Priority = tableInactivePriority;

        if (!PlayerPrefs.HasKey("coins"))
            PlayerPrefs.SetInt("coins", 200);

        UpdateBalanceText();

        if (panelBlackjack != null) panelBlackjack.SetActive(false);
        if (panelPlayer != null) panelPlayer.SetActive(true);

        ClearTexts();
        UpdateBetText();
        ClearCardContainers();
    }
    public void Enter() { }
    public void Exit() { }

    public void Interact()
    {
        bool isOpen = panelBlackjack != null && panelBlackjack.activeSelf;

        if (isOpen)
        {
            CloseTableAndReturnCamera();
        }
        else
        {
            OpenWithCameraBlend();
        }
    }

    private void OpenWithCameraBlend()
    {
        if (mainVcam != null) mainVcam.Priority = mainPriority;
        if (tableVcam != null) tableVcam.Priority = tableActivePriority;

        pendingOpen = true;

        if (blendWaitCo != null) StopCoroutine(blendWaitCo);
        blendWaitCo = StartCoroutine(WaitForCameraBlendThenOpenMenu());
    }

    private IEnumerator WaitForCameraBlendThenOpenMenu()
    {
        if (brain == null)
        {
            ForceOpenTableMenuNow();
            yield break;
        }

        yield return null; // démarrage du blend

        while (brain.IsBlending) yield return null;
        yield return null; // petite frame de sécu

        if (pendingOpen)
        {
            ForceOpenTableMenuNow();
        }
    }

    private void ForceOpenTableMenuNow()
    {
        pendingOpen = false;

        if (panelBlackjack != null) panelBlackjack.SetActive(true);
        if (panelPlayer != null) panelPlayer.SetActive(false);

        // Brancher les boutons à chaque ouverture (comme ta slot)
        if (btnDeal != null)
        {
            btnDeal.onClick.RemoveAllListeners();
            btnDeal.onClick.AddListener(Deal);
            btnDeal.interactable = true;
        }
        if (btnHit != null)
        {
            btnHit.onClick.RemoveAllListeners();
            btnHit.onClick.AddListener(Hit);
            btnHit.interactable = false;
        }
        if (btnStand != null)
        {
            btnStand.onClick.RemoveAllListeners();
            btnStand.onClick.AddListener(Stand);
            btnStand.interactable = false;
        }
        if (btnIncreaseBet != null)
        {
            btnIncreaseBet.onClick.RemoveAllListeners();
            btnIncreaseBet.onClick.AddListener(IncreaseBet);
        }
        if (btnDecreaseBet != null)
        {
            btnDecreaseBet.onClick.RemoveAllListeners();
            btnDecreaseBet.onClick.AddListener(DecreaseBet);
        }

        UpdateBetText();
        UpdateBalanceText();
        ClearTexts();
        ClearCardContainers();

        // Gel du jeu visuel, mais on laisse le script Player lire l’input (pour Interact)
        Time.timeScale = 0f;
        Cursor.lockState = CursorLockMode.None;
    }

    private void CloseTableAndReturnCamera()
    {
        Time.timeScale = 1f;
        Cursor.lockState = CursorLockMode.Locked;

        if (panelBlackjack != null) panelBlackjack.SetActive(false);
        if (panelPlayer != null) panelPlayer.SetActive(true);
        if (resultText != null) resultText.text = "";

        pendingOpen = false;
        if (blendWaitCo != null)
        {
            StopCoroutine(blendWaitCo);
            blendWaitCo = null;
        }

        if (mainVcam != null) mainVcam.Priority = mainPriority;
        if (tableVcam != null) tableVcam.Priority = tableInactivePriority;
    }

    // ======== Mises / Balance ========
    void IncreaseBet()
    {
        if (roundActive) return;
        if (currentBet < maxBet)
        {
            currentBet += 5;
            UpdateBetText();
        }
    }

    void DecreaseBet()
    {
        if (roundActive) return;
        if (currentBet > minBet)
        {
            currentBet -= 5;
            UpdateBetText();
        }
    }

    void UpdateBetText()
    {
        if (betText != null) betText.text = $"Current bet: {currentBet}"; // comme ta slot (juste le nombre)
    }

    void UpdateBalanceText()
    {
        int coins = PlayerPrefs.GetInt("coins");
        if (balanceText != null) balanceText.text = $"Balance : {coins} coins";
    }

    // ======== Affichage (texte + prefabs) ========
    void ClearTexts()
    {
        if (playerHandText != null) playerHandText.text = "Joueur :";
        if (dealerHandText != null) dealerHandText.text = "Croupier :";
        if (resultText != null) resultText.text = "";
    }
    static Quaternion LocalCardOrientation(bool faceDown)
    {
        // visible :  X=-90, Y=90, Z=90
        // cachée  :  X= 90, Y=90, Z=90
        return faceDown
            ? Quaternion.Euler(90f, 90f, 90f)
            : Quaternion.Euler(-90f, 90f, 90f);
    }

    static Transform CreateHolder(Transform parent, int index)
    {
        var holder = new GameObject($"CardHolder_{index}").transform;
        holder.SetParent(parent, false);

        float scale = 1.3f;                      
        float spacing = 0.22f * scale;            

        holder.localPosition = new Vector3(-spacing * index, 0.018f, 0f);
        holder.localRotation = Quaternion.identity;
        holder.localScale = Vector3.one * scale; 

        return holder;
    }

    static Transform GetVisualTransform(Transform cardRoot)
    {
        var mr = cardRoot.GetComponentInChildren<MeshRenderer>();
        if (mr != null) return mr.transform;

        var smr = cardRoot.GetComponentInChildren<SkinnedMeshRenderer>();
        if (smr != null) return smr.transform;

        return cardRoot; // fallback
    }



    static void PoseCard(Transform parent, Transform card, int index, bool faceDown)
    {
        // Crée un frame neutre sous le container
        var holder = CreateHolder(parent, index);

        // Place le root du prefab dans le holder (neutre)
        card.SetParent(holder, false);
        card.localPosition = Vector3.zero;
        card.localEulerAngles = new Vector3(90f, 90f, 90f);
        Debug.Log(card.localRotation);

        // Oriente le VISUEL (mesh) pour imposer X/Y/Z = (-90|90|90) ou (90|90|90)
        var vis = GetVisualTransform(card);
        vis.localScale = Vector3.one * 1.6f;   // grossit seulement la carte, pas le holder

        vis.localPosition = Vector3.zero;
        //vis.localRotation = LocalCardOrientation(faceDown);
        vis.localEulerAngles = faceDown
            ? new Vector3(90f, 90f, 90f)
            : new Vector3(-90f, 90f, 90f);
        Debug.Log(vis.localRotation);
        
    }


    void RefreshHands(bool showDealerHole)
    {
        ClearCardContainers();

        // ----- JOUEUR -----
        for (int i = 0; i < playerCards.Count; i++)
        {
            var pf = GetPrefabForCard(playerCards[i]);
            if (pf != null && playerCardsContainer != null)
            {
                var go = Instantiate(pf);                 // on instancie d’abord sans parent
                PoseCard(playerCardsContainer, go.transform, i, faceDown: false);
            }
        }

        // ----- CROUPIER -----
        for (int i = 0; i < dealerCards.Count; i++)
        {
            bool holeCardHidden = (!showDealerHole && i == 1);

            var pf = GetPrefabForCard(dealerCards[i]);
            if (pf != null && dealerCardsContainer != null)
            {
                var go = Instantiate(pf);
                PoseCard(dealerCardsContainer, go.transform, i, holeCardHidden);

                // si on révèle la hole card (i == 1) : flip 90 -> -90 sur X en local du holder
                // si on révèle la hole card (i == 1) : flip 90 -> -90 une seule fois
                if (showDealerHole && i == 1 && !dealerHoleRevealed)
                {
                    dealerHoleRevealed = true;

                    var e = go.transform.localEulerAngles;
                    e.x = 90f;
                    go.transform.localEulerAngles = e;

                    StartCoroutine(AnimateFlip(go.transform, 90f, -90f, dealerFlipDuration));
                }

            }
        }

        if (playerHandText != null)
            playerHandText.text = "Joueur : " + $" ({HandValue(playerCards)})";

        if (dealerHandText != null)
        {
            if (showDealerHole)
            {
                // Les deux cartes visibles → afficher la valeur totale
                dealerHandText.text = $"Croupier : ({HandValue(dealerCards)})";
            }
            else
            {
                if (dealerCards.Count > 0)
                {
                    int visibleCard = dealerCards[0];                      // première carte
                    int visibleValue = ValueForRank(Rank(visibleCard));    // valeur d’une seule carte

                    dealerHandText.text = $"Croupier : {visibleValue}";
                }
                else
                {
                    dealerHandText.text = "Croupier : ";
                }
            }
        }

    }




    void ClearCardContainers()
    {
        ClearContainer(playerCardsContainer);
        ClearContainer(dealerCardsContainer);
    }

    static void ClearContainer(Transform t)
    {
        if (t == null) return;
        for (int i = t.childCount - 1; i >= 0; i--)
            Object.Destroy(t.GetChild(i).gameObject);
    }

    // ======== BlackJack logique ========
    void Deal()
    {
        if (roundActive) return;

        // Validation minimale pour éviter les NullRef silencieuses
        if (!CanRenderHands())
        {
            if (resultText != null) resultText.text = "Config cartes incomplète (containers/prefabs).";
            return;
        }

        int coins = PlayerPrefs.GetInt("coins");
        if (coins < currentBet)
        {
            if (resultText != null) resultText.text = "Pas assez de coins !";
            return;
        }

        // Débite la mise
        coins -= currentBet;
        PlayerPrefs.SetInt("coins", coins);
        UpdateBalanceText();

        // Init manche
        roundActive = true;
        playerTurn = true;
        if (resultText != null) resultText.text = "";
        dealerHoleRevealed = false;


        BuildAndShuffleDeck();

        playerCards.Clear();
        dealerCards.Clear();

        // Distribution initiale (2-2)
        playerCards.Add(DrawCard());
        dealerCards.Add(DrawCard());
        playerCards.Add(DrawCard());
        dealerCards.Add(DrawCard());

        // Affichage avec carte du croupier cachée
        RefreshHands(showDealerHole: false);

        // Active les actions joueur
        if (btnHit != null) btnHit.interactable = true;
        if (btnStand != null) btnStand.interactable = true;

        // Blackjack naturel ?
        int playerVal = HandValue(playerCards);
        int dealerVal = HandValue(dealerCards);

        if (playerVal == 21 || dealerVal == 21)
        {
            RefreshHands(showDealerHole: true);
            ResolveImmediateNaturals(playerVal, dealerVal);
        }
    }

    void Hit()
    {
        if (!roundActive || !playerTurn) return;

        playerCards.Add(DrawCard());
        RefreshHands(showDealerHole: false);

        int val = HandValue(playerCards);
        if (val > 21)
        {
            EndRound(playerWon: false, isBlackjackNatural: false, isPush: false);
        }
    }

    void Stand()
    {
        if (!roundActive || !playerTurn) return;

        playerTurn = false;
        StartCoroutine(DealerPlayThenResolve());
    }

    IEnumerator DealerPlayThenResolve()
    {
        // Révèle la carte cachée avec anim de flip (géré par RefreshHands(true))
        RefreshHands(showDealerHole: true);

        // petit délai avant que le croupier commence à tirer
        yield return new WaitForSecondsRealtime(dealerDrawDelay);

        // Tirage jusqu'à 17+
        while (HandValue(dealerCards) < 17)
        {
            // délai entre chaque tirage
            yield return new WaitForSecondsRealtime(dealerDrawDelay);

            // ajoute une carte
            dealerCards.Add(DrawCard());

            // Rafraîchit l'affichage (instancie la nouvelle carte à sa position finale)
            RefreshHands(showDealerHole: true);

            // Récupère le dernier "holder" du croupier
            if (dealerCardsContainer != null && dealerCardsContainer.childCount > 0)
            {
                var holder = dealerCardsContainer.GetChild(dealerCardsContainer.childCount - 1);

                // position / rotation finale déjà posées par PoseCard
                Vector3 endPos = holder.localPosition;
                Quaternion endRot = holder.localRotation;

                // point de départ : un peu sur le côté + vers le croupier
                Vector3 startPos = endPos
                    + holder.right * dealerSideOffset   // vers la droite (main du dealer)
                    + holder.forward * 0.25f;           // vers lui
                startPos.y += dealerArcHeight * 0.5f;   // légèrement plus haut

                // rotation de départ : carte un peu tournée comme si elle sortait de la main
                Quaternion startRot = Quaternion.Euler(-60f, 40f, 120f);

                // anim complète (montée + arc + rotation)
                yield return AnimateDealerCard(
                    holder,
                    startPos,
                    endPos,
                    startRot,
                    endRot,
                    dealerFlipDuration,
                    dealerArcHeight
                );
            }
        }

        int p = HandValue(playerCards);
        int d = HandValue(dealerCards);

        if (d > 21) { EndRound(true, false, false); }
        else if (p > d) { EndRound(true, false, false); }
        else if (p < d) { EndRound(false, false, false); }
        else { EndRound(false, false, true); } // push
    }


    void ResolveImmediateNaturals(int playerVal, int dealerVal)
    {
        if (playerVal == 21 && dealerVal == 21)
        {
            EndRound(false, false, true);
        }
        else if (playerVal == 21)
        {
            EndRound(true, true, false);  // 2.5x total
        }
        else if (dealerVal == 21)
        {
            EndRound(false, false, false);
        }
    }

    IEnumerator AnimateDealerCard(
    Transform tr,
    Vector3 startLocalPos,
    Vector3 endLocalPos,
    Quaternion startLocalRot,
    Quaternion endLocalRot,
    float duration,
    float arcHeight)
    {
        float t = 0f;

        tr.localPosition = startLocalPos;
        tr.localRotation = startLocalRot;

        while (t < duration)
        {
            t += Time.unscaledDeltaTime;
            float u = Mathf.Clamp01(t / duration);

            // trajectoire de base linéaire
            Vector3 basePos = Vector3.Lerp(startLocalPos, endLocalPos, u);
            // petit arc vers le haut
            float extraY = Mathf.Sin(u * Mathf.PI) * arcHeight;

            tr.localPosition = basePos + Vector3.up * extraY;
            tr.localRotation = Quaternion.Slerp(startLocalRot, endLocalRot, u);

            yield return null;
        }

        tr.localPosition = endLocalPos;
        tr.localRotation = endLocalRot;
    }

    IEnumerator AnimateFlip(Transform tr, float fromX, float toX, float duration)
    {
        float t = 0f;

        // on part de la rotation actuelle, mais on force l'angle X de départ/fin
        Vector3 startEuler = tr.localEulerAngles;
        startEuler.x = fromX;

        Vector3 endEuler = startEuler;
        endEuler.x = toX;

        Quaternion startRot = Quaternion.Euler(startEuler);
        Quaternion endRot = Quaternion.Euler(endEuler);

        tr.localRotation = startRot;

        while (t < duration)
        {
            t += Time.unscaledDeltaTime;
            float u = Mathf.Clamp01(t / duration);

            tr.localRotation = Quaternion.Slerp(startRot, endRot, u);
            yield return null;
        }

        tr.localRotation = endRot;
    }

    void EndRound(bool playerWon, bool isBlackjackNatural, bool isPush)
    {
        roundActive = false;
        playerTurn = false;

        if (btnHit != null) btnHit.interactable = false;
        if (btnStand != null) btnStand.interactable = false;

        int coins = PlayerPrefs.GetInt("coins");

        if (isPush)
        {
            coins += currentBet;
            if (resultText != null) resultText.text = "Égalité. Mise remboursée.";
        }
        else if (playerWon)
        {
            if (isBlackjackNatural)
            {
                int win = Mathf.RoundToInt(currentBet * 2.5f);
                coins += win;
                if (resultText != null) resultText.text = $"Blackjack ! +{win}";
            }
            else
            {
                int win = currentBet * 2;
                coins += win;
                if (resultText != null) resultText.text = $"Gagné ! +{win}";
            }
        }
        else
        {
            if (resultText != null) resultText.text = "Perdu.";
        }

        PlayerPrefs.SetInt("coins", coins);
        UpdateBalanceText();
    }

    // ======== Deck & Helpers ========
    void BuildAndShuffleDeck()
    {
        deck.Clear();
        for (int i = 0; i < 52; i++) deck.Add(i);

        // Fisher–Yates
        for (int i = deck.Count - 1; i > 0; i--)
        {
            int j = Random.Range(0, i + 1);
            (deck[i], deck[j]) = (deck[j], deck[i]);
        }
    }

    int DrawCard()
    {
        if (deck.Count == 0) BuildAndShuffleDeck();
        int c = deck[deck.Count - 1];
        deck.RemoveAt(deck.Count - 1);
        return c;
    }

    static int Rank(int card) => card % 13;  // 0..12 (A,2..10,J,Q,K)
    static int ValueForRank(int r)
    {
        if (r == 0) return 11;   // As
        if (r >= 10) return 10;  // J,Q,K
        return r + 1;            // 2..10
    }

    static string RankToString(int r)
    {
        switch (r)
        {
            case 0: return "A";
            case 10: return "J";
            case 11: return "Q";
            case 12: return "K";
            default: return (r + 1).ToString();
        }
    }

    static string SuitToString(int card)
    {
        int suit = card / 13; // 0..3
        switch (suit)
        {
            case 0: return "♣";
            case 1: return "♦";
            case 2: return "♥";
            default: return "♠";
        }
    }

    static string CardToString(int card) => $"{RankToString(Rank(card))}{SuitToString(card)}";

    static string HandToString(List<int> cards)
    {
        if (cards.Count == 0) return "";
        System.Text.StringBuilder sb = new System.Text.StringBuilder();
        for (int i = 0; i < cards.Count; i++)
        {
            if (i > 0) sb.Append(" ");
            sb.Append(CardToString(cards[i]));
        }
        return sb.ToString();
    }

    static int HandValue(List<int> cards)
    {
        int total = 0;
        int aces = 0;

        foreach (var c in cards)
        {
            int r = Rank(c);
            int v = ValueForRank(r);
            total += v;
            if (r == 0) aces++;
        }

        while (total > 21 && aces > 0)
        {
            total -= 10; // As 11→1
            aces--;
        }

        return total;
    }

    // ======== Prefabs helpers ========
    GameObject GetPrefabForCard(int card)
    {
        int suit = card / 13;        // 0=♣,1=♦,2=♥,3=♠
        int valueIndex = card % 13;  // 0..12 => A..K

        switch (suit)
        {
            case 0: return SafeCard(clubsPrefabs, valueIndex);
            case 1: return SafeCard(diamondsPrefabs, valueIndex);
            case 2: return SafeCard(heartsPrefabs, valueIndex);
            case 3: return SafeCard(spadesPrefabs, valueIndex);
            default: return backCardPrefab;
        }
    }

    GameObject SafeCard(GameObject[] arr, int idx)
    {
        if (arr != null && idx >= 0 && idx < arr.Length && arr[idx] != null)
            return arr[idx];
        // En cas de carte manquante, on évite la NullRef (on n'instancie rien)
        Debug.LogWarning($"[Blackjack] Carte prefab manquante (idx {idx}).");
        return null;
    }

    bool CanRenderHands()
    {
        if (playerCardsContainer == null || dealerCardsContainer == null) return false;
        if (clubsPrefabs == null || clubsPrefabs.Length != 13) return false;
        if (diamondsPrefabs == null || diamondsPrefabs.Length != 13) return false;
        if (heartsPrefabs == null || heartsPrefabs.Length != 13) return false;
        if (spadesPrefabs == null || spadesPrefabs.Length != 13) return false;
        // backCardPrefab peut être null (on affiche alors rien pour la carte cachée)
        return true;
    }
}
