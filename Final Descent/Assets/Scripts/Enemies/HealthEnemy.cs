
    public SkinnedMeshRenderer mesh;
            FlashOnHit();
            float enemyCurrenhp = GetComponent<HealthEnemy>().health;
            float enemyMaxhp = GetComponent<HealthEnemy>().base_maxHealth;
    public void FlashOnHit()
    {
    }
        StartCoroutine(IenumFlashOnHit());
    IEnumerator IenumFlashOnHit()
    {

        mesh.material.color = new Color(Color.red.r * 100, Color.red.g, Color.red.b * 100);
        mesh.material.color = originalColor;
        yield return new WaitForSeconds(0.2f);
    }
	private void CheckWhatWepon(GameObject other)
	{
		if (other.name == "missile(Clone)" || other.name == "miniMissile")
		{
			TakeDamage(10);
		}
		if (other.name == "missileBig")
		{
			TakeDamage(20);
		}
		if(other.name == "EnergyBullet")
		{
			TakeDamage(10);
		}
		Destroy(other.gameObject);
	}