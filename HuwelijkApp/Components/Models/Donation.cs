namespace HuwelijkApp.Components.Models;

public class Donation {
    public Donation() { }

    public Donation(string header, string text, string id, string link) {
        Header = header;
        Text = text;
        Id = id;
        Link = link;
    }

    public string Link { get; set; } = string.Empty;
    public string Header { get; set; } = string.Empty;
    public string Text { get; set; } = string.Empty;
    public string Id { get; set; } = string.Empty;
}
