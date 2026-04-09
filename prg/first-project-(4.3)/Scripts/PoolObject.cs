using Godot;
using System;

[GlobalClass]
[System.Serializable]
public partial class PoolObject : Resource
{
    [Export]
    public PackedScene Prefab;
    [Export]
    public uint PoolSize;
}