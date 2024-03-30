using System.Collections.Generic;
using UnityEngine;
using System.Xml;
public static class ReadDialogueData
{
    public static (List<string>, List<string>) Read(TextAsset textAsset)
    {
        List<string> names = new List<string>();
        List<string> replicas = new List<string>();

        XmlDocument xmlDoc = new XmlDocument();
        xmlDoc.LoadXml(textAsset.text);

        XmlNodeList dialogNodes = xmlDoc.SelectNodes("/dialogs/dialog");

        foreach (XmlNode dialogNode in dialogNodes)
        {
            string name = dialogNode.SelectSingleNode("name").InnerText;
            string replica = dialogNode.SelectSingleNode("replica").InnerText;

            names.Add(name);
            replicas.Add(replica);
        }
        return (names, replicas);
    }
}
