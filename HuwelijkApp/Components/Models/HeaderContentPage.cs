namespace HuwelijkApp.Components.Models;

public class HeaderContentPage(string link, string text, string icon = null)
{
    public string Link = link;
    public string Text = text;
    public string Icon = icon;
}