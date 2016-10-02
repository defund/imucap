//This script controls the ally spawning functionality. It keeps track of the number of spendable "points" the player has as well as
//the current state of a spawned ally. Note that points for allies in this script are different from the points stored in the Game 
//Manager. Finally, this script is responsible for know if an ally can be spawned and showing the appropriate UI image

using UnityEngine;
using UnityEngine.UI;

public class AllyManager : MonoBehaviour
{
	[SerializeField] int allyCost;				//The amount of points an ally costs to summon
	[SerializeField] GameObject allyPrefab;		//The prefab of the ally to be summoned
	[SerializeField] Transform allySpawnPoint;	//Where the ally should be summoned
	[SerializeField] Image allyImage;			//A reference to the UI image that lets the player know an ally is available

	Ally ally;									//A reference to any currently spawned ally
	int allyPoints;								//How many points the player currently has to spend on allies

	void Awake()
	{
		//Start by instantiating an ally game object from a prefab
		GameObject obj = (GameObject) Instantiate(allyPrefab);
		//Then parent it to this game object
		obj.transform.parent = transform;
		//Then grab a reference to its Ally script
		ally = obj.GetComponent<Ally>();
		//Finally disable it so it is ready to be used later
		obj.SetActive(false);
		//If the allyImage exists, disable it
		if(allyImage != null)
			allyImage.enabled = false;
	}

	//Called from the GameManager script to give the AllyManager some points to
	//spend on allies
	public void AddPoints(int amount)
	{
		//Add the amount of points
		allyPoints += amount;

		//If there is an allyImage and the player can summon an ally, show the image
		if (allyImage != null && CanSummonAlly())
			allyImage.enabled = true;
	}
		
	public bool CanSummonAlly()
	{
		//If the player has enough points and there isn't currently an ally in the scene, return 
		//true (an ally can be spawned), otherwise return false
		return (allyPoints >= allyCost) && !ally.gameObject.activeSelf;
	}

	//This method summons the ally into the scene and returns a reference 
	//to the ally
	public Ally SummonAlly()
	{
		//If an ally can't be summoned, return a value of null (nothing)
		if (!CanSummonAlly())
			return null;

		//Align the ally's position and rotation with the spawn point
		ally.transform.position = allySpawnPoint.position;
		ally.transform.rotation = allySpawnPoint.rotation;
		//Enable the ally and tell it to move towards the enemy's target (which
		//should be the player)
		ally.gameObject.SetActive(true);
		ally.Move(GameManager.Instance.EnemyTarget.position);

		//If there is an allyImage, turn it off
		if(allyImage != null)
			allyImage.enabled = false;
		//Return a reference to the ally back to the caller
		return ally;
	}

	//This method turns the ally off
	public void UnSummonAlly()
	{
		//Once the ally goes away, remove all ally points. This is the prevent the 
		//player from banking a large amount of ally points and then effectively 
		//chain summoning allies (which would not be fun or challenging)
		allyPoints = 0;
		//Turn the ally off
		ally.gameObject.SetActive(false);
	}
}
