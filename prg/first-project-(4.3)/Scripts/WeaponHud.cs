using Godot;

public partial class WeaponHud : Control
{
	public WeaponInventory Inventory;

	private enum OverlayMode
	{
		None,
		Inventory,
		PauseMenu,
	}

	private OverlayMode _overlayMode = OverlayMode.None;
	private ColorRect _overlay;
	private MarginContainer _overlayFrame;
	private PanelContainer _menuPanel;
	private ScrollContainer _menuScroll;
	private VBoxContainer _menuContent;
	private ScrollContainer _bottomBarScroll;
	private HBoxContainer _bottomBar;
	private ProgressBar _nitroBar;
	private Label _nitroLabel;
	private Label _diamondLabel;
	private Label _modeLabel;
	private Label _directorLabel;

	public override void _Ready()
	{
		ProcessMode = ProcessModeEnum.Always;
		SetAnchorsPreset(LayoutPreset.FullRect);

		BuildOverlay();
		BuildTopInfo();
		BuildBottomBar();
		BuildNitroBar();
		UpdateResponsiveLayout();

		CallDeferred(nameof(FindInventory));
	}

	public override void _Notification(int what)
	{
		if (what == NotificationResized)
		{
			UpdateResponsiveLayout();
		}
	}

	private void BuildOverlay()
	{
		_overlay = new ColorRect();
		_overlay.Color = new Color(0.02f, 0.03f, 0.05f, 0.76f);
		_overlay.SetAnchorsPreset(LayoutPreset.FullRect);
		_overlay.Visible = false;
		_overlay.MouseFilter = MouseFilterEnum.Ignore;
		AddChild(_overlay);

		_overlayFrame = new MarginContainer();
		_overlayFrame.SetAnchorsPreset(LayoutPreset.FullRect);
		_overlay.AddChild(_overlayFrame);

		var center = new CenterContainer();
		_overlayFrame.AddChild(center);

		_menuPanel = new PanelContainer();
		_menuPanel.Visible = false;
		_menuPanel.AddThemeStyleboxOverride("panel", CreateCardStyle(new Color(0.9f, 0.75f, 0.35f), true, false));
		center.AddChild(_menuPanel);

		var menuMargin = new MarginContainer();
		menuMargin.AddThemeConstantOverride("margin_left", 22);
		menuMargin.AddThemeConstantOverride("margin_top", 22);
		menuMargin.AddThemeConstantOverride("margin_right", 22);
		menuMargin.AddThemeConstantOverride("margin_bottom", 22);
		_menuPanel.AddChild(menuMargin);

		_menuScroll = new ScrollContainer();
		_menuScroll.SizeFlagsHorizontal = SizeFlags.ExpandFill;
		_menuScroll.SizeFlagsVertical = SizeFlags.ExpandFill;
		menuMargin.AddChild(_menuScroll);

		_menuContent = new VBoxContainer();
		_menuContent.AddThemeConstantOverride("separation", 18);
		_menuContent.SizeFlagsHorizontal = SizeFlags.ExpandFill;
		_menuScroll.AddChild(_menuContent);
	}

	private void BuildTopInfo()
	{
		var infoRoot = new CenterContainer();
		infoRoot.AnchorLeft = 0f;
		infoRoot.AnchorRight = 1f;
		infoRoot.AnchorTop = 0f;
		infoRoot.AnchorBottom = 0f;
		infoRoot.OffsetTop = 12;
		infoRoot.OffsetBottom = 76;
		AddChild(infoRoot);

		var infoBar = new HBoxContainer();
		infoBar.Alignment = BoxContainer.AlignmentMode.Center;
		infoBar.AddThemeConstantOverride("separation", 10);
		infoRoot.AddChild(infoBar);

		_modeLabel = CreateChipLabel("COMBAT", new Color(0.2f, 0.7f, 1f));
		infoBar.AddChild(_modeLabel);

		_diamondLabel = CreateChipLabel("DIAMONDS 0", new Color(0.9f, 0.78f, 0.32f));
		infoBar.AddChild(_diamondLabel);

		_directorLabel = CreateChipLabel("SURVIVE", new Color(0.98f, 0.46f, 0.22f));
		infoBar.AddChild(_directorLabel);
	}

