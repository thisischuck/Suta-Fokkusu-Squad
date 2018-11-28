using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EelAttacks : MonoBehaviour {

    private List<Transform> _playerList;
    public float eelSpeed;
	//public float eelRotation;

    //Tornado
    public bool isTornado;
    private Vector3 centerPosition;
    public float radius;
    public float force;
	private float timeTornading;
	private float TornadoDuration;
	public bool goToExplode;

    //EletricExplosion
    private float nextExplosion;
    public bool isExploding;
    private int explosionCount;
    public ParticleSystem ee;

    //highspeed
    private List<Transform> RHoles;
    private List<Transform> LHoles;
    private Transform selectedHole;
    private bool left;
    private bool goingIn;
    public bool isHighSpeed;
    private int nextHole;
    private Vector3 direction;
	private int voltasCount;
	private int voltas;
	public int randVoltas;
	public bool endHS;
	public Transform caveCenter;

    //calling
    public GameObject enemyBabies;
    private int enemiesPerHole;
	public bool isCalling;

	//Charge
	public bool isCharging;
	public float currentHealth;
	public bool sameCharge;
	private int playerIndex;

	//Bite
	public Transform BottomMouth;
    public bool isBiting;
    private Quaternion openMouth;
    private Quaternion closeMouth;
    private bool mouthClosed;
	private Transform inBiteRange;

	//ShowWave
	public bool isShockWave;
	public ParticleSystem SW;
	private int waveCount;
	private float nextWave;

	//Damage Variables
	public float ExplosionDamage;
	public float BiteDamage;

	// Use this for initialization
	public void Start () {
		isTornado = false;
		TornadoDuration = 5f;
		goToExplode = false;
		isExploding = false;
		RHoles = new List<Transform>();
		LHoles = new List<Transform>();
		goingIn = false;
		isHighSpeed = false;
		enemiesPerHole = 3;
		isCalling = false;
		isCharging = false;
		sameCharge = true;
		isBiting = false;
		isShockWave = false;

        nextExplosion = 0f;
        explosionCount = 0;
        eelSpeed = 15f;
        radius = 20f;
		timeTornading = 0.0f;
		nextWave = 0.0f;

        _playerList = new List<Transform>();

        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        foreach (GameObject player in players)
        {
            _playerList.Add(player.gameObject.transform);
        }

        AddHoles();

		openMouth = Quaternion.Euler(BottomMouth.rotation.y + 20.0f, BottomMouth.rotation.y, BottomMouth.rotation.z);
		closeMouth = BottomMouth.rotation;
	}

	// Update is called once per frame
	void Update()
	{
		if (Input.GetKeyDown(KeyCode.P))
		{
			//isTornado = !isTornado;
			//PrepTornado();
			//isExploding = !isExploding;
			//isHighSpeed = !isHighSpeed;
			//OnEntryHighSpeed();
			//Calling();
			//isBiting = !isBiting;
			//OnEntryBite();
			//isShockWave = !isShockWave;
			isCharging = !isCharging;
			OnEntryCharge();
		}

		if (isExploding)
		{
			Explode();
		}

		if (isTornado)
		{
			if (Time.time >= timeTornading)
				isTornado = false;
			else
				Tornado();
		}
		if (timeTornading >= 5f) isTornado = false;

		if (isHighSpeed)
		{
			HighSpeed();
		}

		if (isCharging)
			Charge();

		if (isBiting)
			Bite();
	}

	#region Tornado
	public void OnEntryTornado()
    {
		Debug.Log("OnEntryTornado");

		isTornado = true;
		timeTornading = Time.time + TornadoDuration;
        centerPosition = transform.position + Vector3.forward * 10f;
		goToExplode = false;
    }

	/*public void OnExitTornado()
	{
		Debug.Log("OnExitTornado");
		transform.position = Vector3.MoveTowards(transform.position, centerPosition, eelSpeed * Time.deltaTime);
		goToExplode = true;
	}*/

	public void Tornado()// colocar a dar damage stun qunado toca na ragdoll da enguia
    {
		if(!isTornado)
		{
			transform.position = Vector3.MoveTowards(transform.position, centerPosition, eelSpeed * Time.deltaTime);
			if (Vector3.Distance(transform.position, centerPosition) <= 3f)
				goToExplode = true;
		}
		if (Time.time >= timeTornading)
		{
			isTornado = false;
			return;
		}

		//CircularMotion //Still needs adjusting
		float rotSpeed = 80f;

        transform.RotateAround(centerPosition, Vector3.up, Time.deltaTime * rotSpeed);
        Vector3 desiredPosition = (transform.position - centerPosition).normalized * radius + centerPosition;
        transform.position = Vector3.MoveTowards(transform.position, desiredPosition, 5f * Time.deltaTime);

		//GetSucked()
        foreach(Transform player in _playerList)
        {
            Vector3 direction = Vector3.zero;
            if (Vector3.Distance(centerPosition, player.position) < 50f)
            {
                direction = player.position - transform.position;
                player.position = Vector3.MoveTowards(player.position, centerPosition, force);
            }
        }
    }
	#endregion

	#region Explode

	public void OnEntryExplode()
	{
		isExploding = true;
	}

	public void Explode()
    {
		Debug.Log("Explode");
		if (explosionCount < 3)
        {
            if (Time.time >= nextExplosion)
            {
                ee.Play(true);
                nextExplosion = Time.time + 2f;
                explosionCount++;
            }
        }
        else
        {
            isExploding = false;
            explosionCount = 0;
        }
    }

	#endregion

	#region HighSpeed

	public void OnEntryHighSpeed()
	{
		isHighSpeed = true;
		int rand = Random.Range(0, 2);
		if (rand == 1)
		{
			nextHole = Random.Range(0, LHoles.Count - 1);
			selectedHole = LHoles[nextHole];
			left = true;
		}
		else if (rand == 0)
		{
			nextHole = Random.Range(0, RHoles.Count - 1);
			selectedHole = RHoles[nextHole];
			left = false;
		}

		endHS = false;
		randVoltas = Random.Range(2, 5);
		voltasCount = 0;
		voltas = randVoltas + 1;
		Debug.Log(voltas);
	}

	public void HighSpeed()
    {
		if (Vector3.Distance(transform.position, caveCenter.position) <= 5f  && endHS == true)
		{
			isHighSpeed = false;
			return;
		}

		Transform previousHole = selectedHole; 
        if (Vector3.Distance(transform.position, selectedHole.position) <= 5f)
        {
            //Look for another hole
            if (left && !goingIn)
            {
				voltasCount++;
				if (voltasCount == voltas)
				{
					selectedHole = caveCenter;
					endHS = true;
				}
				else
				{
					left = true;
					goingIn = true;
					while (selectedHole == previousHole)
					{
						nextHole = Random.Range(0, LHoles.Count - 1);
						selectedHole = LHoles[nextHole];
					}
				}
            }
            else if(!left && !goingIn)
            {
				voltasCount++;
				if (voltasCount == voltas)
				{
					selectedHole = caveCenter;
					endHS = true;
				}
				else
				{
					left = false;
					goingIn = true;
					while (selectedHole == previousHole)
					{
						nextHole = Random.Range(0, RHoles.Count - 1);
						selectedHole = RHoles[nextHole];
					}
				}
            }
            else if(left && goingIn)
            {
                goingIn = false;
                left = false;
                while (selectedHole == previousHole)
                {
                    nextHole = Random.Range(0, RHoles.Count - 1);
                    selectedHole = RHoles[nextHole];
                }
            }
            else if (!left && goingIn)
            {
                goingIn = false;
                left = true;
                while (selectedHole == previousHole)
                {
                    nextHole = Random.Range(0, LHoles.Count - 1);
                    selectedHole = LHoles[nextHole];
                }
            }
			Debug.Log(voltasCount);
		}

		//Movement
        transform.position += transform.forward *( Time.deltaTime * eelSpeed);

        direction = transform.position - selectedHole.position;
        Quaternion targetRot = Quaternion.LookRotation(-direction, Vector3.up);
        transform.rotation = Quaternion.Lerp(transform.rotation, targetRot, 0.04f);
    }

	#endregion

	#region Calling

	public void OnEntryCalling()
	{
		isCalling = true;
	}

	public void Calling() // change position to spawnEnemyPosition
    {
		Debug.Log("Calling");
		foreach (Transform hole in RHoles)
        {
            for(int i=0;i<=enemiesPerHole;i++)
                Instantiate(enemyBabies,hole.position,Quaternion.identity);
        }

        foreach (Transform hole in LHoles)
        {
            for (int i = 0; i <= enemiesPerHole; i++)
                Instantiate(enemyBabies, hole.position, Quaternion.identity);
        }

		isCalling = false;
    }
	#endregion

	#region Charge

	public void OnEntryCharge()
	{
		isCharging = true;
		if (!sameCharge)
		{
			currentHealth = this.GetComponent<HealthEnemy>().health;
			sameCharge = true;
			playerIndex = Random.Range(0, 4);
		}
	}

	public void Charge()// talvez procurar os q estam com stun e dar charge a um deles
	{
		Debug.Log("Charge");
		Transform chosenTarget;
		chosenTarget = _playerList[playerIndex];

		transform.position = Vector3.MoveTowards(transform.position, chosenTarget.position, eelSpeed * Time.deltaTime);

		direction = transform.position - chosenTarget.position;
		Quaternion targetRot = Quaternion.LookRotation(-direction, Vector3.up);
		transform.rotation = Quaternion.Lerp(transform.rotation, targetRot, 0.04f);
	}

	#endregion

	#region Bite

	public void OnEntryBite()
	{
		isBiting = true;
		mouthClosed = true;
	}

	public void Bite()
    {
		Debug.Log("Bite");
		if (mouthClosed)
            BottomMouth.localRotation = Quaternion.RotateTowards(BottomMouth.rotation, openMouth , Time.deltaTime * 8f);
		else
			BottomMouth.localRotation = Quaternion.RotateTowards(BottomMouth.rotation, closeMouth, Time.deltaTime * 8f);

		if (openMouth == BottomMouth.rotation)
		{
			mouthClosed = false;
			inBiteRange.GetComponent<HealthPlayer>().TakeDamage(BiteDamage);
		}
		else if (closeMouth == BottomMouth.rotation)
		{
			mouthClosed = true;
			isBiting = false;
		}
	}

	#endregion

	#region ShockWave

	public void OnEntryShockWave()
	{
		isShockWave = true;
	}

	public void ShockWave()
	{
		Debug.Log("ShockWave");
		if (waveCount < 6)
		{
			if(Time.time >= nextWave)
			{
				SW.Play(true);
				nextWave = Time.time + 2f;
				waveCount++;
			}
		}
		else
		{
			waveCount = 0;
			isShockWave = false;
		}
	}

	#endregion

	public void AddHoles()
    {
        GameObject rholes = GameObject.Find("2Room").transform.Find("Room").transform.Find("RightHoles").gameObject;
        GameObject lholes = GameObject.Find("2Room").transform.Find("Room").transform.Find("LeftHoles").gameObject;

        foreach (Transform child in rholes.transform)
        {
            RHoles.Add(child);
        }
        foreach (Transform child in lholes.transform)
        {
            LHoles.Add(child);
        }
    }

	public float MedPlayersPositions(Vector3 posReference)
	{
		float soma = 0.0f;
		foreach(Transform player in _playerList)
		{
			float dist = Vector3.Distance(posReference, player.position);
			soma += dist;
		}

		return soma / (_playerList.Count - 1);
	}

	public int RandNum()
	{
		return Random.Range(0, 2);
	}

	//Collider da boca
	public void OnTriggerEnter(Collider other)
	{
		if (other.gameObject.tag == "Player")
		{
			isBiting = true;
			inBiteRange = other.transform;
		}
	}
}
