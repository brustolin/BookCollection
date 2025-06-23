using System;

namespace BookTracker.Tools.ChangeTracker;

class ChangeStorageAttribute<TARGET> : Attribute 
    where TARGET : EntityChange, new()
{
    public Type StorageType { get; } = typeof(TARGET);
}