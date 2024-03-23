using System.ComponentModel.DataAnnotations;

namespace RecAll.Contrib.TextItem.Api;

public class CreateTextItemCommand
{
    [Required] public string Content { get; set; }
}
