using System.ComponentModel.DataAnnotations;

namespace RecAll.Contrib.TextItem.Api;

public class UpdateTextItemCommand
{
    [Required] public int Id { get; set; }
    [Required] public string Content { get; set; }
}