	private void BuildBottomBar()
	{
		var bottomCenter = new CenterContainer();
		bottomCenter.AnchorLeft = 0f;
		bottomCenter.AnchorRight = 1f;
		bottomCenter.AnchorTop = 1f;
		bottomCenter.AnchorBottom = 1f;
		bottomCenter.OffsetTop = -112;
		bottomCenter.OffsetBottom = -18;
		AddChild(bottomCenter);

		_bottomBarScroll = new ScrollContainer();
		_bottomBarScroll.CustomMinimumSize = new Vector2(0, 92);
		bottomCenter.AddChild(_bottomBarScroll);

		_bottomBar = new HBoxContainer();
		_bottomBar.Alignment = BoxContainer.AlignmentMode.Center;
		_bottomBar.AddThemeConstantOverride("separation", 12);
		_bottomBarScroll.AddChild(_bottomBar);
	}

	private void BuildNitroBar()
	{
		var nitroContainer = new HBoxContainer();
		nitroContainer.AnchorLeft = 0f;
		nitroContainer.AnchorRight = 0f;
		nitroContainer.AnchorTop = 1f;
		nitroContainer.AnchorBottom = 1f;
		nitroContainer.OffsetLeft = 16;
		nitroContainer.OffsetRight = 260;
		nitroContainer.OffsetTop = -40;
		nitroContainer.OffsetBottom = -12;
		nitroContainer.AddThemeConstantOverride("separation", 8);
		AddChild(nitroContainer);

		_nitroLabel = new Label();
		_nitroLabel.Text = "NITRO";
		_nitroLabel.AddThemeFontSizeOverride("font_size", 13);
		_nitroLabel.AddThemeColorOverride("font_color", new Color(0.24f, 0.9f, 1f));
		nitroContainer.AddChild(_nitroLabel);

		_nitroBar = new ProgressBar();
		_nitroBar.MinValue = 0;
		_nitroBar.MaxValue = 100;
		_nitroBar.Value = 100;
		_nitroBar.ShowPercentage = false;
		_nitroBar.SizeFlagsHorizontal = SizeFlags.ExpandFill;
		_nitroBar.CustomMinimumSize = new Vector2(160, 20);

		var barBg = new StyleBoxFlat();
		barBg.BgColor = new Color(0.1f, 0.12f, 0.16f, 0.92f);
		barBg.SetCornerRadiusAll(6);
		_nitroBar.AddThemeStyleboxOverride("background", barBg);

		var barFill = new StyleBoxFlat();
		barFill.BgColor = new Color(0.1f, 0.85f, 1f, 0.9f);
		barFill.SetCornerRadiusAll(6);
		_nitroBar.AddThemeStyleboxOverride("fill", barFill);
		nitroContainer.AddChild(_nitroBar);
	}

	private void FindInventory()
	{
		if (Inventory == null)
		{
			var game = GetTree().Root.GetNodeOrNull<Game>("Game");
			if (game != null)
			{
				Inventory = game.GetNodeOrNull<WeaponInventory>("WeaponInventory");
			}
		}

		if (Inventory != null)
		{
			Inventory.OnInventoryUpdated += UpdateAll;
		}

		var player = FindPlayer();
		if (player != null)
		{
			player.OnNitroChanged += UpdateNitroBar;
			UpdateNitroBar(player.Nitro);
		}

		UpdateAll();
	}

	public void SetMenuState(bool inventoryOpen, bool pauseMenuOpen)
	{
		if (pauseMenuOpen)
		{
			_overlayMode = OverlayMode.PauseMenu;
		}
		else if (inventoryOpen)
		{
			_overlayMode = OverlayMode.Inventory;
		}
		else
		{
			_overlayMode = OverlayMode.None;
		}

		bool visible = _overlayMode != OverlayMode.None;
		_overlay.Visible = visible;
		_overlay.MouseFilter = visible ? MouseFilterEnum.Stop : MouseFilterEnum.Ignore;
		_menuPanel.Visible = visible;
		UpdateAll();
	}

	public void SetInventoryOpen(bool open)
	{
		SetMenuState(open, false);
	}

	public void SetPauseMenuOpen(bool open)
	{
		SetMenuState(false, open);
	}

	public void SetDirectorStatus(string text)
	{
		if (_directorLabel != null)
		{
			_directorLabel.Text = text;
		}
	}

	private void UpdateNitroBar(float nitro)
	{
		if (_nitroBar == null)
		{
			return;
		}

		_nitroBar.Value = nitro;
		if (_nitroBar.GetThemeStylebox("fill") is StyleBoxFlat fill)
		{
			fill.BgColor = nitro < 20f
				? new Color(1f, 0.3f, 0.2f, 0.9f)
				: new Color(0.1f, 0.85f, 1f, 0.9f);
		}
	}

