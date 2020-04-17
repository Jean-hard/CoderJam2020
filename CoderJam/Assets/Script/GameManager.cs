using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

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
    public GameObject endGamePanel;

    public Text instantScoreText;
    public Text scoreText;
    public Text nbShotText;
    public Text qualityText;
    public Text finalScoreText;
    

    [SerializeField]
    private Color validColor;

    private int score = 0;
    private int instantScore = 0;
    private int totalScore = 0;
    private int currentTemplateIndex = 0;
    
    [System.NonSerialized]
    public int nbShot = 8;

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
        nbShotText.text = nbShot.ToString();
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
        if((piecesList.Count <= 0 && !roundFinished) || nbShot < 0)
        {
            templates[currentTemplateIndex].SetActive(false);
            endRoundPanel.SetActive(true);
            nbShotText.gameObject.SetActive(false);
            switch(currentTemplateIndex)
            {
                case 0:
                    if (score < 170)
                        qualityText.text = "dégueu...";
                    if (score >= 170 && score < 250)
                        qualityText.text = "pas ouf";
                    if (score >= 250 && score < 320)
                        qualityText.text = "de personne normalement constituée";
                    if (score >= 320)
                        qualityText.text = "de malade mental";
                    break;
                case 1:
                    if (score < 190)
                        qualityText.text = "dégueu...";
                    if (score >= 190 && score < 290)
                        qualityText.text = "pas ouf";
                    if (score >= 290 && score < 390)
                        qualityText.text = "de personne normalement constituée";
                    if (score >= 390)
                        qualityText.text = "de malade mental";
                    break;
                case 2:
                    if (score < 210)
                        qualityText.text = "dégueu...";
                    if (score >= 210 && score < 310)
                        qualityText.text = "pas ouf";
                    if (score >= 310 && score < 410)
                        qualityText.text = "de personne normalement constituée";
                    if (score >= 410)
                        qualityText.text = "de malade mental";
                    break;
                case 3:
                    if (score < 210)
                        qualityText.text = "dégueu...";
                    if (score >= 210 && score < 310)
                        qualityText.text = "pas ouf";
                    if (score >= 310 && score < 410)
                        qualityText.text = "de personne normalement constituée";
                    if (score >= 410)
                        qualityText.text = "de malade mental";
                    break;
                case 4:
                    if (score < 210)
                        qualityText.text = "dégueu...";
                    if (score >= 210 && score < 310)
                        qualityText.text = "pas ouf";
                    if (score >= 310 && score < 410)
                        qualityText.text = "de personne normalement constituée";
                    if (score >= 410)
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
            GetMalus(mainCamera.WorldToScreenPoint(projectileCreated.transform.position));
            //Destroy(projectileCreated.gameObject);
            Debug.Log("Trop loin d'une pièce");
        }

        nbShot--;
        nbShotText.text = nbShot.ToString();
        CheckEndRound();
    }

    public void GetMalus(Vector3 bulletPosition)
    {
        StartCoroutine(ShowMalus(bulletPosition));
        score -= 20;
        if (score < 0)
            score = 0;
        scoreText.text = score.ToString();
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
        //Debug.Log(instantScore + " - " + (int)(3 * angle));

        //Si la différence d'angle est trop grande, aucun point gagné, sinon on retire cette différence à l'instantScore
        if ((int)(5 * angle) > instantScore)
            instantScore = 0;
        else
            instantScore -= (int)(3 * angle);

        //Ajoute le score instant au score global
        score += instantScore;
        
        scoreText.text = score.ToString();

        piecesList.Remove(_closestPiece.GetComponent<Piece>());
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

    private IEnumerator ShowMalus(Vector3 bulletPosition)
    {
        Text _instantScore = Instantiate(instantScoreText, bulletPosition, Quaternion.identity, instantScoreParent.transform);
        _instantScore.text = "-20";
        _instantScore.color = Color.red;
        yield return new WaitForSeconds(3f);
        Destroy(_instantScore.gameObject);
    }

    public void DisplayNextTemplate()
    {
        templates[currentTemplateIndex].SetActive(false);
        totalScore += score;

        currentTemplateIndex++;

        switch(currentTemplateIndex)
        {
            case 1:
                nbShot = 10;
                break;
            case 2:
                nbShot = 11;
                break;
            case 3:
                nbShot = 13;
                break;
            case 4:
                nbShot = 14;
                break;
        }

        nbShotText.text = nbShot.ToString();

        if (currentTemplateIndex != 5)
        {
            templates[currentTemplateIndex].SetActive(true);
            endRoundPanel.SetActive(false);
            
            score = 0;
            scoreText.text = score.ToString();
            foreach (Transform bullet in bulletParent.transform)
            {
                Destroy(bullet.gameObject);
            }
            UpdatePieceList();
            roundFinished = false;
            nbShotText.gameObject.SetActive(true);
            nextButton.SetActive(false);
        }
        else if(currentTemplateIndex == 5)
            DisplayEndGame();
    }

    private void DisplayEndGame()
    {
        endGamePanel.SetActive(true);
        finalScoreText.text = totalScore.ToString();
        scoreText.gameObject.SetActive(false);
        nbShotText.gameObject.SetActive(false);
    }

    public void Replay()
    {
        SceneManager.LoadScene("SampleScene");
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
