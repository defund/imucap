//This script handles the slime attack. The slime attack is a targetted attack with a long cooldown. One enemy will be targetted
//by the attack and will get "slimed". The slime prevents them from attacking and causes them to take damage over time. This attack only 
//fires when you release the trigger.

using UnityEngine;

public class SlimeAttack : MonoBehaviour
{
	[Header("Weapon Specs")]
	public float Cooldown = 3.5f;							//Cooldown of the attack

	[SerializeField] LayerMask whatIsShootable;				//Layermask that determines what the attack can hit

	[Header("Weapon References")]
	[SerializeField] SlimeProjectile slimeProjectile;		//Reference to the slime projectile
	[SerializeField] Renderer targetReticule;				//Reference to the targetting reticule

	[Header("Reticule Colors")]
	[SerializeField] Color invalidTargetTint = Color.red;   //Color of invalid targets
	[SerializeField] Color notReadyTint = Color.yellow;  	//Color when the attack isn't ready
	[SerializeField] Color readyTint = Color.green;			//Color of a valid target

	float timeOfLastAttack = -10f;							//The time when the attack was last made, initialized with a dummy value
	Transform target = null;								//The target of the attack
	Vector3 targetPosition;									//The position of a target
	Collider[] testColliders;								//An array for storing a collection of colliders

	//Called from PlayerAttack script. This attack is not void like other
	//attacks and will instead let the PlayerAttack script know if it was successful
	public bool Fire()
	{
		//If the attack has a target...
		if (target != null)
		{
			//...Launch the slime projectile and return true
			LaunchProjectile();
			return true;
		}
		//If attack wasn't successful, return false
		return false;
	}

	void Update()
	{
		//If we don't have a MouseLocation script in the scene or if the position of the mouse isn't valid, leave Update()
		if (MouseLocation.Instance == null || !MouseLocation.Instance.IsValid)
			return;

		//Grab the current position of the mouse
		targetPosition = MouseLocation.Instance.MousePosition;
		//Create a RaycastHit variable
		RaycastHit hit;
		//Cast a ray from the mouse's position straight up along the Y axis. If the raycast hits something record it as the target 
		if (Physics.Raycast(targetPosition, Vector3.up, out hit, 2f, whatIsShootable))
			target = hit.transform;

		UpdateReticule();
	}

	//This method updates the position and color of the reticule
	void UpdateReticule()
	{
		//If there is a target move the reticule to its location
		if (target != null)
			targetReticule.transform.position = target.position;
		//Otherwise, place the reticule where the mouse is
		else
			targetReticule.transform.position = targetPosition;

		//If there is no target, set the invalid tint
		if (target == null)
			targetReticule.material.SetColor("_TintColor", invalidTargetTint);
		//If there is a target but the attack in on cooldown, set the not ready tint
		else if (timeOfLastAttack + Cooldown > Time.time)
			targetReticule.material.SetColor("_TintColor", notReadyTint);
		//Otherwise, the reticule should be set to the ready tint
		else
			targetReticule.material.SetColor("_TintColor", readyTint);
	}

	//This method launches a "seeking" projectile at the targetted enemy
	void LaunchProjectile()
	{
		//Record the current time
		timeOfLastAttack = Time.time;
		//Move the slime position to the attack's position
		slimeProjectile.transform.position = transform.position;
		//Turn the projectile on
		slimeProjectile.gameObject.SetActive(true);
		//Start the projectile along its path to the target
		slimeProjectile.StartPath(target);

		//Forget the current target
		target = null;
	}
}
