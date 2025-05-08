using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using ems_back.Repo.Models;
using ems_back.Repo.Models.Types;

public interface ITrigger
{
    [Required]
    public TriggerType TriggerType { get; }
}