	private void UpdateAll()
	{
		UpdateResponsiveLayout();
		UpdateStatusBar();
		UpdateBottomBar();
		if (_overlayMode != OverlayMode.None)
		{
			UpdateMenu();
		}
	}

	private void UpdateStatusBar()
	{
		if (_diamondLabel != null)
		{
			int diamonds = Inventory?.Diamonds ?? 0;
			_diamondLabel.Text = $"DIAMONDS {diamonds}";
		}

		if (_modeLabel != null)
		{
			_modeLabel.Text = _overlayMode switch
			{
				OverlayMode.Inventory => "INVENTORY",
				OverlayMode.PauseMenu => "PAUSED",
				_ => "COMBAT",
			};
		}
	}

	private void UpdateBottomBar()
	{
		foreach (Node child in _bottomBar.GetChildren())
		{
			child.QueueFree();
		}

		if (Inventory == null)
		{
			return;
		}

		for (int i = 0; i < Inventory.Weapons.Count; i++)
		{
			var weapon = Inventory.Weapons[i];
			bool selected = i == Inventory.CurrentIndex;
			bool locked = !weapon.IsUnlocked;

			var button = new Button();
			button.Flat = true;
			button.FocusMode = FocusModeEnum.None;
			button.CustomMinimumSize = new Vector2(160, 82);
			button.AddThemeStyleboxOverride("normal", CreateCardStyle(weapon.Color, selected, locked));
			button.AddThemeStyleboxOverride("hover", CreateCardStyle(weapon.Color, true, locked));
			button.AddThemeStyleboxOverride("pressed", CreateCardStyle(weapon.Color, true, locked));
			button.AddThemeStyleboxOverride("disabled", CreateCardStyle(weapon.Color, selected, locked));
			button.Pressed += () => OnBottomBarPressed(i);
			_bottomBar.AddChild(button);

			var margin = new MarginContainer();
			margin.AddThemeConstantOverride("margin_left", 10);
			margin.AddThemeConstantOverride("margin_top", 10);
			margin.AddThemeConstantOverride("margin_right", 10);
			margin.AddThemeConstantOverride("margin_bottom", 10);
			button.AddChild(margin);

			var content = new HBoxContainer();
			content.Alignment = BoxContainer.AlignmentMode.Center;
			content.AddThemeConstantOverride("separation", 10);
			margin.AddChild(content);

			var icon = new TextureRect();
			icon.CustomMinimumSize = new Vector2(34, 34);
			icon.ExpandMode = TextureRect.ExpandModeEnum.IgnoreSize;
			icon.StretchMode = TextureRect.StretchModeEnum.KeepAspectCentered;
			if (!string.IsNullOrEmpty(weapon.IconPath))
			{
				icon.Texture = GD.Load<Texture2D>(weapon.IconPath);
			}
			content.AddChild(icon);

			var textColumn = new VBoxContainer();
			textColumn.Alignment = BoxContainer.AlignmentMode.Center;
			textColumn.AddThemeConstantOverride("separation", 2);
			content.AddChild(textColumn);

			var nameLabel = new Label();
			nameLabel.Text = locked ? $"{weapon.Name} LOCKED" : weapon.Name;
			nameLabel.AddThemeFontSizeOverride("font_size", selected ? 14 : 12);
			nameLabel.AddThemeColorOverride("font_color", locked ? new Color(0.82f, 0.82f, 0.86f, 0.7f) : Colors.White);
			textColumn.AddChild(nameLabel);

			var subLabel = new Label();
			subLabel.Text = locked
				? $"Unlock {weapon.UnlockCost} diamonds"
				: $"[{i + 1}]  LVL {weapon.Level}";
			subLabel.AddThemeFontSizeOverride("font_size", 10);
			subLabel.AddThemeColorOverride("font_color", locked ? new Color(0.88f, 0.72f, 0.36f) : new Color(0.75f, 0.82f, 0.9f));
			textColumn.AddChild(subLabel);
		}
	}

	private void UpdateMenu()
	{
		foreach (Node child in _menuContent.GetChildren())
		{
			child.QueueFree();
		}

		switch (_overlayMode)
		{
			case OverlayMode.Inventory:
				BuildInventoryMenu();
				break;
			case OverlayMode.PauseMenu:
				BuildPauseMenu();
				break;
		}
	}

