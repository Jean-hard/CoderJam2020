using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public Camera mainCamera;
    public List<GameObject> templates;
    public GameObject bulletParent;
    public GameObject instantScoreParent;
    public GameObject killZone;
    public GameObject endRoundPanel;
    public GameObject nextButton;
    public GameObject gameCanvas;
    public GameObject tutoCanvas;
    public GameObject turrets;

    public Text instantScoreText;
    public Text scoreText;
    public Text nbShotText;
    public Text qualityText;
    

    [SerializeField]
    private Color validColor;

    private int score = 0;
    private int instantScore = 0;
    private int totalScore = 0;
    private int currentTemplateIndex = 0;

    private List<Piece> piecesList = new List<Piece>();

    private bool roundFinished = false;

    private enum TEMPLATE
    {
        SMILE,
        HEART
    }

    #region Singleton Pattern
    private static GameManager _instance;

    public static GameManager Instance { get { return _instance; } }
    #endregion

    private void Awake()
    {
        _instance = this;
    }

    private void Start()
    {
        UpdatePieceList();
        tutoCanvas.SetActive(true);
        gameCanvas.SetActive(false);
        turrets.SetActive(false);
    }

    private void Update()
    {
        CheckEndRound();
    }

    public void LaunchGame()
    {
        tutoCanvas.SetActive(false);
        gameCanvas.SetActive(true);
        turrets.SetActive(true);
    }

    private void UpdatePieceList()
    {
        piecesList = new List<Piece>();
        foreach (Transform piece in templates[currentTemplateIndex].transform)
        {
            piecesList.Add(piece.GetComponent<Piece>());
        }
    }

    private void CheckEndRound()
    {
        if(piecesList.Count <= 0 && !roundFinished)
        {
            templates[currentTemplateIndex].SetActive(false);
            endRoundPanel.SetActive(true);
            switch(currentTemplateIndex)
            {
                case 0:
                    if (score < 100)
                        qualityText.text = "dégueu...";
                    if (score >= 100 && score < 200)
                        qualityText.text = "pas ouf";
                    if (score >= 200 && score < 300)
                        qualityText.text = "de personne normalement constituée";
                    if (score >= 300)
                        qualityText.text = "de malade mental";
                    break;
                case 1:
                    if (score < 150)
                        qualityText.text = "dégueu...";
                    if (score >= 150 && score < 250)
                        qualityText.text = "pas ouf";
                    if (score >= 250 && score < 350)
                        qualityText.text = "de personne normalement constituée";
                    if (score >= 350)
                        qualityText.text = "de malade mental";
                    break;

            }
            nextButton.SetActive(true);
            roundFinished = true;
        }
    }

    /**
     * Fonction lancée lorsqu'une balle a été tiré
     */
    public void HasShot(Projectil projectileCreated)
    {
        Debug.Log("has shot");
        GameObject closestPiece = CompareToPieces(projectileCreated.transform);
        if (closestPiece != null)
        {
            CalculateScore(projectileCreated.gameObject, closestPiece);
            ChangeColor(projectileCreated, closestPiece);
            StartCoroutine(ShowInstantScore(mainCamera.WorldToScreenPoint(projectileCreated.transform.position)));
        }
        else
        {
            Destroy(projectileCreated.gameObject);
            Debug.Log("Trop loin d'une pièce");
        }
    }

    private GameObject CompareToPieces(Transform _projectile)
    {
        Transform closestPiece = templates[currentTemplateIndex].transform.GetChild(0);

        foreach(Piece piece in piecesList)
        {
            if(Vector3.Distance(_projectile.position, piece.transform.position) < Vector3.Distance(_projectile.position, closestPiece.position))
            {
                closestPiece = piece.transform;
            }
        }
        if (Vector3.Distance(_projectile.position, closestPiece.position) < 1f)
            return closestPiece.gameObject;
        else
            return null;
    }

    private void CalculateScore(GameObject _projectile, GameObject _closestPiece)
    {
        //Calcul du score instant grace à la position
        instantScore = (int)((1 - Vector3.Distance(_projectile.transform.position, _closestPiece.transform.position)) * 100);
        
        //Calcul de différence d'angle
        float angle = Quaternion.Angle(_projectile.transform.rotation, _closestPiece.transform.rotation);
        if (angle > 90)
            angle = 180 - angle;
        Debug.Log(instantScore + " - " + (int)(3 * angle));

        //Si la différence d'angle est trop grande, aucun point gagné, sinon on retire cette différence à l'instantScore
        if ((int)(5 * angle) > instantScore)
            instantScore = 0;
        else
            instantScore -= (int)(3 * angle);

        //Ajoute le score instant au score global
        score += instantScore;
        
        scoreText.text = score.ToString();

        piecesList.Remove(_closestPiece.GetComponent<Piece>());
        //nbShotText.text = shotNb.ToString();
    }

    private void ChangeColor(Projectil _projectile, GameObject closestPiece)
    {
        _projectile.ChangeColorP(validColor);
        closestPiece.GetComponent<SpriteRenderer>().color = validColor;
    }

    private IEnumerator ShowInstantScore(Vector3 spawnPosition)
    {
        Text _instantScore = Instantiate(instantScoreText, spawnPosition, Quaternion.identity, instantScoreParent.transform);
        _instantScore.text = "+ " + instantScore;
        yield return new WaitForSeconds(3f);
        Destroy(_instantScore.gameObject);
    }

    public void DisplayNextTemplate()
    {
        templates[currentTemplateIndex].SetActive(false);

        currentTemplateIndex++;
        templates[currentTemplateIndex].SetActive(true);
        endRoundPanel.SetActive(false);
        score = 0;
        scoreText.text = score.ToString();
        foreach(Transform bullet in bulletParent.transform)
        {
            Destroy(bullet.gameObject);
        }
        UpdatePieceList();
        roundFinished = false;
        nextButton.SetActive(false);
    }
}
