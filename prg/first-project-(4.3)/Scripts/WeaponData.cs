using Godot;

[GlobalClass]
public partial class WeaponData : Resource
{
	[Export] public string Name { get; set; } = "Pistol";
	[Export] public float Damage { get; set; } = 10;
	[Export] public float FireRate { get; set; } = 2;
	[Export] public Color Color { get; set; } = Colors.Yellow;
}
