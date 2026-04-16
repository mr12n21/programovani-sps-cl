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
			Weapons.Add(new WeaponData { Name = "Pistol", Damage = 10, FireRate = 2, Color = Colors.Yellow });
			Weapons.Add(new WeaponData { Name = "Shotgun", Damage = 25, FireRate = 0.8f, Color = Colors.Red });
			Weapons.Add(new WeaponData { Name = "Machine Gun", Damage = 5, FireRate = 8, Color = Colors.Cyan });
			Weapons.Add(new WeaponData { Name = "Sniper", Damage = 80, FireRate = 0.5f, Color = Colors.Purple });
			Weapons.Add(new WeaponData { Name = "Laser", Damage = 15, FireRate = 12, Color = Colors.Green });
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
