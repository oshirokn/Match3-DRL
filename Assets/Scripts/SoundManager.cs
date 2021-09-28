using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
	[SerializeField] AudioSource effectsSource;
	[SerializeField] AudioSource UISource;
	[SerializeField] AudioSource musicSource;
	[SerializeField] AudioClip[] destroyClip;
	[SerializeField] AudioClip[] trashcanFillClip;
	[SerializeField] AudioClip levelCompletedClip;
	[SerializeField] AudioClip loseClip;
	[SerializeField] AudioClip victoryClip;
	[SerializeField] AudioClip musicLoop;
	[SerializeField] AudioClip buttonClip;
	public void PlayDestroyNoise()
	{
		int random = Random.Range(0, 5);
		effectsSource.PlayOneShot(destroyClip[random]);
	}

	public void PlayTrashcanNoise()
	{
		int random = Random.Range(0, 3);
		effectsSource.PlayOneShot(trashcanFillClip[random]);
	}

	public void PlayLevelCompleted()
	{
		musicSource.Pause();
		UISource.PlayOneShot(levelCompletedClip);
	}

	public void PlayVictory()
	{
		musicSource.Pause();
		UISource.PlayOneShot(victoryClip);
	}

	public void PlayLose()
	{
		musicSource.Pause();
		UISource.PlayOneShot(loseClip);
	}

	public void PlayMusic()
	{
		musicSource.Play();
	}

	public void PlayButton()
	{
		UISource.PlayOneShot(buttonClip);
	}
}
