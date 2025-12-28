namespace FuFood.Models.Interfaces;

public interface IHasTimestamp
{
    DateTime UpdatedAt { get; set; }
}