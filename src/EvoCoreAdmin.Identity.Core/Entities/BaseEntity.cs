namespace EvoCoreAdmin.Identity.Core.Entities;

public abstract class BaseEntity<T> where T : struct
{
    public T Id { get; set; }
}