	private void BuildInventoryMenu()
	{
		var header = new VBoxContainer();
		header.AddThemeConstantOverride("separation", 8);
		_menuContent.AddChild(header);

		var title = new Label();
		title.Text = "WEAPON INVENTORY";
		title.HorizontalAlignment = HorizontalAlignment.Center;
		title.AddThemeFontSizeOverride("font_size", 34);
		title.AddThemeColorOverride("font_color", new Color(0.98f, 0.88f, 0.66f));
		header.AddChild(title);

		var subtitle = new Label();
		subtitle.Text = "Click to equip, unlock or upgrade weapons.";
		subtitle.HorizontalAlignment = HorizontalAlignment.Center;
		subtitle.AddThemeFontSizeOverride("font_size", 14);
		subtitle.AddThemeColorOverride("font_color", new Color(0.76f, 0.8f, 0.88f));
		header.AddChild(subtitle);

		var topActions = new HBoxContainer();
		topActions.Alignment = BoxContainer.AlignmentMode.Center;
		topActions.AddThemeConstantOverride("separation", 10);
		_menuContent.AddChild(topActions);

		var pauseButton = CreatePrimaryButton("Pause Menu", new Color(0.98f, 0.58f, 0.22f));
		pauseButton.Pressed += () => FindPlayer()?.OpenPauseMenu();
		topActions.AddChild(pauseButton);

		var resumeButton = CreatePrimaryButton("Resume", new Color(0.2f, 0.78f, 0.46f));
		resumeButton.Pressed += () => FindPlayer()?.CloseMenus();
		topActions.AddChild(resumeButton);

		var cardsCenter = new CenterContainer();
		cardsCenter.SizeFlagsHorizontal = SizeFlags.ExpandFill;
		cardsCenter.SizeFlagsVertical = SizeFlags.ExpandFill;
		_menuContent.AddChild(cardsCenter);

		var grid = new GridContainer();
		grid.Columns = GetInventoryColumnCount();
		grid.AddThemeConstantOverride("h_separation", 16);
		grid.AddThemeConstantOverride("v_separation", 16);
		cardsCenter.AddChild(grid);

		if (Inventory == null)
		{
			return;
		}

		for (int i = 0; i < Inventory.Weapons.Count; i++)
		{
			grid.AddChild(CreateWeaponCard(i, Inventory.Weapons[i]));
		}

		var hint = new Label();
		hint.Text = "Mouse: click cards to equip or buy  |  Mouse wheel / [1-5]: quick switch  |  [TAB]: close";
		hint.HorizontalAlignment = HorizontalAlignment.Center;
		hint.AddThemeFontSizeOverride("font_size", 13);
		hint.AddThemeColorOverride("font_color", new Color(0.72f, 0.76f, 0.84f));
		_menuContent.AddChild(hint);
	}

