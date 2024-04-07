using UnityEngine;
using System.Xml;

public static class ParseDialogFile
{
    static public DialogNode GetDialogTree(TextAsset textAsset)
    {
        XmlDocument xmlDoc = new XmlDocument();
        xmlDoc.LoadXml(textAsset.text);
        XmlNode rootDialogNode = xmlDoc.SelectSingleNode("/dialog");
        return CreateDialogTree(rootDialogNode);
    }

    static private DialogNode CreateDialogTree(XmlNode dialogNode)
    {
        DialogNode node = new DialogNode();

        node.Name = dialogNode.SelectSingleNode("name").InnerText;
        node.Message = dialogNode.SelectSingleNode("message").InnerText;
        XmlNode actionNode = dialogNode.SelectSingleNode("action");
        if (actionNode != null)
            node.ActionId = int.Parse(actionNode.Attributes["id"].Value);
        XmlNode shortNameNode = dialogNode.SelectSingleNode("short_name");
        if (shortNameNode != null)
            node.ShortName = shortNameNode.InnerText;

        foreach (XmlNode childNode in dialogNode.SelectNodes("dialog"))
        {
            DialogNode child = CreateDialogTree(childNode);
            node.Children.Add(child);
        }

        return node;
    }
}
