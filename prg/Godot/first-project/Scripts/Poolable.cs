using System;
using Godot;

public static class Poolable{

	public static void Init<T>(T poolable) where T : Node2D {
		poolable.ProcessMode = Node.ProcessModeEnum.Disabled;
		poolable.Hide();
	}

	public static void Deactivate<T>(T poolable) where T : Node2D {
		Init(poolable);
		Game.Instance.EnqueuePoolObject(poolable);
	}

	public static async void Activate<T>(T poolable)where T : Node2D {
		await poolable.ToSignal(poolable.GetTree(), SceneTree.SignalName.PhysicsFrame);
		Action action = () => {
			poolable.ProcessMode = Node.ProcessModeEnum.Inherit;
			poolable.Show();
		};
		Callable.From(action).CallDeferred();
	}  
}