	private void BuildPauseMenu()
	{
		var title = new Label();
		title.Text = "GAME MENU";
		title.HorizontalAlignment = HorizontalAlignment.Center;
		title.AddThemeFontSizeOverride("font_size", 36);
		title.AddThemeColorOverride("font_color", new Color(0.98f, 0.82f, 0.48f));
		_menuContent.AddChild(title);

		var subtitle = new Label();
		subtitle.Text = "Pause, adjust loadout, then jump back into the run.";
		subtitle.HorizontalAlignment = HorizontalAlignment.Center;
		subtitle.AddThemeFontSizeOverride("font_size", 14);
		subtitle.AddThemeColorOverride("font_color", new Color(0.76f, 0.8f, 0.88f));
		_menuContent.AddChild(subtitle);

		var statsRow = new HBoxContainer();
		statsRow.Alignment = BoxContainer.AlignmentMode.Center;
		statsRow.AddThemeConstantOverride("separation", 12);
		_menuContent.AddChild(statsRow);

		statsRow.AddChild(CreateChipLabel($"DIAMONDS {Inventory?.Diamonds ?? 0}", new Color(0.9f, 0.75f, 0.35f)));
		statsRow.AddChild(CreateChipLabel($"WEAPON {(Inventory?.CurrentWeapon?.Name ?? "NONE")}", new Color(0.24f, 0.72f, 1f)));
		statsRow.AddChild(CreateChipLabel(_directorLabel?.Text ?? "SURVIVE", new Color(0.98f, 0.46f, 0.22f)));

		var buttonColumn = new VBoxContainer();
		buttonColumn.Alignment = BoxContainer.AlignmentMode.Center;
		buttonColumn.AddThemeConstantOverride("separation", 14);
		buttonColumn.SizeFlagsVertical = SizeFlags.ExpandFill;
		_menuContent.AddChild(buttonColumn);

		var resumeButton = CreatePrimaryButton("Resume Run", new Color(0.2f, 0.78f, 0.46f));
		resumeButton.CustomMinimumSize = new Vector2(300, 54);
		resumeButton.Pressed += () => FindPlayer()?.CloseMenus();
		buttonColumn.AddChild(resumeButton);

		var inventoryButton = CreatePrimaryButton("Open Inventory", new Color(0.24f, 0.72f, 1f));
		inventoryButton.CustomMinimumSize = new Vector2(300, 54);
		inventoryButton.Pressed += () => FindPlayer()?.OpenInventoryMenu();
		buttonColumn.AddChild(inventoryButton);

		var quitButton = CreatePrimaryButton("Quit Game", new Color(0.98f, 0.34f, 0.26f));
		quitButton.CustomMinimumSize = new Vector2(300, 54);
		quitButton.Pressed += () => GetTree().Quit();
		buttonColumn.AddChild(quitButton);

		var hint = new Label();
		hint.Text = "[ESC] toggles the menu  |  [TAB] opens the inventory";
		hint.HorizontalAlignment = HorizontalAlignment.Center;
		hint.AddThemeFontSizeOverride("font_size", 13);
		hint.AddThemeColorOverride("font_color", new Color(0.72f, 0.76f, 0.84f));
		_menuContent.AddChild(hint);
	}

	private PanelContainer CreateWeaponCard(int index, WeaponData weapon)
	{
		bool selected = index == Inventory.CurrentIndex;
		bool locked = !weapon.IsUnlocked;

		var card = new PanelContainer();
		card.CustomMinimumSize = GetWeaponCardSize();
		card.AddThemeStyleboxOverride("panel", CreateCardStyle(weapon.Color, selected, locked));

		var margin = new MarginContainer();
		margin.AddThemeConstantOverride("margin_left", 14);
		margin.AddThemeConstantOverride("margin_top", 14);
		margin.AddThemeConstantOverride("margin_right", 14);
		margin.AddThemeConstantOverride("margin_bottom", 14);
		card.AddChild(margin);

		var column = new VBoxContainer();
		column.AddThemeConstantOverride("separation", 8);
		margin.AddChild(column);

		var keyLabel = new Label();
		keyLabel.Text = $"[{index + 1}]";
		keyLabel.HorizontalAlignment = HorizontalAlignment.Center;
		keyLabel.AddThemeFontSizeOverride("font_size", 13);
		keyLabel.AddThemeColorOverride("font_color", new Color(0.72f, 0.78f, 0.86f));
		column.AddChild(keyLabel);

		var imageContainer = new CenterContainer();
		imageContainer.CustomMinimumSize = new Vector2(100, 110);
		column.AddChild(imageContainer);

		var image = new TextureRect();
		image.CustomMinimumSize = new Vector2(88, 88);
		image.ExpandMode = TextureRect.ExpandModeEnum.IgnoreSize;
		image.StretchMode = TextureRect.StretchModeEnum.KeepAspectCentered;
		if (!string.IsNullOrEmpty(weapon.IconPath))
		{
			image.Texture = GD.Load<Texture2D>(weapon.IconPath);
		}
		imageContainer.AddChild(image);

		var nameLabel = new Label();
		nameLabel.Text = weapon.Name;
		nameLabel.HorizontalAlignment = HorizontalAlignment.Center;
		nameLabel.AddThemeFontSizeOverride("font_size", 18);
		nameLabel.AddThemeColorOverride("font_color", Colors.White);
		column.AddChild(nameLabel);

		var descLabel = new Label();
		descLabel.Text = weapon.Description;
		descLabel.HorizontalAlignment = HorizontalAlignment.Center;
		descLabel.AutowrapMode = TextServer.AutowrapMode.WordSmart;
		descLabel.CustomMinimumSize = new Vector2(0, 52);
		descLabel.AddThemeFontSizeOverride("font_size", 11);
		descLabel.AddThemeColorOverride("font_color", new Color(0.8f, 0.84f, 0.9f));
		column.AddChild(descLabel);

		var separator = new HSeparator();
		column.AddChild(separator);

		column.AddChild(CreateStatLabel($"Damage  {weapon.Damage:0.#}", new Color(1f, 0.56f, 0.4f)));
		column.AddChild(CreateStatLabel($"Rate    {weapon.FireRate:0.##}/s", new Color(0.38f, 0.82f, 1f)));
		column.AddChild(CreateStatLabel($"Level   {weapon.Level}/{weapon.MaxLevel}", new Color(0.8f, 1f, 0.56f)));

		var stateLabel = new Label();
		stateLabel.Text = locked
			? $"Locked for {weapon.UnlockCost} diamonds"
			: selected
				? "Equipped"
				: weapon.CanUpgrade()
					? $"Upgrade costs {weapon.GetUpgradePrice()} diamonds"
					: "Fully upgraded";
		stateLabel.HorizontalAlignment = HorizontalAlignment.Center;
		stateLabel.AutowrapMode = TextServer.AutowrapMode.WordSmart;
		stateLabel.CustomMinimumSize = new Vector2(0, 38);
		stateLabel.AddThemeFontSizeOverride("font_size", 11);
		stateLabel.AddThemeColorOverride("font_color", locked ? new Color(0.96f, 0.78f, 0.42f) : weapon.Color);
		column.AddChild(stateLabel);

		var actionButton = CreatePrimaryButton(GetWeaponActionText(index, weapon, selected), weapon.Color);
		actionButton.Disabled = locked ? Inventory.Diamonds < weapon.UnlockCost : selected && !weapon.CanUpgrade();
		actionButton.CustomMinimumSize = new Vector2(0, 46);
		actionButton.Pressed += () => HandleWeaponAction(index);
		column.AddChild(actionButton);

		return card;
	}

