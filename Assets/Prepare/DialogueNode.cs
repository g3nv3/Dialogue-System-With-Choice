
using System.Collections.Generic;
public class DialogNode
{
    public string ShortName { get; set; }
    public string Name { get; set; }
    public string Message { get; set; }
    public List<DialogNode> Children { get; set; }
    public int? ActionId { get; set; }

    public DialogNode()
    {
        Children = new List<DialogNode>();
    }
}