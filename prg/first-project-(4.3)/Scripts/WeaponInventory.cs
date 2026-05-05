using Godot;
using System;
using System.Collections.Generic;

public partial class WeaponInventory : Node
{
	[Export] public Godot.Collections.Array<WeaponData> Weapons { get; set; } = new();
	
	private int _currentIndex = 0;
	private int _diamonds = 0;
	public int CurrentIndex => _currentIndex;
	public WeaponData CurrentWeapon => Weapons.Count > 0 ? Weapons[_currentIndex] : null;
	public int Diamonds => _diamonds;
	
	public event Action<WeaponData, int> OnWeaponChanged;
	public event Action OnInventoryUpdated;
	public event Action<int> OnDiamondsChanged;

	public override void _Ready()
	{
		if (Weapons.Count == 0)
		{
			Weapons.Add(new WeaponData {
				Name = "Pistol",
				Description = "Reliable starter sidearm with balanced fire rate.",
				Damage = 10,
				FireRate = 2,
				Color = Colors.Yellow,
				IconPath = "res://Assets/gun.png",
				StartsUnlocked = true,
				UpgradeCost = 10,
				MaxLevel = 6,
			});
			Weapons.Add(new WeaponData {
				Name = "Shotgun",
				Description = "Heavy burst damage for close encounters.",
				Damage = 25,
				FireRate = 0.8f,
				Color = Colors.Red,
				IconPath = "res://Assets/bullet.png",
				UnlockCost = 18,
				UpgradeCost = 14,
				MaxLevel = 5,
			});
			Weapons.Add(new WeaponData {
				Name = "Machine Gun",
				Description = "High fire rate stream for crowd control.",
				Damage = 5,
				FireRate = 8,
				Color = Colors.Cyan,
				IconPath = "res://Assets/machine_gun.png",
				UnlockCost = 28,
				UpgradeCost = 16,
				MaxLevel = 6,
			});
			Weapons.Add(new WeaponData {
				Name = "Tesla",
				Description = "Slow but devastating precision blasts.",
				Damage = 40,
				FireRate = 1.5f,
				Color = Colors.DodgerBlue,
				IconPath = "res://Assets/lightning1.png",
				UnlockCost = 42,
				UpgradeCost = 22,
				MaxLevel = 4,
			});
			Weapons.Add(new WeaponData {
				Name = "Lightning",
				Description = "Experimental arc weapon with extreme cadence.",
				Damage = 15,
				FireRate = 12,
				Color = Colors.Green,
				IconPath = "res://Assets/lightning2.png",
				UnlockCost = 56,
				UpgradeCost = 30,
				MaxLevel = 5,
			});
		}

		foreach (var weapon in Weapons)
		{
			weapon?.PrepareForRun();
		}

		_currentIndex = GetFirstUnlockedIndex();
		NotifyInventoryUpdated();
	}

	public void SwitchTo(int index)
	{
		if (index < 0 || index >= Weapons.Count) return;
		if (!Weapons[index].IsUnlocked) return;
		_currentIndex = index;
		OnWeaponChanged?.Invoke(CurrentWeapon, _currentIndex);
		OnInventoryUpdated?.Invoke();
	}

	public void Next()
	{
		SwitchRelative(1);
	}

	public void Previous()
	{
		SwitchRelative(-1);
	}

	public void AddDiamonds(int amount)
	{
		if (amount <= 0)
		{
			return;
		}

		_diamonds += amount;
		OnDiamondsChanged?.Invoke(_diamonds);
		OnInventoryUpdated?.Invoke();
	}

	public bool UnlockWeapon(int index)
	{
		if (!IsValidWeaponIndex(index))
		{
			return false;
		}

		var weapon = Weapons[index];
		if (weapon.IsUnlocked)
		{
			return false;
		}

		if (!TrySpendDiamonds(weapon.UnlockCost))
		{
			return false;
		}

		weapon.Unlock();
		SwitchTo(index);
		NotifyInventoryUpdated();
		return true;
	}

	public bool UpgradeWeapon(int index)
	{
		if (!IsValidWeaponIndex(index))
		{
			return false;
		}

		var weapon = Weapons[index];
		if (!weapon.IsUnlocked)
		{
			return false;
		}

		int price = weapon.GetUpgradePrice();
		if (!TrySpendDiamonds(price))
		{
			return false;
		}

		if (!weapon.Upgrade())
		{
			AddDiamonds(price);
			return false;
		}

		if (index == _currentIndex)
		{
			OnWeaponChanged?.Invoke(CurrentWeapon, _currentIndex);
		}

		NotifyInventoryUpdated();
		return true;
	}

	public bool IsUnlocked(int index)
	{
		return IsValidWeaponIndex(index) && Weapons[index].IsUnlocked;
	}

	public int GetUnlockCost(int index)
	{
		return IsValidWeaponIndex(index) ? Weapons[index].UnlockCost : 0;
	}

	public int GetUpgradeCost(int index)
	{
		return IsValidWeaponIndex(index) ? Weapons[index].GetUpgradePrice() : 0;
	}

	private bool IsValidWeaponIndex(int index)
	{
		return index >= 0 && index < Weapons.Count;
	}

	private bool TrySpendDiamonds(int amount)
	{
		if (amount < 0 || _diamonds < amount)
		{
			return false;
		}

		_diamonds -= amount;
		OnDiamondsChanged?.Invoke(_diamonds);
		return true;
	}

	private int GetFirstUnlockedIndex()
	{
		for (int i = 0; i < Weapons.Count; i++)
		{
			if (Weapons[i]?.IsUnlocked == true)
			{
				return i;
			}
		}

		if (Weapons.Count > 0)
		{
			Weapons[0].Unlock();
		}

		return 0;
	}

	private void NotifyInventoryUpdated()
	{
		OnDiamondsChanged?.Invoke(_diamonds);
		OnInventoryUpdated?.Invoke();
	}

	private void SwitchRelative(int direction)
	{
		if (Weapons.Count == 0)
		{
			return;
		}

		for (int offset = 1; offset <= Weapons.Count; offset++)
		{
			int index = (_currentIndex + direction * offset + Weapons.Count * 2) % Weapons.Count;
			if (Weapons[index].IsUnlocked)
			{
				SwitchTo(index);
				return;
			}
		}
	}
}