	private void HandleWeaponAction(int index)
	{
		if (Inventory == null)
		{
			return;
		}

		var weapon = Inventory.Weapons[index];
		if (!weapon.IsUnlocked)
		{
			Inventory.UnlockWeapon(index);
			return;
		}

		if (Inventory.CurrentIndex != index)
		{
			Inventory.SwitchTo(index);
			return;
		}

		Inventory.UpgradeWeapon(index);
	}

	private void OnBottomBarPressed(int index)
	{
		if (Inventory == null)
		{
			return;
		}

		if (Inventory.IsUnlocked(index))
		{
			Inventory.SwitchTo(index);
			return;
		}

		FindPlayer()?.OpenInventoryMenu();
	}

	private Player FindPlayer()
	{
		return GetTree().Root.GetNodeOrNull<Game>("Game")?.GetNodeOrNull<Player>("Player");
	}

	private Label CreateChipLabel(string text, Color accent)
	{
		var label = new Label();
		label.Text = text;
		label.AddThemeFontSizeOverride("font_size", 13);
		label.AddThemeColorOverride("font_color", accent.Lightened(0.2f));
		label.AddThemeStyleboxOverride("normal", CreateChipStyle(accent));
		return label;
	}

	private StyleBoxFlat CreateChipStyle(Color accent)
	{
		var style = new StyleBoxFlat();
		style.BgColor = new Color(0.08f, 0.1f, 0.14f, 0.9f);
		style.BorderColor = accent;
		style.SetBorderWidthAll(2);
		style.SetCornerRadiusAll(999);
		style.ContentMarginLeft = 14;
		style.ContentMarginTop = 8;
		style.ContentMarginRight = 14;
		style.ContentMarginBottom = 8;
		return style;
	}

	private StyleBoxFlat CreateCardStyle(Color accent, bool highlight, bool locked)
	{
		var style = new StyleBoxFlat();
		style.BgColor = locked
			? new Color(0.08f, 0.09f, 0.11f, 0.94f)
			: highlight
				? new Color(accent, 0.24f)
				: new Color(0.09f, 0.1f, 0.13f, 0.94f);
		style.BorderColor = locked
			? new Color(0.78f, 0.68f, 0.36f, 0.7f)
			: highlight
				? accent
				: new Color(0.24f, 0.28f, 0.34f, 0.7f);
		style.SetBorderWidthAll(highlight ? 3 : 1);
		style.SetCornerRadiusAll(18);
		style.SetContentMarginAll(8);
		return style;
	}

