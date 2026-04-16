using Godot;
using System;

public partial class WeaponHud : Control
{
	public WeaponInventory Inventory;
	
	private HBoxContainer _container;
	private bool _inventoryOpen = false;
	private ColorRect _overlay;

	public override void _Ready()
	{
		_container = new HBoxContainer();
		_container.AnchorLeft = 0.5f;
		_container.AnchorRight = 0.5f;
		_container.AnchorTop = 1f;
		_container.AnchorBottom = 1f;
		_container.GrowHorizontal = GrowDirection.Both;
		_container.OffsetTop = -60;
		_container.OffsetBottom = -10;
		_container.OffsetLeft = -200;
		_container.OffsetRight = 200;
		_container.AddThemeConstantOverride("separation", 8);
		AddChild(_container);

		_overlay = new ColorRect();
		_overlay.Color = new Color(0, 0, 0, 0.5f);
		_overlay.SetAnchorsPreset(LayoutPreset.FullRect);
		_overlay.Visible = false;
		_overlay.MouseFilter = MouseFilterEnum.Ignore;
		AddChild(_overlay);
		_overlay.MoveToFront();

		ProcessMode = ProcessModeEnum.Always;

		CallDeferred(nameof(FindInventory));
	}

	private void FindInventory()
	{
		if (Inventory == null)
		{
			var game = GetTree().Root.GetNodeOrNull<Game>("Game");
			if (game != null)
				Inventory = game.GetNodeOrNull<WeaponInventory>("WeaponInventory");
		}
		if (Inventory != null)
		{
			Inventory.OnWeaponChanged += (_, __) => UpdateDisplay();
			UpdateDisplay();
		}
	}

	public void SetInventoryOpen(bool open)
	{
		_inventoryOpen = open;
		_overlay.Visible = open;
		_container.Visible = true;
		if (open)
		{
			_container.AnchorTop = 0.5f;
			_container.AnchorBottom = 0.5f;
			_container.OffsetTop = -50;
			_container.OffsetBottom = 50;
			_container.OffsetLeft = -350;
			_container.OffsetRight = 350;
		}
		else
		{
			_container.AnchorTop = 1f;
			_container.AnchorBottom = 1f;
			_container.OffsetTop = -60;
			_container.OffsetBottom = -10;
			_container.OffsetLeft = -200;
			_container.OffsetRight = 200;
		}
		UpdateDisplay();
	}

	private void UpdateDisplay()
	{
		foreach (var child in _container.GetChildren())
			child.QueueFree();

		if (Inventory == null) return;

		for (int i = 0; i < Inventory.Weapons.Count; i++)
		{
			var weapon = Inventory.Weapons[i];
			bool selected = i == Inventory.CurrentIndex;

			var panel = new PanelContainer();
			panel.SizeFlagsHorizontal = SizeFlags.ExpandFill;
			panel.CustomMinimumSize = _inventoryOpen ? new Vector2(120, 80) : new Vector2(70, 45);

			var styleBox = new StyleBoxFlat();
			styleBox.BgColor = selected ? new Color(weapon.Color, 0.6f) : new Color(0.15f, 0.15f, 0.15f, 0.7f);
			styleBox.BorderColor = selected ? weapon.Color : new Color(0.4f, 0.4f, 0.4f, 0.5f);
			styleBox.SetBorderWidthAll(selected ? 3 : 1);
			styleBox.SetCornerRadiusAll(6);
			panel.AddThemeStyleboxOverride("panel", styleBox);

			var vbox = new VBoxContainer();
			vbox.Alignment = BoxContainer.AlignmentMode.Center;

			var keyLabel = new Label();
			keyLabel.Text = $"[{i + 1}]";
			keyLabel.HorizontalAlignment = HorizontalAlignment.Center;
			keyLabel.AddThemeFontSizeOverride("font_size", 11);

			var nameLabel = new Label();
			nameLabel.Text = weapon.Name;
			nameLabel.HorizontalAlignment = HorizontalAlignment.Center;
			nameLabel.AddThemeFontSizeOverride("font_size", 13);

			vbox.AddChild(keyLabel);
			vbox.AddChild(nameLabel);

			if (_inventoryOpen)
			{
				var dmgLabel = new Label();
				dmgLabel.Text = $"DMG: {weapon.Damage}";
				dmgLabel.HorizontalAlignment = HorizontalAlignment.Center;
				dmgLabel.AddThemeFontSizeOverride("font_size", 10);
				vbox.AddChild(dmgLabel);

				var rateLabel = new Label();
				rateLabel.Text = $"Rate: {weapon.FireRate}/s";
				rateLabel.HorizontalAlignment = HorizontalAlignment.Center;
				rateLabel.AddThemeFontSizeOverride("font_size", 10);
				vbox.AddChild(rateLabel);
			}

			panel.AddChild(vbox);
			_container.AddChild(panel);
		}
	}
}
