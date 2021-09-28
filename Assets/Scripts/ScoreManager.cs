using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScoreManager : MonoBehaviour
{

	private Board board;
	[SerializeField] Text scoreText, scoreNeededText, levelText;
	[SerializeField] Button next;
	[SerializeField] Image tips, victory, instructions;
	SoundManager soundManager;
	private int score;

	// Use this for initialization
	void Start()
	{

		
	}

	// Update is called once per frame
	void Update()
	{

	}

	public void NextLevel()
    {

		board.Restart();
	}

	public void IncreaseScore(int amountToIncrease)
	{
		score += amountToIncrease;
	}
}