	private Label CreateStatLabel(string text, Color color)
	{
		var label = new Label();
		label.Text = text;
		label.HorizontalAlignment = HorizontalAlignment.Center;
		label.AddThemeFontSizeOverride("font_size", 12);
		label.AddThemeColorOverride("font_color", color);
		return label;
	}

	private Button CreatePrimaryButton(string text, Color accent)
	{
		var button = new Button();
		button.Text = text;
		button.Flat = true;
		button.FocusMode = FocusModeEnum.None;
		button.AddThemeFontSizeOverride("font_size", 13);
		button.AddThemeColorOverride("font_color", Colors.White);

		var normal = new StyleBoxFlat();
		normal.BgColor = new Color(accent, 0.22f);
		normal.BorderColor = accent;
		normal.SetBorderWidthAll(2);
		normal.SetCornerRadiusAll(12);
		normal.SetContentMarginAll(12);

		var hover = new StyleBoxFlat();
		hover.BgColor = new Color(accent, 0.32f);
		hover.BorderColor = accent.Lightened(0.1f);
		hover.SetBorderWidthAll(2);
		hover.SetCornerRadiusAll(12);
		hover.SetContentMarginAll(12);

		var disabled = new StyleBoxFlat();
		disabled.BgColor = new Color(0.14f, 0.15f, 0.18f, 0.92f);
		disabled.BorderColor = new Color(0.36f, 0.38f, 0.42f, 0.8f);
		disabled.SetBorderWidthAll(1);
		disabled.SetCornerRadiusAll(12);
		disabled.SetContentMarginAll(12);

		button.AddThemeStyleboxOverride("normal", normal);
		button.AddThemeStyleboxOverride("hover", hover);
		button.AddThemeStyleboxOverride("pressed", hover);
		button.AddThemeStyleboxOverride("disabled", disabled);
		return button;
	}

	private string GetWeaponActionText(int index, WeaponData weapon, bool selected)
	{
		if (!weapon.IsUnlocked)
		{
			return $"Unlock {weapon.UnlockCost}";
		}

		if (!selected)
		{
			return "Equip";
		}

		if (!weapon.CanUpgrade())
		{
			return "Maxed Out";
		}

		return $"Upgrade {weapon.GetUpgradePrice()}";
	}

	private void UpdateResponsiveLayout()
	{
		Vector2 viewportSize = GetViewportRect().Size;
		if (viewportSize == Vector2.Zero)
		{
			return;
		}

		if (_overlayFrame != null)
		{
			int horizontalMargin = (int)Mathf.Clamp(viewportSize.X * 0.035f, 14f, 42f);
			int topMargin = (int)Mathf.Clamp(viewportSize.Y * 0.04f, 14f, 40f);
			int bottomMargin = (int)Mathf.Clamp(viewportSize.Y * 0.14f, 88f, 130f);
			_overlayFrame.AddThemeConstantOverride("margin_left", horizontalMargin);
			_overlayFrame.AddThemeConstantOverride("margin_top", topMargin);
			_overlayFrame.AddThemeConstantOverride("margin_right", horizontalMargin);
			_overlayFrame.AddThemeConstantOverride("margin_bottom", bottomMargin);
		}

		if (_menuPanel != null)
		{
			float panelWidth = Mathf.Clamp(viewportSize.X - 80f, 300f, 1180f);
			float panelHeight = Mathf.Clamp(viewportSize.Y - 130f, 320f, 760f);
			_menuPanel.CustomMinimumSize = new Vector2(panelWidth, panelHeight);
		}

		if (_bottomBarScroll != null)
		{
			float width = Mathf.Clamp(viewportSize.X - 32f, 260f, 940f);
			_bottomBarScroll.CustomMinimumSize = new Vector2(width, 92f);
		}
	}

	private int GetInventoryColumnCount()
	{
		float viewportWidth = GetViewportRect().Size.X;
		if (viewportWidth < 720f)
		{
			return 1;
		}

		if (viewportWidth < 980f)
		{
			return 2;
		}

		if (viewportWidth < 1320f)
		{
			return 3;
		}

		return 5;
	}

	private Vector2 GetWeaponCardSize()
	{
		float viewportWidth = GetViewportRect().Size.X;
		if (viewportWidth < 720f)
		{
			return new Vector2(250f, 350f);
		}

		if (viewportWidth < 980f)
		{
			return new Vector2(220f, 360f);
		}

		return new Vector2(180f, 370f);
	}
}
