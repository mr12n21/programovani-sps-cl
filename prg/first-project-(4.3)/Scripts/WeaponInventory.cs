using Godot;
using System;
using System.Collections.Generic;

public partial class WeaponInventory : Node
{
	[Export] public Godot.Collections.Array<WeaponData> Weapons { get; set; } = new();
	
	private int _currentIndex = 0;
	public int CurrentIndex => _currentIndex;
	public WeaponData CurrentWeapon => Weapons.Count > 0 ? Weapons[_currentIndex] : null;
	
	public event Action<WeaponData, int> OnWeaponChanged;

	public override void _Ready()
	{
		if (Weapons.Count == 0)
		{
			Weapons.Add(new WeaponData { Name = "Pistol", Damage = 10, FireRate = 2, Color = Colors.Yellow, IconPath = "res://Assets/gun.png" });
			Weapons.Add(new WeaponData { Name = "Shotgun", Damage = 25, FireRate = 0.8f, Color = Colors.Red, IconPath = "res://Assets/bullet.png" });
			Weapons.Add(new WeaponData { Name = "Machine Gun", Damage = 5, FireRate = 8, Color = Colors.Cyan, IconPath = "res://Assets/machine_gun.png" });
			Weapons.Add(new WeaponData { Name = "Tesla", Damage = 40, FireRate = 1.5f, Color = Colors.DodgerBlue, IconPath = "res://Assets/lightning1.png" });
			Weapons.Add(new WeaponData { Name = "Lightning", Damage = 15, FireRate = 12, Color = Colors.Green, IconPath = "res://Assets/lightning2.png" });
		}
	}

	public void SwitchTo(int index)
	{
		if (index < 0 || index >= Weapons.Count) return;
		_currentIndex = index;
		OnWeaponChanged?.Invoke(CurrentWeapon, _currentIndex);
	}

	public void Next()
	{
		SwitchTo((_currentIndex + 1) % Weapons.Count);
	}

	public void Previous()
	{
		SwitchTo((_currentIndex - 1 + Weapons.Count) % Weapons.Count);
	}
}
