using System.Xml;
namespace PermissionsBot;

public class Texts
{
    private XmlElement? _xmlRoot;
    public Texts(string fileName)
    {
         XmlDocument xmlDocument = new XmlDocument();
         xmlDocument.Load(fileName);
         _xmlRoot = xmlDocument.DocumentElement;
         if (_xmlRoot == null)
         {
             throw new NullReferenceException("xml document hasn't a root.");
         }
    }

    public string GetButtonText(string buttonName)
    {
        return _xmlRoot.SelectSingleNode($"//buttons/{buttonName}").InnerText;
    }

    public string GetMessageText(string messageName)
    {
        return _xmlRoot.SelectSingleNode($"//messages//default/{messageName}").InnerText;
    }

    public string GetErrorText(string errorName)
    {
        return _xmlRoot.SelectSingleNode($"//messages//errors/{errorName}").InnerText;
    }

    public string GetCommandDescriptionText(string commandName)
    {
        return _xmlRoot.SelectSingleNode($"//commands/{commandName}").InnerText;
    }
}