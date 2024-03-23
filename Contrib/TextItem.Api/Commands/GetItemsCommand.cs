using System.ComponentModel.DataAnnotations;

namespace RecAll.Contrib.TextItem.Api;

public class GetItemsCommand
{
    [Required] public IEnumerable<int> Ids { get; set; }
}
