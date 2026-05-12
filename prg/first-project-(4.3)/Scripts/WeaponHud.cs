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
		_overlay.Color = new Color(0.01f, 0.02f, 0.05f, 0.9f);
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
		_menuPanel.AddThemeStyleboxOverride("panel", CreateCardStyle(new Color(0.96f, 0.74f, 0.34f), true, false));
		center.AddChild(_menuPanel);

		var menuMargin = new MarginContainer();
		menuMargin.AddThemeConstantOverride("margin_left", 24);
		menuMargin.AddThemeConstantOverride("margin_top", 24);
		menuMargin.AddThemeConstantOverride("margin_right", 24);
		menuMargin.AddThemeConstantOverride("margin_bottom", 24);
		_menuPanel.AddChild(menuMargin);

		_menuContent = new VBoxContainer();
		_menuContent.AddThemeConstantOverride("separation", 22);
		_menuContent.SizeFlagsHorizontal = SizeFlags.ExpandFill;
		_menuContent.SizeFlagsVertical = SizeFlags.ExpandFill;
		menuMargin.AddChild(_menuContent);
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

		_modeLabel = CreateChipLabel("COMBAT", new Color(0.52f, 0.78f, 1f));
		infoBar.AddChild(_modeLabel);

		_diamondLabel = CreateChipLabel("DIAMONDS 0", new Color(0.7f, 0.8f, 0.92f));
		infoBar.AddChild(_diamondLabel);

		_directorLabel = CreateChipLabel("SURVIVE", new Color(0.62f, 0.88f, 1f));
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
		_nitroLabel.AddThemeColorOverride("font_color", new Color(0.62f, 0.88f, 1f));
		nitroContainer.AddChild(_nitroLabel);

		_nitroBar = new ProgressBar();
		_nitroBar.MinValue = 0;
		_nitroBar.MaxValue = 100;
		_nitroBar.Value = 100;
		_nitroBar.ShowPercentage = false;
		_nitroBar.SizeFlagsHorizontal = SizeFlags.ExpandFill;
		_nitroBar.CustomMinimumSize = new Vector2(160, 20);

		var barBg = new StyleBoxFlat();
		barBg.BgColor = new Color(0.06f, 0.08f, 0.12f, 0.94f);
		barBg.SetCornerRadiusAll(6);
		_nitroBar.AddThemeStyleboxOverride("background", barBg);

		var barFill = new StyleBoxFlat();
		barFill.BgColor = new Color(0.4f, 0.78f, 1f, 0.92f);
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
				? new Color(0.97f, 0.45f, 0.35f, 0.9f)
				: new Color(0.4f, 0.78f, 1f, 0.92f);
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
			int buttonIndex = i;
			var weapon = Inventory.Weapons[buttonIndex];
			bool selected = buttonIndex == Inventory.CurrentIndex;
			bool locked = !weapon.IsUnlocked;

			var button = new Button();
			button.Flat = true;
			button.FocusMode = FocusModeEnum.None;
			button.CustomMinimumSize = new Vector2(148, 82);
			button.AddThemeStyleboxOverride("normal", CreateCardStyle(weapon.Color, selected, locked));
			button.AddThemeStyleboxOverride("hover", CreateCardStyle(weapon.Color, true, locked));
			button.AddThemeStyleboxOverride("pressed", CreateCardStyle(weapon.Color, true, locked));
			button.AddThemeStyleboxOverride("disabled", CreateCardStyle(weapon.Color, selected, locked));
			button.Pressed += () => OnBottomBarPressed(buttonIndex);
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
			nameLabel.AddThemeFontSizeOverride("font_size", selected ? 15 : 13);
			nameLabel.AddThemeColorOverride("font_color", locked ? new Color(0.82f, 0.82f, 0.86f, 0.7f) : Colors.White);
			textColumn.AddChild(nameLabel);

			var subLabel = new Label();
			subLabel.Text = locked
				? $"Unlock {weapon.UnlockCost}"
				: $"[{buttonIndex + 1}]  LVL {weapon.Level}";
			subLabel.AddThemeFontSizeOverride("font_size", 11);
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
		header.AddThemeConstantOverride("separation", 10);
		_menuContent.AddChild(header);

		var title = new Label();
		title.Text = "WEAPON INVENTORY";
		title.HorizontalAlignment = HorizontalAlignment.Center;
		title.AddThemeFontSizeOverride("font_size", 40);
		title.AddThemeColorOverride("font_color", new Color(1f, 0.9f, 0.68f));
		header.AddChild(title);

		var subtitle = new Label();
		subtitle.Text = "All weapons stay visible at once. Click any card to equip, unlock or upgrade.";
		subtitle.HorizontalAlignment = HorizontalAlignment.Center;
		subtitle.AddThemeFontSizeOverride("font_size", 16);
		subtitle.AddThemeColorOverride("font_color", new Color(0.82f, 0.86f, 0.92f));
		header.AddChild(subtitle);

		_menuContent.AddChild(CreateSummaryRow());
		_menuContent.AddChild(CreateMenuNavigation(OverlayMode.Inventory));

		var cardsCenter = new CenterContainer();
		cardsCenter.SizeFlagsHorizontal = SizeFlags.ExpandFill;
		cardsCenter.SizeFlagsVertical = SizeFlags.ExpandFill;
		_menuContent.AddChild(cardsCenter);

		var grid = new GridContainer();
		grid.Columns = GetInventoryColumnCount();
		grid.AddThemeConstantOverride("h_separation", 12);
		grid.AddThemeConstantOverride("v_separation", 12);
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
		hint.Text = "Click a card to equip or buy  |  Mouse wheel or [1-5] for quick switch  |  [TAB] closes inventory";
		hint.HorizontalAlignment = HorizontalAlignment.Center;
		hint.AddThemeFontSizeOverride("font_size", 14);
		hint.AddThemeColorOverride("font_color", new Color(0.78f, 0.82f, 0.9f));
		_menuContent.AddChild(hint);
	}

	private void BuildPauseMenu()
	{
		var header = new VBoxContainer();
		header.AddThemeConstantOverride("separation", 10);
		_menuContent.AddChild(header);

		var title = new Label();
		title.Text = "GAME MENU";
		title.HorizontalAlignment = HorizontalAlignment.Center;
		title.AddThemeFontSizeOverride("font_size", 42);
		title.AddThemeColorOverride("font_color", new Color(1f, 0.84f, 0.52f));
		header.AddChild(title);

		var subtitle = new Label();
		subtitle.Text = "High-contrast pause screen with direct access to loadout and test currency.";
		subtitle.HorizontalAlignment = HorizontalAlignment.Center;
		subtitle.AddThemeFontSizeOverride("font_size", 16);
		subtitle.AddThemeColorOverride("font_color", new Color(0.82f, 0.86f, 0.92f));
		header.AddChild(subtitle);

		_menuContent.AddChild(CreateSummaryRow());
		_menuContent.AddChild(CreateMenuNavigation(OverlayMode.PauseMenu));

		var sectionGrid = new GridContainer();
		sectionGrid.Columns = GetPauseSectionColumnCount();
		sectionGrid.SizeFlagsHorizontal = SizeFlags.ExpandFill;
		sectionGrid.SizeFlagsVertical = SizeFlags.ExpandFill;
		sectionGrid.AddThemeConstantOverride("h_separation", 18);
		sectionGrid.AddThemeConstantOverride("v_separation", 18);
		_menuContent.AddChild(sectionGrid);

		sectionGrid.AddChild(CreatePauseActionSection());
		sectionGrid.AddChild(CreateDiamondDebugSection());

		var hint = new Label();
		hint.Text = "[ESC] closes the menu  |  [TAB] opens inventory  |  test diamonds update instantly";
		hint.HorizontalAlignment = HorizontalAlignment.Center;
		hint.AddThemeFontSizeOverride("font_size", 14);
		hint.AddThemeColorOverride("font_color", new Color(0.78f, 0.82f, 0.9f));
		_menuContent.AddChild(hint);
	}

	private PanelContainer CreatePauseActionSection()
	{
		var panel = CreateMenuSection(new Color(0.28f, 0.72f, 1f));
		var column = AddSectionContent(panel);

		column.AddChild(CreateSectionTitle("RUN CONTROLS", new Color(0.72f, 0.88f, 1f)));
		column.AddChild(CreateSectionText("Large buttons for the three actions you use most while paused."));

		var resumeButton = CreatePrimaryButton("Resume Run", new Color(0.2f, 0.78f, 0.46f));
		resumeButton.CustomMinimumSize = new Vector2(0, 58);
		resumeButton.SizeFlagsHorizontal = SizeFlags.ExpandFill;
		resumeButton.Pressed += () => FindPlayer()?.CloseMenus();
		column.AddChild(resumeButton);

		var inventoryButton = CreatePrimaryButton("Open Inventory", new Color(0.24f, 0.72f, 1f));
		inventoryButton.CustomMinimumSize = new Vector2(0, 58);
		inventoryButton.SizeFlagsHorizontal = SizeFlags.ExpandFill;
		inventoryButton.Pressed += () => FindPlayer()?.OpenInventoryMenu();
		column.AddChild(inventoryButton);

		var quitButton = CreatePrimaryButton("Quit Game", new Color(0.98f, 0.34f, 0.26f));
		quitButton.CustomMinimumSize = new Vector2(0, 58);
		quitButton.SizeFlagsHorizontal = SizeFlags.ExpandFill;
		quitButton.Pressed += () => GetTree().Quit();
		column.AddChild(quitButton);

		return panel;
	}

	private PanelContainer CreateDiamondDebugSection()
	{
		var panel = CreateMenuSection(new Color(0.96f, 0.74f, 0.34f));
		var column = AddSectionContent(panel);

		column.AddChild(CreateSectionTitle("TEST DIAMONDS", new Color(1f, 0.88f, 0.54f)));
		column.AddChild(CreateSectionText("Change your current diamonds instantly while testing unlocks and upgrades."));

		var amountLabel = new Label();
		amountLabel.Text = $"CURRENT: {Inventory?.Diamonds ?? 0}";
		amountLabel.HorizontalAlignment = HorizontalAlignment.Center;
		amountLabel.AddThemeFontSizeOverride("font_size", 24);
		amountLabel.AddThemeColorOverride("font_color", new Color(1f, 0.96f, 0.82f));
		column.AddChild(amountLabel);

		var buttonRow = new GridContainer();
		buttonRow.Columns = 2;
		buttonRow.SizeFlagsHorizontal = SizeFlags.ExpandFill;
		buttonRow.AddThemeConstantOverride("h_separation", 12);
		buttonRow.AddThemeConstantOverride("v_separation", 12);
		column.AddChild(buttonRow);

		var minusButton = CreatePrimaryButton("-25", new Color(0.98f, 0.4f, 0.32f));
		minusButton.SizeFlagsHorizontal = SizeFlags.ExpandFill;
		minusButton.CustomMinimumSize = new Vector2(0, 52);
		minusButton.Pressed += () => Inventory?.ChangeDiamonds(-25);
		buttonRow.AddChild(minusButton);

		var plusButton = CreatePrimaryButton("+25", new Color(0.96f, 0.74f, 0.34f));
		plusButton.SizeFlagsHorizontal = SizeFlags.ExpandFill;
		plusButton.CustomMinimumSize = new Vector2(0, 52);
		plusButton.Pressed += () => Inventory?.ChangeDiamonds(25);
		buttonRow.AddChild(plusButton);

		var plusBigButton = CreatePrimaryButton("+100", new Color(0.46f, 0.86f, 0.46f));
		plusBigButton.SizeFlagsHorizontal = SizeFlags.ExpandFill;
		plusBigButton.CustomMinimumSize = new Vector2(0, 52);
		plusBigButton.Pressed += () => Inventory?.ChangeDiamonds(100);
		buttonRow.AddChild(plusBigButton);

		var resetButton = CreatePrimaryButton("Reset 0", new Color(0.54f, 0.66f, 0.92f));
		resetButton.SizeFlagsHorizontal = SizeFlags.ExpandFill;
		resetButton.CustomMinimumSize = new Vector2(0, 52);
		resetButton.Pressed += () => Inventory?.SetDiamonds(0);
		buttonRow.AddChild(resetButton);

		return panel;
	}

	private PanelContainer CreateWeaponCard(int index, WeaponData weapon)
	{
		bool selected = index == Inventory.CurrentIndex;
		bool locked = !weapon.IsUnlocked;

		var card = new PanelContainer();
		card.CustomMinimumSize = GetWeaponCardSize();
		card.AddThemeStyleboxOverride("panel", CreateCardStyle(weapon.Color, selected, locked));

		var margin = new MarginContainer();
		margin.AddThemeConstantOverride("margin_left", 12);
		margin.AddThemeConstantOverride("margin_top", 12);
		margin.AddThemeConstantOverride("margin_right", 12);
		margin.AddThemeConstantOverride("margin_bottom", 12);
		card.AddChild(margin);

		var column = new VBoxContainer();
		column.AddThemeConstantOverride("separation", 6);
		margin.AddChild(column);

		var keyLabel = new Label();
		keyLabel.Text = $"[{index + 1}]";
		keyLabel.HorizontalAlignment = HorizontalAlignment.Center;
		keyLabel.AddThemeFontSizeOverride("font_size", 12);
		keyLabel.AddThemeColorOverride("font_color", new Color(0.72f, 0.78f, 0.86f));
		column.AddChild(keyLabel);

		var imageContainer = new CenterContainer();
		imageContainer.CustomMinimumSize = new Vector2(88, 74);
		column.AddChild(imageContainer);

		var image = new TextureRect();
		image.CustomMinimumSize = new Vector2(60, 60);
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
		nameLabel.AddThemeFontSizeOverride("font_size", 16);
		nameLabel.AddThemeColorOverride("font_color", Colors.White);
		column.AddChild(nameLabel);

		var descLabel = new Label();
		descLabel.Text = weapon.Description;
		descLabel.HorizontalAlignment = HorizontalAlignment.Center;
		descLabel.AutowrapMode = TextServer.AutowrapMode.WordSmart;
		descLabel.CustomMinimumSize = new Vector2(0, 40);
		descLabel.AddThemeFontSizeOverride("font_size", 12);
		descLabel.AddThemeColorOverride("font_color", new Color(0.8f, 0.84f, 0.9f));
		column.AddChild(descLabel);

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
		stateLabel.CustomMinimumSize = new Vector2(0, 34);
		stateLabel.AddThemeFontSizeOverride("font_size", 12);
		stateLabel.AddThemeColorOverride("font_color", locked ? new Color(0.96f, 0.78f, 0.42f) : weapon.Color);
		column.AddChild(stateLabel);

		var actionButton = CreatePrimaryButton(GetWeaponActionText(index, weapon, selected), weapon.Color);
		actionButton.Disabled = locked ? Inventory.Diamonds < weapon.UnlockCost : selected && !weapon.CanUpgrade();
		actionButton.CustomMinimumSize = new Vector2(0, 44);
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

	private HBoxContainer CreateSummaryRow()
	{
		var statsRow = new HBoxContainer();
		statsRow.Alignment = BoxContainer.AlignmentMode.Center;
		statsRow.AddThemeConstantOverride("separation", 12);
		statsRow.SizeFlagsHorizontal = SizeFlags.ExpandFill;
		statsRow.AddChild(CreateChipLabel($"DIAMONDS {Inventory?.Diamonds ?? 0}", new Color(0.94f, 0.76f, 0.34f)));
		statsRow.AddChild(CreateChipLabel($"WEAPON {(Inventory?.CurrentWeapon?.Name ?? "NONE")}", new Color(0.32f, 0.76f, 1f)));
		statsRow.AddChild(CreateChipLabel(_directorLabel?.Text ?? "SURVIVE", new Color(0.98f, 0.46f, 0.22f)));
		return statsRow;
	}

	private HBoxContainer CreateMenuNavigation(OverlayMode activeMode)
	{
		var nav = new HBoxContainer();
		nav.Alignment = BoxContainer.AlignmentMode.Center;
		nav.AddThemeConstantOverride("separation", 12);
		nav.SizeFlagsHorizontal = SizeFlags.ExpandFill;

		var inventoryButton = CreatePrimaryButton("Inventory", new Color(0.3f, 0.72f, 1f));
		inventoryButton.CustomMinimumSize = new Vector2(180, 50);
		inventoryButton.Disabled = activeMode == OverlayMode.Inventory;
		inventoryButton.Pressed += () => FindPlayer()?.OpenInventoryMenu();
		nav.AddChild(inventoryButton);

		var pauseButton = CreatePrimaryButton("Game Menu", new Color(0.98f, 0.6f, 0.26f));
		pauseButton.CustomMinimumSize = new Vector2(180, 50);
		pauseButton.Disabled = activeMode == OverlayMode.PauseMenu;
		pauseButton.Pressed += () => FindPlayer()?.OpenPauseMenu();
		nav.AddChild(pauseButton);

		var resumeButton = CreatePrimaryButton("Resume", new Color(0.22f, 0.78f, 0.46f));
		resumeButton.CustomMinimumSize = new Vector2(180, 50);
		resumeButton.Pressed += () => FindPlayer()?.CloseMenus();
		nav.AddChild(resumeButton);

		return nav;
	}

	private PanelContainer CreateMenuSection(Color accent)
	{
		var panel = new PanelContainer();
		panel.SizeFlagsHorizontal = SizeFlags.ExpandFill;
		panel.SizeFlagsVertical = SizeFlags.ExpandFill;
		panel.CustomMinimumSize = new Vector2(0f, 240f);
		panel.AddThemeStyleboxOverride("panel", CreateCardStyle(accent, true, false));
		return panel;
	}

	private VBoxContainer AddSectionContent(PanelContainer panel)
	{
		var margin = new MarginContainer();
		margin.AddThemeConstantOverride("margin_left", 18);
		margin.AddThemeConstantOverride("margin_top", 18);
		margin.AddThemeConstantOverride("margin_right", 18);
		margin.AddThemeConstantOverride("margin_bottom", 18);
		panel.AddChild(margin);

		var column = new VBoxContainer();
		column.AddThemeConstantOverride("separation", 12);
		margin.AddChild(column);
		return column;
	}

	private Label CreateSectionTitle(string text, Color color)
	{
		var label = new Label();
		label.Text = text;
		label.HorizontalAlignment = HorizontalAlignment.Center;
		label.AddThemeFontSizeOverride("font_size", 24);
		label.AddThemeColorOverride("font_color", color);
		return label;
	}

	private Label CreateSectionText(string text)
	{
		var label = new Label();
		label.Text = text;
		label.HorizontalAlignment = HorizontalAlignment.Center;
		label.AutowrapMode = TextServer.AutowrapMode.WordSmart;
		label.AddThemeFontSizeOverride("font_size", 15);
		label.AddThemeColorOverride("font_color", new Color(0.82f, 0.86f, 0.92f));
		return label;
	}

	private Label CreateChipLabel(string text, Color accent)
	{
		accent = NormalizeAccent(accent);
		var label = new Label();
		label.Text = text;
		label.AddThemeFontSizeOverride("font_size", 15);
		label.AddThemeColorOverride("font_color", accent.Lightened(0.24f));
		label.AddThemeStyleboxOverride("normal", CreateChipStyle(accent));
		return label;
	}

	private StyleBoxFlat CreateChipStyle(Color accent)
	{
		var style = new StyleBoxFlat();
		style.BgColor = new Color(0.05f, 0.07f, 0.11f, 0.92f);
		style.BorderColor = accent;
		style.SetBorderWidthAll(2);
		style.SetCornerRadiusAll(999);
		style.ContentMarginLeft = 16;
		style.ContentMarginTop = 9;
		style.ContentMarginRight = 16;
		style.ContentMarginBottom = 9;
		return style;
	}

	private StyleBoxFlat CreateCardStyle(Color accent, bool highlight, bool locked)
	{
		accent = NormalizeAccent(accent);
		var style = new StyleBoxFlat();
		style.BgColor = locked
			? new Color(0.06f, 0.07f, 0.1f, 0.95f)
			: highlight
				? new Color(accent, 0.22f)
				: new Color(0.07f, 0.09f, 0.12f, 0.97f);
		style.BorderColor = locked
			? new Color(0.48f, 0.58f, 0.7f, 0.7f)
			: highlight
				? accent
				: new Color(0.2f, 0.27f, 0.36f, 0.72f);
		style.SetBorderWidthAll(highlight ? 3 : 2);
		style.SetCornerRadiusAll(18);
		style.SetContentMarginAll(8);
		return style;
	}

	private Label CreateStatLabel(string text, Color color)
	{
		var label = new Label();
		label.Text = text;
		label.HorizontalAlignment = HorizontalAlignment.Center;
		label.AddThemeFontSizeOverride("font_size", 13);
		label.AddThemeColorOverride("font_color", color);
		return label;
	}

	private Button CreatePrimaryButton(string text, Color accent)
	{
		accent = NormalizeAccent(accent);
		var button = new Button();
		button.Text = text;
		button.Flat = true;
		button.FocusMode = FocusModeEnum.None;
		button.AddThemeFontSizeOverride("font_size", 16);
		button.AddThemeColorOverride("font_color", Colors.White);

		var normal = new StyleBoxFlat();
		normal.BgColor = new Color(accent, 0.22f);
		normal.BorderColor = accent;
		normal.SetBorderWidthAll(2);
		normal.SetCornerRadiusAll(14);
		normal.SetContentMarginAll(14);

		var hover = new StyleBoxFlat();
		hover.BgColor = new Color(accent, 0.32f);
		hover.BorderColor = accent.Lightened(0.1f);
		hover.SetBorderWidthAll(2);
		hover.SetCornerRadiusAll(14);
		hover.SetContentMarginAll(14);

		var disabled = new StyleBoxFlat();
		disabled.BgColor = new Color(0.1f, 0.11f, 0.15f, 0.92f);
		disabled.BorderColor = new Color(0.28f, 0.33f, 0.4f, 0.8f);
		disabled.SetBorderWidthAll(1);
		disabled.SetCornerRadiusAll(14);
		disabled.SetContentMarginAll(14);

		button.AddThemeStyleboxOverride("normal", normal);
		button.AddThemeStyleboxOverride("hover", hover);
		button.AddThemeStyleboxOverride("pressed", hover);
		button.AddThemeStyleboxOverride("disabled", disabled);
		return button;
	}

	private Color NormalizeAccent(Color accent)
	{
		Color baseAccent = new Color(0.58f, 0.78f, 1f, accent.A);
		return accent.Lerp(baseAccent, 0.65f);
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
			int horizontalMargin = (int)Mathf.Clamp(viewportSize.X * 0.018f, 10f, 24f);
			int topMargin = (int)Mathf.Clamp(viewportSize.Y * 0.025f, 10f, 26f);
			int bottomMargin = (int)Mathf.Clamp(viewportSize.Y * 0.08f, 42f, 82f);
			_overlayFrame.AddThemeConstantOverride("margin_left", horizontalMargin);
			_overlayFrame.AddThemeConstantOverride("margin_top", topMargin);
			_overlayFrame.AddThemeConstantOverride("margin_right", horizontalMargin);
			_overlayFrame.AddThemeConstantOverride("margin_bottom", bottomMargin);
		}

		if (_menuPanel != null)
		{
			float panelWidth = Mathf.Clamp(viewportSize.X - 28f, 320f, 1400f);
			float panelHeight = Mathf.Clamp(viewportSize.Y - 68f, 380f, 840f);
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

		if (viewportWidth < 900f)
		{
			return 2;
		}

		if (Inventory?.Weapons.Count > 0)
		{
			return Inventory.Weapons.Count;
		}

		return 5;
	}

	private int GetPauseSectionColumnCount()
	{
		return GetViewportRect().Size.X < 1080f ? 1 : 2;
	}

	private Vector2 GetWeaponCardSize()
	{
		float viewportWidth = GetViewportRect().Size.X;
		if (viewportWidth < 720f)
		{
			return new Vector2(220f, 300f);
		}

		if (viewportWidth < 900f)
		{
			return new Vector2(190f, 300f);
		}

		if (viewportWidth < 1100f)
		{
			return new Vector2(148f, 304f);
		}

		if (viewportWidth < 1360f)
		{
			return new Vector2(164f, 312f);
		}

		return new Vector2(182f, 320f);
	}
}
