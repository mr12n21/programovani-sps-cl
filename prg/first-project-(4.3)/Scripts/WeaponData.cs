using Godot;

[GlobalClass]
public partial class WeaponData : Resource
{
	[Export] public string Name { get; set; } = "Pistol";
	[Export] public float Damage { get; set; } = 10;
	[Export] public float FireRate { get; set; } = 2;
	[Export] public Color Color { get; set; } = Colors.Yellow;
	[Export] public string IconPath { get; set; } = "res://Assets/gun.png";
	[Export(PropertyHint.MultilineText)] public string Description { get; set; } = "Reliable sidearm.";
	[Export] public float BaseDamage { get; set; } = 10;
	[Export] public float DamagePerLevel { get; set; } = 2;
	[Export] public float BaseFireRate { get; set; } = 2;
	[Export] public float FireRatePerLevel { get; set; } = 0.15f;
	[Export] public int UnlockCost { get; set; } = 0;
	[Export] public int UpgradeCost { get; set; } = 12;
	[Export] public int MaxLevel { get; set; } = 5;
	[Export] public bool StartsUnlocked { get; set; } = false;

	public bool IsUnlocked { get; private set; }
	public int Level { get; private set; } = 1;

	public void PrepareForRun()
	{
		if (BaseDamage <= 0f)
		{
			BaseDamage = Damage;
		}

		if (BaseFireRate <= 0f)
		{
			BaseFireRate = FireRate;
		}

		Level = 1;
		IsUnlocked = StartsUnlocked || UnlockCost <= 0;
		Damage = GetDamageForLevel(Level);
		FireRate = GetFireRateForLevel(Level);
	}

	public float GetDamageForLevel(int level)
	{
		return BaseDamage + DamagePerLevel * Mathf.Max(0, level - 1);
	}

	public float GetFireRateForLevel(int level)
	{
		return BaseFireRate + FireRatePerLevel * Mathf.Max(0, level - 1);
	}

	public int GetUpgradePrice()
	{
		return UpgradeCost * Level;
	}

	public bool CanUpgrade()
	{
		return Level < MaxLevel;
	}

	public void Unlock()
	{
		IsUnlocked = true;
	}

	public bool Upgrade()
	{
		if (!CanUpgrade())
		{
			return false;
		}

		Level += 1;
		Damage = GetDamageForLevel(Level);
		FireRate = GetFireRateForLevel(Level);
		return true;
	}
}
